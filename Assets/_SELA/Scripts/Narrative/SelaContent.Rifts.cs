using System.Collections.Generic;

namespace SELA.Narrative
{
    public static partial class SelaContent
    {
        public static partial class Rifts
        {
            public const int Count = 3;

            /// <summary>Pembuka adegan, maksimal 30 kata. Dua varian sesuai riwayat (GDD 3.8).</summary>
            public static string Opening(int rift, bool hasOffer, bool hasE1)
            {
                if (rift == 1)
                {
                    if (hasE1)
                    {
                        return "Meja terasa lebih jauh dari biasanya. Ada lembar penolakan tergantung di antara kamu dan meja.";
                    }
                    return "Meja terasa lebih jauh dari biasanya. Kamu belum sempat menyelesaikan contoh antarmuka.";
                }

                if (rift == 2)
                {
                    if (hasOffer)
                    {
                        return "Di lorong ada tiga pintu: DITERIMA, TERLAMBAT, dan 07. Yang dua membuatmu ragu.";
                    }
                    return "Di lorong ada tiga pintu: DITERIMA, TERLAMBAT, dan 07. Tidak ada yang menyebut yang mana.";
                }

                if (hasOffer)
                {
                    return "Ada kursi tambahan di sekeliling meja, dan semuanya kosong. Tidak ada yang duduk di sana.";
                }
                return "Ada kursi tambahan di sekeliling meja, dan semuanya kosong. Sepertinya menunggu jawaban.";
            }

            /// <summary>Umpan balik setelah benda dipasang di jangkar yang benar.</summary>
            public static string Solved(int rift)
            {
                switch (rift)
                {
                    case 1:
                        return "Kartu itu pas di penjepit sudut foto." +
                               " Ruang kembali pendek, dan tulisan di lembar penolakan berubah" +
                               " menjadi keterangan tentang proyek yang faktual.";
                    case 2:
                        return "Kunci 07 masuk, dan pintu 07 terbuka." +
                               " Dua pintu yang lain berubah menjadi bidang dinding." +
                               " Nomor pada pintu camamu masih terbaca.";
                    default:
                        return "Kartu pesan Ibu menempel di dudukan kursi dekat jendela." +
                               " Kursi tambahan hilang satu per satu." +
                               " Kipas kembali terdengar, dan satu kursi asli tetap ada.";
                }
            }

            /// <summary>Petunjuk bertahap, maksimal 35 kata.</summary>
            public static string Hint(int rift)
            {
                switch (rift)
                {
                    case 1:
                        return "Petunjuk: cari kartu yang ada sejak awal, di dekat rak. Bawa ke ujung meja, di penjepit yang bentuknya seperti sudut foto.";
                    case 2:
                        return "Petunjuk: nomor 07 ada di gantungan kunci di bangku lorong. Bawa kunci itu ke pintu yang nomornya sama.";
                    default:
                        return "Petunjuk: pesan Ibu ada di kartu kertas sejak pagi. Bawa ke kursi dekat jendela, dan cari dudukan yang datar.";
                }
            }

            /// <summary>Respons saat tujuan salah, maksimal 35 kata.</summary>
            public static string WrongTarget(int rift)
            {
                switch (rift)
                {
                    case 1:
                        return "Benda itu tidak punya tempat di sini. Yang membuat ruang kembali normal bukan benda ini, tapi bukti yang pernah kamu kerjakan sendiri.";
                    case 2:
                        return "Kunci ini bukan untuk pintu ini. Yang memisahkan pintu yang benar bukan tulisan di atasnya, tapi nomor pada Kamar kamu.";
                    default:
                        return "Dudukan itu bukan tempat untuk kartu ini. Yang ingin di sini bukan kabar soal pekerjaan, tapi kabar dari rumah.";
                }
            }

            /// <summary>Teks yang tampil saat benda sedang dibawa (DESIGN 9.12).</summary>
            public static string CarryingLabel(string objectName)
            {
                return "Dibawa: " + objectName;
            }

            /// <summary>Nama benda puzzle untuk label bawaan.</summary>
            public static string PuzzleObjectName(int rift)
            {
                switch (rift)
                {
                    case 1: return "Kartu Proyek";
                    case 2: return "Kunci 07";
                    default: return "Kartu Pesan Ibu";
                }
            }
        }
    }
}
