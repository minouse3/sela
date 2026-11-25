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
        public bool MissedInterview;
        public InterviewOutcome? InterviewOutcome;
    }

    /// <summary>
    /// Pure UI-free core loop engine following SELA GDD §3.1 - §3.13.
    /// Subscribe to its events from any UI, audio, lighting, or world system.
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
            LastStatusMessage = "Permainan baru dimulai: Hari 1 · Pagi.";
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
        /// Executes one timed session activity following GDD §3.2 resolution order:
        /// 1. Validate availability
        /// 2. Apply missed-interview penalty if replacing a scheduled interview
        /// 3. Apply activity effects + stat deltas once
        /// 4. Check Jeda (Tekanan == 6, once/day)
        /// 5. Check scheduled Retakan Ruang (Day 2/4/6 Siang)
        /// 6. Advance session (or lock finished night) & autosave on stable state
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

            // 1. Handle missed interview if player replaced their scheduled Siang interview.
            if (availability.WillMissScheduledInterview)
            {
                var missedApp = State.GetApplication(availability.MissedInterviewCompany);
                if (missedApp != null && missedApp.status == ApplicationStatus.Scheduled)
                {
                    missedApp.status = ApplicationStatus.Rejected;
                    missedApp.missedInterview = true;
                    missedApp.rejectionReasons.Clear();
                    missedApp.rejectionReasons.Add("Interview terlewat.");
                    State.pressure += 1;
                    exec.MissedInterview = true;
                    extraNotes += $" Interview {ActivitySystem.GetCompanyName(availability.MissedInterviewCompany)} terlewat (Tekanan +1).";
                }
            }

            // 2. Apply base stat deltas.
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
                        app.submittedSession = State.session;
                        app.interviewDay = State.day + 1;

                        // Freeze CV snapshot (only attach evidence actually owned & selected).
                        app.snapshotClaim1 = State.cvDraft.claim1;
                        app.snapshotClaim2 = State.cvDraft.claim2;
                        app.snapshotHasE1 = State.hasE1 && State.cvDraft.attachE1;
                        app.snapshotHasE2 = State.hasE2 && State.cvDraft.attachE2;

                        extraNotes += $" Lamaran terkirim. Interview dijadwalkan Hari {app.interviewDay} · Siang (perlu Tenaga 1).";
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
                            State.pressure += 1; // GDD §3.7: rejection adds +1 Tekanan before Jeda check.
                            extraNotes += $" Lamaran ditolak ({outcome.PrimaryReason}) Tekanan +1.";
                        }
                    }
                    break;

                case ActivityType.Rest:
                    extraNotes += " Istirahat singkat memulihkan tenaga.";
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
            }

            State.ClampStats();
            State.sessionResolved = true;

            exec.Summary = $"{ActivitySystem.GetActivityTitle(request)} selesai.{extraNotes}";
            LastStatusMessage = exec.Summary;

            // 4. Check automatic Jeda (once per day when Tekanan hits 6).
            if (State.pressure >= GameState.MaxPressure && !State.jedaTriggeredToday)
            {
                State.phase = GamePhase.AwaitingJeda;
                exec.TriggeredJeda = true;
                OnActivityResolved?.Invoke(request, exec);
                OnJedaRequired?.Invoke(State);
                NotifyStateChanged();
                return exec;
            }

            // 5. Continue to Rift check / Session advance / Autosave.
            int rift = ContinueAfterActivityOrJeda();
            exec.TriggeredRift = rift;
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
            LastStatusMessage = $"Jeda selesai ({label}): Tekanan menjadi 4, Tenaga minimal 1 (tanpa sesi).";
            OnJedaResolved?.Invoke(choice, State);

            ContinueAfterActivityOrJeda();
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

            LastStatusMessage = skippedViaHelp
                ? $"Retakan Ruang {solvedIndex} dipulihkan melalui bantuan. Ruang kembali normal."
                : $"Retakan Ruang {solvedIndex} terselesaikan. Ruang kembali normal.";

            OnRiftResolved?.Invoke(solvedIndex, skippedViaHelp, State);

            AdvanceSessionAndAutosave();
            NotifyStateChanged();
            return true;
        }

        /// <summary>
        /// Called from the Bed after Night finishes (session == Malam && sessionResolved).
        /// Days 1-6: sleeps, recovers Tenaga +2 / Tekanan -1, resets daily flags, saves day_start + autosave.
        /// Day 7: opens FinalDecision phase (preserving pre-finalization autosave).
        /// </summary>
        public bool SleepOrOpenFinalDecision()
        {
            if (State.phase != GamePhase.FreeRoam || !State.IsNightFinished)
            {
                LastStatusMessage = "Belum waktunya menutup hari. Selesaikan sesi Malam terlebih dahulu.";
                return false;
            }

            if (State.day >= GameState.MaxDay)
            {
                // Save pre-finalization autosave so Continue after epilogue returns to Night 7 decision.
                LastSaveResult = Save.SaveAutosave(State);
                State.phase = GamePhase.FinalDecision;
                LastStatusMessage = "Hari 7 · Malam selesai. Tentukan keputusan akhir Raka.";
                OnFinalDecisionOpened?.Invoke(State);
                NotifyStateChanged();
                return true;
            }

            State.day += 1;
            State.session = SessionType.Pagi;
            State.sessionResolved = false;
            State.energy += 2;
            State.pressure -= 1;
            State.ClampStats();

            // Reset daily flags & cluttered desk for new day.
            State.deskTidiedToday = false;
            State.jedaTriggeredToday = false;
            State.ditaCalledToday = false;
            State.isDeskTidy = false;

            Save.SaveDayStartCheckpoint(State);
            LastSaveResult = Save.SaveAutosave(State);

            string interviewNote = State.HasScheduledInterviewOn(State.day, out CompanyId scheduled)
                ? $" Pengingat: Interview {ActivitySystem.GetCompanyName(scheduled)} siang ini memerlukan Tenaga 1."
                : "";
            LastStatusMessage = $"Hari {State.day} · Pagi dimulai (Tenaga {State.energy}/4, Tekanan {State.pressure}/6).{interviewNote}";

            OnDayStarted?.Invoke(State.day, State);
            NotifyStateChanged();
            return true;
        }

        // --- Free (Zero-Session) Interactions ---

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
                LastStatusMessage = "Keputusan akhir hanya dapat dikunci pada akhir Malam Hari 7.";
                return false;
            }

            if (!ActivitySystem.IsDecisionNoteValidForChoice(State.offerChoice, State.decisionNote))
            {
                LastStatusMessage = State.offerChoice == OfferChoice.AksesPrima
                    ? "Konfirmasi pemahaman syarat berisiko Akses Prima terlebih dahulu."
                    : "Pilih satu catatan batas kerja atau langkah pulang terlebih dahulu.";
                return false;
            }

            // Keep pre-finalization autosave intact per GDD §3.13 (Continue returns to Day 7 Night decision).
            State.ending = ActivitySystem.ResolveEnding(State);
            State.phase = GamePhase.Epilogue;
            LastStatusMessage = $"Keputusan dikunci. Ending: {State.ending}.";

            OnEndingReached?.Invoke(State.ending, State);
            NotifyStateChanged();
            return true;
        }

        // --- Internal Resolution Helpers ---

        int ContinueAfterActivityOrJeda()
        {
            int scheduledRift = ActivitySystem.GetScheduledRiftFor(State);
            if (scheduledRift > 0)
            {
                State.phase = GamePhase.InRift;
                State.activeRiftIndex = scheduledRift;
                LastStatusMessage += $" Retakan Ruang {scheduledRift} dimulai!";
                OnRiftTriggered?.Invoke(scheduledRift, State);
                return scheduledRift;
            }

            AdvanceSessionAndAutosave();
            return 0;
        }

        void AdvanceSessionAndAutosave()
        {
            if (State.session == SessionType.Pagi)
            {
                State.session = SessionType.Siang;
                State.sessionResolved = false;
            }
            else if (State.session == SessionType.Siang)
            {
                State.session = SessionType.Malam;
                State.sessionResolved = false;
            }
            else
            {
                // Night finished: stay on Malam with sessionResolved = true until sleeping.
                State.sessionResolved = true;
            }

            AutosaveIfStable();
        }

        void AutosaveIfStable()
        {
            if (State.phase == GamePhase.FreeRoam)
                LastSaveResult = Save.SaveAutosave(State);
        }

        void NotifyStateChanged()
        {
            OnStateChanged?.Invoke(State);
        }
    }
}
