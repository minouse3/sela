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
        ImproveUiExample = 0,  // Rapikan contoh antarmuka -> E1
        CompleteTestNotes = 1, // Lengkapi catatan pengujian -> E2
        VerifyCompany = 2,     // Periksa perusahaan (A/B/C)
        SubmitApplication = 3, // Kirim lamaran (A/B)
        AttendInterview = 4,   // Ikuti interview (A/B)
        Rest = 5,              // Beristirahat
        CallDita = 6,          // Hubungi Dita
        SitWithoutScreen = 7,  // Duduk tanpa layar
        TidyDesk = 8           // Rapikan meja
    }

    public enum JedaChoice
    {
        SitBriefly = 0,
        MessageDita = 1
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
}
