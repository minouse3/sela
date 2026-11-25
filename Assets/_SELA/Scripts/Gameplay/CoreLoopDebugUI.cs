using UnityEngine;
using UnityEngine.InputSystem;
using SELA.Core;

/// <summary>
/// Temporary developer OnGUI overlay so the revised SELA core loop can be playtested
/// in Play Mode before final uGUI screens are built.
/// Press Tab or F1 (or interact with room props) to toggle the control panel.
/// </summary>
public class CoreLoopDebugUI : MonoBehaviour
{
    [SerializeField] bool startPanelOpen = false;

    bool panelOpen;
    string activeSection = "Overview";
    Vector2 scrollPos;

    // Pending confirmation / interview inputs.
    ActivityRequest? pendingRequest;
    bool interviewCorrectClaims = true;
    int interviewQ1Choice;
    int interviewQ2Choice;

    void Start()
    {
        panelOpen = startPanelOpen;
        if (GameLoopController.Instance != null)
            GameLoopController.Instance.OnWorldFocusRequested += OpenStation;
        SyncCursorAndBlock();
    }

    void OnDestroy()
    {
        if (GameLoopController.Instance != null)
            GameLoopController.Instance.OnWorldFocusRequested -= OpenStation;
        PlayerInteractor.Blocked = false;
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.f1Key.wasPressedThisFrame || kb.tabKey.wasPressedThisFrame)
        {
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

        // Auto-open overlay when Jeda, Rift, FinalDecision, or Epilogue requires input.
        var ctrl = GameLoopController.Instance;
        if (ctrl != null && ctrl.State != null && ctrl.State.phase != GamePhase.FreeRoam && !panelOpen)
        {
            panelOpen = true;
            SyncCursorAndBlock();
        }
    }

    void OpenStation(string station)
    {
        activeSection = station;
        panelOpen = true;
        pendingRequest = null;
        SyncCursorAndBlock();
    }

    void SyncCursorAndBlock()
    {
        PlayerInteractor.Blocked = panelOpen;
        Cursor.lockState = panelOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = panelOpen;
    }

