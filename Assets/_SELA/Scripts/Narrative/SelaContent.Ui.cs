using System.Collections.Generic;

namespace SELA.Narrative
{
    public static partial class SelaContent
    {
        /// <summary>Semua label dan judul panel. Satu sumber teks agar tidak ada versi berbeda antar layar (DESIGN 11).</summary>
        public static partial class Ui
        {
            public static readonly string[] MainMenuItems =
            {
                "Mulai baru",
                "Lanjutkan",
                "Pengaturan",
                "Credits dan Sumber",
                "Keluar"
            };

            public static readonly string[] PauseItems =
            {
                "Lanjutkan",
                "Pengaturan",
                "Ulangi dari checkpoint",
                "Kembali ke menu",
                "Keluar"
            };

            public const string NoSaveReason = "Belum ada simpanan";

            /// <summary>Alasan melanjutkan, menyebut posisi simpanan yang sebenarnya.</summary>
            public static string ResumeReason(string where)
            {
                return "Lanjutkan dari " + where;
            }

            public const string TabApplications = "Lamaran";
            public const string TabMessages = "Pesan";
            public const string TabNotes = "Catatan";

            public const string NotesTitle = "Catatan harian Raka";
            public const string NotesEvidence = "Bukti";
            public const string NotesSchedule = "Jadwal";
            public const string NotesRoomFacts = "Fakta ruang";
            public const string NotesHistory = "Riwayat hasil";

            public const string Back = "Kembali";
            public const string Close = "Tutup";
            public const string Cancel = "Batal";
            public const string Confirm = "Konfirmasi";
            public const string UseSession = "Gunakan satu sesi";
            public const string ReadFree = "Baca gratis sesi";
            public const string SendApplication = "Kirim lamaran";
            public const string EditDraft = "Susun draf";
            public const string VerifyCompany = "Periksa perusahaan";
            public const string ReadVerification = "Baca hasil verifikasi";
            public const string FollowInterview = "Ikuti interview";
            public const string OpenCv = "Buka CV";
            public const string CheckResult = "Lihat hasil";

            public const string OneSession = "1 sesi";
            public const string SessionCost = "Tenaga -1, Tekanan +1";

            public const string EnergyLabel = "Tenaga";
            public const string PressureLabel = "Tekanan";
            public const string PressureCaption = "Kondisi permainan, bukan diagnosis";
            public const string PromptKey = "E";
            public const string EscBack = "Esc - Kembali";
            public const string TabHint = "Tab - Catatan";

            public const string SettingsTitle = "Pengaturan";
            public const string SettingsAudio = "Audio";
            public const string SettingsCamera = "Kamera";
            public const string SettingsReading = "Keterbacaan";
            public const string VolumeMaster = "Master";
            public const string VolumeAmbience = "Ambience";
            public const string VolumeEffects = "Effects";
            public const string Sensitivity = "Sensitivitas";
            public const string InvertY = "Invert Y";
            public const string FieldOfView = "Field of view";
            public const string ReduceMotion = "Reduce Motion";
            public const string TextSize = "Ukuran teks";
            public const string Atmosphere = "Intensitas atmosfer";
            public const string AtmosphereNormal = "Normal";
            public const string AtmosphereLow = "Rendah";
            public const string On = "Aktif";
            public const string Off = "Nonaktif";
            public const string TextScale100 = "100%";
            public const string TextScale125 = "125%";
            public const string TextScale150 = "150%";

            public const string CreditsTitle = "Credits dan Sumber";
            public const string HelpTitle = "Refleksi dan Bantuan";
            public const string ContentInfoTitle = "Informasi konten";
            public const string ContentInfoBackToMenu = "Kembali ke menu";
            public const string ContentInfoContinue = "Lanjutkan cerita";

            public const string VerificationTitle = "Verifikasi tawaran";
            public const string VerificationMarks = "Penilaianmu";
            public const string VerificationInitial = "Pernyataan awal";
            public const string VerificationClarification = "Jawaban klarifikasi";
            public const string MarkUnset = "Belum ditandai";
            public const string MarkClear = "Jelas";
            public const string MarkAsk = "Perlu ditanyakan";
            public const string MarkRisky = "Berisiko";

            public const string InterviewCorrection = "Luruskan seluruh klaim";
            public const string InterviewContinue = "Lanjut dengan klaim terkirim";
            public const string InterviewNext = "Lanjut";
            public const string QuestionIndex = "Pertanyaan";

            public const string BreakTitle = "Jeda";
            public const string BreakSit = "Duduk sebentar";
            public const string BreakMessageDita = "Kirim pesan kepada Dita";
            public const string BreakSummary = "Tekanan menjadi 4; Tenaga minimal 1; tanpa memakai sesi";

