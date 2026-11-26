using System.Collections.Generic;

namespace SELA.Narrative
{
    public static partial class SelaContent
    {
        public static partial class Recovery
        {
            /// <summary>Teks singkat untuk aktivitas istirahat, maksimal 20 kata.</summary>
            public static string RestText()
            {
                return "Tidur beberapa saat. Bangun dengan kepala yang sedikit lebih ringan.";
            }

            /// <summary>Dua catatan benda saat merapikan meja, maksimal 25 kata (GDD 3.15).</summary>
            public static string TidyNote(int index)
            {
                if (index == 0)
                {
                    return "Kabel sambungan diberi label: layar. Sekarang tidak ada yang tersambung ke hal yang salah.";
                }
                return "Buku yang paling sering dipinjam dipindah ke rak atas, supaya tidak diambil pada saat gelap.";
            }

            /// <summary>Panel pemulihan menyebut pengalaman yang menyertai angka (GDD 3.3).</summary>
            public static string RecoveryNote(SelaActivity activity)
            {
                switch (activity)
                {
                    case SelaActivity.Beristirahat:
                        return "Beristirahat memulihkan Tenaga paling banyak. Tidak ada sesi yang terpakai untuk hal lain.";
                    case SelaActivity.HubungiDita:
                        return "Dita memberi percakapan pertama hari itu. Membaca pesan masuk saja tidak memberi pemulihan.";
                    case SelaActivity.DudukTanpaLayar:
                        return "Duduk memberi pengamatan singkat. Animasinya bisa dilewati.";
                    case SelaActivity.RapikanMeja:
                        return "Merapikan meja memperlihatkan susunan benda yang berbeda. Tidak memengaruhi penilaian interview.";
                    default:
                        return "Tenaga berkurang satu sesi, tekanan bertambah satu. Tidak ada biaya tersembunyi.";
                }
            }

            /// <summary>Pengulangan memakai respons singkat (GDD 3.3).</summary>
            public static string RepeatNote(SelaActivity activity)
            {
                switch (activity)
                {
                    case SelaActivity.Beristirahat:
                        return "Kamar tetap sama. strconv napasmu lebih pelan.";
                    case SelaActivity.HubungiDita:
                        return "Dita: Sama seperti tadi. Tidak ada yang berubah dari sisiku.";
                    case SelaActivity.DudukTanpaLayar:
                        return "Jendela masih di tempat yang sama.";
                    default:
                        return "Meja sudah rapi. Tidak ada yang bisa dirapikan lagi hari ini.";
                }
            }
        }
    }
}
