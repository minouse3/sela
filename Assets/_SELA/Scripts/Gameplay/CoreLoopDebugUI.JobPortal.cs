using UnityEngine;
using SELA.Core;

public partial class CoreLoopDebugUI
{
    CompanyId selectedJobCompany = CompanyId.StudioLajur;

    static readonly Color PortalSurface = new Color32(249, 244, 233, 255);
    static readonly Color PortalSidebar = new Color32(236, 227, 207, 255);
    static readonly Color PortalSelected = new Color32(252, 248, 240, 255);
    static readonly Color PortalAccent = new Color32(89, 108, 128, 255);
    static readonly Color PortalClay = new Color32(174, 116, 98, 255);
    static readonly Color PortalSage = new Color32(113, 133, 121, 255);
    static readonly Color PortalBadgeBg = new Color32(226, 217, 198, 255);
    static readonly Color PortalWarnBg = new Color32(242, 213, 202, 255);
    static readonly Color PortalSafeBg = new Color32(215, 228, 218, 255);

    GUIStyle portalTitleStyle;
    GUIStyle portalRoleStyle;
    GUIStyle portalSmallStyle;
    GUIStyle portalBadgeStyle;
    GUIStyle portalWarnBoxStyle;
    GUIStyle portalSafeBoxStyle;
    GUIStyle portalPrimaryBtnStyle;
    Texture2D portalPrimaryTex;
    Texture2D portalPrimaryHoverTex;
    Texture2D portalSafeTex;

    struct JobPostingData
    {
        public CompanyId Company;
        public string RoleTitle;
        public string WorkMode;
        public string SalaryTag;
        public string DeadlineNote;
        public string Summary;
        public string Requirements;
        public string VerifyCompensation;
        public string VerifyHours;
        public string VerifyScope;
        public string VerifyDocuments;
        public bool HasRedFlags;
    }

    static readonly JobPostingData[] JobPostings =
    {
        new JobPostingData
        {
            Company = CompanyId.StudioLajur,
            RoleTitle = "Junior UI / Frontend",
            WorkMode = "Hybrid",
            SalaryTag = "Rp 4,8 jt / bln",
            DeadlineNote = "Tutup: Hari 3, 22:00",
            Summary = "Merapikan tampilan formulir dan alur interaksi web bersama tim produk.",
            Requirements = "Syarat: Lampiran E1 (Perbandingan Antarmuka)",
            VerifyCompensation = "Gaji pokok + transport, kontrak 6 bulan.",
            VerifyHours = "Senin–Jumat 09:00–17:30, lembur tercatat.",
            VerifyScope = "Pendampingan 1 bulan, tugas sesuai level junior.",
            VerifyDocuments = "Salinan KTP & ijazah. Tidak menahan dokumen asli.",
            HasRedFlags = false
        },
        new JobPostingData
        {
            Company = CompanyId.KoperasiPijar,
            RoleTitle = "Junior QA & Uji Formulir",
            WorkMode = "Onsite",
            SalaryTag = "Rp 4,5 jt / bln",
            DeadlineNote = "Buka Hari 3  ·  Tutup Hari 5, 22:00",
            Summary = "Menguji formulir layanan anggota, mencatat bug, dan memverifikasi perbaikan.",
            Requirements = "Syarat: Lampiran E1 & E2 (Antarmuka + Catatan Uji)",
            VerifyCompensation = "Gaji UMR + BPJS aktif sejak bulan pertama.",
            VerifyHours = "Senin–Jumat 08:30–17:00.",
            VerifyScope = "Fokus pengujian manual dan laporan rutin.",
            VerifyDocuments = "Verifikasi dokumen saat tanda tangan, asli tetap di pegawai.",
            HasRedFlags = false
        },
        new JobPostingData
        {
            Company = CompanyId.AksesPrima,
            RoleTitle = "IT Support & Admin",
            WorkMode = "Tawaran Langsung",
            SalaryTag = "Rp 5,5 jt / bln",
            DeadlineNote = "Masuk Hari 4  ·  Tanpa interview",
            Summary = "Posisi operasional cepat tanpa tahap seleksi.",
            Requirements = "Syarat: Tanpa portofolio",
            VerifyCompensation = "Ada penalti jika berhenti sebelum 2 tahun.",
            VerifyHours = "Siaga malam & akhir pekan tanpa batas lembur tertulis.",
            VerifyScope = "Tugas campuran IT, admin, dan operasional lapangan.",
            VerifyDocuments = "Menahan ijazah asli selama masa ikatan dinas.",
            HasRedFlags = true
        }
    };

