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

    public struct SleepExecutionResult
    {
        public bool Executed;
        public SleepTarget Target;
        public string Summary;
        public string WakeCaption;
        public bool AdvancedDay;
        public bool OpenedFinalDecision;
        public bool MissedInterview;
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
        public event Action<GameState> OnJedaResolved;
        public event Action<int, GameState> OnRiftTriggered;
        public event Action<int, bool, GameState> OnRiftResolved;
        public event Action<int, GameState> OnDayStarted;
        public event Action<SleepTarget, SleepExecutionResult, GameState> OnSleepResolved;
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
            LastStatusMessage = "Hari 1 dimulai.";
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

        public SleepPreview GetSleepPreview(SleepTarget target)
        {
            return ActivitySystem.BuildSleepPreview(State, target);
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

            switch (request.Type)
            {
                case ActivityType.ImproveUiExample:
                    State.AddEvidence(EvidenceId.E1);
                    State.isDeskTidy = false;
                    extraNotes += " Bukti E1 siap.";
                    break;

                case ActivityType.CompleteTestNotes:
                    State.AddEvidence(EvidenceId.E2);
                    State.isDeskTidy = false;
                    extraNotes += " Bukti E2 siap.";
                    break;

                case ActivityType.VerifyCompany:
                    var ver = State.GetVerification(request.Company);
                    if (ver != null) ver.isVerified = true;
                    extraNotes += $" Info {ActivitySystem.GetCompanyName(request.Company)} terbuka.";
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

                        app.snapshotClaim1 = State.cvDraft.claim1;
                        app.snapshotClaim2 = State.cvDraft.claim2;
                        app.snapshotHasE1 = State.hasE1 && State.cvDraft.attachE1;
                        app.snapshotHasE2 = State.hasE2 && State.cvDraft.attachE2;

                        extraNotes += $" Terkirim. Interview Hari {app.interviewDay}, jam 13:00.";
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
                            extraNotes += $" {ActivitySystem.GetCompanyName(request.Company)} kirim tawaran.";
                        }
                        else
                        {
                            State.pressure += 1;
                            extraNotes += $" Ditolak ({outcome.PrimaryReason}) +1 Tekanan.";
                        }
                    }
                    break;

                case ActivityType.Rest:
                    extraNotes += " Badan terasa lebih ringan.";
                    break;

                case ActivityType.Shower:
                    State.showeredToday = true;
                    State.showerObservationsSeen++;
                    extraNotes += " Kepala lebih segar.";
                    break;

                case ActivityType.CallDita:
                    bool firstToday = !State.ditaCalledToday;
                    State.ditaCalledToday = true;
                    extraNotes += firstToday ? " Ngobrol dengan Dita." : " Mampir lagi ke kamar Dita.";
                    break;

                case ActivityType.SitWithoutScreen:
                    State.sitObservationsSeen++;
                    extraNotes += " Sebentar jauh dari layar.";
                    break;

                case ActivityType.TidyDesk:
                    State.deskTidiedToday = true;
                    State.isDeskTidy = true;
                    State.tidyObservationsSeen++;
                    extraNotes += " Meja rapi lagi.";
                    break;

                case ActivityType.FinalPrep:
                    extraNotes += " Tas siap. Tinggal memutuskan.";
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

        public bool ResolveJeda()
        {
            if (State.phase != GamePhase.AwaitingJeda)
                return false;

            State.jedaTriggeredToday = true;
            State.pressure = 4;
            State.energy = Mathf.Max(1, State.energy);
            State.ClampStats();
            State.phase = GamePhase.FreeRoam;

            LastStatusMessage = "Jeda selesai (Duduk sebentar): Tekanan menjadi 4, Tenaga minimal 1 (tanpa memajukan jam).";
            OnJedaResolved?.Invoke(State);

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
        /// Sleep until a chosen time of day:
        /// - UntilNoon (12:00 Siang, available Pagi): +2 Tenaga, -1 Tekanan
        /// - UntilNight (18:00 Malam, available Pagi/Siang): +3 Tenaga, -2 Tekanan (misses 13:00 interview if pending today)
        /// - UntilMorning (08:00 Pagi next day, or FinalDecision on Day 7): +2 Tenaga, -1 Tekanan, resets daily flags & saves DayStart
        /// </summary>
        public SleepExecutionResult SleepUntil(SleepTarget target)
        {
            var preview = ActivitySystem.BuildSleepPreview(State, target);
            if (!preview.IsAvailable)
            {
                LastStatusMessage = preview.UnavailableReason;
                return new SleepExecutionResult { Executed = false, Target = target, Summary = preview.UnavailableReason };
            }

            var res = new SleepExecutionResult { Executed = true, Target = target };
            string extraNotes = "";

            // Day 7 overnight sleep opens FinalDecision directly.
            if (preview.OpensFinalDecision)
            {
                LastSaveResult = Save.SaveAutosave(State);
                State.phase = GamePhase.FinalDecision;
                res.OpenedFinalDecision = true;
                res.WakeCaption = $"Hari {State.day} · Keputusan Akhir";
                res.Summary = $"Hari {State.day} · {State.FormattedTime}. Tentukan keputusan akhir Raka.";
                LastStatusMessage = res.Summary;
                OnSleepResolved?.Invoke(target, res, State);
                OnFinalDecisionOpened?.Invoke(State);
                NotifyStateChanged();
                return res;
            }

            // Handle missed interview if sleeping past today's 13:00 scheduled interview.
            if (preview.WillMissScheduledInterview)
            {
                var missedApp = State.GetApplication(preview.MissedInterviewCompany);
                if (missedApp != null && missedApp.status == ApplicationStatus.Scheduled)
                {
                    missedApp.status = ApplicationStatus.Rejected;
                    missedApp.missedInterview = true;
                    missedApp.rejectionReasons.Clear();
                    missedApp.rejectionReasons.Add("Interview pukul 13:00 terlewat karena tertidur.");
                    State.pressure += 1;
                    res.MissedInterview = true;
                    extraNotes += $" Interview {ActivitySystem.GetCompanyName(preview.MissedInterviewCompany)} terlewat (Tekanan +1).";
                }
            }

            ActivitySystem.GetSleepDeltas(target, out int eDelta, out int pDelta);
            State.energy += eDelta;
            State.pressure += pDelta;

            if (target == SleepTarget.UntilMorning)
            {
                State.day += 1;
                State.currentHour = GameState.StartHour;
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

                res.AdvancedDay = true;
                res.WakeCaption = $"Hari {State.day}  ·  08:00  ·  Pagi";
                string interviewNote = State.HasScheduledInterviewOn(State.day, out CompanyId scheduled)
                    ? $" Interview {ActivitySystem.GetCompanyName(scheduled)} jam 13:00."
                    : "";
                res.Summary = $"Hari {State.day} dimulai.{extraNotes}{interviewNote}";
                LastStatusMessage = res.Summary;

                OnSleepResolved?.Invoke(target, res, State);
                OnDayStarted?.Invoke(State.day, State);
                NotifyStateChanged();
                return res;
            }

            State.currentHour = preview.TargetHour;
            State.ClampStats();
            LastSaveResult = Save.SaveAutosave(State);

            res.WakeCaption = $"Hari {State.day}  ·  {State.FormattedTime}  ·  {State.session}";
            res.Summary = $"Bangun — {State.FormattedTime} ({State.session}).{extraNotes}";
            LastStatusMessage = res.Summary;

            OnSleepResolved?.Invoke(target, res, State);
            NotifyStateChanged();
            return res;
        }

        /// <summary>
        /// Called from the Bed anytime during Malam (currentHour >= 18:00).
        /// Preserves existing Malam-sleep contract for callers & tests.
        /// </summary>
        public bool SleepOrOpenFinalDecision()
        {
            if (State.phase != GamePhase.FreeRoam || !State.CanSleepNow)
            {
                LastStatusMessage = "Belum waktunya tidur (tidur malam tersedia mulai pukul 18:00 · Malam).";
                return false;
            }

            return SleepUntil(SleepTarget.UntilMorning).Executed;
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
