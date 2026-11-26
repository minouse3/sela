using SELA.Narrative;
using UnityEngine;

namespace SELA.Gameplay
{
    /// <summary>
    /// Aturan sesi, hari, dan biaya aktivitas (GDD 3.2 dan 3.3).
    /// Panel UI memanggil kelas ini dan tidak menyimpan aturan sendiri, supaya biaya,
    /// ketersediaan, dan perpindahan sesi hanya ada di satu tempat.
    /// </summary>
    public static class SelaSessionFlow
    {
        /// <summary>Aktivitas yang hanya boleh dipakai sekali dalam satu hari.</summary>
        private static bool IsDailyLimited(SelaActivity activity)
        {
            return activity == SelaActivity.RapikanMeja;
        }

        /// <summary>Aktivitas pemulihan selalu boleh dipakai, termasuk saat Tenaga 0 (GDD 3.3).</summary>
        public static bool NeedsEnergy(SelaActivity activity)
        {
            return activity != SelaActivity.Beristirahat &&
                   activity != SelaActivity.HubungiDita &&
                   activity != SelaActivity.DudukTanpaLayar &&
                   activity != SelaActivity.RapikanMeja;
        }

        public static int EnergyDelta(SelaActivity activity)
        {
            switch (activity)
            {
                case SelaActivity.Beristirahat: return 3;
                case SelaActivity.HubungiDita:
                case SelaActivity.DudukTanpaLayar: return 2;
                case SelaActivity.RapikanMeja: return 1;
                default: return -1;
            }
        }

        public static int PressureDelta(SelaActivity activity)
        {
            switch (activity)
            {
                case SelaActivity.Beristirahat:
                case SelaActivity.HubungiDita:
                case SelaActivity.DudukTanpaLayar: return -2;
                case SelaActivity.RapikanMeja: return -1;
                case SelaActivity.PeriksaPerusahaan: return 0;
                case SelaActivity.IkutiInterview: return 2;
                default: return 1;
            }
        }

        public static int PreviewEnergy(SelaState state, SelaActivity activity)
        {
            int next = state.Energy + EnergyDelta(activity);
            return Mathf.Clamp(next, 0, SelaState.MaxEnergy);
        }

        public static int PreviewPressure(SelaState state, SelaActivity activity)
        {
            int next = state.Pressure + PressureDelta(activity);
            return Mathf.Clamp(next, 0, SelaState.MaxPressure);
        }

        /// <summary>
        /// Alasan aktivitas tidak bisa dipakai, atau null bila boleh. Alasan selalu ditulis,
        /// tidak memakai ikon gembok (GDD 3.3 dan DESIGN 9.5).
        /// </summary>
        public static string BlockedReason(SelaState state, SelaActivity activity)
        {
            if (state.SessionResolved)
            {
                return state.Session == SelaSession.Malam
                    ? "Malam sudah selesai. Tombol aktivitas satu sesi tidak aktif."
                    : "Sesi ini sudah dipakai. Tutup panel dan lanjutkan ke sesi berikutnya.";
            }

            if (activity == SelaActivity.IkutiInterview && state.InterviewOfferToday() == null)
            {
                return "Undangan interview belum jatuh pada hari ini.";
            }

            if (activity == SelaActivity.RapikanMeja && state.TidiedToday)
            {
                return "Meja sudah dirapikan hari ini. Tersedia lagi besok.";
            }

            if (activity == SelaActivity.RapikanContohAntarmuka && state.HasEvidence(EvidenceId.E1))
            {
                return "E1 sudah tersedia. Hasilnya bisa dibaca di Catatan.";
            }

            if (activity == SelaActivity.LengkapiCatatanPengujian)
            {
                if (state.HasEvidence(EvidenceId.E2))
                {
                    return "E2 sudah tersedia. Hasilnya bisa dibaca di Catatan.";
                }
                if (state.Day < 3)
                {
                    return "Tersedia mulai hari 3.";
                }
                if (!state.HasEvidence(EvidenceId.E1))
                {
                    return "Selesaikan antarmuka terlebih dahulu.";
                }
            }

            if (NeedsEnergy(activity) && state.Energy < 1)
            {
                return "Tenaga tidak cukup. Aktivitas pemulihan tetap tersedia.";
            }

            return null;
        }