    static JobPostingData GetPosting(CompanyId company)
    {
        for (int i = 0; i < JobPostings.Length; i++)
            if (JobPostings[i].Company == company) return JobPostings[i];
        return JobPostings[0];
    }

    void EnsureJobPortalStyles()
    {
        if (portalTitleStyle != null) return;

        portalPrimaryTex = MakeTex(PortalAccent);
        portalPrimaryHoverTex = MakeTex(new Color32(71, 88, 105, 255));
        portalSafeTex = MakeTex(PortalSafeBg);

        portalTitleStyle = LockTextColor(new GUIStyle(headerStyle) { fontSize = 16 }, InkColor);
        portalRoleStyle = LockTextColor(new GUIStyle(subStyle) { fontSize = 15 }, InkColor);
        portalSmallStyle = LockTextColor(new GUIStyle(mutedStyle) { fontSize = 12 }, MutedInk);
        portalBadgeStyle = LockTextColor(new GUIStyle(GUI.skin.label)
        {
            fontSize = 11,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        }, InkColor);
        portalWarnBoxStyle = LockTextColor(new GUIStyle(warnStyle)
        {
            fontSize = 12,
            padding = new RectOffset(10, 10, 8, 8)
        }, new Color(0.45f, 0.14f, 0.08f, 1f));
        portalSafeBoxStyle = LockTextColor(new GUIStyle(noticeStyle)
        {
            fontSize = 12,
            normal = { background = portalSafeTex },
            hover = { background = portalSafeTex },
            active = { background = portalSafeTex },
            focused = { background = portalSafeTex },
            padding = new RectOffset(10, 10, 8, 8)
        }, InkColor);
        portalPrimaryBtnStyle = LockTextColor(new GUIStyle(buttonStyle)
        {
            wordWrap = true,
            normal = { background = portalPrimaryTex },
            hover = { background = portalPrimaryHoverTex },
            active = { background = portalPrimaryHoverTex },
            focused = { background = portalPrimaryTex },
            onNormal = { background = portalPrimaryHoverTex },
            onHover = { background = portalPrimaryHoverTex },
            onActive = { background = portalPrimaryHoverTex },
            onFocused = { background = portalPrimaryHoverTex }
        }, DeviceLight);
    }

    void DestroyJobPortalTextures()
    {
        if (portalPrimaryTex != null) Destroy(portalPrimaryTex);
        if (portalPrimaryHoverTex != null) Destroy(portalPrimaryHoverTex);
        if (portalSafeTex != null) Destroy(portalSafeTex);
    }

