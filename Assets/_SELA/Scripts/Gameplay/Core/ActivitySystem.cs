using System;
using System.Collections.Generic;
using UnityEngine;

namespace SELA.Core
{
    public struct ActivityRequest
    {
        public ActivityType Type;
        public CompanyId Company;

        public ActivityRequest(ActivityType type, CompanyId company = CompanyId.StudioLajur)
        {
            Type = type;
            Company = company;
        }
    }

    public struct InterviewInput
    {
        public bool CorrectExaggeratedClaims;
        public int Question1ChoiceIndex; // 0 = best (if evidence attached), 1 = mid, 2 = weak
        public int Question2ChoiceIndex; // 0 = best, 1 = mid, 2 = weak
    }

    public struct InterviewOutcome
    {
        public bool Offered;
        public int Q1Score;
        public int Q2Score;
        public string PrimaryReason;
        public List<string> AllReasons;
    }

    public struct ActivityAvailability
    {
        public bool IsAvailable;
        public string Reason;
        public bool WillMissScheduledInterview;
        public CompanyId MissedInterviewCompany;
    }

    public struct ActivityPreview
    {
        public ActivityRequest Request;
        public string Title;
        public string Location;
        public int DurationHours;
        public int StartHour;
        public int EndHour;
        public SessionType NextSession; // Environment Pagi / Siang / Malam after activity
        public bool NextSessionIsNightEnd;
        public int EnergyBefore;
        public int EnergyAfter;
        public int PressureBefore;
        public int PressureAfter;
        public int WillTriggerRift; // 0 = none, 1..3 = Rift index triggered at Tekanan >= 5
        public bool WillTriggerJeda;
        public bool RejectionCouldTriggerRiftOrJeda;
        public ActivityAvailability Availability;
    }

    public struct SleepPreview
    {
        public SleepTarget Target;
        public string Label;
        public bool IsAvailable;
        public string UnavailableReason;
        public int TargetDay;
        public int TargetHour;
        public SessionType TargetSession;
        public bool OpensFinalDecision;
        public bool WillMissScheduledInterview;
        public CompanyId MissedInterviewCompany;
        public int EnergyBefore;
        public int EnergyAfter;
        public int PressureBefore;
        public int PressureAfter;
    }

    public static class ActivitySystem
    {
        public static string GetCompanyName(CompanyId company)
        {
            switch (company)
            {
                case CompanyId.StudioLajur: return "Studio Lajur";
                case CompanyId.KoperasiPijar: return "Koperasi Pijar";
                case CompanyId.AksesPrima: return "Akses Prima";
                default: return company.ToString();
            }
        }

        public static string GetActivityTitle(ActivityRequest req)
        {
            switch (req.Type)
            {
                case ActivityType.ImproveUiExample: return "Rapikan bukti E1";
                case ActivityType.CompleteTestNotes: return "Lengkapi catatan E2";
                case ActivityType.VerifyCompany: return "Cek " + GetCompanyName(req.Company);
                case ActivityType.SubmitApplication: return "Lamar " + GetCompanyName(req.Company);
                case ActivityType.AttendInterview: return "Interview " + GetCompanyName(req.Company);
                case ActivityType.Rest: return "Istirahat";
                case ActivityType.Shower: return "Mandi";
                case ActivityType.CallDita: return "Temui Dita";
                case ActivityType.SitWithoutScreen: return "Duduk di jendela";
                case ActivityType.TidyDesk: return "Rapikan meja";
                case ActivityType.FinalPrep: return "Kemas barang & putuskan";
                default: return req.Type.ToString();
            }
        }

        public static string GetActivityLocation(ActivityType type)
        {
            switch (type)
            {
                case ActivityType.ImproveUiExample:
                case ActivityType.CompleteTestNotes: return "Papan Bukti";
                case ActivityType.VerifyCompany:
                case ActivityType.SubmitApplication:
                case ActivityType.AttendInterview: return "Laptop";
                case ActivityType.Rest: return "Kasur";
                case ActivityType.Shower: return "Kamar Mandi";
                case ActivityType.CallDita: return "Kamar Dita (06)";
                case ActivityType.SitWithoutScreen: return "Kursi jendela";
                case ActivityType.TidyDesk: return "Meja";
                case ActivityType.FinalPrep: return "Tas / Map Tawaran";
                default: return "Kamar 07";
            }
        }

