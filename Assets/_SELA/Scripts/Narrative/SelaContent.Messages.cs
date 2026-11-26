using System.Collections.Generic;

namespace SELA.Narrative
{
    public static partial class SelaContent
    {
        public static partial class Messages
        {
            /// <summary>Pesan hari 1-7 beserta sisipan konteks hari 5-7.</summary>
            public static List<Message> Daily(int day, OfferContext context)
            {
                switch (day)
                {
                    case 1: return Day1();
                    case 2: return Day2();
                    case 3: return Day3();
                    case 4: return Day4();
                    case 5: return Day5(context);
                    case 6: return Day6(context);
                    case 7: return Day7(context);
                    default: return new List<Message>();
                }
            }

            private static List<Message> Day1()
            {
                return new List<Message>
                {
                    new Message(MsgSender.Ibu, "Tiket", "Tiket bus sudah Ibu beli, Ka. Berangkat pagi tanggal kedelapan, jadi tinggal enam hari lagi. Tolong kabari sebelum berangkat, tidak usah panjang. Ibu cuma mau tahu ekonominya aman."),
                    new Message(MsgSender.Dita, "Tempat tinggal sementara", "Kalau nanti kamu ambil kerja di kota, tempatku masih kosong satu bulan. Aku serius, bukan basa-basi. Yang penting kamu tinggal dekat tempat kamu bekerja, biar tidak kelelahan di hari pertama."),
                    new Message(MsgSender.Perusahaan, "Studio Lajur membuka lowongan", "Posisi Junior UI Implementer. Pendaftaran dibuka sampai malam hari ketiga. Bukti E1 harus ikut terkirim di lamaran. Tidak ada trial, tapi ada sesi penjelasan portofolio."),
                    new Message(MsgSender.Sistem, "Isi kamar", "Di meja ada kartu proyek dari kuliah, tiket pulang di dalam tas, dan nomor 07 pada pintu. Semua bisa dibaca tanpa memakai sesi. Mulailah dari apa yang ada di depan mata.")
                };
            }

            private static List<Message> Day2()
            {
                return new List<Message>
                {
                    new Message(MsgSender.Dita, "Yang lama itu belum selesai", "Kalau surat yang lama itu muncul lagi di kotak masuk, itu bukan kabar baru. Yang baru itu yang kamu kerjakan minggu ini. Aku baca blog kamu soal antarmuka itu, bagian formulirnya masuk akal."),
                    new Message(MsgSender.Ibu, "Sudah makan?", "Ibu cuma tanya, sudah makan? Kalau belum, warung di ujung lorong saja. Jangan ditunda sampai malam, nanti kamumu bukan lapar, tapi pusing."),
                    new Message(MsgSender.Sistem, "Surat penolakan lama", "Surat penolakan lama ada di kotak masuk sejak sebelum permainan dimulai. Membacanya tidak memakai sesi dan tidak mengurangi apa pun. Boleh dibaca, boleh dilewati.")
                };
            }

            private static List<Message> Day3()
            {
                return new List<Message>
                {
                    new Message(MsgSender.Perusahaan, "Koperasi Pijar membuka lowongan", "Posisi Web Support untuk pemeliharaan formulir. Pendaftaran dibuka pagi ini sampai malam hari kelima. E1 dan E2 harus ikut dalam lamaran. Ada pendampingan untuk dua bulan pertama."),
                    new Message(MsgSender.Dita, "Portofolio versi kedua", "Besok pagi aku buka jam dua belas, boleh lihat formulirnya. Satu yang perlu diperbaiki: kalimat 'mengarahkan tim' kelihatan besar untuk proyek yang kamu kerjakan sendiri."),
                    new Message(MsgSender.Sistem, "Catatan pengujian", "Lengkapi catatan pengujian sekarang sudah bisa. Butuh E1 selesai lebih dulu, karena tanpa hasil antarmuka tidak ada yang perlu diuji.")
                };
            }

            private static List<Message> Day4()
            {
                return new List<Message>
                {
                    new Message(MsgSender.AdminAksesPrima, "Kesempatan tanpa tes", "Kami menyediakan penempatan untuk yang mau langsung bekerja. Penghasilan sampai dua belas juta per bulan. Tidak perlu proses lama. Kirim data pribadi, langsung bisa mulai minggu ini. Balas pesan ini untuk kualifikasi."),
                    new Message(MsgSender.Dita, "Yang satu ini berbeda", "Baca yang tertulis, bukan yang diceritakan orang lain. Kalau memang diminta dokumen asli, tanya dulu untuk apa dan disimpan berapa lama. Kamu boleh menunda. Kebanyakan lowongan cuma langkah pertama, dan langkah pertama boleh pelan.")
                };
            }