    void DrawJobPortalApp(GameLoopEngine engine, GameState state)
    {
        EnsureJobPortalStyles();

        // Clean Portal Header
        GUILayout.BeginHorizontal();
        GUILayout.Label("KerjaLokal.id", portalTitleStyle);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Buka CV Builder", buttonStyle, GUILayout.Width(140), GUILayout.Height(30)))
            SetLaptopPage(LaptopPage.CvBuilder);
        GUILayout.EndHorizontal();

        GUILayout.Space(8);

        // 2-Column Master-Detail Job Board
        GUILayout.BeginHorizontal();

        // LEFT COLUMN: Job Feed
        GUILayout.BeginVertical(GUILayout.Width(300));
        for (int i = 0; i < JobPostings.Length; i++)
        {
            DrawJobListCard(state, JobPostings[i]);
            GUILayout.Space(8);
        }
        GUILayout.EndVertical();

        GUILayout.Space(14);

        // RIGHT COLUMN: Selected Job Detail
        GUILayout.BeginVertical(cardStyle, GUILayout.ExpandWidth(true));
        DrawJobDetailPane(engine, state, GetPosting(selectedJobCompany));
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    void DrawJobListCard(GameState state, JobPostingData job)
    {
        bool selected = selectedJobCompany == job.Company;
        var app = state.GetApplication(job.Company);
        var ver = state.GetVerification(job.Company);

        string badge;
        Color badgeColor;
        if (app != null)
        {
            switch (app.status)
            {
                case ApplicationStatus.Scheduled:
                    badge = $"Interview H{app.interviewDay} · {app.interviewHour:00}:00";
                    badgeColor = DesktopGold;
                    break;
                case ApplicationStatus.Offered:
                    badge = "Tawaran Masuk";
                    badgeColor = PortalSafeBg;
                    break;
                case ApplicationStatus.Rejected:
                    badge = "Tidak Lanjut";
                    badgeColor = PortalWarnBg;
                    break;
                default:
                    bool open = ActivitySystem.IsApplicationWindowOpen(state, job.Company, out _);
                    badge = open ? "Dibuka" : (state.day < 3 && job.Company == CompanyId.KoperasiPijar ? "Buka Hari 3" : "Ditutup");
                    badgeColor = open ? PortalBadgeBg : PortalSidebar;
                    break;
            }
        }
        else
        {
            badge = state.day >= 4 ? "Tawaran Langsung" : "Hari 4";
            badgeColor = state.day >= 4 ? PortalWarnBg : PortalSidebar;
        }

        var rect = GUILayoutUtility.GetRect(290f, 94f, GUILayout.ExpandWidth(true));
        DeviceFill(rect, selected ? PortalSelected : PortalSidebar, 5f);
        if (selected)
            DeviceFill(new Rect(rect.x, rect.y + 4f, 4f, rect.height - 8f), job.HasRedFlags ? PortalClay : PortalAccent, 2f);

        GUI.Label(new Rect(rect.x + 14f, rect.y + 8f, rect.width - 24f, 22f), job.RoleTitle, subStyle);
        GUI.Label(new Rect(rect.x + 14f, rect.y + 30f, rect.width - 24f, 20f),
            $"{ActivitySystem.GetCompanyName(job.Company)}  ·  {job.SalaryTag}", portalSmallStyle);

        var badgeRect = new Rect(rect.x + 14f, rect.y + 60f, 162f, 23f);
        DeviceFill(badgeRect, badgeColor, 4f);
        GUI.Label(badgeRect, badge, portalBadgeStyle);

        if (ver != null && ver.isVerified)
        {
            var chkRect = new Rect(rect.xMax - 78f, rect.y + 60f, 68f, 23f);
            DeviceFill(chkRect, job.HasRedFlags ? PortalWarnBg : PortalSafeBg, 4f);
            GUI.Label(chkRect, job.HasRedFlags ? "Risiko!" : "Dicek ✓", portalBadgeStyle);
        }

        if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
            selectedJobCompany = job.Company;
    }

    void DrawJobDetailPane(GameLoopEngine engine, GameState state, JobPostingData job)
    {
        var app = state.GetApplication(job.Company);
        var ver = state.GetVerification(job.Company);

        GUILayout.Label(job.RoleTitle, portalRoleStyle);
        GUILayout.Label($"{ActivitySystem.GetCompanyName(job.Company)}  ·  {job.WorkMode}  ·  {job.SalaryTag}", portalSmallStyle);
        GUILayout.Label(job.DeadlineNote, portalSmallStyle);
        GUILayout.Space(8);

        GUILayout.Label(job.Summary, bodyStyle);
        GUILayout.Space(4);
        GUILayout.Label(job.Requirements, portalSmallStyle);
        GUILayout.Space(10);

        // Company Contract Info
        GUILayout.Label("Profil & Kontrak", subStyle);
        if (ver != null && ver.isVerified)
        {
            string report =
                $"• Kompensasi: {job.VerifyCompensation}\n" +
                $"• Jam kerja: {job.VerifyHours}\n" +
                $"• Lingkup: {job.VerifyScope}\n" +
                $"• Dokumen: {job.VerifyDocuments}";
            GUILayout.Box(report, job.HasRedFlags ? portalWarnBoxStyle : portalSafeBoxStyle);
        }
        else
        {
            var verReq = new ActivityRequest(ActivityType.VerifyCompany, job.Company);
            var verPrev = engine.GetPreview(verReq);
            GUILayout.Space(4);
            GUI.enabled = verPrev.Availability.IsAvailable;
            if (GUILayout.Button($"Cek Perusahaan ({verPrev.DurationHours} jam)", buttonStyle, GUILayout.MinWidth(210), GUILayout.ExpandWidth(false), GUILayout.Height(30)))
            {
                pendingRequest = verReq;
                scrollPos = Vector2.zero;
            }
            GUI.enabled = true;
            if (!verPrev.Availability.IsAvailable)
                GUILayout.Label(verPrev.Availability.Reason, portalSmallStyle);
        }

        GUILayout.Space(12);

        // Application / Interview / Offer Actions
        if (app != null)
        {
            if (app.status == ApplicationStatus.NotSent)
            {
                DrawJobApplyFooter(engine, state, job);
            }
            else if (app.status == ApplicationStatus.Scheduled)
            {
                var intReq = new ActivityRequest(ActivityType.AttendInterview, job.Company);
                var intPrev = engine.GetPreview(intReq);
                GUILayout.Box($"Jadwal interview: Hari {app.interviewDay} · {app.interviewHour:00}:00", noticeStyle);
                GUILayout.Space(4);
                GUI.enabled = intPrev.Availability.IsAvailable;
                if (GUILayout.Button($"Masuk Interview ({intPrev.DurationHours} jam)", portalPrimaryBtnStyle, GUILayout.MinWidth(220), GUILayout.ExpandWidth(false), GUILayout.Height(34)))
                {
                    pendingRequest = intReq;
                    scrollPos = Vector2.zero;
                }
                GUI.enabled = true;
                if (!intPrev.Availability.IsAvailable)
                    GUILayout.Label(intPrev.Availability.Reason, portalSmallStyle);
            }
            else if (app.status == ApplicationStatus.Offered)
            {
                GUILayout.Box("Tawaran kerja diterima.", portalSafeBoxStyle);
                GUILayout.Space(4);
                DrawJobOfferToggle(engine, state, job.Company);
            }
            else if (app.status == ApplicationStatus.Rejected)
            {
                string reasons = app.rejectionReasons != null && app.rejectionReasons.Count > 0
                    ? string.Join("\n• ", app.rejectionReasons)
                    : "Tidak dilanjutkan.";
                GUILayout.Box($"Tidak dilanjutkan:\n• {reasons}", portalWarnBoxStyle);
            }
        }
        else
        {
            if (state.day < 4)
            {
                GUILayout.Label("Belum tersedia.", portalSmallStyle);
            }
            else
            {
                GUILayout.Box("Tawaran langsung tanpa interview.", portalWarnBoxStyle);
                GUILayout.Space(4);
                DrawJobOfferToggle(engine, state, CompanyId.AksesPrima);
            }
        }
    }

    void DrawJobApplyFooter(GameLoopEngine engine, GameState state, JobPostingData job)
    {
        var d = state.cvDraft;
        string att = $"CV: E0 ✓  ·  E1 {(d.attachE1 ? "✓" : "–")}  ·  E2 {(d.attachE2 ? "✓" : "–")}";

        GUILayout.BeginHorizontal();
        GUILayout.Label(att, portalSmallStyle, GUILayout.ExpandWidth(true));
        GUILayout.Space(8);
        if (GUILayout.Button("Ubah CV", buttonStyle, GUILayout.Width(96), GUILayout.Height(26)))
            SetLaptopPage(LaptopPage.CvBuilder);
        GUILayout.EndHorizontal();
        GUILayout.Space(6);

        var applyReq = new ActivityRequest(ActivityType.SubmitApplication, job.Company);
        var applyPrev = engine.GetPreview(applyReq);
        GUI.enabled = applyPrev.Availability.IsAvailable;
        if (GUILayout.Button($"Kirim Lamaran ({applyPrev.DurationHours} jam)", portalPrimaryBtnStyle, GUILayout.MinWidth(215), GUILayout.ExpandWidth(false), GUILayout.Height(34)))
        {
            pendingRequest = applyReq;
            scrollPos = Vector2.zero;
        }
        GUI.enabled = true;
        GUILayout.Space(2);
        GUILayout.Label(applyPrev.Availability.IsAvailable
            ? $"{applyPrev.StartHour:00}:00 → {applyPrev.EndHour:00}:00  ·  Tenaga {applyPrev.EnergyBefore}→{applyPrev.EnergyAfter}"
            : applyPrev.Availability.Reason, portalSmallStyle);
    }

    void DrawJobOfferToggle(GameLoopEngine engine, GameState state, CompanyId company)
    {
        OfferChoice target = company == CompanyId.StudioLajur ? OfferChoice.StudioLajur
            : company == CompanyId.KoperasiPijar ? OfferChoice.KoperasiPijar
            : OfferChoice.AksesPrima;
        bool isPicked = state.offerChoice == target;

        GUILayout.BeginHorizontal();
        if (isPicked)
        {
            if (GUILayout.Button("Dipilih Sementara ✓  (Batal)", buttonStyle, GUILayout.MinWidth(245), GUILayout.ExpandWidth(false), GUILayout.Height(32)))
                engine.WithdrawOfferChoice();
        }
        else
        {
            if (GUILayout.Button("Simpan Pilihan", portalPrimaryBtnStyle, GUILayout.MinWidth(180), GUILayout.ExpandWidth(false), GUILayout.Height(32)))
                engine.SetOfferChoice(target);
        }
        GUILayout.EndHorizontal();
    }
}
