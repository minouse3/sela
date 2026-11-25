using System;
using UnityEngine;

namespace SELA.Core
{
    public struct ActivityExecutionResult
    {
        public bool Executed;
        public string Summary;
        public bool TriggeredJeda;
        public int TriggeredRift;
        public bool OpenedFinalDecision;
        public bool MissedInterview;
        public InterviewOutcome? InterviewOutcome;
    }

    /// <summary>
    /// Pure UI-free core loop engine following revised SELA GDD §3.1 - §3.13:
    /// - Action-driven clock (08:00 - 22:00) with Pagi/Siang/Malam environment lighting
    /// - Option B: Pure Pressure-Driven Retakan Ruang (Tekanan >= 5, unlocked Day 1/3/5)
    /// - Option 2A: 7-day ceiling + flexible 1 Final Prep Session (Day 4+ / Day 7)
    /// - Option 3A: Bathroom Mandi recovery (+2 Tenaga, -3 Tekanan, 1 jam, 1x/day)
    /// </summary>
    public class GameLoopEngine
    {
        public GameState State { get; private set; }
        public SaveSystem Save { get; }
        public string LastStatusMessage { get; private set; } = "";
        public SaveOperationResult LastSaveResult { get; private set; }

        public event Action<GameState> OnStateChanged;
        public event Action<ActivityRequest, ActivityExecutionResult> OnActivityResolved;
        public event Action<GameState> OnJedaRequired;
        public event Action<JedaChoice, GameState> OnJedaResolved;
        public event Action<int, GameState> OnRiftTriggered;
        public event Action<int, bool, GameState> OnRiftResolved;
        public event Action<int, GameState> OnDayStarted;
        public event Action<GameState> OnFinalDecisionOpened;
        public event Action<EndingType, GameState> OnEndingReached;

        public GameLoopEngine(SaveSystem saveSystem = null)
        {
            Save = saveSystem ?? new SaveSystem();
            State = GameState.CreateInitial();
        }

        public void StartNewGame(bool saveInitialCheckpoints = true)
        {
            State = GameState.CreateInitial();
            LastStatusMessage = "Permainan baru dimulai: Hari 1 · 08:00 (Pagi).";
            if (saveInitialCheckpoints)
            {
                Save.SaveDayStartCheckpoint(State);
                LastSaveResult = Save.SaveAutosave(State);
            }
            OnDayStarted?.Invoke(State.day, State);
            NotifyStateChanged();
        }

        public LoadOperationResult LoadAutosave(bool allowFallback = true)
        {
            var result = Save.LoadAutosave(allowFallback);
            if (result.Success && result.State != null)
            {
                State = result.State;
                LastStatusMessage = result.Message;
                NotifyStateChanged();
            }
            else
            {
                LastStatusMessage = result.Message;
            }
            return result;
        }

        public LoadOperationResult LoadDayStartCheckpoint()
        {
            var result = Save.LoadDayStartCheckpoint();
            if (result.Success && result.State != null)
            {
                State = result.State;
                LastSaveResult = Save.SaveAutosave(State);
                LastStatusMessage = result.Message;
                NotifyStateChanged();
            }
            else
            {
                LastStatusMessage = result.Message;
            }
            return result;
        }

        public ActivityPreview GetPreview(ActivityRequest request)
        {
            return ActivitySystem.BuildPreview(State, request);
        }