        public static int GetDurationHours(ActivityType type)
        {
            switch (type)
            {
                case ActivityType.ImproveUiExample: return 3;
                case ActivityType.CompleteTestNotes: return 3;
                case ActivityType.VerifyCompany: return 2;
                case ActivityType.SubmitApplication: return 2;
                case ActivityType.AttendInterview: return 2;
                case ActivityType.Rest: return 2;
                case ActivityType.Shower: return 1;
                case ActivityType.CallDita: return 2;
                case ActivityType.SitWithoutScreen: return 1;
                case ActivityType.TidyDesk: return 1;
                case ActivityType.FinalPrep: return 2;
                default: return 1;
            }
        }

        public static void GetBaseDeltas(ActivityType type, out int energyDelta, out int pressureDelta)
        {
            switch (type)
            {
                case ActivityType.ImproveUiExample: energyDelta = -1; pressureDelta = +1; break;
                case ActivityType.CompleteTestNotes: energyDelta = -1; pressureDelta = +1; break;
                case ActivityType.VerifyCompany: energyDelta = -1; pressureDelta = 0; break;
                case ActivityType.SubmitApplication: energyDelta = -1; pressureDelta = +1; break;
                case ActivityType.AttendInterview: energyDelta = -1; pressureDelta = +2; break;
                case ActivityType.Rest: energyDelta = +3; pressureDelta = -2; break;
                case ActivityType.Shower: energyDelta = +2; pressureDelta = -3; break;
                case ActivityType.CallDita: energyDelta = +2; pressureDelta = -2; break;
                case ActivityType.SitWithoutScreen: energyDelta = +2; pressureDelta = -2; break;
                case ActivityType.TidyDesk: energyDelta = +1; pressureDelta = -1; break;
                case ActivityType.FinalPrep: energyDelta = 0; pressureDelta = 0; break;
                default: energyDelta = 0; pressureDelta = 0; break;
            }
        }

        public static int ComputeStartHour(GameState state, ActivityRequest req)
        {
            if (req.Type == ActivityType.AttendInterview)
                return Mathf.Max(state.currentHour, GameState.InterviewHour);
            return state.currentHour;
        }

        public static int ComputeEndHour(GameState state, ActivityRequest req)
        {
            int start = ComputeStartHour(state, req);
            int end = start + GetDurationHours(req.Type);
            if (req.Type == ActivityType.FinalPrep)
                return Mathf.Min(GameState.LateNightHour, end);
            return end;
        }

        public static bool IsCompanyOpenForVerification(GameState state, CompanyId company, out string unavailableReason)
        {
            switch (company)
            {
                case CompanyId.StudioLajur:
                    unavailableReason = "";
                    return true;
                case CompanyId.KoperasiPijar:
                    if (state.day < 3)
                    {
                        unavailableReason = "Buka mulai Hari 3.";
                        return false;
                    }
                    unavailableReason = "";
                    return true;
                case CompanyId.AksesPrima:
                    if (state.day < 4)
                    {
                        unavailableReason = "Masuk mulai Hari 4.";
                        return false;
                    }
                    unavailableReason = "";
                    return true;
                default:
                    unavailableReason = "Perusahaan tidak dikenal.";
                    return false;
            }
        }

        public static bool IsApplicationWindowOpen(GameState state, CompanyId company, out string unavailableReason)
        {
            switch (company)
            {
                case CompanyId.StudioLajur:
                    if (state.day > 3)
                    {
                        unavailableReason = "Tutup di akhir Hari 3.";
                        return false;
                    }
                    unavailableReason = "";
                    return true;
                case CompanyId.KoperasiPijar:
                    if (state.day < 3)
                    {
                        unavailableReason = "Buka mulai Hari 3.";
                        return false;
                    }
                    if (state.day > 5)
                    {
                        unavailableReason = "Tutup di akhir Hari 5.";
                        return false;
                    }
                    unavailableReason = "";
                    return true;
                case CompanyId.AksesPrima:
                    unavailableReason = "Tawaran langsung, tanpa lamaran.";
                    return false;
                default:
                    unavailableReason = "Perusahaan tidak dikenal.";
                    return false;
            }
        }