    void OnGUI()
    {
        var ctrl = GameLoopController.Instance;
        if (ctrl == null || ctrl.Engine == null || ctrl.State == null) return;

        var engine = ctrl.Engine;
        var state = ctrl.State;

        DrawCompactHud(state);

        if (!panelOpen) return;

        float w = Mathf.Min(680f, Screen.width - 40f);
        float h = Mathf.Min(580f, Screen.height - 105f);
        Rect panelRect = new Rect((Screen.width - w) * 0.5f, 88f, w, h);

        GUI.Box(panelRect, "");
        GUILayout.BeginArea(new Rect(panelRect.x + 12f, panelRect.y + 10f, panelRect.width - 24f, panelRect.height - 20f));

        GUILayout.BeginHorizontal();
        GUILayout.Label($"<b>SELA Core Loop Prototype</b> — {activeSection}", RichHeader());
        GUILayout.FlexibleSpace();
        if (state.phase == GamePhase.FreeRoam && GUILayout.Button("Tutup [Tab / Esc]", GUILayout.Width(130)))
        {
            panelOpen = false;
            pendingRequest = null;
            SyncCursorAndBlock();
        }
        GUILayout.EndHorizontal();

        if (!string.IsNullOrEmpty(engine.LastStatusMessage))
        {
            GUI.color = new Color(1f, 0.95f, 0.75f);
            GUILayout.Box(engine.LastStatusMessage);
            GUI.color = Color.white;
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
    }

    void DrawCompactHud(GameState state)
    {
        string energyBoxes = new string('■', state.energy) + new string('□', GameState.MaxEnergy - state.energy);
        string pressureBars = new string('|', state.pressure) + new string('·', GameState.MaxPressure - state.pressure);
        string objective = ActivitySystem.GetHudObjectiveHint(state);

        GUI.Box(new Rect(14, 10, 520, 70), "");
        GUI.Label(new Rect(24, 14, 500, 20),
            $"Hari {state.day}/7 · {state.FormattedTime} ({state.session})   |   Fase: {state.phase}");
        GUI.Label(new Rect(24, 34, 500, 20),
            $"Tenaga [{energyBoxes}] {state.energy}/4    Tekanan [{pressureBars}] {state.pressure}/6    [Tab/F1 Panel]");
        GUI.Label(new Rect(24, 54, 500, 20),
            $"Tujuan: {objective}");
    }

    void DrawFreeRoamControlPanel(GameLoopEngine engine, GameState state)
    {
        GUILayout.BeginHorizontal();
        string[] tabs = { "Overview", "Laptop", "EvidenceBoard", "Bathroom", "Bed", "Phone", "BagAndOffers", "Save" };
        foreach (string tab in tabs)
        {
            GUI.enabled = activeSection != tab;
            if (GUILayout.Button(tab)) activeSection = tab;
            GUI.enabled = true;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(6);

        if (activeSection == "Overview" || activeSection == "Calendar" || activeSection == "Desk" || activeSection == "WindowChair")
        {
            GUILayout.Label("<b>Aktivitas Portofolio (Papan Bukti)</b>", RichSub());
            DrawActivityButton(engine, new ActivityRequest(ActivityType.ImproveUiExample));
            DrawActivityButton(engine, new ActivityRequest(ActivityType.CompleteTestNotes));

            GUILayout.Space(6);
            GUILayout.Label("<b>Aktivitas Pemulihan</b>", RichSub());
            DrawActivityButton(engine, new ActivityRequest(ActivityType.Rest));
            DrawActivityButton(engine, new ActivityRequest(ActivityType.Shower));
            DrawActivityButton(engine, new ActivityRequest(ActivityType.CallDita));
            DrawActivityButton(engine, new ActivityRequest(ActivityType.SitWithoutScreen));
            DrawActivityButton(engine, new ActivityRequest(ActivityType.TidyDesk));

            GUILayout.Space(6);
            GUILayout.Label("<b>Penutup Hari & Keputusan Akhir</b>", RichSub());
            DrawActivityButton(engine, new ActivityRequest(ActivityType.FinalPrep));
            DrawSleepSection(engine, state);
        }
        else if (activeSection == "Bed")
        {
            DrawActivityButton(engine, new ActivityRequest(ActivityType.Rest));
            GUILayout.Space(6);
            DrawSleepSection(engine, state);
        }
        else if (activeSection == "Bathroom")
        {
            GUILayout.Label("Kamar Mandi: Mandi (1 jam, +2 Tenaga, -3 Tekanan, sekali per hari).");
            DrawActivityButton(engine, new ActivityRequest(ActivityType.Shower));
        }
        else if (activeSection == "Phone" || activeSection == "DitaRoom")
        {
            GUILayout.Label("Kamar Dita (06) / Ponsel: Baca pesan Ibu & Dita gratis waktu, atau Temui / Hubungi Dita (2 jam).");
            DrawActivityButton(engine, new ActivityRequest(ActivityType.CallDita));
        }
        else if (activeSection == "EvidenceBoard")
        {
            GUILayout.Label($"Bukti Dimiliki: E0={(state.hasE0 ? "Ya" : "Tidak")}, E1={(state.hasE1 ? "Ya" : "Tidak")}, E2={(state.hasE2 ? "Ya" : "Tidak")}");
            DrawActivityButton(engine, new ActivityRequest(ActivityType.ImproveUiExample));
            DrawActivityButton(engine, new ActivityRequest(ActivityType.CompleteTestNotes));
            GUILayout.Space(8);
            DrawCvDraftEditor(engine, state);
        }
        else if (activeSection == "Laptop")
        {
            DrawCvDraftEditor(engine, state);
            GUILayout.Space(8);
            GUILayout.Label("<b>Lowongan, Verifikasi & Interview</b>", RichSub());
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
            GUILayout.Label("<b>Persiapan Keputusan Akhir (tersedia mulai Hari 4)</b>", RichSub());
            DrawActivityButton(engine, new ActivityRequest(ActivityType.FinalPrep));
        }
        else if (activeSection == "Save")
        {
            GUILayout.Label($"Autosave: {(engine.Save.HasAutosave() ? "Ada" : "Belum ada")}");
            GUILayout.Label($"Checkpoint Awal Hari: {(engine.Save.HasDayStartCheckpoint() ? "Ada" : "Belum ada")}");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Muat Autosave")) engine.LoadAutosave(true);
            if (GUILayout.Button("Ulangi Awal Hari")) engine.LoadDayStartCheckpoint();
            if (GUILayout.Button("Mulai Baru (Hari 1)")) engine.StartNewGame(true);
            GUILayout.EndHorizontal();
        }
    }

    void DrawActivityButton(GameLoopEngine engine, ActivityRequest req)
    {
        var preview = engine.GetPreview(req);
        GUILayout.BeginHorizontal();
        GUI.enabled = preview.Availability.IsAvailable;
        string warn = preview.Availability.WillMissScheduledInterview ? " [LEWATKAN INTERVIEW!]" : "";
        if (preview.WillTriggerRift > 0) warn += $" [RETAKAN {preview.WillTriggerRift}!]";
        if (GUILayout.Button($"{preview.Title} ({preview.DurationHours}j){warn}", GUILayout.Width(285)))
            pendingRequest = req;
        GUI.enabled = true;

        string info = preview.Availability.IsAvailable
            ? $"{preview.StartHour:00}:00→{preview.EndHour:00}:00 | Tenaga {preview.EnergyBefore}→{preview.EnergyAfter} | Tekanan {preview.PressureBefore}→{preview.PressureAfter}"
            : preview.Availability.Reason;
        GUILayout.Label(info);
        GUILayout.EndHorizontal();
    }

    void DrawSleepSection(GameLoopEngine engine, GameState state)
    {
        GUI.enabled = state.CanSleepNow;
        string label = state.day >= GameState.MaxDay
            ? "Kasur: Tinjau Keputusan Akhir (Hari 7 · Malam)"
            : $"Kasur: Tidur / Tutup Hari {state.day} → Hari {state.day + 1} 08:00 (Tenaga +2, Tekanan -1)";
        if (GUILayout.Button(label, GUILayout.Height(30)))
            engine.SleepOrOpenFinalDecision();
        GUI.enabled = true;
    }

    void DrawCvDraftEditor(GameLoopEngine engine, GameState state)
    {
        GUILayout.Label("<b>Draf CV (Gratis waktu)</b>", RichSub());
        var d = state.cvDraft;
        bool c1Exag = GUILayout.Toggle(d.claim1 == ClaimChoice.Exaggerated, "Klaim 1 Berlebihan (Memimpin sistem produksi — bukti belum mendukung)");
        bool c2Exag = GUILayout.Toggle(d.claim2 == ClaimChoice.Exaggerated, "Klaim 2 Berlebihan (Menjamin bebas seluruh kesalahan — bukti belum mendukung)");

        GUI.enabled = state.hasE1;
        bool attE1 = GUILayout.Toggle(d.attachE1, "Lampirkan Bukti E1 (Antarmuka formulir)");
        GUI.enabled = state.hasE2;
        bool attE2 = GUILayout.Toggle(d.attachE2, "Lampirkan Bukti E2 (Catatan pengujian)");
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
        string status = app != null ? app.status.ToString() : (state.day >= 4 ? "Tawaran Langsung Aktif" : "Belum Masuk (Hari 4)");
        GUILayout.BeginVertical("box");
        GUILayout.Label($"<b>{ActivitySystem.GetCompanyName(company)}</b> — Status: {status} | Verifikasi: {(ver.isVerified ? "Sudah" : "Belum")}", RichSub());
        DrawActivityButton(engine, new ActivityRequest(ActivityType.VerifyCompany, company));
        if (app != null)
        {
            DrawActivityButton(engine, new ActivityRequest(ActivityType.SubmitApplication, company));
            DrawActivityButton(engine, new ActivityRequest(ActivityType.AttendInterview, company));
        }
        GUILayout.EndVertical();
    }

    void DrawOfferChoiceSection(GameLoopEngine engine, GameState state)
    {
        GUILayout.Label($"<b>Pilihan Tawaran Sementara:</b> {state.offerChoice}", RichSub());
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Pulang (None)")) engine.WithdrawOfferChoice();
        GUI.enabled = state.HasOffer(CompanyId.StudioLajur);
        if (GUILayout.Button("Pilih Studio Lajur")) engine.SetOfferChoice(OfferChoice.StudioLajur);
        GUI.enabled = state.HasOffer(CompanyId.KoperasiPijar);
        if (GUILayout.Button("Pilih Koperasi Pijar")) engine.SetOfferChoice(OfferChoice.KoperasiPijar);
        GUI.enabled = state.HasOffer(CompanyId.AksesPrima);
        if (GUILayout.Button("Pilih Akses Prima [Berisiko]")) engine.SetOfferChoice(OfferChoice.AksesPrima);
        GUI.enabled = true;
        GUILayout.EndHorizontal();
    }

    void DrawConfirmationView(GameLoopEngine engine, GameState state, ActivityRequest req)
    {
        var preview = engine.GetPreview(req);
        GUILayout.Label($"<b>Konfirmasi Aktivitas ({preview.DurationHours} Jam)</b>", RichSub());
        GUILayout.Label($"Aktivitas : {preview.Title}");
        GUILayout.Label($"Lokasi    : {preview.Location}");
        GUILayout.Label($"Waktu     : {preview.StartHour:00}:00 → {preview.EndHour:00}:00 ({preview.NextSession})");
        GUILayout.Label($"Tenaga    : {preview.EnergyBefore} → {preview.EnergyAfter}");

        string pressureNote = preview.WillTriggerRift > 0
            ? $" (Memicu Retakan Ruang {preview.WillTriggerRift} → turun ke 3 setelah selesai)"
            : preview.WillTriggerJeda
                ? " (Memicu Jeda cadangan → turun ke 4)"
                : preview.RejectionCouldTriggerRiftOrJeda
                    ? " (Jika ditolak, +1 Tekanan dapat memicu Retakan/Jeda)"
                    : "";
        GUILayout.Label($"Tekanan   : {preview.PressureBefore} → {preview.PressureAfter}{pressureNote}");

        if (preview.Availability.WillMissScheduledInterview)
        {
            GUI.color = new Color(1f, 0.7f, 0.7f);
            GUILayout.Box($"PERINGATAN: Aktivitas ini melewati interview pukul 13:00 {ActivitySystem.GetCompanyName(preview.Availability.MissedInterviewCompany)} (Ditolak, Tekanan +1).");
            GUI.color = Color.white;
        }

        if (req.Type == ActivityType.AttendInterview)
        {
            var app = state.GetApplication(req.Company);
            GUILayout.Space(6);
            GUILayout.Label("<b>Simulasi Jawaban Interview (GDD §3.7)</b>", RichSub());
            if (app != null && app.SnapshotHasExaggeration)
                interviewCorrectClaims = GUILayout.Toggle(interviewCorrectClaims, "Luruskan seluruh klaim CV berlebihan di awal interview");

            GUILayout.Label("Pertanyaan 1:");
            interviewQ1Choice = GUILayout.SelectionGrid(interviewQ1Choice, new[] { "Jelaskan detail + Bukti (2)", "Penjelasan umum (1)", "Klaim dangkal (0)" }, 1);
            GUILayout.Label("Pertanyaan 2:");
            interviewQ2Choice = GUILayout.SelectionGrid(interviewQ2Choice, new[] { "Batas tugas + pendampingan (2)", "Minta contoh dulu (1)", "Setujui semua dulu (0)" }, 1);
        }

        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Jalankan aktivitas", GUILayout.Height(30)))
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
        if (GUILayout.Button("Batal", GUILayout.Height(30)))
            pendingRequest = null;
        GUILayout.EndHorizontal();
    }

    void DrawJedaView(GameLoopEngine engine)
    {
        GUILayout.Label("<b>Jeda Cadangan (Tekanan mencapai 6)</b>", RichSub());
        GUILayout.Label("Tekanan akan turun ke 4 dan Tenaga minimal 1, tanpa memajukan jam.");
        GUILayout.Space(8);
        if (GUILayout.Button("Duduk sebentar", GUILayout.Height(32)))
            engine.ResolveJeda(JedaChoice.SitBriefly);
        if (GUILayout.Button("Kirim pesan kepada Dita", GUILayout.Height(32)))
            engine.ResolveJeda(JedaChoice.MessageDita);
    }

    void DrawRiftView(GameLoopEngine engine, GameState state)
    {
        string riftTitle = state.activeRiftIndex == 1 ? "Retakan 1: Meja yang Menjauh (Pasang E0)"
            : state.activeRiftIndex == 2 ? "Retakan 2: Pintu Bernama (Gunakan Kunci 07 pada Pintu 07)"
            : "Retakan 3: Kursi Kosong (Pasang Kartu Pesan Ibu di Kursi Jendela)";

        GUILayout.Label($"<b>{riftTitle}</b>", RichSub());
        GUILayout.Label("Dipicu oleh Tekanan ≥ 5. Menyelesaikan Retakan menurunkan Tekanan ke 3 dan Tenaga minimal 1.");
        GUILayout.Space(8);
        if (GUILayout.Button("Selesaikan Puzzle Retakan", GUILayout.Height(32)))
            engine.ResolveRift(skippedViaHelp: false);
        if (GUILayout.Button("Selesaikan Adegan (Bantuan Pause)", GUILayout.Height(28)))
            engine.ResolveRift(skippedViaHelp: true);
    }

    void DrawFinalDecisionView(GameLoopEngine engine, GameState state)
    {
        GUILayout.Label($"<b>Hari {state.day} · {state.FormattedTime} — Keputusan Akhir</b>", RichSub());
        DrawOfferChoiceSection(engine, state);
        GUILayout.Space(8);

        GUILayout.Label($"<b>Catatan Keputusan (`decisionNote`):</b> {state.decisionNote}", RichSub());
        if (state.offerChoice == OfferChoice.StudioLajur || state.offerChoice == OfferChoice.KoperasiPijar)
        {
            if (GUILayout.Button("Catatan: Pendampingan dan lingkup tugas"))
                engine.SetDecisionNote(DecisionNoteType.WorkMentoringAndScope);
            if (GUILayout.Button("Catatan: Persetujuan dan pencatatan lembur"))
                engine.SetDecisionNote(DecisionNoteType.WorkOvertimeApproval);
        }
        else if (state.offerChoice == OfferChoice.None)
        {
            if (GUILayout.Button("Langkah Pulang: Menata bukti proyek"))
                engine.SetDecisionNote(DecisionNoteType.HomeOrganizeEvidence);
            if (GUILayout.Button("Langkah Pulang: Menyusun jadwal lamaran terbatas"))
                engine.SetDecisionNote(DecisionNoteType.HomeLimitedSchedule);
        }
        else if (state.offerChoice == OfferChoice.AksesPrima)
        {
            GUI.color = new Color(1f, 0.7f, 0.7f);
            GUILayout.Box("PERINGATAN BERISIKO: Akses Prima meminta penahanan dokumen asli dan membatasi pengunduran diri.");
            GUI.color = Color.white;
            if (GUILayout.Button("Saya memahami syarat berisiko ini"))
                engine.SetDecisionNote(DecisionNoteType.RiskyOfferAcknowledged);
        }

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Selesaikan Keputusan", GUILayout.Height(32)))
            engine.ConfirmFinalDecision();
        if (GUILayout.Button("Kembali Membaca", GUILayout.Height(32)))
            engine.ReturnFromFinalDecision();
        GUILayout.EndHorizontal();
    }

    void DrawEpilogueView(GameLoopEngine engine, GameState state)
    {
        GUILayout.Label($"<b>EPILOG SELA — {state.ending}</b>", RichHeader());
        GUILayout.Label($"Hari Final    : Hari {state.day} · {state.FormattedTime}");
        GUILayout.Label($"Pilihan Akhir : {state.offerChoice}");
        GUILayout.Label($"Catatan       : {state.decisionNote}");
        GUILayout.Label($"Bukti Selesai : E0={state.hasE0}, E1={state.hasE1}, E2={state.hasE2}");
        GUILayout.Label($"Retakan       : 1={state.IsRiftSolved(1)}, 2={state.IsRiftSolved(2)}, 3={state.IsRiftSolved(3)}");
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Lanjutkan (Kembali ke Keputusan Terakhir)", GUILayout.Height(30)))
            engine.LoadAutosave(true);
        if (GUILayout.Button("Mulai Permainan Baru", GUILayout.Height(30)))
            engine.StartNewGame(true);
        GUILayout.EndHorizontal();
    }

    static GUIStyle RichHeader() => new GUIStyle(GUI.skin.label) { richText = true, fontSize = 15 };
    static GUIStyle RichSub() => new GUIStyle(GUI.skin.label) { richText = true, fontSize = 13 };
}
