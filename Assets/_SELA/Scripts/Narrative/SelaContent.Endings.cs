using System.Collections.Generic;

namespace SELA.Narrative
{
    public static partial class SelaContent
    {
        public static partial class Endings
        {
            /// <summary>Empat teks epilog, 120-180 kata per teks (GDD 3.15).</summary>
            public static string Epilog(OfferChoice choice, EvidenceId bestEvidence)
            {
                if (choice == OfferChoice.A)
                {
                    return "Raka mengonfirmasi tanggal mulai dan batas tugasnya dalam satu pesan." +
                           " Dia menulis bahwa bagian antarmuka menjadi tanggung jawabnya," +
                           " dan bagian server akan dikerjakan dengan pendampingan." +
                           " Pesan itu dikirim sebelum dia memberitahu Ibu tanggal keberangkatannya," +
                           " karena seperti kata Dita, lebih jelas dari awal siapa yang bantu." +
                           " Hari kedelapan, bus berangkat tanpa Raka di dalamnya." +
                           " Di kos, kamar 07 dikosongkan, dan menyisakan" +
                           " satu ruang yang tidak lagi terlihat panjang." +
                           " Yang dibawa pulang bukan portofolio sempurna," +
                           " melainkan daftar kerja yang bisa ditunjuk, dan satu batas yang bisa diucapkan." +
                           " Pekerjaan berikutnya belum jelas." +
                           " Yang jelas adalah siapa yang akan membantu, dan bagian mana yang menjadi tanggung jawabnya sendiri.";
                }

                if (choice == OfferChoice.B)
                {
                    return "Raka mengonfirmasi jadwal kerja dan batas tugasnya." +
                           " Dalam pesannya ia menyebutkan pendampingan dua bulan pertama," +
                           " dan bahwa tugas awalnya dibatasi pada formulir." +
                           " Dia tidak menulis janji apa pun tentang bulan ketiga." +
                           " Bus pagi tanggal kedelapan berangkat tanpa dia," +
                           " dan ia menyimpan kartu proyek di dalam tas, bukan meninggalkannya di meja." +
                           " Kontrak tiga bulan itu lebih pendek daripada yang lain," +
                           " dan ketidakpastian itu yang membuatnya bisa diterima." +
                           " Di kos, nomor 07 tetap di pintu," +
                           " dan kalender masih menunjukkan akhir masa sewa yang sudah lewat." +
                           " Yang belum terjawab: apa yang terjadi setelah kontrak ini selesai." +
                           " Yang sudah terjawab: ia memutuskan dengan syarat yang ia baca," +
                           " bukan dengan suara paling keras di kepalanya.";
                }

                if (choice == OfferChoice.C)
                {
                    return "Raka menyetujui syarat yang menyerahkan kendali dokumennya." +
                           " Admin memberi tahu dia harus datang hari Senin dengan berkas asli," +
                           " dan jadwal berikutnya mengikuti kebutuhan perusahaan." +
                           " Pesan yang dibalas Raka hanya berisi satu baris," +
                           " dan setelah itu nomor itu tidak lagi dibalas." +
                           " Penyesalan datang dua hari kemudian, seperti biasanya," +
                           " dan tidak ada yang berubah karena dia menyesal." +
                           " Yang tersisa adalah pengakuan bahwa dia tahu syaratnya," +
                           " dan tetap menandatanganinya." +
                           " Ini bukan hadiah dan bukan hukuman." +
                           " Yang ditunjukkan: sebuah keputusan bisa dibuat dengan mata terbuka," +
                           " dan tetap menyakitkan." +
                           " Halaman bantuan menyebut orang tepercaya, konselor kampus," +
                           " dan layanan kesehatan setempat.";
                }

                return "Raka naik bus pagi tanggal kedelapan." +
                       " Tiket yang dibeli Ibu tetap terpakai," +
                       " dan di tas ada kartu proyek yang disusun ulang," +
                       " bukan lembar yang dibuang." +
                       " Ia membawa pulang bukti yang benar-benar sudah selesai." +
                       " Di kos, kamar 07 disegel, dan papan bukti dikosongkan." +
                       " Sebelum berangkat ia menuliskan rencana:" +
                       " bukti yang sudah ada dirapikan, dan jadwal lamaran berikutnya dibatasi." +
                       " Bukan langkah yang besar, dan belum selesai sebelum bus sampai." +
                       " Tapi itu langkah yang bisa dimulai minggu depan," +
                       " dan itu yang membuat pulang bukan berarti berhenti." +
                       " Raka mengirim satu pesan kepada Dita sebelum naik bus." +
                       " Isinya pendek, dan tidak meminta apa pun." +
                       " Yang belum selesai memang belum selesai." +
                       " Yang sudah selesai: ia tahu belum apa yang sedang ia kerjakan," +
                       " dan mengapa ia memilih menunggu.";
            }