        public static ActivityAvailability CheckAvailability(GameState state, ActivityRequest req)
        {
            var result = new ActivityAvailability { IsAvailable = false, Reason = "" };

            if (state.phase != GamePhase.FreeRoam)
            {
                result.Reason = "Selesaikan momen ini dulu.";
                return result;
            }

            if (req.Type != ActivityType.FinalPrep)
            {
                if (state.IsLateNight)
                {
                    result.Reason = "Sudah jam 22:00. Tidur dulu.";
                    return result;
                }

                int endHour = ComputeEndHour(state, req);
                if (endHour > GameState.LateNightHour)
                {
                    int remaining = GameState.LateNightHour - state.currentHour;
                    result.Reason = $"Sisa waktu cuma {remaining} jam sebelum 22:00.";
                    return result;
                }
            }

            GetBaseDeltas(req.Type, out int energyDelta, out _);
            if (energyDelta < 0 && state.energy < -energyDelta)
            {
                result.Reason = "Tenaga habis. Istirahat dulu.";
                return result;
            }

            switch (req.Type)
            {
                case ActivityType.ImproveUiExample:
                    if (state.hasE1)
                    {
                        result.Reason = "E1 sudah beres.";
                        return result;
                    }
                    break;

                case ActivityType.CompleteTestNotes:
                    if (state.hasE2)
                    {
                        result.Reason = "E2 sudah beres.";
                        return result;
                    }
                    if (state.day < 3)
                    {
                        result.Reason = "Buka mulai Hari 3.";
                        return result;
                    }
                    if (!state.hasE1)
                    {
                        result.Reason = "Bereskan E1 dulu.";
                        return result;
                    }
                    break;

                case ActivityType.VerifyCompany:
                    if (!IsCompanyOpenForVerification(state, req.Company, out string openReason))
                    {
                        result.Reason = openReason;
                        return result;
                    }
                    var ver = state.GetVerification(req.Company);
                    if (ver != null && ver.isVerified)
                    {
                        result.Reason = "Sudah dicek.";
                        return result;
                    }
                    break;

                case ActivityType.SubmitApplication:
                    if (!IsApplicationWindowOpen(state, req.Company, out string windowReason))
                    {
                        result.Reason = windowReason;
                        return result;
                    }
                    var app = state.GetApplication(req.Company);
                    if (app == null)
                    {
                        result.Reason = "Tidak menerima lamaran.";
                        return result;
                    }
                    if (app.status != ApplicationStatus.NotSent)
                    {
                        result.Reason = "Lamaran sudah terkirim.";
                        return result;
                    }
                    int interviewDay = state.day + 1;
                    if (state.HasScheduledInterviewOn(interviewDay, out CompanyId conflictCompany))
                    {
                        result.Reason = $"Bentrok dengan interview {GetCompanyName(conflictCompany)} besok jam 13:00.";
                        return result;
                    }
                    break;

                case ActivityType.AttendInterview:
                    var intApp = state.GetApplication(req.Company);
                    if (intApp == null || intApp.status != ApplicationStatus.Scheduled)
                    {
                        result.Reason = "Belum ada jadwal interview.";
                        return result;
                    }
                    if (state.day != intApp.interviewDay)
                    {
                        result.Reason = $"Jadwalnya Hari {intApp.interviewDay}, jam 13:00.";
                        return result;
                    }
                    if (state.currentHour > GameState.InterviewHour)
                    {
                        result.Reason = "Jam 13:00 sudah lewat.";
                        return result;
                    }
                    break;

                case ActivityType.Rest:
                    if (state.CanSleepNow)
                    {
                        result.Reason = "Sudah malam. Tidur saja sampai pagi.";
                        return result;
                    }
                    break;

                case ActivityType.Shower:
                    if (state.showeredToday)
                    {
                        result.Reason = "Sudah mandi hari ini.";
                        return result;
                    }
                    break;

                case ActivityType.TidyDesk:
                    if (state.deskTidiedToday)
                    {
                        result.Reason = "Meja sudah rapi hari ini.";
                        return result;
                    }
                    break;

                case ActivityType.FinalPrep:
                    if (state.day < GameState.EarlyFinalizationMinDay)
                    {
                        result.Reason = $"Buka mulai Hari {GameState.EarlyFinalizationMinDay}.";
                        return result;
                    }
                    break;
            }

            // Warn if choosing another activity whose end time runs past today's 13:00 interview.
            if (state.HasScheduledInterviewOn(state.day, out CompanyId scheduledToday))
            {
                bool attendingThisInterview = req.Type == ActivityType.AttendInterview && req.Company == scheduledToday;
                int projectedEnd = ComputeEndHour(state, req);
                if (!attendingThisInterview && projectedEnd > GameState.InterviewHour)
                {
                    result.WillMissScheduledInterview = true;
                    result.MissedInterviewCompany = scheduledToday;
                }
            }

            result.IsAvailable = true;
            return result;
        }