        /// <summary>
        /// Applying biaya dan hasil satu kali. Sesi belum berpindah di sini; perpindahan
        /// terjadi setelah Jeda dan Retakan terjadwal selesai (GDD 3.2).
        /// </summary>
        public static void Resolve(SelaState state, SelaActivity activity)
        {
            state.SetEnergy(PreviewEnergy(state, activity));
            state.SetPressure(PreviewPressure(state, activity));
            state.ActivityDone[(int)activity] = true;
            state.SessionResolved = true;

            if (IsDailyLimited(activity))
            {
                state.TidiedToday = true;
            }
        }

        /// <summary>Memindahkan sesi. Malam tidak berpindah; malam hanya ditutup lewat kasur.</summary>
        public static void AdvanceSession(SelaState state)
        {
            switch (state.Session)
            {
                case SelaSession.Pagi:
                    state.Session = SelaSession.Siang;
                    state.SessionResolved = false;
                    break;
                case SelaSession.Siang:
                    // Undangan interview siang yang tidak dipakai berstatus ditolak (GDD 3.2).
                    MarkMissedInterview(state);
                    state.Session = SelaSession.Malam;
                    state.SessionResolved = false;
                    break;
                default:
                    state.SessionResolved = true;
                    break;
            }
        }

        /// <summary>
        /// Kalau ada undangan interview pada sesi siang hari ini dan interview tidak dilakukan,
        /// lamaran itu ditolak dan Tekanan naik satu. Hasilnya disimpan satu kali (GDD 3.7).
        /// </summary>
        private static void MarkMissedInterview(SelaState state)
        {
            OfferId? invitation = state.InterviewOfferToday();
            if (!invitation.HasValue)
            {
                return;
            }

            int index = (int)invitation.Value;
            if (state.ApplicationInterviewScore[index] != -1)
            {
                return;
            }

            state.ApplicationInterviewScore[index] = -3;
            state.SetStatus(invitation.Value, ApplicationStatus.Ditolak);
            state.ApplicationOutcome[index] = SelaContent.Interviews.MissedInterview();
            state.SetPressure(state.Pressure + 1);
        }

        /// <summary>Jeda otomatis aktif sekali per hari saat Tekanan mencapai 6 (GDD 3.4).</summary>
        public static bool ShouldTriggerBreak(SelaState state)
        {
            return state.Pressure >= SelaState.MaxPressure && !state.BreakUsedToday;
        }

        /// <summary>Retakan terjadi sesudah aktivitas siang pada hari 2, 4, dan 6 (GDD 3.8).</summary>
        public static bool ShouldTriggerRift(SelaState state)
        {
            return state.Session == SelaSession.Siang &&
                   RiftIndexFor(state) > 0 &&
                   !state.RiftSolved[RiftIndexFor(state) - 1];
        }

        /// <summary>Nomor retakan untuk hari ini, atau 0 bila hari ini tidak punya retakan.</summary>
        public static int RiftIndexFor(SelaState state)
        {
            switch (state.Day)
            {
                case 2: return 1;
                case 4: return 2;
                case 6: return 3;
                default: return 0;
            }
        }

        /// <summary>Menutup hari: memulihkan tenaga, mengurangi tekanan, dan memulai hari berikutnya.</summary>
        public static void Sleep(SelaState state)
        {
            state.SetEnergy(Mathf.Min(state.Energy + 2, SelaState.MaxEnergy));
            state.SetPressure(Mathf.Max(state.Pressure - 1, 0));
            state.Day = Mathf.Min(state.Day + 1, 7);
            state.Session = SelaSession.Pagi;
            state.SessionResolved = false;
            state.TidiedToday = false;
            state.BreakUsedToday = false;
        }
    }
}
