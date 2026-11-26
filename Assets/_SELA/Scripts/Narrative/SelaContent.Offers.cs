using System.Collections.Generic;

namespace SELA.Narrative
{
    public struct OfferAspect
    {
        public string Name;
        public string Initial;
        public string Clarification;

        public OfferAspect(string name, string initial, string clarification)
        {
            Name = name;
            Initial = initial;
            Clarification = clarification;
        }
    }

    public static partial class SelaContent
    {
        public static partial class Offers
        {
            public static readonly string[] AspectNames =
            {
                "Tugas",
                "Kompensasi",
                "Jam kerja dan istirahat",
                "Dokumen pribadi"
            };

            /// <summary>Dokumen tawaran fiktif, maksimal 180 kata (GDD 3.15).</summary>
            public static string Document(OfferId offer)
            {
                switch (offer)
                {
                    case OfferId.A:
                        return "Studio Lajur membuka posisi Junior UI Implementer." +
                               "Bagianmu adalah mengerjakan antarmuka produk: merapikan formulir," +
                               " menulis komponen yang bisa dipakai ulang, dan menulis panduan singkat untuk tim." +
                               " Gaji Rp4.200.000 per bulan, jam kerja empat puluh jam seminggu, dua hari istirahat." +
                               " Ada pendamping yang ditetapkan sejak hari pertama." +
                               " Lembur perlu persetujuan atasan dan dicatat beserta kompensasinya." +
                               " Dokumen asli seperti ijazah dan KTP tetap dipegang pekerja dan tidak ditahan oleh perusahaan." +
                               " Jaminan sosial disebutkan secara tertulis dalam kontrak." +
                               " Lamaran disertai satu bukti hasil pekerjaan antarmuka.";

                    case OfferId.B:
                        return "Koperasi Pijar membuka posisi Web Support untuk pemeliharaan formulir." +
                               " Tugas awal dibatasi pada perbaikan formulir dan pencatatan hasil pengujian," +
                               " dengan pendampingan selama dua bulan pertama." +
                               " Kontrak berlangsung tiga bulan. Gaji Rp3.800.000 per bulan," +
                               " jam kerja tiga puluh lima jam seminggu, dua hari istirahat." +
                               " Jaminan sosial disebutkan secara tertulis dalam kontrak." +
                               " Evaluasi menjelang akhir kontrak tidak menjamin perpanjangan." +
                               " Perusahaan tidak meminta dokumen asli." +
                               " Lamaran disertai dua bukti: satu hasil pekerjaan antarmuka dan satu catatan pengujian.";

                    default:
                        return "Akses Prima membuka penempatan Operator Digital." +
                               " Penghasilan diklaim sampai Rp12.000.000 per bulan." +
                               " Tidak ada lamaran dan tidak ada interview: cukup kirim data pribadi dan mulai minggu ini." +
                               " Jadwal mengikuti kebutuhan perusahaan, tanpa batas jam kerja yang tertulis." +
                               " Kontrak meminta penyerahan dokumen asli," +
                               " dan membatasi pengunduran diri." +
                               " Rincian tugas harian tidak dijelaskan." +
                               " Tawaran berlaku sampai malam hari ketujuh." +
                               " Admin menolak merinci pekerjaan dan mempertahankan syarat dokumen.";
                }
            }