        /// <summary>
        /// Option B (Pure Pressure-Driven Rifts):
        /// Triggers the next unsolved unlocked Rift when Tekanan >= 5 (max 1 Rift per day).
        /// Rift 1: Day 1+, Rift 2: Day 3+, Rift 3: Day 5+.
        /// </summary>
        public static int GetEligiblePressureRift(GameState state, int pressureValue)
        {
            if (pressureValue < GameState.RiftPressureThreshold || state.riftTriggeredToday)
                return 0;

            if (!state.IsRiftSolved(1) && state.day >= 1) return 1;
            if (!state.IsRiftSolved(2) && state.day >= 3) return 2;
            if (!state.IsRiftSolved(3) && state.day >= 5) return 3;
            return 0;
        }

        public static ActivityPreview BuildPreview(GameState state, ActivityRequest req)
        {
            var availability = CheckAvailability(state, req);
            GetBaseDeltas(req.Type, out int eDelta, out int pDelta);

            int startHour = ComputeStartHour(state, req);
            int endHour = Mathf.Min(GameState.LateNightHour, ComputeEndHour(state, req));
            int missPenalty = availability.WillMissScheduledInterview ? 1 : 0;

            int rawEnergy = Mathf.Clamp(state.energy + eDelta, GameState.MinEnergy, GameState.MaxEnergy);
            int rawPressure = Mathf.Clamp(state.pressure + pDelta + missPenalty, GameState.MinPressure, GameState.MaxPressure);

            int willTriggerRift = req.Type == ActivityType.FinalPrep
                ? 0
                : GetEligiblePressureRift(state, rawPressure);

            bool triggersJeda = req.Type != ActivityType.FinalPrep &&
                                willTriggerRift == 0 &&
                                rawPressure >= GameState.MaxPressure &&
                                !state.jedaTriggeredToday;

            int finalEnergy = triggersJeda ? Mathf.Max(1, rawEnergy) : rawEnergy;
            int finalPressure = triggersJeda ? 4 : rawPressure;

            bool rejectionCouldTrigger = false;
            if (req.Type == ActivityType.AttendInterview && willTriggerRift == 0 && !triggersJeda)
            {
                int rejPressure = Mathf.Clamp(rawPressure + 1, GameState.MinPressure, GameState.MaxPressure);
                rejectionCouldTrigger = GetEligiblePressureRift(state, rejPressure) > 0 ||
                                        (rejPressure >= GameState.MaxPressure && !state.jedaTriggeredToday);
            }

            return new ActivityPreview
            {
                Request = req,
                Title = GetActivityTitle(req),
                Location = GetActivityLocation(req.Type),
                DurationHours = GetDurationHours(req.Type),
                StartHour = startHour,
                EndHour = endHour,
                NextSession = GameState.GetTimeOfDayForHour(endHour),
                NextSessionIsNightEnd = endHour >= GameState.LateNightHour,
                EnergyBefore = state.energy,
                EnergyAfter = finalEnergy,
                PressureBefore = state.pressure,
                PressureAfter = finalPressure,
                WillTriggerRift = willTriggerRift,
                WillTriggerJeda = triggersJeda,
                RejectionCouldTriggerRiftOrJeda = rejectionCouldTrigger,
                Availability = availability
            };
        }

        public static void GetSleepDeltas(SleepTarget target, out int energyDelta, out int pressureDelta)
        {
            switch (target)
            {
                case SleepTarget.UntilNoon:
                    energyDelta = +2;
                    pressureDelta = -1;
                    break;
                case SleepTarget.UntilNight:
                    energyDelta = +3;
                    pressureDelta = -2;
                    break;
                case SleepTarget.UntilMorning:
                default:
                    energyDelta = +2;
                    pressureDelta = -1;
                    break;
            }
        }

