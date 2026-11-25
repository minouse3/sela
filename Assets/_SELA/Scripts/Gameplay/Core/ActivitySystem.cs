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
        public int EnergyBefore;
        public int EnergyAfter;
        public int PressureBefore;
        public int PressureAfter;
        public SessionType NextSession;
        public bool NextSessionIsNightEnd;
        public bool WillTriggerJeda;
        public ActivityAvailability Availability;
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
                case ActivityType.ImproveUiExample: return "Rapikan contoh antarmuka";
                case ActivityType.CompleteTestNotes: return "Lengkapi catatan pengujian";
                case ActivityType.VerifyCompany: return "Periksa " + GetCompanyName(req.Company);
                case ActivityType.SubmitApplication: return "Kirim lamaran " + GetCompanyName(req.Company);
                case ActivityType.AttendInterview: return "Ikuti interview " + GetCompanyName(req.Company);
                case ActivityType.Rest: return "Beristirahat";
                case ActivityType.CallDita: return "Hubungi Dita";
                case ActivityType.SitWithoutScreen: return "Duduk tanpa layar";
                case ActivityType.TidyDesk: return "Rapikan meja";
                default: return req.Type.ToString();
            }
        }

        public static string GetActivityLocation(ActivityType type)
        {
            switch (type)
            {
                case ActivityType.ImproveUiExample:
                case ActivityType.CompleteTestNotes: return "Meja / Laptop";
                case ActivityType.VerifyCompany:
                case ActivityType.SubmitApplication:
                case ActivityType.AttendInterview: return "Laptop";
                case ActivityType.Rest: return "Kasur";
                case ActivityType.CallDita: return "Ponsel";
                case ActivityType.SitWithoutScreen: return "Kursi jendela";
                case ActivityType.TidyDesk: return "Meja";
                default: return "Kamar 07";
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
                case ActivityType.CallDita: energyDelta = +2; pressureDelta = -2; break;
                case ActivityType.SitWithoutScreen: energyDelta = +2; pressureDelta = -2; break;
                case ActivityType.TidyDesk: energyDelta = +1; pressureDelta = -1; break;
                default: energyDelta = 0; pressureDelta = 0; break;
            }
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
                        unavailableReason = "Lowongan Koperasi Pijar dibuka mulai Hari 3.";
                        return false;
                    }
                    unavailableReason = "";
                    return true;
                case CompanyId.AksesPrima:
                    if (state.day < 4)
                    {
                        unavailableReason = "Pesan Akses Prima belum masuk (hadir mulai Hari 4).";
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
                        unavailableReason = "Jendela lamaran Studio Lajur sudah berakhir (Hari 3 Malam).";
                        return false;
                    }
                    unavailableReason = "";
                    return true;
                case CompanyId.KoperasiPijar:
                    if (state.day < 3)
                    {
                        unavailableReason = "Lamaran Koperasi Pijar dibuka mulai Hari 3 Pagi.";
                        return false;
                    }
                    if (state.day > 5)
                    {
                        unavailableReason = "Jendela lamaran Koperasi Pijar sudah berakhir (Hari 5 Malam).";
                        return false;
                    }
                    unavailableReason = "";
                    return true;
                case CompanyId.AksesPrima:
                    unavailableReason = "Akses Prima merupakan tawaran langsung tanpa lamaran.";
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
                result.Reason = "Selesaikan peristiwa yang sedang berlangsung terlebih dahulu.";
                return result;
            }

            if (state.IsNightFinished)
            {
                result.Reason = "Malam sudah selesai. Gunakan kasur untuk menutup hari.";
                return result;
            }

            GetBaseDeltas(req.Type, out int energyDelta, out _);
            if (energyDelta < 0 && state.energy < -energyDelta)
            {
                result.Reason = "Tenaga tidak cukup (butuh Tenaga 1). Aktivitas pemulihan tetap tersedia.";
                return result;
            }

            switch (req.Type)
            {
                case ActivityType.ImproveUiExample:
                    if (state.hasE1)
                    {
                        result.Reason = "E1 sudah tersedia.";
                        return result;
                    }
                    break;

                case ActivityType.CompleteTestNotes:
                    if (state.hasE2)
                    {
                        result.Reason = "E2 sudah tersedia.";
                        return result;
                    }
                    if (state.day < 3)
                    {
                        result.Reason = "Tersedia mulai Hari 3.";
                        return result;
                    }
                    if (!state.hasE1)
                    {
                        result.Reason = "Selesaikan contoh antarmuka (E1) terlebih dahulu.";
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
                        result.Reason = "Perusahaan sudah diperiksa. Hasil dapat dibaca gratis.";
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
                        result.Reason = "Perusahaan ini tidak menerima lamaran.";
                        return result;
                    }
                    if (app.status != ApplicationStatus.NotSent)
                    {
                        result.Reason = "Lamaran ke " + GetCompanyName(req.Company) + " sudah pernah dikirim.";
                        return result;
                    }
                    int interviewDay = state.day + 1;
                    if (state.HasScheduledInterviewOn(interviewDay, out CompanyId conflictCompany))
                    {
                        result.Reason = $"Hari {interviewDay} Siang sudah dipakai untuk interview {GetCompanyName(conflictCompany)}.";
                        return result;
                    }
                    break;

                case ActivityType.AttendInterview:
                    var intApp = state.GetApplication(req.Company);
                    if (intApp == null || intApp.status != ApplicationStatus.Scheduled)
                    {
                        result.Reason = "Tidak ada undangan interview aktif untuk " + GetCompanyName(req.Company) + ".";
                        return result;
                    }
                    if (state.day != intApp.interviewDay || state.session != SessionType.Siang)
                    {
                        result.Reason = $"Interview terjadwal pada Hari {intApp.interviewDay} · Siang.";
                        return result;
                    }
                    break;

                case ActivityType.TidyDesk:
                    if (state.deskTidiedToday)
                    {
                        result.Reason = "Meja sudah dirapikan hari ini.";
                        return result;
                    }
                    break;
            }

            // Warn if choosing a different activity during a scheduled interview Siang slot.
            if (state.session == SessionType.Siang &&
                state.HasScheduledInterviewOn(state.day, out CompanyId scheduledToday))
            {
                bool attendingThisInterview = req.Type == ActivityType.AttendInterview && req.Company == scheduledToday;
                if (!attendingThisInterview)
                {
                    result.WillMissScheduledInterview = true;
                    result.MissedInterviewCompany = scheduledToday;
                }
            }

            result.IsAvailable = true;
            return result;
        }

        public static ActivityPreview BuildPreview(GameState state, ActivityRequest req)
        {
            var availability = CheckAvailability(state, req);
            GetBaseDeltas(req.Type, out int eDelta, out int pDelta);

            int missPenalty = availability.WillMissScheduledInterview ? 1 : 0;
            int rawEnergy = Mathf.Clamp(state.energy + eDelta, GameState.MinEnergy, GameState.MaxEnergy);
            int rawPressure = Mathf.Clamp(state.pressure + pDelta + missPenalty, GameState.MinPressure, GameState.MaxPressure);

            bool triggersJeda = rawPressure >= GameState.MaxPressure && !state.jedaTriggeredToday;
            int finalEnergy = triggersJeda ? Mathf.Max(1, rawEnergy) : rawEnergy;
            int finalPressure = triggersJeda ? 4 : rawPressure;

            SessionType nextSession = state.session == SessionType.Pagi
                ? SessionType.Siang
                : SessionType.Malam;

            return new ActivityPreview
            {
                Request = req,
                Title = GetActivityTitle(req),
                Location = GetActivityLocation(req.Type),
                EnergyBefore = state.energy,
                EnergyAfter = finalEnergy,
                PressureBefore = state.pressure,
                PressureAfter = finalPressure,
                NextSession = nextSession,
                NextSessionIsNightEnd = state.session == SessionType.Malam,
                WillTriggerJeda = triggersJeda,
                Availability = availability
            };
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
                reasons.Add("Bukti wajib belum ikut dikirim pada snapshot lamaran.");
            if (app.HasUncorrectedExaggeration)
                reasons.Add("Klaim CV yang berlebihan belum diluruskan saat interview.");
            if (q1Score < 1 || q2Score < 1 || (q1Score + q2Score) < 3)
                reasons.Add("Penjelasan jawaban belum cukup menunjukkan bukti dan ruang lingkup kerja.");

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

        public static int GetScheduledRiftFor(GameState state)
        {
            if (state.session != SessionType.Siang) return 0;
            int rift = 0;
            if (state.day == 2) rift = 1;
            else if (state.day == 4) rift = 2;
            else if (state.day == 6) rift = 3;

            if (rift > 0 && !state.IsRiftSolved(rift))
                return rift;
            return 0;
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
    }
}
