using System.Collections.Generic;

namespace SELA.Narrative
{
    public static partial class SelaContent
    {
        public static partial class Cv
        {
            /// <summary>Dua baris klaim, masing-masing dengan dua pilihan (GDD 3.5).</summary>
            public static readonly string[] ClaimLines =
            {
                "Bagian yang dikerjakan",
                "Cara menguji hasil kerja"
            };

            public static string[] ClaimOptions(int line)
            {
                if (line == 0)
                {
                    return new[]
                    {
                        "Membantu pengembangan antarmuka proyek kuliah",
                        "Memimpin sistem produksi perusahaan"
                    };
                }

                return new[]
                {
                    "Menguji formulir secara manual",
                    "Menjamin sistem bebas seluruh kesalahan"
                };
            }

            /// <summary>Pilihan kedua pada kedua baris adalah klaim yang berlebihan (GDD 3.5).</summary>
            public static bool IsExcessive(int line, int option)
            {
                return option == 1;
            }

            public static string UnsupportedNote()
            {
                return "Bukti belum mendukung kalimat ini";
            }

            public static string ExcessWarning()
            {
                return "Klaim ini lebih besar daripada bukti yang kamu kirim." +
                       " Boleh tetap dipakai, tapi akan mendapat catatan saat interview.";
            }

            /// <summary>Peringatan sebelum pengiriman (DESIGN 9.8).</summary>
            public static string IncompleteEvidenceWarning()
            {
                return "Bukti wajib belum ikut dikirim; tawaran tidak dapat diberikan dari snapshot ini." +
                       " Bukti yang dibuat nanti tidak memperbarui lamaran ini.";
            }

            public static string NoEvidenceWarning()
            {
                return "Belum ada bukti yang dipasang. Lamaran tetap boleh dikirim," +
                       " tetapi tawaran tidak dapat diberikan dari snapshot ini.";
            }

            /// <summary>
            /// Bukti mana yang bisa mendukung klaim pada baris tertentu. Baris pertama soal
            /// bagian yang dikerjakan, baris kedua soal cara menguji hasil kerja (GDD 3.5).
            /// </summary>
            public static bool SupportsClaim(int line, EvidenceId id)
            {
                if (line == 0)
                {
                    return id == EvidenceId.E0 || id == EvidenceId.E1;
                }
                return id == EvidenceId.E2;
            }

            /// <summary>
            /// Ringkasan dukungan bukti untuk satu klaim. Klaim yang lebih besar daripada bukti
            /// yang dikirim ditulis apa adanya, tanpa ikon gembok (DESIGN 9.2).
            /// </summary>
            public static string ClaimSupportSummary(int line, int option, List<EvidenceId> attached)
            {
                if (IsExcessive(line, option))
                {
                    return UnsupportedNote();
                }

                List<string> names = new List<string>();
                for (int i = 0; i < attached.Count; i++)
                {
                    if (SupportsClaim(line, attached[i]))
                    {
                        names.Add(Evidence.SlotLabel(attached[i]));
                    }
                }

                if (names.Count == 0)
                {
                    return UnsupportedNote();
                }

                return "Didukung oleh: " + string.Join(", ", names.ToArray());
            }

            /// <summary>Label status bukti pada panel CV.</summary>
            public static string EvidenceSlotState(bool owned, bool attached)
            {
                if (attached)
                {
                    return "Terpasang";
                }
                if (owned)
                {
                    return "Dimiliki, belum dipasang";
                }
                return "Belum ada bukti";
            }
        }
    }
}