        public static SleepPreview BuildSleepPreview(GameState state, SleepTarget target)
        {
            var preview = new SleepPreview
            {
                Target = target,
                IsAvailable = false,
                EnergyBefore = state.energy,
                PressureBefore = state.pressure
            };

            if (state.phase != GamePhase.FreeRoam)
            {
                preview.UnavailableReason = "Selesaikan momen ini dulu.";
                return preview;
            }

            GetSleepDeltas(target, out int eDelta, out int pDelta);

            switch (target)
            {
                case SleepTarget.UntilNoon:
                    preview.Label = "Tidur sampai Siang · 12:00";
                    preview.TargetDay = state.day;
                    preview.TargetHour = GameState.SiangStartHour;
                    preview.TargetSession = SessionType.Siang;
                    if (state.currentHour >= GameState.SiangStartHour)
                    {
                        preview.UnavailableReason = "Sudah lewat jam 12:00.";
                        return preview;
                    }
                    preview.IsAvailable = true;
                    break;

                case SleepTarget.UntilNight:
                    preview.Label = "Tidur sampai Malam · 18:00";
                    preview.TargetDay = state.day;
                    preview.TargetHour = GameState.MalamStartHour;
                    preview.TargetSession = SessionType.Malam;
                    if (state.currentHour >= GameState.MalamStartHour)
                    {
                        preview.UnavailableReason = "Sudah malam.";
                        return preview;
                    }
                    if (state.HasScheduledInterviewOn(state.day, out CompanyId missedNoon) &&
                        state.currentHour <= GameState.InterviewHour)
                    {
                        preview.WillMissScheduledInterview = true;
                        preview.MissedInterviewCompany = missedNoon;
                    }
                    preview.IsAvailable = true;
                    break;

                case SleepTarget.UntilMorning:
                    if (state.day >= GameState.MaxDay)
                    {
                        preview.Label = "Tutup Hari 7 → Keputusan Akhir";
                        preview.TargetDay = state.day;
                        preview.TargetHour = state.currentHour;
                        preview.TargetSession = state.session;
                        preview.OpensFinalDecision = true;
                        preview.EnergyAfter = state.energy;
                        preview.PressureAfter = state.pressure;
                        preview.IsAvailable = true;
                        return preview;
                    }
                    preview.Label = $"Tidur sampai Pagi · Hari {state.day + 1}";
                    preview.TargetDay = state.day + 1;
                    preview.TargetHour = GameState.StartHour;
                    preview.TargetSession = SessionType.Pagi;
                    if (state.HasScheduledInterviewOn(state.day, out CompanyId missedToday) &&
                        state.currentHour <= GameState.InterviewHour)
                    {
                        preview.WillMissScheduledInterview = true;
                        preview.MissedInterviewCompany = missedToday;
                    }
                    preview.IsAvailable = true;
                    break;
            }

            int missPenalty = preview.WillMissScheduledInterview ? 1 : 0;
            preview.EnergyAfter = Mathf.Clamp(state.energy + eDelta, GameState.MinEnergy, GameState.MaxEnergy);
            preview.PressureAfter = Mathf.Clamp(state.pressure + pDelta + missPenalty, GameState.MinPressure, GameState.MaxPressure);
            return preview;
        }