            /// <summary>Empat aspek yang harus diperiksa pemain (GDD 3.6).</summary>
            public static OfferAspect[] Aspects(OfferId offer)
            {
                switch (offer)
                {
                    case OfferId.A:
                        return new[]
                        {
                            new OfferAspect("Tugas",
                                "Merapikan antarmuka formulir dan menulis komponen yang bisa dipakai ulang.",
                                "Boleh. Bagianmu antarmuka. Tidak ada server yang harus kamu tangani sendiri."),
                            new OfferAspect("Kompensasi",
                                "Rp4.200.000 per bulan, dibayar bersama jaminan sosial.",
                                "Tetap. Tidak ada bonus yang dijamin di bulan pertama."),
                            new OfferAspect("Jam kerja dan istirahat",
                                "Empat puluh jam seminggu, dua hari istirahat, lembur perlu persetujuan.",
                                "Lembur jarang, tapi kalau ada harus disetujui atasan dan kompensasinya dicatat."),
                            new OfferAspect("Dokumen pribadi",
                                "Dokumen asli tetap dipegang pekerja, tidak ditahan perusahaan.",
                                "Benar. Tidak ada permintaan menit dengan dokumen.")
                        };

                    case OfferId.B:
                        return new[]
                        {
                            new OfferAspect("Tugas",
                                "Pemeliharaan formulir, dengan pendampingan dua bulan pertama.",
                                "Tugas awal dibatasi formulir. Perbaikan lain menunggu evaluasi kontrak."),
                            new OfferAspect("Kompensasi",
                                "Rp3.800.000 per bulan selama kontrak tiga bulan.",
                                "Tidak berubah selama kontrak. Setelah tiga bulan belum ada kepastian."),
                            new OfferAspect("Jam kerja dan istirahat",
                                "Tiga puluh lima jam seminggu, dua hari istirahat.",
                                "Untuk kontrak ini tidak ada lembur yang dijadwalkan."),
                            new OfferAspect("Dokumen pribadi",
                                "Dokumen asli tidak diminta.",
                                "Cukup fotokopi untuk admisi. Tidak ada serah terima dokumen asli.")
                        };

                    default:
                        return new[]
                        {
                            new OfferAspect("Tugas",
                                "Rincian tugas harian tidak dijelaskan.",
                                "Tidak bisa dirinci. Kerjamanya mengikuti kebutuhan."),
                            new OfferAspect("Kompensasi",
                                "Sampai Rp12.000.000, bergantung pada jumlah jam.",
                                "Angkanya bisa lebih tinggi kalau jamnya banyak."),
                            new OfferAspect("Jam kerja dan istirahat",
                                "Jadwal mengikuti kebutuhan, tanpa batas jam tertulis.",
                                "Tidak ada batasnya. Kalau ada urgent, ya sudah."),
                            new OfferAspect("Dokumen pribadi",
                                "Kontrak meminta penyerahan dokumen asli.",
                                "Itu syarat. Datang dengan berkas, baru mulai bekerja.")
                        };
                }
            }

            /// <summary>Pertanyaan utama yang dijawab pada pemeriksaan perusahaan (GDD 3.6).</summary>
            public static string VerificationQuestion(OfferId offer)
            {
                switch (offer)
                {
                    case OfferId.A:
                        return "Siapa yang mendampingi pekerjaan baru, dan bagaimana lembur disetujui?";
                    case OfferId.B:
                        return "Apa tugas awalnya, dan bagaimana kepastian setelah kontrak tiga bulan?";
                    default:
                        return "Apa pekerjaan hariannya, dan alasan meminta dokumen asli?";
                }
            }

            /// <summary>Label status lowongan pada daftar Lamaran (DESIGN 9.9).</summary>
            public static string StatusLabel(ApplicationStatus status, bool opened)
            {
                if (!opened)
                {
                    return "Belum dibuka";
                }

                switch (status)
                {
                    case ApplicationStatus.Terjadwal: return "Interview terjadwal";
                    case ApplicationStatus.Ditolak: return "Ditolak";
                    case ApplicationStatus.Ditawari: return "Tawaran tersedia";
                    default: return "Terbuka sampai hari ketiga malam";
                }
            }

            /// <summary>Alasan tombol kirim tidak aktif (DESIGN 9.9).</summary>
            public static string SendBlockedReason(ApplicationStatus status, bool scheduleTaken)
            {
                if (status != ApplicationStatus.BelumDikirim)
                {
                    return "Lamaran untuk perusahaan ini sudah dikirim. Satu kesempatan pengiriman per perusahaan.";
                }
                if (scheduleTaken)
                {
                    return "Sesi siang besok sudah dipakai untuk interview. Kirim di hari lain selama jendela lamaran masih terbuka.";
                }
                return null;
            }
        }
    }
}