            public const string DecisionTitle = "Keputusan akhir";
            public const string DecisionCurrent = "Pilihan saat ini";
            public const string DecisionWithdraw = "Tarik pilihan";
            public const string DecisionFinalize = "Selesaikan keputusan";
            public const string DecisionOfferA = "Studio Lajur";
            public const string DecisionOfferB = "Koperasi Pijar";
            public const string DecisionOfferC = "Akses Prima";
            public const string DecisionHome = "Pulang";
            public const string DecisionNotObtained = "Belum ada tawaran";
            public const string DecisionNoteLajur = "Pendampingan dan lingkup tugas";
            public const string DecisionNotePijar = "Persetujuan dan pencatatan lembur";
            public const string DecisionNoteHomeA = "Menata bukti proyek";
            public const string DecisionNoteHomeB = "Menyusun jadwal lamaran terbatas";
            public const string DecisionRiskWarning = "Syarat berisiko: kontrak meminta penyerahan dokumen asli dan membatasi pengunduran diri.";

            public const string RiftHint = "Petunjuk";
            public const string RiftSkip = "Selesaikan adegan";
            public const string RiftReturn = "Kembalikan";

            public const string SaveStored = "Tersimpan";
            public const string SaveFailed = "Penulisan gagal. Progres terbaru belum tersimpan.";
            public const string SaveUnreadable = "Autosave tidak terbaca";
            public const string LoadDayStart = "Muat awal hari";

            public const string ExitConfirm = "Kembali ke menu atau keluar?";
            public const string ExitWarning = "Bagian yang belum mencapai checkpoint akan diulang.";
            public const string ExitCheckpoint = "Autosave terakhir";
            public const string ExitDayStart = "Awal hari";
            public const string NewGameWarning = "Progres lama akan ditimpa.";
            public const string RestartDayWarning = "Progres setelah hari ini akan diganti.";

            public const string EnergyValueFormat = "{0}/4";
            public const string PressureValueFormat = "{0}/6";

            /// <summary>Status hari dan sesi untuk HUD.</summary>
            public static string HudDaySession(int day, SelaSession session)
            {
                return "Hari " + day + " - " + SelaContent.SessionLabel(session);
            }

            /// <summary>Label aktivitas di panel konfirmasi (GDD 3.3).</summary>
            public static string ActivityTitle(SelaActivity activity)
            {
                switch (activity)
                {
                    case SelaActivity.RapikanContohAntarmuka: return "Rapikan contoh antarmuka";
                    case SelaActivity.LengkapiCatatanPengujian: return "Lengkapi catatan pengujian";
                    case SelaActivity.PeriksaPerusahaan: return "Periksa perusahaan";
                    case SelaActivity.KirimLamaran: return "Kirim lamaran";
                    case SelaActivity.IkutiInterview: return "Ikuti interview";
                    case SelaActivity.Beristirahat: return "Beristirahat";
                    case SelaActivity.HubungiDita: return "Hubungi Dita";
                    case SelaActivity.DudukTanpaLayar: return "Duduk tanpa layar";
                    default: return "Rapikan meja";
                }
            }

            public static string ActivityLocation(SelaActivity activity)
            {
                switch (activity)
                {
                    case SelaActivity.RapikanContohAntarmuka: return "Meja dan laptop";
                    case SelaActivity.LengkapiCatatanPengujian: return "Meja dan laptop";
                    case SelaActivity.PeriksaPerusahaan: return "Laptop";
                    case SelaActivity.KirimLamaran: return "Laptop";
                    case SelaActivity.IkutiInterview: return "Laptop";
                    case SelaActivity.Beristirahat: return "Kasur";
                    case SelaActivity.HubungiDita: return "Ponsel di meja";
                    case SelaActivity.DudukTanpaLayar: return "Kursi dekat jendela";
                    default: return "Meja fisik";
                }
            }

            /// <summary>Perubahan stat aktivitas, ditulis dengan nama stat (DESIGN 9.5).</summary>
            public static string ActivityCost(SelaActivity activity)
            {
                switch (activity)
                {
                    case SelaActivity.Beristirahat: return "Tenaga +3, Tekanan -2";
                    case SelaActivity.HubungiDita: return "Tenaga +2, Tekanan -2";
                    case SelaActivity.DudukTanpaLayar: return "Tenaga +2, Tekanan -2";
                    case SelaActivity.RapikanMeja: return "Tenaga +1, Tekanan -1";
                    case SelaActivity.PeriksaPerusahaan: return "Tenaga -1, Tekanan tetap";
                    case SelaActivity.IkutiInterview: return "Tenaga -1, Tekanan +2";
                    default: return "Tenaga -1, Tekanan +1";
                }
            }
        }
    }
}
