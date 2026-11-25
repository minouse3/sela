using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using SELA.Core;
using SELA.UI;

public class CoreLoopDebugUI : MonoBehaviour
{
    public static bool IsPanelOpen { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        IsPanelOpen = false;
    }

    [SerializeField] bool startPanelOpen = false;

    bool panelOpen;
    string activeSection = "Overview";
    Vector2 scrollPos;

    ActivityRequest? pendingRequest;
    bool interviewCorrectClaims = true;
    int interviewQ1Choice;
    int interviewQ2Choice;

    Texture2D paperTex;
    Texture2D borderTex;
    Texture2D cardTex;
    Texture2D btnNormalTex;
    Texture2D btnHoverTex;
    Texture2D btnActiveTex;
    Texture2D noticeTex;
    Texture2D warnTex;

    GUIStyle panelStyle;
    GUIStyle cardStyle;
    GUIStyle noticeStyle;
    GUIStyle warnStyle;
    GUIStyle headerStyle;
    GUIStyle subStyle;
    GUIStyle bodyStyle;
    GUIStyle mutedStyle;
    GUIStyle buttonStyle;
    GUIStyle toggleStyle;

    static readonly Color InkColor = new Color(0.176f, 0.227f, 0.204f, 1f);
    static readonly Color MutedInk = new Color(0.353f, 0.400f, 0.376f, 1f);

    void Start()
    {
        panelOpen = startPanelOpen;
        IsPanelOpen = panelOpen;
        if (GameLoopController.Instance != null)
            GameLoopController.Instance.OnWorldFocusRequested += OpenStation;
        SyncCursorAndBlock();
    }

    void OnDestroy()
    {
        if (GameLoopController.Instance != null)
            GameLoopController.Instance.OnWorldFocusRequested -= OpenStation;
        DestroyTextures();
        IsPanelOpen = false;
        PlayerInteractor.Blocked = false;
    }

    void DestroyTextures()
    {
        if (paperTex != null) Destroy(paperTex);
        if (borderTex != null) Destroy(borderTex);
        if (cardTex != null) Destroy(cardTex);
        if (btnNormalTex != null) Destroy(btnNormalTex);
        if (btnHoverTex != null) Destroy(btnHoverTex);
        if (btnActiveTex != null) Destroy(btnActiveTex);
        if (noticeTex != null) Destroy(noticeTex);
        if (warnTex != null) Destroy(warnTex);
    }

    void Update()
    {
        if (PauseMenuController.IsPaused || SleepMenuController.IsOpen || SleepMenuController.IsSleeping) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.f1Key.wasPressedThisFrame || kb.tabKey.wasPressedThisFrame)
        {
            if (!panelOpen && PlayerInteractor.Blocked) return;
            panelOpen = !panelOpen;
            if (!panelOpen) pendingRequest = null;
            SyncCursorAndBlock();
        }
        else if (panelOpen && kb.escapeKey.wasPressedThisFrame)
        {
            if (pendingRequest.HasValue)
            {
                pendingRequest = null;
            }
            else if (GameLoopController.Instance != null &&
                     GameLoopController.Instance.State.phase == GamePhase.FreeRoam)
            {
                panelOpen = false;
                SyncCursorAndBlock();
            }
        }