        /// <summary>
        /// Executes one timed activity following revised GDD §3.2 resolution order:
        /// 1. Validate availability & remaining hours before 22:00
        /// 2. Apply missed-interview penalty if activity crosses 13:00 interview
        /// 3. Apply activity effects + stat deltas (summed before clamp) & advance clock
        /// 4. If FinalPrep -> save pre-finalization checkpoint & open FinalDecision
        /// 5. Save stable checkpoint BEFORE Rift/Jeda (so quitting mid-Rift never erases interviews)
        /// 6. Check Option B Pressure Rift (Tekanan >= 5) -> else Jeda fallback (Tekanan == 6)
        /// </summary>
        public ActivityExecutionResult ExecuteActivity(ActivityRequest request, InterviewInput? interviewInput = null)
        {
            var availability = ActivitySystem.CheckAvailability(State, request);
            if (!availability.IsAvailable)
            {
                LastStatusMessage = availability.Reason;
                return new ActivityExecutionResult { Executed = false, Summary = availability.Reason };
            }

            var exec = new ActivityExecutionResult { Executed = true };
            string extraNotes = "";
            int endHour = ActivitySystem.ComputeEndHour(State, request);

            // 1. Handle missed interview if player ran past today's 13:00 interview.
            if (availability.WillMissScheduledInterview)
            {
                var missedApp = State.GetApplication(availability.MissedInterviewCompany);
                if (missedApp != null && missedApp.status == ApplicationStatus.Scheduled)
                {
                    missedApp.status = ApplicationStatus.Rejected;
                    missedApp.missedInterview = true;
                    missedApp.rejectionReasons.Clear();
                    missedApp.rejectionReasons.Add("Interview pukul 13:00 terlewat.");
                    State.pressure += 1;
                    exec.MissedInterview = true;
                    extraNotes += $" Interview {ActivitySystem.GetCompanyName(availability.MissedInterviewCompany)} terlewat (Tekanan +1).";
                }
            }

            // 2. Apply base stat deltas (summed before clamp).
            ActivitySystem.GetBaseDeltas(request.Type, out int eDelta, out int pDelta);
            State.energy += eDelta;
            State.pressure += pDelta;

            // 3. Apply activity-specific domain effects.
            switch (request.Type)
            {
                case ActivityType.ImproveUiExample:
                    State.AddEvidence(EvidenceId.E1);
                    State.isDeskTidy = false;
                    extraNotes += " Kartu bukti E1 diperoleh.";
                    break;

                case ActivityType.CompleteTestNotes:
                    State.AddEvidence(EvidenceId.E2);
                    State.isDeskTidy = false;
                    extraNotes += " Kartu bukti E2 diperoleh.";
                    break;

                case ActivityType.VerifyCompany:
                    var ver = State.GetVerification(request.Company);
                    if (ver != null) ver.isVerified = true;
                    extraNotes += $" Ringkasan verifikasi {ActivitySystem.GetCompanyName(request.Company)} terbuka.";
                    break;

                case ActivityType.SubmitApplication:
                    var app = State.GetApplication(request.Company);
                    if (app != null)
                    {
                        app.status = ApplicationStatus.Scheduled;
                        app.submittedDay = State.day;
                        app.submittedHour = State.currentHour;
                        app.submittedSession = State.session;
                        app.interviewDay = State.day + 1;
                        app.interviewHour = GameState.InterviewHour;

                        // Freeze CV snapshot (only attach evidence actually owned & selected).
                        app.snapshotClaim1 = State.cvDraft.claim1;
                        app.snapshotClaim2 = State.cvDraft.claim2;
                        app.snapshotHasE1 = State.hasE1 && State.cvDraft.attachE1;
                        app.snapshotHasE2 = State.hasE2 && State.cvDraft.attachE2;

                        extraNotes += $" Lamaran terkirim. Interview dijadwalkan Hari {app.interviewDay} · 13:00 (perlu Tenaga 1).";
                    }
                    break;

                case ActivityType.AttendInterview:
                    var intApp = State.GetApplication(request.Company);
                    if (intApp != null)
                    {
                        var input = interviewInput ?? new InterviewInput
                        {
                            CorrectExaggeratedClaims = false,
                            Question1ChoiceIndex = 1,
                            Question2ChoiceIndex = 1
                        };
                        var outcome = ActivitySystem.EvaluateInterview(intApp, input);
                        exec.InterviewOutcome = outcome;

                        if (outcome.Offered)
                        {
                            extraNotes += $" {ActivitySystem.GetCompanyName(request.Company)} memberikan tawaran kerja!";
                        }
                        else
                        {
                            State.pressure += 1; // Rejection adds +1 Tekanan before Rift/Jeda check.
                            extraNotes += $" Lamaran ditolak ({outcome.PrimaryReason}) Tekanan +1.";
                        }
                    }
                    break;

                case ActivityType.Rest:
                    extraNotes += " Istirahat singkat memulihkan tenaga.";
                    break;

                case ActivityType.Shower:
                    State.showeredToday = true;
                    State.showerObservationsSeen++;
                    extraNotes += " Mandi segar meredakan tekanan secara signifikan.";
                    break;

                case ActivityType.CallDita:
                    bool firstToday = !State.ditaCalledToday;
                    State.ditaCalledToday = true;
                    extraNotes += firstToday
                        ? " Percakapan harian dengan Dita selesai."
                        : " Pesan singkat dengan Dita selesai.";
                    break;

                case ActivityType.SitWithoutScreen:
                    State.sitObservationsSeen++;
                    extraNotes += " Duduk sejenak menjauh dari layar.";
                    break;

                case ActivityType.TidyDesk:
                    State.deskTidiedToday = true;
                    State.isDeskTidy = true;
                    State.tidyObservationsSeen++;
                    extraNotes += " Meja terlihat lebih rapi.";
                    break;

                case ActivityType.FinalPrep:
                    extraNotes += " Barang dikemas dan jawaban siap dikunci.";
                    break;
            }

            // Advance clock and sync Pagi / Siang / Malam environment state.
            State.currentHour = endHour;
            State.ClampStats();

            exec.Summary = $"{ActivitySystem.GetActivityTitle(request)} selesai ({State.FormattedTime} · {State.session}).{extraNotes}";
            LastStatusMessage = exec.Summary;

            // 4. FinalPrep opens FinalDecision immediately (saving pre-finalization checkpoint first).
            if (request.Type == ActivityType.FinalPrep)
            {
                LastSaveResult = Save.SaveAutosave(State);
                State.phase = GamePhase.FinalDecision;
                exec.OpenedFinalDecision = true;
                OnActivityResolved?.Invoke(request, exec);
                OnFinalDecisionOpened?.Invoke(State);
                NotifyStateChanged();
                return exec;
            }

            // 5. Save stable checkpoint BEFORE Rift/Jeda so quitting mid-Rift never erases completed work/interviews.
            LastSaveResult = Save.SaveAutosave(State);

            // 6. Option B: Check Pressure-Driven Retakan Ruang (Tekanan >= 5, unlocked by Day 1/3/5, max 1/day).
            int eligibleRift = ActivitySystem.GetEligiblePressureRift(State, State.pressure);
            if (eligibleRift > 0)
            {
                State.phase = GamePhase.InRift;
                State.activeRiftIndex = eligibleRift;
                State.riftTriggeredToday = true;
                exec.TriggeredRift = eligibleRift;
                LastStatusMessage += $" Tekanan mencapai {State.pressure} — Retakan Ruang {eligibleRift} dimulai!";
                OnActivityResolved?.Invoke(request, exec);
                OnRiftTriggered?.Invoke(eligibleRift, State);
                NotifyStateChanged();
                return exec;
            }

            // 7. Fallback Jeda (only if Tekanan == 6 when no Rift is eligible, once per day).
            if (State.pressure >= GameState.MaxPressure && !State.jedaTriggeredToday)
            {
                State.phase = GamePhase.AwaitingJeda;
                exec.TriggeredJeda = true;
                OnActivityResolved?.Invoke(request, exec);
                OnJedaRequired?.Invoke(State);
                NotifyStateChanged();
                return exec;
            }

            OnActivityResolved?.Invoke(request, exec);
            NotifyStateChanged();
            return exec;
        }

