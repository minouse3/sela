using System.Collections.Generic;

namespace SELA.Narrative
{
    public class InterviewQuestion
    {
        public string Question;
        public string[] Responses;

        /// <summary>Nilai internal 2/1/0. Tidak pernah ditampilkan ke pemain (DESIGN 9.10).</summary>
        public int[] Values;

        public InterviewQuestion(string question, string[] responses, int[] values)
        {
            Question = question;
            Responses = responses;
            Values = values;
        }
    }

    public static partial class SelaContent
    {
        public static partial class Interviews
        {
            public static string RecruiterName(OfferId offer)
            {
                return offer == OfferId.A ? "Nara" : "Bagas";
            }

            public static string RecruiterRole(OfferId offer)
            {
                return offer == OfferId.A ? "Studio Lajur" : "Koperasi Pijar";
            }

            /// <summary>Kalimat pembuka interview, maksimal 220 kata per interview (GDD 3.15).</summary>
            public static string Opening(OfferId offer)
            {
                if (offer == OfferId.A)
                {
                    return "Nara: Terima kasih sudah meluangkan waktu." +
                           " Aku Nara dari Studio Lajur." +
                           " Aku baca lamaranmu. Satu bagian yang bikin kami tertarik adalah catatan antarmuka formulirmu." +
                           " Ada dua pertanyaan, dan tidak ada waktu batas." +
                           " Kalau ada yang mau kamu luruskan dari klaim di lamaran, sekarang saja.";
                }

                return "Bagas: Selamat datang. Aku Bagas dari Koperasi Pijar." +
                       " Kami butuh orang yang bisa menjaga formulir tetap jalan, dan catatan pengujianmu membantu kami menilai itu." +
                       " Ada dua pertanyaan saja." +
                       " Kalau ada klaim di lamaran yang menurutmu berlebihan, boleh kamu luruskan sebelum kita lanjut.";
            }

            public static InterviewQuestion[] Questions(OfferId offer)
            {
                if (offer == OfferId.A)
                {
                    return new[]
                    {
                        new InterviewQuestion(
                            "Apa yang diperbaiki dan bagaimana kamu memeriksanya?",
                            new[]
                            {
                                "Saya memperjelas label input dan pesan kesalahannya, lalu saya uji sendiri tiga kasus: kolom kosong, format salah, dan data valid. Hasilnya saya catat di bukti antarmuka.",
                                "Labelnya perlu lebih spesifik supaya orang tahu bagian mana yang salah. Saya sudah mengubahnya, tapi saya belum menguji semua kemungkinan.",
                                "Satu pengiriman berhasil sudah membuktikan pesan kesalahannya benar, jadi saya tidak perlu menguji kasus lain."
                            },
                            new[] { 2, 1, 0 }),
                        new InterviewQuestion(
                            "Bagaimana kamu menangani bagian yang belum dikuasai?",
                            new[]
                            {
                                "Bagian antarmuka saya kerjakan sendiri. Untuk bagian server saya minta pendampingan, dan saya menuliskan batas itu sejak awal.",
                                "Saya minta contoh tugas dulu, baru saya perkirakan kebutuhan belajarnya, supaya tidak menebak-nebak.",
                                "Saya setuju dengan seluruh tugasnya, lalu kita bahas kebutuhan bantuan setelah tenggat."
                            },
                            new[] { 2, 1, 0 })
                    };
                }

                return new[]
                {
                    new InterviewQuestion(
                        "Apa yang dapat disimpulkan dari catatan pengujianmu?",
                        new[]
                        {
                            "Tiga kasus tercatat: kolom kosong, format salah, dan data valid. Dua pertama sudah diperbaiki, yang ketiga berjalan sesuai. Batas cakupannya ada di catatan itu.",
                            "Perbaikannya perlu diulang setelah diubah, jadi saya akan menguji ulang nanti. Saya belum merujuk kasus yang tercatat.",
                            "Percobaan dengan data valid sudah cukup untuk menyatakan formulir siap pada semua kondisi."
                        },
                        new[] { 2, 1, 0 }),
                    new InterviewQuestion(
                        "Bagaimana kamu memulai pekerjaan pemeliharaan?",
                        new[]
                        {
                            "Saya minta daftar prioritas dan ruang lingkup perbaikannya dulu, lalu saya periksa dampaknya sebelum mengubah apa pun.",
                            "Saya minta satu contoh laporan yang paling mendesak, lalu saya tangani itu dahulu.",
                            "Saya mengubah setiap laporan sesuai urutan masuk, tanpa memastikan prioritas dan dampaknya."
                        },
                        new[] { 2, 1, 0 })
                };
            }

