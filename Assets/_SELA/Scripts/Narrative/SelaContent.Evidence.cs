using System.Collections.Generic;

namespace SELA.Narrative
{
    public static partial class SelaContent
    {
        public static partial class Evidence
        {
            /// <summary>Isi kartu bukti, maksimal 80 kata per kartu (GDD 3.15).</summary>
            public static string CardText(EvidenceId id)
            {
                switch (id)
                {
                    case EvidenceId.E0:
                        return "Foto layar formulir yang selesai dikerjakan." +
                               " Di pojok kiri bawah catatan bahwa Raka adalah anggota tim," +
                               " bukan satu orang yang mengerjakan seluruh bagian." +
                               " Bagian yang dikerjakan Raka adalah label input dan pesan kesalahan." +
                               " Hasilnya nyata, dan hasilnya milik tim, bukan milik Raka seorang.";
                    case EvidenceId.E1:
                        return "Dua tangkapan layar formulir, sebelum dan sesudah." +
                               " Yang pertama memakai label Input dan Error," +
                               " sehingga pengguna tidak tahu bagian mana yang salah." +
                               " Yang kedua memakai Alamat email dan Isi alamat email," +
                               " dengan pesan yang menyebut format yang benar." +
                               " Perubahan ini yang membuat formulir bisa diisi tanpa menebak.";
                    default:
                        return "Catatan pengujian atas formulir." +
                               " Tiga kasus tercatat: kolom kosong, format salah, dan data valid." +
                               " Dua kasus pertama ditangani, kasus ketiga berjalan sesuai." +
                               " Batasnya jelas: hanya tiga kasus ini yang diuji," +
                               " dan belum ada pengujian di luar daftar itu.";
                }
            }

            /// <summary>Kartu yang belum diperoleh menampilkan siluet dan cara mendapatkannya (DESIGN 9.8).</summary>
            public static string MissingText(EvidenceId id)
            {
                switch (id)
                {
                    case EvidenceId.E1:
                        return "Belum ada bukti. Rapikan contoh antarmuka di meja atau laptop.";
                    case EvidenceId.E2:
                        return "Belum ada bukti. Lengkapi catatan pengujian, dan selesaikan E1 lebih dulu.";
                    default:
                        return "Belum ada bukti.";
                }
            }

            public static string SlotLabel(EvidenceId id)
            {
                switch (id)
                {
                    case EvidenceId.E0: return "Proyek kuliah";
                    case EvidenceId.E1: return "Antarmuka formulir";
                    default: return "Catatan pengujian";
                }
            }

            /// <summary>Pasangan label dan pesan kesalahan untuk pilihan aktivitas E1.</summary>
            public static readonly string[] E1PairBefore = { "Input", "Error" };
            public static readonly string[] E1PairAfter = { "Alamat email", "Isi alamat email" };

            public static string E1Question()
            {
                return "Pesan mana yang lebih membantu pengguna?";
            }

            public static string E1ChoiceCorrect()
            {
                return "Alamat email dan Isi alamat email";
            }

            public static string E1ChoiceWrong()
            {
                return "Input dan Error";
            }

            public static string E1Feedback(bool correct)
            {
                if (correct)
                {
                    return "Label yang menyebut bagian formulir membuat pengguna tahu bagian mana yang harus diperbaiki." +
                           " Ini bukan soal benar atau salah secara teknis, tapi soal apakah pesan itu bisa dipakai." +
                           " Kartu E1 dicatat sebagai bukti antarmuka formulir.";
                }
                return "Input dan Error masih bisa ditebak, tapi tidak masuk akal bagi pengguna yang baru pertama kali mengisi formulir." +
                       " Coba bandingkan lagi dengan pasangan yang menyebut bagian formulir secara langsung." +
                       " Tidak ada biaya atau stat yang berubah karena pilihan ini.";
            }

            public static string E2Question()
            {
                return "Tiga kasus tercatat. Apa yang bisa disimpulkan?";
            }

            public static string E2ChoiceCorrect()
            {
                return "Tiga kasus yang dicatat berjalan sesuai hasil ini";
            }

            public static string E2ChoiceWrong()
            {
                return "Seluruh kemungkinan kesalahan sudah teruji";
            }

            public static string E2Feedback(bool correct)
            {
                if (correct)
                {
                    return "Kesimpulan ini mengikuti data: tiga kasus tercatat, tiga berjalan." +
                           " Batas cakupannya juga ikut tercatat, dan itu bagian dari hasil kerja yang baik." +
                           " Kartu E2 dicatat sebagai bukti catatan pengujian.";
                }
                return "Tiga kasus yang tercatat tidak mewakili seluruh kemungkinan kesalahan." +
                       " Batas pengujian adalah bagian dari hasil, bukan kelemahan yang disembunyikan." +
                       " Tidak ada biaya atau stat yang berubah karena pilihan ini.";
            }
        }
    }
}
