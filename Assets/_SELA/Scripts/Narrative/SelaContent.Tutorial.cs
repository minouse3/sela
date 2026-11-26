namespace SELA.Narrative
{
    public static partial class SelaContent
    {
        /// <summary>
        /// Pengenalan pertama. Urutan mengikuti DESIGN 9.15: baca gratis, buka/batal aktivitas,
        /// hasil satu aktivitas pilihan, pergantian sesi, draf CV, panel pengiriman.
        /// </summary>
        public static partial class Tutorial
        {
            public const int StepCount = 6;

            public static string Title()
            {
                return "Pengenalan singkat";
            }

            public static string StepLabel(int step)
            {
                return "Langkah " + (step + 1) + " dari " + StepCount;
            }

            public static string StepTitle(int step)
            {
                switch (step)
                {
                    case 0: return "Baca gratis";
                    case 1: return "Buka dan batalkan aktivitas";
                    case 2: return "Hasil satu aktivitas pilihan";
                    case 3: return "Pergantian sesi";
                    case 4: return "Draf CV";
                    default: return "Panel pengiriman";
                }
            }

            public static string StepBody(int step)
            {
                switch (step)
                {
                    case 0:
                        return "Membaca pesan, surat tawaran, lowongan, dan catatan tidak memakai sesi." +
                               " Tenagamu tidak berkurang karena membaca, jadi jangan buru-buru menutup apa pun" +
                               " yang belum sempat dibaca.";
                    case 1:
                        return "Setiap aktivitas yang mengubah keadaan memakai satu sesi." +
                               " Panel aktivitas lebih dulu menampilkan biaya dan kondisi sebelum dan sesudah." +
                               " Tombol Batal menutup panel tanpa mengubah tenaga, tekanan, atau sesi.";
                    case 2:
                        return "Beberapa aktivitas meminta satu pilihan setelah biayanya dikonfirmasi." +
                               " Pilihan yang tepat memberi kartu bukti." +
                               " Pilihan yang kurang tepat menjelaskan alasannya dan dapat diulang" +
                               " tanpa biaya dan tanpa perubahan statistik.";
                    case 3:
                        return "Satu hari punya tiga sesi: Pagi, Siang, dan Malam." +
                               " Memakai satu sesi membuat sesi berikutnya dimulai." +
                               " Kalau tekanan sudah penuh, jeda muncul lebih dulu dan tidak ada pilihan yang terkunci.";
                    case 4:
                        return "Draf CV berisi dua klaim tentang hasil kerja." +
                               " Bukti yang dipasang di Meja Bukti menentukan klaim mana yang sudah didukung." +
                               " Klaim yang lebih besar daripada bukti ditulis apa adanya, tanpa penalti diam-diam.";
                    default:
                        return "Snapshot lamaran mengunci klaim dan bukti yang kamu kirim." +
                               " Bukti yang dibuat setelah lamaran dikirim tidak memperbarui lamaran itu." +
                               " Tawaran dan jadwal interview dibaca ulang dari snapshot, bukan dari keadaan terbaru.";
                }
            }

            public static string Next()
            {
                return "Lanjut";
            }

            public static string Skip()
            {
                return "Lewati";
            }

            public static string Start()
            {
                return "Mulai";
            }
        }
    }
}