            /// <summary>Sisipan progres bukti pada epilog pulang, maksimal 30 kata (GDD 3.15).</summary>
            public static string ProgressInsert(EvidenceId bestEvidence)
            {
                if (bestEvidence == EvidenceId.E0)
                {
                    return "Yang dibawa pulang baru kartu proyek. Langkah berikutnya: menyelesaikan contoh antarmuka.";
                }
                if (bestEvidence == EvidenceId.E1)
                {
                    return "Yang dibawa pulang sudah termasuk hasil antarmuka. Langkah berikutnya: merapikan bukti dan menjadwalkan lamaran berikutnya.";
                }
                return "Yang dibawa pulang sudah termasuk hasil antarmuka dan catatan pengujian. Langkah berikutnya: merapikan bukti dan menjadwalkan lamaran berikutnya.";
            }

            /// <summary>Halaman refleksi dan bantuan (GDD 3.12).</summary>
            public static string Reflection()
            {
                return "SELA bukan diagnosis dan bukan nasihat hukum." +
                       " Cerita ini menggambarkan tekanan yang sering muncul saat transisi kuliah ke dunia kerja:" +
                       " penolakan yang menumpuk, informasi lowongan yang tidak jelas," +
                       " dan rasa bersalah karena masih menerima bantuan keluarga." +
                       " Tekanan dalam game ini adalah kebutuhan permainan, bukan ukuran kewarasan." +
                       " Kalau sedang menghadapi hal serupa, langkah yang paling berguna sering bukan solusi," +
                       " tapi mencari satu orang yang bisa didengar." +
                       " Bicara dengan orang tepercaya, konselor kampus, atau layanan kesehatan setempat." +
                       " Untuk pertanyaan tentang hak pekerja, gunakan kanal resmi ketenagakerjaan." +
                       " Kamu boleh keluar kapan saja; progres terakhir yang stabil tetap bisa dilanjutkan.";
            }

            /// <summary>Informasi konten sebelum permainan dimulai (DESIGN 9.15).</summary>
            public static string ContentInfo()
            {
                return "SELA membahas:" +
                       "\n- penolakan kerja dan penumpukan surat penolakan" +
                       "\n- tekanan finansial dan rasa bersalah menerima bantuan keluarga" +
                       "\n- perbandingan sosial dengan teman sebaya" +
                       "\n- rekrutmen yang tidak jelas syaratnya" +
                       "\n\nTidak ada adegan kekerasan grafis, tidak ada visual melukai diri," +
                       " dan tidak ada adegan bunuh diri." +
                       " Semua nama perusahaan dan angka dalam cerita bersifat fiktif." +
                       " Kamu bisa mengatur ukuran teks, mengurangi intensitas atmosfer," +
                       " dan keluar tanpa kehilangan checkpoint terakhir.";
            }

            /// <summary>Halaman Credits dan Sumber (DESIGN 9.15).</summary>
            public static string Credits()
            {
                return "Tim" +
                       "\nNaila Rifda Rahmah - 3D dan Audio" +
                       "\nLuhur Pangaribowo Handaruaji - Narrative dan UI" +
                       "\nSteven Bong - Gameplay dan Sistem" +
                       "\n\nAset dan lisensi" +
                       "\nModel Ren - RoloArt, lisensi Asset Store" +
                       "\nShader lilToon - MIT License" +
                       "\nAnimasi demo Unity-Chan - Unity Companion License 3.0" +
                       "\nFont Noto Sans - SIL Open Font License 1.1" +
                       "\nFont Liberation Sans - SIL Open Font License 1.1, dari TextMesh Pro Essential Resources" +
                       "\nModel, tekstur, dan material kamar kos - dibuat kelompok SELA" +
                       "\n\nSumber tema" +
                       "\nUnited Nations, SDG 3 dan SDG 8" +
                       "\nWorld Health Organization, Mental health at work" +
                       "\nWouters dkk. (2013), meta-analysis serious games" +
                       "\n\nCatatan" +
                       "\nSeluruh nama perusahaan, kontrak, dan nominal dalam cerita bersifat fiktif.";
            }

            /// <summary>Label konfirmasi terakhir pada panel keputusan (GDD 3.12).</summary>
            public static string DecisionConfirmLabel(OfferChoice choice)
            {
                switch (choice)
                {
                    case OfferChoice.A: return "Konfirmasi Studio Lajur";
                    case OfferChoice.B: return "Konfirmasi Koperasi Pijar";
                    case OfferChoice.C: return "Konfirmasi Akses Prima";
                    default: return "Konfirmasi pulang";
                }
            }
        }
    }
}
