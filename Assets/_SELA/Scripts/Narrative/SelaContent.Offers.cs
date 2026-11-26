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

            /// <summary>
            /// Dokumen tawaran fiktif, maksimal 180 kata (GDD 3.15).
            /// Dicetak sebagai beberapa paragraf supaya mudah dibaca, bukan satu blok rapat.
            /// </summary>
            public static string Document(OfferId offer)
            {
                switch (offer)
                {
                    case OfferId.A:
                        return "Studio Lajur membuka posisi Junior UI Implementer." +
                               " Bagianmu adalah mengerjakan antarmuka produk: merapikan formulir," +
                               " menulis komponen yang bisa dipakai ulang, dan menulis panduan singkat untuk tim." +
                               "\n\nGaji Rp4.200.000 per bulan, jam kerja empat puluh jam seminggu, dua hari istirahat." +
                               " Ada pendamping yang ditetapkan sejak hari pertama." +
                               " Lembur perlu persetujuan atasan dan dicatat beserta kompensasinya." +
                               "\n\nDokumen asli seperti ijazah dan KTP tetap dipegang pekerja dan tidak ditahan oleh perusahaan." +
                               " Jaminan sosial disebutkan secara tertulis dalam kontrak." +
                               " Lamaran disertai satu bukti hasil pekerjaan antarmuka.";

                    case OfferId.B:
                        return "Koperasi Pijar membuka posisi Web Support untuk pemeliharaan formulir." +
                               " Tugas awal dibatasi pada perbaikan formulir dan pencatatan hasil pengujian," +
                               " dengan pendampingan selama dua bulan pertama." +
                               "\n\nKontrak berlangsung tiga bulan. Gaji Rp3.800.000 per bulan," +
                               " jam kerja tiga puluh lima jam seminggu, dua hari istirahat." +
                               " Jaminan sosial disebutkan secara tertulis dalam kontrak." +
                               "\n\nEvaluasi menjelang akhir kontrak tidak menjamin perpanjangan." +
                               " Perusahaan tidak meminta dokumen asli." +
                               " Lamaran disertai dua bukti: satu hasil pekerjaan antarmuka dan satu catatan pengujian.";

                    default:
                        return "Akses Prima membuka penempatan Operator Digital." +
                               " Penghasilan diklaim sampai Rp12.000.000 per bulan." +
                               " Tidak ada lamaran dan tidak ada interview: cukup kirim data pribadi dan mulai minggu ini." +
                               "\n\nJadwal mengikuti kebutuhan perusahaan, tanpa batas jam kerja yang tertulis." +
                               " Kontrak meminta penyerahan dokumen asli, dan membatasi pengunduran diri." +
                               " Rincian tugas harian tidak dijelaskan." +
                               "\n\nTawaran berlaku sampai malam hari ketujuh." +
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
            /// <summary>
            /// Hari pertama lowongan perusahaan terbit. Studio Lajur terbit hari 1,
            /// Koperasi Pijar hari 3, dan Akses Prima hari 4 sebagai tawaran langsung (GDD 3.4).
            /// </summary>
            public static int OpeningDay(OfferId offer)
            {
                switch (offer)
                {
                    case OfferId.A: return 1;
                    case OfferId.B: return 3;
                    default: return 4;
                }
            }

            /// <summary>
            /// Hari terakhir lamaran diterima. Batas Studio Lajur malam hari 3,
            /// Koperasi Pijar malam hari 5, dan Akses Prima malam hari 7 (GDD 3.4).
            /// </summary>
            public static int ClosingDay(OfferId offer)
            {
                switch (offer)
                {
                    case OfferId.A: return 3;
                    case OfferId.B: return 5;
                    default: return 7;
                }
            }

            /// <summary>Status lamaran yang tampil di daftar, selalu disertai keterangan (DESIGN 9.9).</summary>
            public static string StatusLabel(ApplicationStatus status, OfferId offer, int day)
            {
                if (day < OpeningDay(offer))
                {
                    return "Belum dibuka";
                }

                switch (status)
                {
                    case ApplicationStatus.Terjadwal: return "Interview terjadwal";
                    case ApplicationStatus.Ditolak: return "Ditolak";
                    case ApplicationStatus.Ditawari: return "Tawaran tersedia";
                }

                if (offer == OfferId.C)
                {
                    return "Tawaran langsung tersedia";
                }

                if (day > ClosingDay(offer))
                {
                    return "Jendela lamaran berakhir";
                }

                return "Terbuka sampai malam hari " + ClosingDay(offer);
            }

            /// <summary>Alasan lowongan belum bisa dibaca, selalu menyebut tanggal (DESIGN 9.9).</summary>
            public static string NotPublishedReason(OfferId offer)
            {
                int day = OpeningDay(offer);
                if (offer == OfferId.C)
                {
                    return "Akses Prima belum membuka tawaran langsung. Kabar itu sampai hari " + day + ".";
                }

                return OfferName(offer) + " belum membuka lowongan. Iklannya terbit mulai hari " + day + ".";
            }
            /// <summary>Alasan tombol kirim tidak aktif, memperhitungkan hari dan jadwal (DESIGN 9.9).</summary>
            public static string SendBlockedReason(ApplicationStatus status, OfferId offer, int day, bool scheduleTaken)
            {
                if (status != ApplicationStatus.BelumDikirim)
                {
                    return "Lamaran untuk perusahaan ini sudah dikirim. Satu kesempatan pengiriman per perusahaan.";
                }
                if (day < OpeningDay(offer))
                {
                    return NotPublishedReason(offer);
                }
                if (offer != OfferId.C && day > ClosingDay(offer))
                {
                    return "Jendela lamaran " + OfferName(offer) + " berakhir pada malam hari " +
                           ClosingDay(offer) + ".";
                }
                if (scheduleTaken)
                {
                    return "Sesi siang besok sudah dipakai untuk interview. Kirim di hari lain selama jendela lamaran masih terbuka.";
                }
                return null;
            }        }
    }
}