            private static List<Message> Day5(OfferContext context)
            {
                var list = new List<Message>
                {
                    new Message(MsgSender.Perusahaan, "Pendaftaran ditutup malam ini", "Pendaftaran Koperasi Pijar ditutup malam ini. Lamaran yang sudah terkirim tetap diproses. Yang belum dikirim tidak bisa dijadwalkan setelah hari ini.")
                };

                switch (context)
                {
                    case OfferContext.DuaTawaran:
                        list.Add(new Message(MsgSender.Dita, "Dua surat di tangan", "Sekarang kamu punya dua. Jangan buru-buru pilih yang gajinya lebih besar. Tunjuk satu syarat dari masing-masing yang paling memengaruhi keseharianmu, lalu tanyakan sisanya. Aku tidak akan memilih buat kamu."));
                        break;
                    case OfferContext.SatuTawaran:
                        list.Add(new Message(MsgSender.Dita, "Satu surat saja", "Yang satu itu belum tentu habis. Yang belum ada belum tentu paling buruk. Baca ulang syarat yang belum kamu periksa, lalu putuskan apa yang masih boleh dikirim sebelum malam ini."));
                        break;
                    default:
                        list.Add(new Message(MsgSender.Dita, "Belum ada yang masuk", "Belum ada lamaran yang masuk. Jangan dibaca sebagai vonis. Yang bisa kamu kerjakan sekarang: rapikan bukti yang sudah ada, dan periksa lagi syarat yang belum kamu lihat. Besok masih ada kesempatan."));
                        break;
                }

                return list;
            }

            private static List<Message> Day6(OfferContext context)
            {
                var list = new List<Message>
                {
                    new Message(MsgSender.Ibu, "Tiketnya", "Tiketnya tetap boleh dipakai, Ka. Kalau jadi menunda, tidak apa-apa. Ibu cuma mau tahu kabarnya. Tidak apa-apa juga kalau hari ini belum ada yang bisa dibawa pulang."),
                    new Message(MsgSender.Sistem, "Interview hari ini", "Kalau ada undangan, interview berlangsung sesi siang hari. Kalau kamu memilih aktivitas lain saat itu, undangan terlewat dan menambah tekanan satu. Hasilnya bisa dibaca ulang tanpa memakai sesi.")
                };

                switch (context)
                {
                    case OfferContext.DuaTawaran:
                        list.Add(new Message(MsgSender.Dita, "Sudah hampir selesai", "Besok pagi Ibu nanya jadwalnya. Siapkan satu kalimat yang bisa kamu ucapkan ke dia, sekecil apa pun. Kalau belum siap, itu juga jawaban."));
                        break;
                    case OfferContext.SatuTawaran:
                        list.Add(new Message(MsgSender.Dita, "Satu batas saja", "Cukup satu hal yang mau kamu batasi. Tidak perlu daftar panjang. Kalau memang tidak ada yang mau kamu batasi, katakan itu juga."));
                        break;
                    default:
                        list.Add(new Message(MsgSender.Dita, "Rencana pulang", "Kalau belum ada yang bisa kamu terima, siapkan langkah kecil yang mau kamu kerjakan setelah pulang. Bukan yang paling ideal, tapi yang benar-benar bisa kamu mulai minggu depan."));
                        break;
                }

                return list;
            }

            private static List<Message> Day7(OfferContext context)
            {
                var list = new List<Message>
                {
                    new Message(MsgSender.Ibu, "Jadwalnya", "Jadwal busnya pukul berapa, Ka? Kalau sudah tidak cocok, sebut saja. Ibu tidak sedang mengetes jawaban. Ibu mau tahu kamu pulang jam berapa, biar Ibu siapkan makanannya.")
                };

                switch (context)
                {
                    case OfferContext.DuaTawaran:
                        list.Add(new Message(MsgSender.Dita, "Satu kalimat saja", "Dua surat, satu catatan. Besok pagi bus berangkat jam berapa pun, kalimat itu tetap kamu bawa pulang."));
                        break;
                    case OfferContext.SatuTawaran:
                        list.Add(new Message(MsgSender.Dita, "Tulis ulang biar jelas", "Surat yang kamu punya, tulis ulang batasnya dalam satu kalimat. Kalau tidak bisa, itu juga informasi berguna."));
                        break;
                    default:
                        list.Add(new Message(MsgSender.Dita, "Pulang itu pilihan", "Belum dapat kerja bukan berarti gagal. Yang kamu bawa pulang dari sini bukan cuma lembar lamaran, tapi juga cara kamu menilai diri selama tujuh hari ini."));
                        break;
                }

                return list;
            }
        }
    }
}