        var ctrl = GameLoopController.Instance;
        if (ctrl != null && ctrl.State != null && ctrl.State.phase != GamePhase.FreeRoam && !panelOpen)
        {
            panelOpen = true;
            SyncCursorAndBlock();
        }
    }

    void OpenStation(string station)
    {
        if (PauseMenuController.IsPaused) return;
        activeSection = station;
        panelOpen = true;
        pendingRequest = null;
        SyncCursorAndBlock();
    }

    void SyncCursorAndBlock()
    {
        IsPanelOpen = panelOpen;
        PlayerInteractor.Blocked = panelOpen;
        Cursor.lockState = panelOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = panelOpen;
    }

    void EnsureStyles()
    {
        if (paperTex == null)
        {
            paperTex = MakeTex(new Color(0.949f, 0.906f, 0.812f, 0.98f));
            borderTex = MakeTex(new Color(0.176f, 0.227f, 0.204f, 1f));
            cardTex = MakeTex(new Color(0.914f, 0.867f, 0.769f, 1f));
            btnNormalTex = MakeTex(new Color(0.980f, 0.953f, 0.890f, 1f));
            btnHoverTex = MakeTex(new Color(0.902f, 0.835f, 0.706f, 1f));
            btnActiveTex = MakeTex(new Color(0.835f, 0.745f, 0.584f, 1f));
            noticeTex = MakeTex(new Color(0.878f, 0.827f, 0.718f, 1f));
            warnTex = MakeTex(new Color(0.937f, 0.808f, 0.753f, 1f));
        }

        if (panelStyle == null)
        {
            panelStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = paperTex, textColor = InkColor },
                padding = new RectOffset(16, 16, 14, 14)
            };
            cardStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = cardTex, textColor = InkColor },
                padding = new RectOffset(10, 10, 8, 8)
            };
            noticeStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = noticeTex, textColor = InkColor },
                wordWrap = true,
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(10, 10, 6, 6)
            };
            warnStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = warnTex, textColor = new Color(0.45f, 0.14f, 0.08f, 1f) },
                wordWrap = true,
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(10, 10, 6, 6)
            };
            headerStyle = new GUIStyle(GUI.skin.label)
            {
                richText = true,
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                normal = { textColor = InkColor }
            };
            subStyle = new GUIStyle(GUI.skin.label)
            {
                richText = true,
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = InkColor }
            };
            bodyStyle = new GUIStyle(GUI.skin.label)
            {
                richText = true,
                fontSize = 13,
                wordWrap = true,
                normal = { textColor = InkColor }
            };
            mutedStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                wordWrap = true,
                normal = { textColor = MutedInk }
            };
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(10, 10, 6, 6),
                normal = { background = btnNormalTex, textColor = InkColor },
                hover = { background = btnHoverTex, textColor = InkColor },
                active = { background = btnActiveTex, textColor = InkColor },
                onNormal = { background = btnHoverTex, textColor = InkColor }
            };
            toggleStyle = new GUIStyle(GUI.skin.toggle)
            {
                fontSize = 13,
                wordWrap = true,
                normal = { textColor = InkColor },
                hover = { textColor = InkColor },
                onNormal = { textColor = InkColor },
                onHover = { textColor = InkColor }
            };
        }
    }

    static Texture2D MakeTex(Color c)
    {
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, c);
        tex.Apply();
        return tex;
    }

    void OnGUI()
    {
        if (!panelOpen) return;
        if (PauseMenuController.IsPaused || SleepMenuController.IsOpen || SleepMenuController.IsSleeping) return;

        var ctrl = GameLoopController.Instance;
        if (ctrl == null || ctrl.Engine == null || ctrl.State == null) return;

        EnsureStyles();

        float baseScale = Mathf.Min(Screen.width / 1152f, Screen.height / 648f);
        float uiScale = Mathf.Clamp(baseScale * GameSettings.TextScaleMultiplier, 0.75f, 2.8f);
        float vWidth = Screen.width / uiScale;
        float vHeight = Screen.height / uiScale;

        Matrix4x4 prevMatrix = GUI.matrix;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(uiScale, uiScale, 1f));

        var engine = ctrl.Engine;
        var state = ctrl.State;

        float w = Mathf.Min(760f, vWidth - 48f);
        float h = Mathf.Min(520f, vHeight - 64f);
        Rect panelRect = new Rect((vWidth - w) * 0.5f, (vHeight - h) * 0.5f, w, h);

        GUI.DrawTexture(new Rect(panelRect.x + 5f, panelRect.y + 5f, panelRect.width, panelRect.height), borderTex);
        GUI.DrawTexture(new Rect(panelRect.x - 2f, panelRect.y - 2f, panelRect.width + 4f, panelRect.height + 4f), borderTex);
        GUI.Box(panelRect, "", panelStyle);

        GUILayout.BeginArea(new Rect(panelRect.x + 18f, panelRect.y + 14f, panelRect.width - 36f, panelRect.height - 28f));

        GUILayout.BeginHorizontal();
        GUILayout.Label($"Hari {state.day}  ·  {state.FormattedTime} ({state.session})", headerStyle);
        GUILayout.FlexibleSpace();
        if (state.phase == GamePhase.FreeRoam && GUILayout.Button("Tutup [Esc]", buttonStyle, GUILayout.Width(110)))
        {
            panelOpen = false;
            pendingRequest = null;
            SyncCursorAndBlock();
        }
        GUILayout.EndHorizontal();

        GUILayout.Label($"Tenaga {state.energy}/4   ·   Tekanan {state.pressure}/6   ·   {SectionLabel(activeSection)}", mutedStyle);
        GUILayout.Space(6);

        if (!string.IsNullOrEmpty(engine.LastStatusMessage))
        {
            GUILayout.Box(engine.LastStatusMessage, noticeStyle);
            GUILayout.Space(6);
        }

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        switch (state.phase)
        {
            case GamePhase.AwaitingJeda:
                DrawJedaView(engine);
                break;
            case GamePhase.InRift:
                DrawRiftView(engine, state);
                break;
            case GamePhase.FinalDecision:
                DrawFinalDecisionView(engine, state);
                break;
            case GamePhase.Epilogue:
                DrawEpilogueView(engine, state);
                break;
            default:
                if (pendingRequest.HasValue)
                    DrawConfirmationView(engine, state, pendingRequest.Value);
                else
                    DrawFreeRoamControlPanel(engine, state);
                break;
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();

        GUI.matrix = prevMatrix;
    }

    static string SectionLabel(string section)
    {
        switch (section)
        {
            case "Overview": return "Ringkasan";
            case "Laptop": return "Laptop";
            case "EvidenceBoard": return "Papan Bukti";
            case "Bathroom": return "Kamar Mandi";
            case "Bed": return "Kasur";
            case "Phone": return "Ponsel";
            case "DitaRoom": return "Kamar Dita";
            case "BagAndOffers": return "Map Tawaran";
            case "Save": return "Simpanan";
            default: return section;
        }
    }

    void DrawFreeRoamControlPanel(GameLoopEngine engine, GameState state)
    {
        GUILayout.BeginHorizontal();
        string[] tabs = { "Overview", "Laptop", "EvidenceBoard", "Bathroom", "Bed", "Phone", "BagAndOffers", "Save" };
        foreach (string tab in tabs)
        {
            GUI.enabled = activeSection != tab;
            if (GUILayout.Button(SectionLabel(tab), buttonStyle)) activeSection = tab;
            GUI.enabled = true;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(8);

        if (activeSection == "Overview" || activeSection == "Calendar" || activeSection == "Desk" || activeSection == "WindowChair")
        {
            GUILayout.Label("Portofolio", subStyle);
            DrawActivityButton(engine, new ActivityRequest(ActivityType.ImproveUiExample));
            DrawActivityButton(engine, new ActivityRequest(ActivityType.CompleteTestNotes));

            GUILayout.Space(8);
            GUILayout.Label("Istirahat & Pemulihan", subStyle);
            DrawActivityButton(engine, new ActivityRequest(ActivityType.Rest));
            DrawActivityButton(engine, new ActivityRequest(ActivityType.Shower));
            DrawActivityButton(engine, new ActivityRequest(ActivityType.CallDita));
            DrawActivityButton(engine, new ActivityRequest(ActivityType.SitWithoutScreen));
            DrawActivityButton(engine, new ActivityRequest(ActivityType.TidyDesk));

            GUILayout.Space(8);
            GUILayout.Label("Akhir Hari", subStyle);
            DrawActivityButton(engine, new ActivityRequest(ActivityType.FinalPrep));
            DrawSleepSection(engine);
        }
        else if (activeSection == "Bed")
        {
            DrawActivityButton(engine, new ActivityRequest(ActivityType.Rest));
            GUILayout.Space(8);
            DrawSleepSection(engine);
        }
        else if (activeSection == "Bathroom")
        {
            GUILayout.Label("Mandi 1 jam (+2 Tenaga, -3 Tekanan). Sekali sehari.", mutedStyle);
            GUILayout.Space(4);
            DrawActivityButton(engine, new ActivityRequest(ActivityType.Shower));
        }
        else if (activeSection == "Phone" || activeSection == "DitaRoom")
        {
            GUILayout.Label("Ngobrol sama Dita di Kamar 06 atau lewat pesan (2 jam).", mutedStyle);
            GUILayout.Space(4);
            DrawActivityButton(engine, new ActivityRequest(ActivityType.CallDita));
        }
        else if (activeSection == "EvidenceBoard")
        {
            GUILayout.Label($"Bukti: E0 {(state.hasE0 ? "✓" : "–")}   ·   E1 {(state.hasE1 ? "✓" : "–")}   ·   E2 {(state.hasE2 ? "✓" : "–")}", bodyStyle);
            DrawActivityButton(engine, new ActivityRequest(ActivityType.ImproveUiExample));
            DrawActivityButton(engine, new ActivityRequest(ActivityType.CompleteTestNotes));
            GUILayout.Space(8);
            DrawCvDraftEditor(engine, state);
        }
        else if (activeSection == "Laptop")
        {
            DrawCvDraftEditor(engine, state);
            GUILayout.Space(8);
            GUILayout.Label("Lowongan & Interview", subStyle);
            DrawCompanyBlock(engine, state, CompanyId.StudioLajur);
            DrawCompanyBlock(engine, state, CompanyId.KoperasiPijar);
            DrawCompanyBlock(engine, state, CompanyId.AksesPrima);
            GUILayout.Space(8);
            DrawOfferChoiceSection(engine, state);
        }
        else if (activeSection == "BagAndOffers")
        {
            DrawOfferChoiceSection(engine, state);
            GUILayout.Space(8);
            GUILayout.Label("Keputusan Akhir (mulai Hari 4)", subStyle);
            DrawActivityButton(engine, new ActivityRequest(ActivityType.FinalPrep));
        }
        else if (activeSection == "Save")
        {
            GUILayout.Label($"Autosave: {(engine.Save.HasAutosave() ? "Tersedia" : "Kosong")}", bodyStyle);
            GUILayout.Label($"Awal Hari: {(engine.Save.HasDayStartCheckpoint() ? "Tersedia" : "Kosong")}", bodyStyle);
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Muat Autosave", buttonStyle)) engine.LoadAutosave(true);
            if (GUILayout.Button("Ulangi Awal Hari", buttonStyle)) engine.LoadDayStartCheckpoint();
            if (GUILayout.Button("Mulai Baru", buttonStyle)) engine.StartNewGame(true);
            GUILayout.EndHorizontal();
        }
    }

    void DrawActivityButton(GameLoopEngine engine, ActivityRequest req)
    {
        var preview = engine.GetPreview(req);
        GUILayout.BeginHorizontal();
        GUI.enabled = preview.Availability.IsAvailable;
        string warn = preview.Availability.WillMissScheduledInterview ? " [LEWAT INTERVIEW]" : "";
        if (preview.WillTriggerRift > 0) warn += $" [RETAKAN {preview.WillTriggerRift}]";
        if (GUILayout.Button($"{preview.Title} ({preview.DurationHours}j){warn}", buttonStyle, GUILayout.Width(285)))
            pendingRequest = req;
        GUI.enabled = true;

        string info = preview.Availability.IsAvailable
            ? $"{preview.StartHour:00}:00 → {preview.EndHour:00}:00  ·  Tenaga {preview.EnergyBefore}→{preview.EnergyAfter}  ·  Tekanan {preview.PressureBefore}→{preview.PressureAfter}"
            : preview.Availability.Reason;
        GUILayout.Label(info, preview.Availability.IsAvailable ? bodyStyle : mutedStyle);
        GUILayout.EndHorizontal();
    }

    void DrawSleepSection(GameLoopEngine engine)
    {
        GUILayout.Label("Tidur", subStyle);
        DrawSleepOptionButton(engine, SleepTarget.UntilNoon);
        DrawSleepOptionButton(engine, SleepTarget.UntilNight);
        DrawSleepOptionButton(engine, SleepTarget.UntilMorning);
    }

    void DrawSleepOptionButton(GameLoopEngine engine, SleepTarget target)
    {
        var p = engine.GetSleepPreview(target);
        GUILayout.BeginHorizontal();
        GUI.enabled = p.IsAvailable;
        string warn = p.WillMissScheduledInterview ? " [LEWAT INTERVIEW]" : "";
        if (GUILayout.Button($"{p.Label}{warn}", buttonStyle, GUILayout.Width(285)))
            engine.SleepUntil(target);
        GUI.enabled = true;

        string info = p.IsAvailable
            ? (p.OpensFinalDecision
                ? "Buka Keputusan Akhir"
                : $"Tenaga {p.EnergyBefore}→{p.EnergyAfter}  ·  Tekanan {p.PressureBefore}→{p.PressureAfter}")
            : p.UnavailableReason;
        GUILayout.Label(info, p.IsAvailable ? bodyStyle : mutedStyle);
        GUILayout.EndHorizontal();
    }

    void DrawCvDraftEditor(GameLoopEngine engine, GameState state)
    {
        GUILayout.Label("Draf CV (tanpa menghabiskan waktu)", subStyle);
        var d = state.cvDraft;
        bool c1Exag = GUILayout.Toggle(d.claim1 == ClaimChoice.Exaggerated, "Klaim 1 berlebihan: pernah pimpin sistem produksi", toggleStyle);
        bool c2Exag = GUILayout.Toggle(d.claim2 == ClaimChoice.Exaggerated, "Klaim 2 berlebihan: proyek bebas bug sepenuhnya", toggleStyle);

        GUI.enabled = state.hasE1;
        bool attE1 = GUILayout.Toggle(d.attachE1, "Lampirkan Bukti E1 (contoh antarmuka)", toggleStyle);
        GUI.enabled = state.hasE2;
        bool attE2 = GUILayout.Toggle(d.attachE2, "Lampirkan Bukti E2 (catatan pengujian)", toggleStyle);
        GUI.enabled = true;

        ClaimChoice nextC1 = c1Exag ? ClaimChoice.Exaggerated : ClaimChoice.Honest;
        ClaimChoice nextC2 = c2Exag ? ClaimChoice.Exaggerated : ClaimChoice.Honest;
        if (nextC1 != d.claim1 || nextC2 != d.claim2 || attE1 != d.attachE1 || attE2 != d.attachE2)
            engine.UpdateCvDraft(nextC1, nextC2, attE1, attE2);
    }

    void DrawCompanyBlock(GameLoopEngine engine, GameState state, CompanyId company)
    {
        var ver = state.GetVerification(company);
        var app = state.GetApplication(company);
        string status = app != null ? app.status.ToString() : (state.day >= 4 ? "Tawaran langsung" : "Masuk Hari 4");
        GUILayout.BeginVertical(cardStyle);
        GUILayout.Label($"{ActivitySystem.GetCompanyName(company)}  ·  {status}  ·  Cek: {(ver.isVerified ? "Sudah" : "Belum")}", subStyle);
        DrawActivityButton(engine, new ActivityRequest(ActivityType.VerifyCompany, company));
        if (app != null)
        {
            DrawActivityButton(engine, new ActivityRequest(ActivityType.SubmitApplication, company));
            DrawActivityButton(engine, new ActivityRequest(ActivityType.AttendInterview, company));
        }
        GUILayout.EndVertical();
        GUILayout.Space(4);
    }

    void DrawOfferChoiceSection(GameLoopEngine engine, GameState state)
    {
        GUILayout.Label($"Pilihan sementara: {state.offerChoice}", subStyle);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Pulang", buttonStyle)) engine.WithdrawOfferChoice();
        GUI.enabled = state.HasOffer(CompanyId.StudioLajur);
        if (GUILayout.Button("Studio Lajur", buttonStyle)) engine.SetOfferChoice(OfferChoice.StudioLajur);
        GUI.enabled = state.HasOffer(CompanyId.KoperasiPijar);
        if (GUILayout.Button("Koperasi Pijar", buttonStyle)) engine.SetOfferChoice(OfferChoice.KoperasiPijar);
        GUI.enabled = state.HasOffer(CompanyId.AksesPrima);
        if (GUILayout.Button("Akses Prima (Berisiko)", buttonStyle)) engine.SetOfferChoice(OfferChoice.AksesPrima);
        GUI.enabled = true;
        GUILayout.EndHorizontal();
    }

    void DrawConfirmationView(GameLoopEngine engine, GameState state, ActivityRequest req)
    {
        var preview = engine.GetPreview(req);
        GUILayout.Label($"{preview.Title} ({preview.DurationHours} jam)", headerStyle);
        GUILayout.Label($"{preview.Location}  ·  {preview.StartHour:00}:00 → {preview.EndHour:00}:00 ({preview.NextSession})", mutedStyle);
        GUILayout.Label($"Tenaga {preview.EnergyBefore} → {preview.EnergyAfter}   ·   Tekanan {preview.PressureBefore} → {preview.PressureAfter}", bodyStyle);

        if (preview.WillTriggerRift > 0)
            GUILayout.Box($"Tekanan naik ke batas Retakan {preview.WillTriggerRift}.", warnStyle);
        else if (preview.WillTriggerJeda)
            GUILayout.Box("Tekanan mencapai puncak — perlu ambil Jeda.", warnStyle);

        if (preview.Availability.WillMissScheduledInterview)
            GUILayout.Box($"Melewati interview {ActivitySystem.GetCompanyName(preview.Availability.MissedInterviewCompany)} jam 13:00 (lamaran gugur, +1 Tekanan).", warnStyle);

        if (req.Type == ActivityType.AttendInterview)
        {
            var app = state.GetApplication(req.Company);
            GUILayout.Space(6);
            GUILayout.Label("Jawaban Interview", subStyle);
            if (app != null && app.SnapshotHasExaggeration)
                interviewCorrectClaims = GUILayout.Toggle(interviewCorrectClaims, "Luruskan klaim CV yang berlebihan di awal", toggleStyle);

            GUILayout.Label("Pertanyaan 1:", bodyStyle);
            interviewQ1Choice = GUILayout.SelectionGrid(interviewQ1Choice, new[] { "Tunjukkan detail + bukti", "Jelaskan secara umum", "Jawab singkat tanpa bukti" }, 1, buttonStyle);
            GUILayout.Label("Pertanyaan 2:", bodyStyle);
            interviewQ2Choice = GUILayout.SelectionGrid(interviewQ2Choice, new[] { "Tanya batas tugas & pendampingan", "Minta contoh tugas dulu", "Iya-kan semua dulu" }, 1, buttonStyle);
        }

        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Jalankan", buttonStyle, GUILayout.Height(32)))
        {
            InterviewInput? input = req.Type == ActivityType.AttendInterview
                ? new InterviewInput
                {
                    CorrectExaggeratedClaims = interviewCorrectClaims,
                    Question1ChoiceIndex = interviewQ1Choice,
                    Question2ChoiceIndex = interviewQ2Choice
                }
                : (InterviewInput?)null;

            engine.ExecuteActivity(req, input);
            pendingRequest = null;
        }
        if (GUILayout.Button("Batal", buttonStyle, GUILayout.Height(32)))
            pendingRequest = null;
        GUILayout.EndHorizontal();
    }

    void DrawJedaView(GameLoopEngine engine)
    {
        GUILayout.Label("Tarik napas dulu.", headerStyle);
        GUILayout.Label("Tekanan memuncak. Ambil jeda sebentar (Tekanan turun ke 4, waktu tidak maju).", bodyStyle);
        GUILayout.Space(8);
        if (GUILayout.Button("Duduk sebentar", buttonStyle, GUILayout.Height(34)))
            engine.ResolveJeda(JedaChoice.SitBriefly);
        if (GUILayout.Button("Kirim pesan ke Dita", buttonStyle, GUILayout.Height(34)))
            engine.ResolveJeda(JedaChoice.MessageDita);
    }

    void DrawRiftView(GameLoopEngine engine, GameState state)
    {
        string riftTitle = state.activeRiftIndex == 1 ? "Retakan 1: Meja yang Menjauh"
            : state.activeRiftIndex == 2 ? "Retakan 2: Pintu 07"
            : "Retakan 3: Kursi Kosong";

        GUILayout.Label(riftTitle, headerStyle);
        GUILayout.Label("Kamar terasa menjauh. Pegang benda jangkar untuk tenang lagi (Tekanan turun ke 3).", bodyStyle);
        GUILayout.Space(8);
        if (GUILayout.Button("Selesaikan Retakan", buttonStyle, GUILayout.Height(34)))
            engine.ResolveRift(skippedViaHelp: false);
        if (GUILayout.Button("Lewati adegan ini", buttonStyle, GUILayout.Height(30)))
            engine.ResolveRift(skippedViaHelp: true);
    }

    void DrawFinalDecisionView(GameLoopEngine engine, GameState state)
    {
        GUILayout.Label($"Hari {state.day}  ·  Keputusan Akhir", headerStyle);
        DrawOfferChoiceSection(engine, state);
        GUILayout.Space(8);

        GUILayout.Label("Catatan keputusan:", subStyle);
        if (state.offerChoice == OfferChoice.StudioLajur || state.offerChoice == OfferChoice.KoperasiPijar)
        {
            if (GUILayout.Button("Minta batas tugas dan pendampingan jelas", buttonStyle))
                engine.SetDecisionNote(DecisionNoteType.WorkMentoringAndScope);
            if (GUILayout.Button("Minta persetujuan tertulis untuk lembur", buttonStyle))
                engine.SetDecisionNote(DecisionNoteType.WorkOvertimeApproval);
        }
        else if (state.offerChoice == OfferChoice.None)
        {
            if (GUILayout.Button("Pulang dulu, rapikan bukti proyek", buttonStyle))
                engine.SetDecisionNote(DecisionNoteType.HomeOrganizeEvidence);
            if (GUILayout.Button("Pulang dulu, atur jadwal lamaran pelan-pelan", buttonStyle))
                engine.SetDecisionNote(DecisionNoteType.HomeLimitedSchedule);
        }
        else if (state.offerChoice == OfferChoice.AksesPrima)
        {
            GUILayout.Box("Akses Prima menahan ijazah asli dan membatasi pengunduran diri.", warnStyle);
            if (GUILayout.Button("Saya paham risikonya", buttonStyle))
                engine.SetDecisionNote(DecisionNoteType.RiskyOfferAcknowledged);
        }

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Kunci Keputusan", buttonStyle, GUILayout.Height(34)))
            engine.ConfirmFinalDecision();
        if (GUILayout.Button("Tinjau Lagi", buttonStyle, GUILayout.Height(34)))
            engine.ReturnFromFinalDecision();
        GUILayout.EndHorizontal();
    }

    void DrawEpilogueView(GameLoopEngine engine, GameState state)
    {
        GUILayout.Label($"Epilog  ·  {state.ending}", headerStyle);
        GUILayout.Label($"Hari {state.day} · {state.FormattedTime}  |  Pilihan: {state.offerChoice}", bodyStyle);
        GUILayout.Label($"Bukti: E0={(state.hasE0 ? "✓" : "–")}, E1={(state.hasE1 ? "✓" : "–")}, E2={(state.hasE2 ? "✓" : "–")}", mutedStyle);
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Tinjau Keputusan Terakhir", buttonStyle, GUILayout.Height(32)))
            engine.LoadAutosave(true);
        if (GUILayout.Button("Mulai Baru", buttonStyle, GUILayout.Height(32)))
            engine.StartNewGame(true);
        if (Application.CanStreamedLevelBeLoaded("MainMenu") && GUILayout.Button("Menu Utama", buttonStyle, GUILayout.Height(32)))
        {
            IsPanelOpen = false;
            PlayerInteractor.Blocked = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("MainMenu");
        }
        GUILayout.EndHorizontal();
    }
}