        public bool ResolveJeda(JedaChoice choice)
        {
            if (State.phase != GamePhase.AwaitingJeda)
                return false;

            State.jedaTriggeredToday = true;
            State.pressure = 4;
            State.energy = Mathf.Max(1, State.energy);
            State.ClampStats();
            State.phase = GamePhase.FreeRoam;

            string label = choice == JedaChoice.SitBriefly ? "Duduk sebentar" : "Kirim pesan kepada Dita";
            LastStatusMessage = $"Jeda selesai ({label}): Tekanan menjadi 4, Tenaga minimal 1 (tanpa memajukan jam).";
            OnJedaResolved?.Invoke(choice, State);

            AutosaveIfStable();
            NotifyStateChanged();
            return true;
        }

        public bool ResolveRift(bool skippedViaHelp = false)
        {
            if (State.phase != GamePhase.InRift || State.activeRiftIndex <= 0)
                return false;

            int solvedIndex = State.activeRiftIndex;
            State.MarkRiftSolved(solvedIndex);
            State.activeRiftIndex = 0;
            State.phase = GamePhase.FreeRoam;

            // Option B payoff: grounding Raka relieves Tekanan to 3 and guarantees Tenaga >= 1.
            State.pressure = 3;
            State.energy = Mathf.Max(1, State.energy);
            State.ClampStats();

            LastStatusMessage = skippedViaHelp
                ? $"Retakan Ruang {solvedIndex} dipulihkan melalui bantuan. Tekanan turun ke 3, Tenaga minimal 1."
                : $"Retakan Ruang {solvedIndex} terselesaikan. Ruang kembali normal (Tekanan → 3, Tenaga ≥ 1).";

            OnRiftResolved?.Invoke(solvedIndex, skippedViaHelp, State);

            AutosaveIfStable();
            NotifyStateChanged();
            return true;
        }