        public static InterviewOutcome EvaluateInterview(ApplicationRecord app, InterviewInput input)
        {
            if (input.CorrectExaggeratedClaims && app.SnapshotHasExaggeration)
                app.claimsCorrectedInInterview = true;

            bool hasMandatoryEvidence = app.company == CompanyId.StudioLajur
                ? app.snapshotHasE1
                : (app.snapshotHasE1 && app.snapshotHasE2);

            bool hasQ1Evidence = app.company == CompanyId.StudioLajur
                ? app.snapshotHasE1
                : app.snapshotHasE2;

            int q1Score = input.Question1ChoiceIndex == 0
                ? (hasQ1Evidence ? 2 : 0)
                : (input.Question1ChoiceIndex == 1 ? 1 : 0);

            int q2Score = input.Question2ChoiceIndex == 0
                ? 2
                : (input.Question2ChoiceIndex == 1 ? 1 : 0);

            app.q1Score = q1Score;
            app.q2Score = q2Score;

            var reasons = new List<string>();
            if (!hasMandatoryEvidence)
                reasons.Add("Bukti wajib belum ikut terlampir.");
            if (app.HasUncorrectedExaggeration)
                reasons.Add("Klaim berlebihan di CV belum diluruskan.");
            if (q1Score < 1 || q2Score < 1 || (q1Score + q2Score) < 3)
                reasons.Add("Jawaban belum cukup meyakinkan.");

            bool offered = reasons.Count == 0;
            app.status = offered ? ApplicationStatus.Offered : ApplicationStatus.Rejected;
            app.rejectionReasons = reasons;

            return new InterviewOutcome
            {
                Offered = offered,
                Q1Score = q1Score,
                Q2Score = q2Score,
                PrimaryReason = reasons.Count > 0 ? reasons[0] : "",
                AllReasons = reasons
            };
        }

        public static bool IsDecisionNoteValidForChoice(OfferChoice choice, DecisionNoteType note)
        {
            switch (choice)
            {
                case OfferChoice.StudioLajur:
                case OfferChoice.KoperasiPijar:
                    return note == DecisionNoteType.WorkMentoringAndScope ||
                           note == DecisionNoteType.WorkOvertimeApproval;
                case OfferChoice.None:
                    return note == DecisionNoteType.HomeOrganizeEvidence ||
                           note == DecisionNoteType.HomeLimitedSchedule;
                case OfferChoice.AksesPrima:
                    return note == DecisionNoteType.RiskyOfferAcknowledged;
                default:
                    return false;
            }
        }

        public static EndingType ResolveEnding(GameState state)
        {
            switch (state.offerChoice)
            {
                case OfferChoice.StudioLajur:
                    return state.HasOffer(CompanyId.StudioLajur)
                        ? EndingType.MulaiDenganBatas
                        : EndingType.PulangBukanSelesai;
                case OfferChoice.KoperasiPijar:
                    return state.HasOffer(CompanyId.KoperasiPijar)
                        ? EndingType.MulaiDenganBatas
                        : EndingType.PulangBukanSelesai;
                case OfferChoice.AksesPrima:
                    return state.HasOffer(CompanyId.AksesPrima)
                        ? EndingType.KontrakYangMengunci
                        : EndingType.PulangBukanSelesai;
                default:
                    return EndingType.PulangBukanSelesai;
            }
        }

        public static string GetObjectiveHint(GameState state)
        {
            if (state == null) return "";

            switch (state.phase)
            {
                case GamePhase.InRift:
                    return $"Retakan {state.activeRiftIndex} — temukan benda jangkar di kamar.";
                case GamePhase.AwaitingJeda:
                    return "Tekanan memuncak — ambil jeda sebentar.";
                case GamePhase.FinalDecision:
                    return "Tentukan tawaran atau pulang, lalu kunci keputusan.";
                case GamePhase.Epilogue:
                    return "Cerita selesai.";
            }

            if (state.HasScheduledInterviewOn(state.day, out CompanyId scheduled) &&
                state.currentHour <= GameState.InterviewHour)
            {
                string company = GetCompanyName(scheduled);
                return state.energy < 1
                    ? $"Isi tenaga dulu sebelum interview {company} jam 13:00."
                    : $"Interview {company} hari ini jam 13:00.";
            }

            if (state.day >= GameState.MaxDay)
                return "Hari terakhir. Kemas barang saat kamu siap memutuskan.";

            if (state.IsLateNight)
                return "Sudah 22:00. Waktunya tidur.";

            if (state.pressure >= 4)
                return "Kepala mulai penuh. Mandi atau istirahat dulu.";

            if (state.CanSleepNow)
                return "Malam tiba. Lanjut sebentar atau langsung tidur.";

            if (!state.hasE1)
                return "Rapikan bukti E1 di papan, atau cek Studio Lajur di laptop.";

            if (state.day >= GameState.EarlyFinalizationMinDay)
                return "Cek tawaran di laptop, atau mulai kemas barang.";

            return "Atur waktu antara bukti, lamaran, dan istirahat.";
        }
    }
}
