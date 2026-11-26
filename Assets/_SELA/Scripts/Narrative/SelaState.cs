using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SELA.Narrative
{
    /// <summary>
    /// Sumber data yang dibaca seluruh UI. Cerminan variabel state pada GDD 3.1.
    /// Logika sesi (biaya aktivitas, pergantian hari, aturan ending) dimiliki anggota Gameplay;
    /// kelas ini sengaja hanya menyimpan nilai dan memberi tahu perubahan.
    /// </summary>
    [Serializable]
    public class SelaState
    {
        public const int MaxEnergy = 4;
        public const int MaxPressure = 6;

        public int Day = 1;
        public SelaSession Session = SelaSession.Pagi;
        public bool SessionResolved;

        public int Energy = 3;
        public int Pressure = 1;

        public List<EvidenceId> Evidence = new List<EvidenceId> { EvidenceId.E0 };
        public int[] CvClaim = { 0, 0 };
        public List<EvidenceId> CvAttached = new List<EvidenceId>();

        public ApplicationStatus[] Application =
        {
            ApplicationStatus.BelumDikirim, ApplicationStatus.BelumDikirim, ApplicationStatus.BelumDikirim
        };
        public bool[] ApplicationOpened = { false, false, false };
        public bool[] ApplicationCorrected = { false, false, false };
        public int[] ApplicationInterviewScore = { -1, -1, -1 };

        /// <summary>Hari lamaran dikirim per perusahaan, -1 bila belum dikirim. Interview jatuh pada hari berikutnya.</summary>
        public int[] ApplicationSentDay = { -1, -1, -1 };

        /// <summary>Snapshot klaim dan bukti yang ikut saat lamaran dikirim. Format: "0,1" dan "1,2".</summary>
        public string[] ApplicationSentClaims = { string.Empty, string.Empty, string.Empty };
        public string[] ApplicationSentEvidence = { string.Empty, string.Empty, string.Empty };

        /// <summary>Catatan hasil interview per perusahaan, dibaca di laptop dan Catatan.</summary>
        public string[] ApplicationOutcome = { string.Empty, string.Empty, string.Empty };

        public bool[] VerificationDone = { false, false, false };
        public List<VerificationMark> Verification = new List<VerificationMark>();

        public OfferChoice OfferChoice = OfferChoice.TidakAda;
        public string DecisionNote;

        public bool[] RiftSolved = { false, false, false };
        public bool TidiedToday;
        public bool BreakUsedToday;
        public bool[] ActivityDone = new bool[9];

        public List<int> ReadMessages = new List<int>();
        public bool SeenContentInfo;

        /// <summary>Pengenalan sudah pernah dibuka. Disimpan supaya tidak muncul lagi (DESIGN 9.15).</summary>
        public bool TutorialSeen;

        /// <summary>
        /// Sinyal perubahan state. Bersifat statis supaya langganan tetap berlaku ketika
        /// objek state diganti, misalnya saat mulai baru atau memuat simpanan.
        /// </summary>
        public static event Action Changed;

        public SelaState()
        {
            for (int i = 0; i < 12; i++)
            {
                Verification.Add(VerificationMark.Belum);
            }
        }

        public static SelaState Current { get; private set; } = new SelaState();

        public static SelaState Reset()
        {
            Current = new SelaState();
            Current.NotifyChanged();
            return Current;
        }

        /// <summary>Mengganti state aktif dengan hasil bacaan simpanan.</summary>
        public static void Adopt(SelaState state)
        {
            Current = state ?? new SelaState();
            Current.NotifyChanged();
        }

        public void NotifyChanged()
        {
            if (Changed != null)
            {
                Changed();
            }
        }

        public bool HasEvidence(EvidenceId id)
        {
            return Evidence.Contains(id);
        }

        public bool IsAttached(EvidenceId id)
        {
            return CvAttached.Contains(id);
        }

        public void SetEnergy(int value)
        {
            Energy = Mathf.Clamp(value, 0, MaxEnergy);
        }

        public void SetPressure(int value)
        {
            Pressure = Mathf.Clamp(value, 0, MaxPressure);
        }

        public ApplicationStatus StatusOf(OfferId offer)
        {
            return Application[(int)offer];
        }

        public void SetStatus(OfferId offer, ApplicationStatus status)
        {
            Application[(int)offer] = status;
        }

        public VerificationMark MarkOf(OfferId offer, int aspect)
        {
            return Verification[(int)offer * 4 + aspect];
        }

        public void SetMark(OfferId offer, int aspect, VerificationMark mark)
        {
            Verification[(int)offer * 4 + aspect] = mark;
        }

        /// <summary>Bukti terbaik yang dimiliki pemain, dipakai pada epilog pulang.</summary>
        public EvidenceId BestEvidence()
        {
            if (HasEvidence(EvidenceId.E2))
            {
                return EvidenceId.E2;
            }
            if (HasEvidence(EvidenceId.E1))
            {
                return EvidenceId.E1;
            }
            return EvidenceId.E0;
        }

        /// <summary>
        /// Mencatat snapshot lamaran saat dikirim. Klaim dan bukti tersimpan apa adanya,
        /// sehingga bukti yang dibuat belakangan tidak ikut memperbarui lamaran lama (GDD 3.5).
        /// </summary>
        public void RecordSentApplication(OfferId offer, int[] claims, IEnumerable<EvidenceId> evidence)
        {
            int index = (int)offer;
            ApplicationSentClaims[index] = (claims[0]) + "," + (claims[1]);

            List<string> ids = new List<string>();
            foreach (EvidenceId id in evidence)
            {
                ids.Add(((int)id).ToString());
            }
            ApplicationSentEvidence[index] = string.Join(",", ids.ToArray());

            ApplicationSentDay[index] = Day;
            SetStatus(offer, ApplicationStatus.Terjadwal);
        }

        /// <summary>Bukti apakah evidence ikut dalam snapshot lamaran yang diberikan.</summary>
        public bool SentHasEvidence(OfferId offer, EvidenceId id)
        {
            string raw = ApplicationSentEvidence[(int)offer];
            if (string.IsNullOrEmpty(raw))
            {
                return false;
            }

            string[] parts = raw.Split(',');
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == ((int)id).ToString())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>True bila snapshot lamaran memuat klaim yang berlebihan (GDD 3.5).</summary>
        public bool SentHasExcessiveClaim(OfferId offer)
        {
            string raw = ApplicationSentClaims[(int)offer];
            if (string.IsNullOrEmpty(raw))
            {
                return false;
            }

            string[] parts = raw.Split(',');
            for (int line = 0; line < parts.Length && line < 2; line++)
            {
                int option;
                if (int.TryParse(parts[line], out option) && SelaContent.Cv.IsExcessive(line, option))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Perusahaan yang interviewnya jatuh pada hari ini, atau null bila tidak ada undangan
        /// hari ini. Interview selalu pada sesi siang setelah lamaran dikirim (GDD 3.2).
        /// </summary>
        public OfferId? InterviewOfferToday()
        {
            for (int i = 0; i < ApplicationSentDay.Length; i++)
            {
                int sent = ApplicationSentDay[i];
                if (sent > 0 && sent + 1 == Day)
                {
                    return (OfferId)i;
                }
            }
            return null;
        }

        /// <summary>Konteks pesan hari 5-7: dua tawaran, satu tawaran, atau belum ada (GDD 2.3).</summary>
        public OfferContext CurrentOfferContext()
        {
            int official = 0;
            if (StatusOf(OfferId.A) != ApplicationStatus.BelumDikirim)
            {
                official++;
            }
            if (StatusOf(OfferId.B) != ApplicationStatus.BelumDikirim)
            {
                official++;
            }

            if (official >= 2)
            {
                return OfferContext.DuaTawaran;
            }
            if (official == 1)
            {
                return OfferContext.SatuTawaran;
            }
            return OfferContext.BelumAdaTawaran;
        }
    }

    /// <summary>
    /// Penyimpanan sementara state ke berkas JSON. Save/load final adalah bagian anggota Gameplay (GDD 3.13);
    /// implementasi ini hanya supaya tombol Lanjutkan di menu punya sesuatu untuk dibaca.
    /// </summary>
    public static class SelaSaveStore
    {
        private const string FileName = "sela-save.json";

        private static string Path
        {
            get { return System.IO.Path.Combine(Application.persistentDataPath, FileName); }
        }

        public static bool Exists()
        {
            return File.Exists(Path);
        }

        /// <summary>Hasil penulisan terakhir. Panel Jeda memakai ini untuk status "Tersimpan" (DESIGN 9.14).</summary>
        public static bool LastWriteSucceeded { get; private set; }

        public static void Write(SelaState state)
        {
            string json = JsonUtility.ToJson(state);
            string temp = Path + ".tmp";
            try
            {
                File.WriteAllText(temp, json);
                if (File.Exists(Path))
                {
                    File.Delete(Path);
                }
                File.Move(temp, Path);
                LastWriteSucceeded = true;
            }
            catch (Exception)
            {
                LastWriteSucceeded = false;
            }
        }

        public static SelaState Read()
        {
            if (!Exists())
            {
                return null;
            }

            try
            {
                SelaState state = JsonUtility.FromJson<SelaState>(File.ReadAllText(Path));
                if (state == null)
                {
                    return null;
                }
                if (state.Verification == null || state.Verification.Count < 12)
                {
                    state.Verification = new List<VerificationMark>();
                    for (int i = 0; i < 12; i++)
                    {
                        state.Verification.Add(VerificationMark.Belum);
                    }
                }

                // Simpanan lama dibuat saat masih ada dua tawaran; diperpanjang ke tiga agar indeks tidak salah.
                state.Application = Fit(state.Application, 3, ApplicationStatus.BelumDikirim);
                state.ApplicationOpened = Fit(state.ApplicationOpened, 3, false);
                state.ApplicationCorrected = Fit(state.ApplicationCorrected, 3, false);
                state.ApplicationInterviewScore = Fit(state.ApplicationInterviewScore, 3, -1);
                state.ApplicationSentDay = Fit(state.ApplicationSentDay, 3, -1);
                state.ApplicationSentClaims = Fit(state.ApplicationSentClaims, 3, string.Empty);
                state.ApplicationSentEvidence = Fit(state.ApplicationSentEvidence, 3, string.Empty);
                state.ApplicationOutcome = Fit(state.ApplicationOutcome, 3, string.Empty);
                return state;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void Clear()
        {
            if (Exists())
            {
                File.Delete(Path);
            }
        }

        private static ApplicationStatus[] Fit(ApplicationStatus[] source, int length, ApplicationStatus fill)
        {
            ApplicationStatus[] result = new ApplicationStatus[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = source != null && i < source.Length ? source[i] : fill;
            }
            return result;
        }

        private static bool[] Fit(bool[] source, int length, bool fill)
        {
            bool[] result = new bool[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = source != null && i < source.Length ? source[i] : fill;
            }
            return result;
        }

        private static int[] Fit(int[] source, int length, int fill)
        {
            int[] result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = source != null && i < source.Length ? source[i] : fill;
            }
            return result;
        }

        private static string[] Fit(string[] source, int length, string fill)
        {
            string[] result = new string[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = source != null && i < source.Length ? source[i] : fill;
            }
            return result;
        }
    }
}