        /// <summary>
        /// Called from the Bed anytime during Malam (currentHour >= 18:00).
        /// Days 1-6: sleeps, advances to Day+1 08:00 (Pagi), recovers Tenaga +2 / Tekanan -1, resets daily flags.
        /// Day 7: opens FinalDecision directly (preserving pre-finalization autosave).
        /// </summary>
        public bool SleepOrOpenFinalDecision()
        {
            if (State.phase != GamePhase.FreeRoam || !State.CanSleepNow)
            {
                LastStatusMessage = "Belum waktunya tidur (tidur tersedia mulai pukul 18:00 · Malam).";
                return false;
            }

            if (State.day >= GameState.MaxDay)
            {
                LastSaveResult = Save.SaveAutosave(State);
                State.phase = GamePhase.FinalDecision;
                LastStatusMessage = $"Hari 7 · {State.FormattedTime}. Tentukan keputusan akhir Raka.";
                OnFinalDecisionOpened?.Invoke(State);
                NotifyStateChanged();
                return true;
            }

            State.day += 1;
            State.currentHour = GameState.StartHour;
            State.energy += 2;
            State.pressure -= 1;
            State.ClampStats();

            // Reset daily flags & cluttered desk for new day.
            State.deskTidiedToday = false;
            State.showeredToday = false;
            State.jedaTriggeredToday = false;
            State.ditaCalledToday = false;
            State.riftTriggeredToday = false;
            State.isDeskTidy = false;

            Save.SaveDayStartCheckpoint(State);
            LastSaveResult = Save.SaveAutosave(State);

            string interviewNote = State.HasScheduledInterviewOn(State.day, out CompanyId scheduled)
                ? $" Pengingat: Interview {ActivitySystem.GetCompanyName(scheduled)} hari ini pukul 13:00 (perlu Tenaga 1)."
                : "";
            LastStatusMessage = $"Hari {State.day} · 08:00 (Pagi) dimulai (Tenaga {State.energy}/4, Tekanan {State.pressure}/6).{interviewNote}";

            OnDayStarted?.Invoke(State.day, State);
            NotifyStateChanged();
            return true;
        }

        // --- Free (Zero-Time) Interactions ---

        public void UpdateCvDraft(ClaimChoice claim1, ClaimChoice claim2, bool attachE1, bool attachE2)
        {
            State.cvDraft.claim1 = claim1;
            State.cvDraft.claim2 = claim2;
            State.cvDraft.attachE1 = attachE1 && State.hasE1;
            State.cvDraft.attachE2 = attachE2 && State.hasE2;
            AutosaveIfStable();
            NotifyStateChanged();
        }

