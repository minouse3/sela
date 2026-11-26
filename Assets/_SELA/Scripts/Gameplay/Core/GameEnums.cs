namespace SELA.Core
{
    public enum SessionType
    {
        Pagi = 0,
        Siang = 1,
        Malam = 2
    }

    public enum CompanyId
    {
        StudioLajur = 0,   // Offer A
        KoperasiPijar = 1, // Offer B
        AksesPrima = 2     // Offer C
    }

    public enum EvidenceId
    {
        E0 = 0, // Proyek kuliah
        E1 = 1, // Antarmuka formulir
        E2 = 2  // Catatan pengujian
    }

    public enum ApplicationStatus
    {
        NotSent = 0,
        Scheduled = 1,
        Rejected = 2,
        Offered = 3
    }

    public enum OfferChoice
    {
        None = 0,
        StudioLajur = 1,
        KoperasiPijar = 2,
        AksesPrima = 3
    }

    public enum VerificationAspect
    {
        Tasks = 0,
        Compensation = 1,
        WorkHoursAndRest = 2,
        PersonalDocuments = 3
    }

    public enum RiskTag
    {
        Unmarked = 0,
        Clear = 1,
        NeedsClarification = 2,
        Risky = 3
    }

    public enum ClaimChoice
    {
        Honest = 0,
        Exaggerated = 1
    }

    public enum ActivityType
    {
        ImproveUiExample = 0,  // Rapikan contoh antarmuka -> E1 (3 jam)
        CompleteTestNotes = 1, // Lengkapi catatan pengujian -> E2 (3 jam)
        VerifyCompany = 2,     // Periksa perusahaan A/B/C (2 jam)
        SubmitApplication = 3, // Kirim lamaran A/B (2 jam)
        AttendInterview = 4,   // Ikuti interview A/B (2 jam, 13:00-15:00)
        Rest = 5,              // Beristirahat (2 jam, Pagi-Siang)
        CallDita = 6,          // Temui Dita (2 jam)
        SitWithoutScreen = 7,  // Duduk tanpa layar (1 jam)
        TidyDesk = 8,          // Rapikan meja (1 jam, 1x/hari)
        Shower = 9,            // Mandi di kamar mandi (1 jam, 1x/hari, +2 Tenaga / -3 Tekanan)
        FinalPrep = 10         // Kemas barang & siapkan jawaban (2 jam, Hari 4+ / Hari 7 -> Keputusan Akhir)
    }

    public enum DecisionNoteType
    {
        None = 0,
        WorkMentoringAndScope = 1,  // Pendampingan dan lingkup tugas (A/B)
        WorkOvertimeApproval = 2,   // Persetujuan dan pencatatan lembur (A/B)
        HomeOrganizeEvidence = 3,   // Menata bukti proyek (Pulang)
        HomeLimitedSchedule = 4,    // Menyusun jadwal lamaran terbatas (Pulang)
        RiskyOfferAcknowledged = 5  // Pengakuan syarat berisiko (C)
    }

    public enum EndingType
    {
        None = 0,
        MulaiDenganBatas = 1,    // Offer A or B
        PulangBukanSelesai = 2,  // None
        KontrakYangMengunci = 3  // Offer C
    }

    public enum GamePhase
    {
        FreeRoam = 0,
        AwaitingJeda = 1,
        InRift = 2,
        FinalDecision = 3,
        Epilogue = 4
    }

    public enum SleepTarget
    {
        UntilNoon = 0,    // Tidur sampai Siang (12:00)
        UntilNight = 1,   // Tidur sampai Malam (18:00)
        UntilMorning = 2  // Tidur sampai Pagi (08:00 hari berikutnya / Keputusan Akhir di Hari 7)
    }
}
