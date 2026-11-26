using System.Collections.Generic;

namespace SELA.Narrative
{
    public static partial class SelaContent
    {
        public static partial class Dita
        {
            /// <summary>Percakapan pertama tiap hari, 60-100 kata per hari (GDD 3.15).</summary>
            public static List<string> Conversation(int day, ApplicationStatus statusA, ApplicationStatus statusB)
            {
                var lines = new List<string>();
                switch (day)
                {
                    case 1:
                        lines.Add("Raka: Kalau pulang sekarang, nanti ditanya kerja di mana.");
                        lines.Add("Dita: Jawab saja masih mencari. Ibu nanya jadwal busnya, bukan nanya rapormu.");
                        lines.Add("Dita: Soal tempat tinggal sementara, itu serius. Kamumu kosong sebulan, dan jaraknya ke pusat kota tidak jauh.");
                        lines.Add("Raka: Aku belum tahu mau kerja di kota atau belum.");
                        lines.Add("Dita: Makanya belum harus tahu. Yang penting hari ini kamu isi dengan satu hal yang bisa ditunjukkan.");
                        break;

                    case 2:
                        lines.Add("Dita: Yang bagian formulir itu kamu kerjakan sendiri?");
                        lines.Add("Raka: Formulirnya. Yang server dibantu Arman.");
                        lines.Add("Dita: Ya, tulis begitu. Orangnya berbeda, hasil kerjamu tetap ada.");
                        lines.Add("Raka: Aku kirim tautan blog, tapi rasanya seperti menagih.");
                        lines.Add("Dita: Blog itu untuk orang yang kebetulan membuka. Lamaran butuh yang bisa dibaca sepuluh menit.");
                        lines.Add("Dita: Dan kalau kamumu begini, itu bukan masalah. Yang perlu diperbaiki besok pagi, bukan hari ini.");
                        break;

                    case 3:
                        lines.Add("Dita: Aku tadi langsung bahas lamaran lagi. Kamu sebenarnya mau cerita dulu?");
                        lines.Add("Raka: Iya. Aku sudah baca sarannya, cuma belum sanggup buka laptop lagi.");
                        lines.Add("Dita: Tidak usah buru-buru. Yang perlu kamu tahu cuma satu: dua versi portofolio itu berbeda jauh.");
                        lines.Add("Raka: Yang kedua lebih rapi.");
                        lines.Add("Dita: Yang kedua jujur. Di versi pertama ada kalimat yang bukan kamu yang mengatakannya.");
                        lines.Add("Raka: Itu kelamaan untuk dihapus.");
                        lines.Add("Dita: Kelamaan untuk dihapus, ya. Tidak buru-buru untuk dibaca, juga tidak.");
                        break;

                    case 4:
                        lines.Add("Raka: Ada yang menawarkan kerja hari ini. Gaji besar, proses cepat.");
                        lines.Add("Dita: Yang berisiko itu biasanya bukan yang cepat, melainkan yang tidak mau menjelaskan.");
                        lines.Add("Raka: Mereka minta kirim dokumen asli.");
                        lines.Add("Dita: Untuk apa?");
                        lines.Add("Raka: Tidak dijelaskan. Cuma untuk berkas.");
                        lines.Add("Dita: Ya. Yang belum kamu periksa belum menjadi keputusan, dan jangan jadi alasan berhenti mencari yang lain.");
                        break;

                    case 5:
                        lines.Add("Dita: Besok aku tidak bisa lama. Tapi satu pertanyaan boleh kamu bawa.");
                        lines.Add("Raka: Tanya apa?");
                        lines.Add("Dita: Dari tawaran yang ada, syarat mana yang paling membuat capek tiap hari. Bukan yang paling besar angkanya.");
                        lines.Add("Raka: Aku belum tahu.");
                        lines.Add("Dita: Belum tahu itu jawaban yang jujur. Batas pendaftaran malam ini masih ada, jadi masih sempat diperiksa.");
                        lines.Add("Raka: Aku periksa dulu.");
                        lines.Add("Dita: Bagus. Kalau ada yang perlu ditanyakan, tulis saja. Aku baca besok.");
                        break;

                    case 6:
                        lines.Add("Dita: Interview-nya jam berapa?");
                        lines.Add("Raka: Siang. Aku masih punya sesi pagi.");
                        lines.Add("Dita: Pakai pagi itu untuk memulihkan tenaga, jangan untuk panik.");
                        lines.Add("Raka: Kalau aku telat?");
                        lines.Add("Dita: Itu pilihanmu, dan berarti interview-nya terlewat. Aku lebih lihat kamu jelas soal batasmu daripada tanda tangan pertama.");
                        lines.Add("Raka: Dita, aku agak menyesal tidak bawa barang.");
                        lines.Add("Dita: Bawa satu kalimat saja. Besok pagi Ibu nanya, dan jawabannya belum harus final.");
                        break;

                    case 7:
                        lines.Add("Dita: Jadi, mana?");
                        lines.Add("Raka: Belum. Aku mau lihat lagi semalam.");
                        lines.Add("Dita: Lihat. Tapi bukan untuk mencari alasan menunda, yang tidak ada gunanya.");
                        lines.Add("Raka: Kalau aku pulang?");
                        lines.Add("Dita: Itu bukan pilihan yang gagal. Itu pilihan yang belum selesai. Beda.");
                        lines.Add("Dita: Yang aku khawatirkan, kamu pulang dengan yang bisa ditunjukkan, bukan yang paling membuat lega.");
                        break;
                }

                lines.AddRange(InsertFor(statusA, statusB));
                return lines;
            }

            /// <summary>Sisipan singkat sesuai hasil lamaran, maksimal 25 kata (GDD 3.15).</summary>
            private static List<string> InsertFor(ApplicationStatus statusA, ApplicationStatus statusB)
            {
                var lines = new List<string>();
                if (statusA == ApplicationStatus.Ditolak || statusB == ApplicationStatus.Ditolak)
                {
                    lines.Add("Dita: Yang ditolak tidak berarti kamu yang salah. Banyak yang ditolak karena mereka butuh orang yang sudah siap di tempat lain.");
                }
                if (statusA == ApplicationStatus.Terjadwal || statusB == ApplicationStatus.Terjadwal)
                {
                    lines.Add("Dita: Ada interview yang masuk. Jangan dianggap lulus, tapi juga jangan dianggap gagal. Itu cuma tahap.");
                }
                if (statusA == ApplicationStatus.Ditawari || statusB == ApplicationStatus.Ditawari)
                {
                    lines.Add("Dita: Ada surat masuk. Baca syaratnya satu per satu, jangan cuma gaji.");
                }
                return lines;
            }

            /// <summary>Respons singkat saat mengulangi percakapan hari yang sama, maksimal 20 kata.</summary>
            public static string RepeatResponse()
            {
                return "Dita: Aku mengerti. Kalau mau cerita panjang, aku ada.";
            }

            /// <summary>Pengamatan pemulihan: duduk tanpa layar, tiga teks untuk awal, tengah, dan akhir minggu.</summary>
            public static string SittingObservation(int day)
            {
                if (day <= 2)
                {
                    return "Dari kursi ini jendela terlihat lebih lebar daripada dari meja. Di luar, siluet kota masih sama seperti semalam.";
                }
                if (day <= 4)
                {
                    return "Angin masuk sedikit. Tirai bergerak satu kali lalu berhenti, seperti Embun yang sedang berpikir.";
                }
                return "Langit di luar jendela sudah gelap. Kamar terasa lebih besar sekarang, bukan lebih kecil.";
            }
        }
    }
}