        public void SetRiskTag(CompanyId company, VerificationAspect aspect, RiskTag tag)
        {
            var ver = State.GetVerification(company);
            if (ver == null) return;
            ver.SetTag(aspect, tag);
            AutosaveIfStable();
            NotifyStateChanged();
        }

        public bool SetOfferChoice(OfferChoice choice)
        {
            if (choice == OfferChoice.StudioLajur && !State.HasOffer(CompanyId.StudioLajur))
            {
                LastStatusMessage = "Belum ada tawaran resmi dari Studio Lajur.";
                return false;
            }
            if (choice == OfferChoice.KoperasiPijar && !State.HasOffer(CompanyId.KoperasiPijar))
            {
                LastStatusMessage = "Belum ada tawaran resmi dari Koperasi Pijar.";
                return false;
            }
            if (choice == OfferChoice.AksesPrima && !State.HasOffer(CompanyId.AksesPrima))
            {
                LastStatusMessage = "Tawaran Akses Prima belum tersedia (hadir mulai Hari 4).";
                return false;
            }

            if (State.offerChoice != choice)
            {
                State.offerChoice = choice;
                if (!ActivitySystem.IsDecisionNoteValidForChoice(choice, State.decisionNote))
                    State.decisionNote = DecisionNoteType.None;
            }

            LastStatusMessage = choice == OfferChoice.None
                ? "Pilihan sementara dikembalikan ke: Pulang (tidak ada kontrak dipilih)."
                : $"Pilihan sementara ditetapkan ke: {choice}.";

            AutosaveIfStable();
            NotifyStateChanged();
            return true;
        }

        public void WithdrawOfferChoice()
        {
            SetOfferChoice(OfferChoice.None);
        }

        public bool SetDecisionNote(DecisionNoteType note)
        {
            if (note != DecisionNoteType.None && !ActivitySystem.IsDecisionNoteValidForChoice(State.offerChoice, note))
            {
                LastStatusMessage = "Catatan keputusan tidak sesuai dengan pilihan tawaran saat ini.";
                return false;
            }

            State.decisionNote = note;
            AutosaveIfStable();
            NotifyStateChanged();
            return true;
        }

        public void ReturnFromFinalDecision()
        {
            if (State.phase == GamePhase.FinalDecision)
            {
                State.phase = GamePhase.FreeRoam;
                LastStatusMessage = "Kembali meninjau tawaran sebelum mengunci keputusan.";
                NotifyStateChanged();
            }
        }

        public bool ConfirmFinalDecision()
        {
            if (State.phase != GamePhase.FinalDecision)
            {
                LastStatusMessage = "Jalankan persiapan keputusan akhir terlebih dahulu.";
                return false;
            }

            if (!ActivitySystem.IsDecisionNoteValidForChoice(State.offerChoice, State.decisionNote))
            {
                LastStatusMessage = State.offerChoice == OfferChoice.AksesPrima
                    ? "Konfirmasi pemahaman syarat berisiko Akses Prima terlebih dahulu."
                    : "Pilih satu catatan batas kerja atau langkah pulang terlebih dahulu.";
                return false;
            }

            // Preserve pre-finalization autosave so Continue after epilogue returns to review state.
            State.ending = ActivitySystem.ResolveEnding(State);
            State.phase = GamePhase.Epilogue;
            LastStatusMessage = $"Keputusan dikunci pada Hari {State.day}. Ending: {State.ending}.";

            OnEndingReached?.Invoke(State.ending, State);
            NotifyStateChanged();
            return true;
        }

        // --- Internal Helpers ---

        void AutosaveIfStable()
        {
            if (State.phase == GamePhase.FreeRoam)
            {
                LastSaveResult = Save.SaveAutosave(State);
            }
            else if (State.phase == GamePhase.FinalDecision)
            {
                // Persist offerChoice / decisionNote changes while keeping reloadable FreeRoam state.
                var snapshot = State.Clone();
                snapshot.phase = GamePhase.FreeRoam;
                LastSaveResult = Save.SaveAutosave(snapshot);
            }
        }

        void NotifyStateChanged()
        {
            OnStateChanged?.Invoke(State);
        }
    }
}