            /// <summary>Surat tawaran yang tampil setelah interview berhasil (GDD 3.7).</summary>
            public static string OfferLetter(OfferId offer)
            {
                if (offer == OfferId.A)
                {
                    return "Surat tawaran Studio Lajur" +
                           "\nPosisi: Junior UI Implementer" +
                           "\nGaji: Rp4.200.000 per bulan, jaminan sosial tertulis" +
                           "\nJam kerja: 40 jam seminggu, dua hari istirahat, lembur dengan persetujuan dan pencatatan" +
                           "\nMulai kerja: tanggal 9, satu hari setelah masa sewa berakhir" +
                           "\nPembayaran pertama: dibayarkan bersama gaji bulan pertama" +
                           "\nTugas: mengerjakan antarmuka produk, dengan pendamping yang ditetapkan sejak hari pertama.";
                }

                return "Surat tawaran Koperasi Pijar" +
                       "\nPosisi: Web Support" +
                       "\nGaji: Rp3.800.000 per bulan selama kontrak tiga bulan, jaminan sosial tertulis" +
                       "\nJam kerja: 35 jam seminggu, dua hari istirahat" +
                       "\nMulai kerja: tanggal 9, satu hari setelah masa sewa berakhir" +
                       "\nPembayaran pertama: dibayarkan sebelum masa tinggal sementara berakhir" +
                       "\nTugas: pemeliharaan formulir dengan pendampingan dua bulan pertama." +
                       "\nCatatan: evaluasi menjelang akhir kontrak tidak menjamin perpanjangan.";
            }

            /// <summary>Tiga alasan penolakan berurutan (DESIGN 9.10).</summary>
            public static string[] RejectionReasons()
            {
                return new[]
                {
                    "Bukti wajib belum ikut dikirim, jadi lamaran ini tidak bisa dinilai dari portofolio yang lengkap.",
                    "Ada klaim yang belum diluruskan, dan kami menilai itu karena bagian itu jadi bagian dari penilaian.",
                    "Penjelasan jawabannya belum cukup menunjukkan batas pekerjaan yang kamu kerjakan sendiri."
                };
            }

            /// <summary>Template yang tampil ketika undangan interview terlewat (GDD 3.7).</summary>
            public static string MissedInterview()
            {
                return "Interview terlewat" +
                       "\nUndangan interview hari ini tidak kamu ikuti." +
                       " Lamaran ini tidak diproses lebih lanjut." +
                       " Ini bukan penilaian atas kemampuan kamu; jadwalnya yang tidak kamu pakai." +
                       " Tekanan bertambah satu, dan hasilnya sudah tercatat.";
            }

            /// <summary>CatatanAjakan meluruskan klaim (DESIGN 9.8).</summary>
            public static string CorrectionPrompt()
            {
                return "Ada klaim yang lebih besar daripada bukti yang kamu kirim." +
                       " Kamu boleh meluruskannya sekarang. Catatan ini masuk ke penjelasan interview untuk lamaran ini." +
                       " Klaim asli tetap tersimpan di riwayat, dan bukti yang tidak ikut dikirim tidak bertambah.";
            }
        }
    }
}
