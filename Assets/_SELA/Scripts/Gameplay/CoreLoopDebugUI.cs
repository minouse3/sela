using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using SELA.Core;
using SELA.Narrative;
using SELA.UI;

public partial class CoreLoopDebugUI : MonoBehaviour
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
    Vector2 logScroll;

    ActivityRequest? pendingRequest;
    LaptopInteraction laptop;
    PlayerCarry phoneOwner;
    DitaConversationShot ditaShot;
    ActivityPreview ditaPreview;
    enum LaptopPage { Home, Applications, CvBuilder, WorkMessages, Notes }
    enum PhonePage { Home, Inbox, Mother, Dita }
    PhonePage phonePage;
    LaptopPage laptopPage;
    CompanyId interviewCompany;
    DialogueSession dialogue;
    Action finishDialogue;
    int dialogueChoice = -1;
    DialogueLine thought;
    GameState thoughtLogDestination;
    float thoughtTime;
    bool initialThoughtChecked;

    static readonly ActivityRequest DitaRequest = new ActivityRequest(ActivityType.CallDita);
    static readonly string[] GeneralTabs = { "Overview", "EvidenceBoard", "Bathroom", "Bed", "BagAndOffers", "Save" };

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
    GUIStyle dialogueChoiceStyle;
    GUIStyle selectedChoiceStyle;
    GUIStyle toggleStyle;

    static readonly Color InkColor = new Color(0.176f, 0.227f, 0.204f, 1f);
    static readonly Color MutedInk = new Color(0.353f, 0.400f, 0.376f, 1f);

    void Start()
    {
        panelOpen = startPanelOpen;
        if (GameLoopController.Instance is GameLoopController loop)
        {
            loop.OnWorldFocusRequested += OpenStation;
            loop.Engine.OnActivityResolved += OnActivityResolved;
        }
        SyncCursorAndBlock();
    }

    void OnDestroy()
    {
        if (GameLoopController.Instance is GameLoopController loop)
        {
            loop.OnWorldFocusRequested -= OpenStation;
            loop.Engine.OnActivityResolved -= OnActivityResolved;
        }
        ReleaseLaptop();
        ReleaseDitaShot();
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
        DestroyJobPortalTextures();
    }

    void Update()
    {
        if (PauseMenuController.IsPaused || SleepMenuController.IsOpen || SleepMenuController.IsSleeping) return;

        var state = GameLoopController.Instance?.State;
        if (state == null) return;
        if (state.phase != GamePhase.FreeRoam && (IsDedicatedStation || !panelOpen))
        {
            ReleaseLaptop();
            ReleaseDitaShot();
            ResetPage();
            panelOpen = true;
            SyncCursorAndBlock();
        }
        else if (phoneOwner != null && !phoneOwner.HasPhoneInHand)
        {
            ClosePanel();
        }

        if (!initialThoughtChecked)
        {
            initialThoughtChecked = true;
            if (state.day == 1 && state.currentHour == GameState.StartHour && !state.hasE1)
                ShowThought(NarrativeContent.MorningThought);
        }
        if (CanShowThought) thoughtTime -= Time.unscaledDeltaTime;

        var kb = Keyboard.current;
        if (kb == null) return;
        if (dialogue != null)
        {
            if (!kb.escapeKey.wasPressedThisFrame) ReadDialogueInput(kb);
        }
        else if (state.phase == GamePhase.FreeRoam && laptop == null && !IsDedicatedStation &&
                 !kb.escapeKey.wasPressedThisFrame && (kb.f1Key.wasPressedThisFrame || kb.tabKey.wasPressedThisFrame))
        {
            if (panelOpen) ClosePanel();
            else if (!PlayerInteractor.Blocked) OpenStation(kb.tabKey.wasPressedThisFrame ? "Notes" : "Overview");
        }
    }

    void LateUpdate()
    {
        var kb = Keyboard.current;
        if (kb != null && kb.escapeKey.wasPressedThisFrame && !PauseMenuController.IsPaused)
            BackOrClose();
    }

    void BackOrClose()
    {
        if (dialogue != null)
        {
            if (ditaShot != null) ClosePanel();
            else CancelDialogue();
        }
        else if (pendingRequest.HasValue) BackFromConfirmation();
        else if (laptop != null && laptopPage != LaptopPage.Home) SetLaptopPage(LaptopPage.Home);
        else if (phoneOwner != null && phonePage != PhonePage.Home)
            SetPhonePage(phonePage == PhonePage.Inbox ? PhonePage.Home : PhonePage.Inbox);
        else if ((panelOpen || laptop != null) && GameLoopController.Instance?.State.phase == GamePhase.FreeRoam)
            ClosePanel();
    }

    bool CanOpenStation()
    {
        return isActiveAndEnabled && !panelOpen && !PlayerInteractor.Blocked && !PauseMenuController.IsPaused &&
               GameLoopController.Instance?.State.phase == GamePhase.FreeRoam;
    }

    void OpenStation(string station)
    {
        if (!CanOpenStation()) return;
        ResetPage();
        activeSection = station;
        panelOpen = true;
        SyncCursorAndBlock();
    }

    bool IsDedicatedStation => laptop != null || phoneOwner != null || ditaShot != null;
    bool CanShowThought => thoughtTime > 0 && !panelOpen && !PlayerInteractor.Blocked;

    internal bool OpenPhone(PlayerCarry owner)
    {
        if (!CanOpenStation() || !owner || !owner.HasPhoneInHand) return false;
        ResetPage();
        phoneOwner = owner;
        panelOpen = true;
        SyncCursorAndBlock();
        return true;
    }

    internal void OpenDita(Transform dita)
    {
        if (!CanOpenStation() || SeatInteraction.Current != null) return;
        var player = FindFirstObjectByType<FirstPersonController>();
        if (!player || !player.TryGetComponent<PlayerInteractor>(out var interactor) ||
            !DitaConversationShot.TryBegin(dita, player, interactor.cam, out var shot)) return;

        ResetPage();
        ditaShot = shot;
        var engine = GameLoopController.Instance.Engine;
        ditaPreview = engine.GetPreview(DitaRequest);
        if (ditaPreview.Availability.IsAvailable)
            StartDialogue(NarrativeContent.StartDitaConversation(engine.State.ditaCalledToday), () => FinishDita(engine));
        panelOpen = true;
        SyncCursorAndBlock();
    }

    void FinishDita(GameLoopEngine engine)
    {
        engine.ExecuteActivity(DitaRequest);
        if (engine.State.phase == GamePhase.FreeRoam) ClosePanel();
        else
        {
            ReleaseDitaShot();
            ResetPage();
        }
    }

    void SetLaptopPage(LaptopPage page)
    {
        laptopPage = page;
        scrollPos = Vector2.zero;
    }

    void SetPhonePage(PhonePage page)
    {
        phonePage = page;
        scrollPos = Vector2.zero;
    }

    internal bool BeginLaptop(LaptopInteraction source)
    {
        if (panelOpen || laptop != null) return false;
        laptop = source;
        return true;
    }

    internal void ShowLaptop(LaptopInteraction source)
    {
        if (laptop != source) return;
        ResetPage();
        panelOpen = true;
        SyncCursorAndBlock();
    }

    internal void OnLaptopClosed(LaptopInteraction source)
    {
        if (laptop != source) return;
        laptop = null;
        ClosePanel();
    }

    void ReleaseLaptop()
    {
        var source = laptop;
        laptop = null;
        if (source != null) source.Close();
    }

    void ReleaseDitaShot()
    {
        ditaShot?.Restore();
        ditaShot = null;
    }

    void ClosePanel()
    {
        panelOpen = false;
        ReleaseLaptop();
        ReleaseDitaShot();
        ResetPage();
        SyncCursorAndBlock();
    }

    void ResetPage()
    {
        activeSection = "Overview";
        phoneOwner = null;
        laptopPage = LaptopPage.Home;
        phonePage = PhonePage.Home;
        pendingRequest = null;
        ClearDialogue();
        scrollPos = Vector2.zero;
    }

    public void ShowNoticeThought(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        ShowThought(new DialogueLine("Raka", text));
    }

    public void RequestConfirmation(ActivityRequest req)
    {
        if (!CanOpenStation()) return;
        ResetPage();
        pendingRequest = req;
        panelOpen = true;
        SyncCursorAndBlock();
    }

    void BackFromConfirmation()
    {
        pendingRequest = null;
        scrollPos = Vector2.zero;
    }

    void StartDialogue(DialogueSession session, Action onComplete)
    {
        pendingRequest = null;
        dialogue = session;
        dialogue.RecordTo(GameLoopController.Instance.State.dialogueLog);
        finishDialogue = onComplete;
        dialogueChoice = -1;
        scrollPos = Vector2.zero;
    }

    void CancelDialogue()
    {
        ClearDialogue();
        scrollPos = Vector2.zero;
    }

    void ClearDialogue()
    {
        dialogue = null;
        finishDialogue = null;
        dialogueChoice = -1;
    }

    void AdvanceDialogue(int choice = -1)
    {
        if (!dialogue.Advance(choice)) return;
        dialogueChoice = -1;
        scrollPos = Vector2.zero;
        if (!dialogue.IsComplete) return;
        var onComplete = finishDialogue;
        ClearDialogue();
        onComplete();
    }

    void ReadDialogueInput(Keyboard kb)
    {
        int count = dialogue.Current.ChoiceCount;
        if (count > 0)
        {
            if (kb.digit1Key.wasPressedThisFrame) { AdvanceDialogue(0); return; }
            if (kb.digit2Key.wasPressedThisFrame) { AdvanceDialogue(1); return; }
            if (kb.digit3Key.wasPressedThisFrame) { AdvanceDialogue(2); return; }
            if (kb.downArrowKey.wasPressedThisFrame) dialogueChoice = (dialogueChoice + 1) % count;
            if (kb.upArrowKey.wasPressedThisFrame) dialogueChoice = dialogueChoice <= 0 ? count - 1 : dialogueChoice - 1;
        }

        if (kb.enterKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame)
            AdvanceDialogue(dialogueChoice);
    }

    void OnActivityResolved(ActivityRequest request, ActivityExecutionResult result)
    {
        if (result.Executed && request.Type == ActivityType.ImproveUiExample)
            ShowThought(NarrativeContent.EvidenceThought);
    }

    void ShowThought(DialogueLine line)
    {
        thought = line;
        thoughtLogDestination = GameLoopController.Instance.State;
        thoughtTime = 5f;
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
            Color warnInk = new Color(0.45f, 0.14f, 0.08f, 1f);

            panelStyle = LockTextColor(new GUIStyle(GUI.skin.box)
            {
                normal = { background = paperTex },
                padding = new RectOffset(16, 16, 14, 14)
            }, InkColor);
            cardStyle = LockTextColor(new GUIStyle(GUI.skin.box)
            {
                normal = { background = cardTex },
                padding = new RectOffset(10, 10, 8, 8)
            }, InkColor);
            noticeStyle = LockTextColor(new GUIStyle(GUI.skin.box)
            {
                normal = { background = noticeTex },
                wordWrap = true,
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(10, 10, 6, 6)
            }, InkColor);
            warnStyle = LockTextColor(new GUIStyle(GUI.skin.box)
            {
                normal = { background = warnTex },
                wordWrap = true,
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(10, 10, 6, 6)
            }, warnInk);
            headerStyle = LockTextColor(new GUIStyle(GUI.skin.label)
            {
                richText = true,
                fontSize = 17,
                fontStyle = FontStyle.Bold
            }, InkColor);
            subStyle = LockTextColor(new GUIStyle(GUI.skin.label)
            {
                richText = true,
                fontSize = 14,
                fontStyle = FontStyle.Bold
            }, InkColor);
            bodyStyle = LockTextColor(new GUIStyle(GUI.skin.label)
            {
                richText = true,
                fontSize = 13,
                wordWrap = true
            }, InkColor);
            mutedStyle = LockTextColor(new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                wordWrap = true
            }, MutedInk);
            buttonStyle = LockTextColor(new GUIStyle(GUI.skin.button)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(10, 10, 6, 6),
                normal = { background = btnNormalTex },
                hover = { background = btnHoverTex },
                active = { background = btnActiveTex },
                focused = { background = btnNormalTex },
                onNormal = { background = btnHoverTex },
                onHover = { background = btnHoverTex },
                onActive = { background = btnActiveTex },
                onFocused = { background = btnHoverTex }
            }, InkColor);
            dialogueChoiceStyle = LockTextColor(new GUIStyle(buttonStyle) { wordWrap = true, alignment = TextAnchor.MiddleLeft }, InkColor);
            selectedChoiceStyle = LockTextColor(new GUIStyle(dialogueChoiceStyle), InkColor);
            selectedChoiceStyle.normal.background = btnHoverTex;
            selectedChoiceStyle.focused.background = btnHoverTex;
            toggleStyle = LockTextColor(new GUIStyle(GUI.skin.toggle)
            {
                fontSize = 13,
                wordWrap = true
            }, InkColor);
        }
    }

    static GUIStyle LockTextColor(GUIStyle style, Color color)
    {
        style.normal.textColor = color;
        style.hover.textColor = color;
        style.active.textColor = color;
        style.focused.textColor = color;
        style.onNormal.textColor = color;
        style.onHover.textColor = color;
        style.onActive.textColor = color;
        style.onFocused.textColor = color;
        return style;
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
        var pause = PauseMenuController.Instance;
        bool showLog = pause != null && pause.IsLogOpen;
        if (!showLog && (!panelOpen && !CanShowThought || PauseMenuController.IsPaused)) return;
        if (SleepMenuController.IsOpen || SleepMenuController.IsSleeping) return;

        var ctrl = GameLoopController.Instance;
        if (ctrl == null || ctrl.Engine == null || ctrl.State == null) return;

        EnsureStyles();

        float baseScale = Mathf.Min(Screen.width / 1152f, Screen.height / 648f);
        float uiScale = Mathf.Clamp(baseScale * GameSettings.TextScaleMultiplier, 0.75f, 2.8f);
        float vWidth = Screen.width / uiScale;
        float vHeight = Screen.height / uiScale;

        Matrix4x4 prevMatrix = GUI.matrix;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(uiScale, uiScale, 1f));
        if (showLog)
        {
            DrawDialogueLog(pause, ctrl.State, vWidth, vHeight);
        }
        else if (!panelOpen)
        {
            DrawThought(vWidth, vHeight);
            if (Event.current.type == EventType.Repaint && ReferenceEquals(thoughtLogDestination, ctrl.State))
            {
                ctrl.State.dialogueLog.Add(new DialogueLogEntry(thought.Speaker, thought.Text));
                thoughtLogDestination = null;
            }
        }
        else if (!DrawFocusedSurface(ctrl.Engine, ctrl.State, vWidth, vHeight))
        {
            DrawGeneralPanel(ctrl.Engine, ctrl.State, vWidth, vHeight);
        }
        GUI.matrix = prevMatrix;
    }

    void DrawDialogueLog(PauseMenuController pause, GameState state, float width, float height)
    {
        float w = Mathf.Min(width - 48f, 620f);
        float h = Mathf.Min(height - 64f, 450f);
        var panel = new Rect((width - w) * 0.5f, (height - h) * 0.5f, w, h);
        GUI.DrawTexture(new Rect(panel.x - 2f, panel.y - 2f, panel.width + 4f, panel.height + 4f), borderTex);
        GUI.Box(panel, "", panelStyle);
        GUILayout.BeginArea(new Rect(panel.x + 16f, panel.y + 12f, panel.width - 32f, panel.height - 24f));
        GUILayout.BeginHorizontal();
        GUILayout.Label("Log", headerStyle);
        GUILayout.FlexibleSpace();
        bool back = GUILayout.Button("Kembali [Esc]", buttonStyle);
        GUILayout.EndHorizontal();
        GUILayout.Space(8);

        var entries = state.dialogueLog;
        if (entries.Count == 0)
        {
            GUILayout.Label("Belum ada dialog.", mutedStyle);
        }
        else
        {
            logScroll = GUILayout.BeginScrollView(logScroll);
            foreach (var entry in entries)
            {
                GUILayout.BeginVertical(cardStyle);
                GUILayout.Label(entry.speaker, subStyle);
                GUILayout.Label(entry.text, bodyStyle);
                GUILayout.EndVertical();
                GUILayout.Space(4);
            }
            GUILayout.EndScrollView();
        }
        GUILayout.EndArea();
        if (back) pause.CloseLog();
    }

    bool DrawFocusedSurface(GameLoopEngine engine, GameState state, float width, float height)
    {
        if (state.phase != GamePhase.FreeRoam) return false;
        if (laptop != null) DrawLaptopDevice(engine, state, width, height);
        else if (phoneOwner != null) DrawPhoneDevice(state, width, height);
        else if (ditaShot != null) DrawDitaScene(width, height);
        else if (activeSection == "Calendar") DrawCalendarView(state, width, height);
        else if (activeSection == "EvidenceBoard") DrawEvidenceBoardView(engine, state, width, height);
        else return false;
        return true;
    }

    void DrawGeneralPanel(GameLoopEngine engine, GameState state, float vWidth, float vHeight)
    {
        float w = Mathf.Min(760f, vWidth - 48f);
        float h = Mathf.Min(520f, vHeight - 64f);
        Rect panelRect = new Rect((vWidth - w) * 0.5f, (vHeight - h) * 0.5f, w, h);

        GUI.DrawTexture(new Rect(panelRect.x + 5f, panelRect.y + 5f, panelRect.width, panelRect.height), borderTex);
        GUI.DrawTexture(new Rect(panelRect.x - 2f, panelRect.y - 2f, panelRect.width + 4f, panelRect.height + 4f), borderTex);
        GUI.Box(panelRect, "", panelStyle);

        GUILayout.BeginArea(new Rect(panelRect.x + 18f, panelRect.y + 14f, panelRect.width - 36f, panelRect.height - 28f));

        GUILayout.BeginHorizontal();
        bool notesOpen = state.phase == GamePhase.FreeRoam && activeSection == "Notes";
        GUILayout.Label(notesOpen ? "Catatan" : $"Hari {state.day}  ·  {state.FormattedTime}  ·  {state.session}", headerStyle);
        GUILayout.FlexibleSpace();
        bool canClose = state.phase == GamePhase.FreeRoam || dialogue != null;
        bool closeClicked = canClose && GUILayout.Button(dialogue != null ? "Batal [Esc]" : "Tutup [Esc]", buttonStyle, GUILayout.Width(110));
        GUILayout.EndHorizontal();

        if (!notesOpen && state.phase == GamePhase.FreeRoam) GUILayout.Label(SectionLabel(activeSection), mutedStyle);
        GUILayout.Space(6);

        if (!notesOpen && !string.IsNullOrEmpty(engine.LastStatusMessage))
        {
            GUILayout.Box(engine.LastStatusMessage, noticeStyle);
            GUILayout.Space(6);
        }

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        DrawPanelContents(engine, state);
        GUILayout.EndScrollView();
        GUILayout.EndArea();
        if (closeClicked) BackOrClose();
    }

    void DrawPanelContents(GameLoopEngine engine, GameState state)
    {
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
                else if (activeSection == "Notes") DrawNotes(state);
                else DrawFreeRoamControlPanel(engine, state);
                break;
        }
    }

    void DrawDitaScene(float width, float height)
    {
        var line = dialogue?.Current;
        float w = Mathf.Min(width - 40f, 850f);
        float h = Mathf.Min(height * 0.33f, line != null && line.ChoiceCount > 0 ? 204f : 153f);
        var box = new Rect((width - w) * 0.5f, height - h - 14f, w, h);
        GUI.DrawTexture(new Rect(box.x - 2f, box.y - 2f, box.width + 4f, box.height + 4f), borderTex);
        GUI.Box(box, "", panelStyle);
        GUILayout.BeginArea(new Rect(box.x + 16f, box.y + 8f, box.width - 32f, box.height - 16f));
        GUILayout.BeginHorizontal();
        GUILayout.Label(line?.Speaker ?? "Dita", headerStyle);
        GUILayout.FlexibleSpace();
        if (line != null && !ditaPreview.Availability.WillMissScheduledInterview)
            GUILayout.Label($"{ditaPreview.DurationHours} jam jika selesai", mutedStyle, GUILayout.Height(26));
        bool close = GUILayout.Button(line != null ? "Batal [Esc]" : "Tutup [Esc]", buttonStyle);
        GUILayout.EndHorizontal();
        if (line != null && ditaPreview.Availability.WillMissScheduledInterview)
            GUILayout.Label("Jika selesai, interview pukul 13:00 akan terlewat.", warnStyle);

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        int? advance = null;
        if (line == null)
        {
            GUILayout.Label(ditaPreview.Availability.Reason, bodyStyle);
        }
        else
        {
            GUILayout.Label(line.Text, bodyStyle);
            GUILayout.Space(4);
            advance = DrawDialogueOptions(line, "[Enter] Lanjut", "[Enter] Selesai");
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
        if (close) ClosePanel();
        else if (advance.HasValue) AdvanceDialogue(advance.Value);
    }

    void DrawThought(float width, float height)
    {
        float w = Mathf.Min(width - 48f, 540f);
        var text = new GUIContent($"{thought.Speaker}: {thought.Text}");
        float h = noticeStyle.CalcHeight(text, w);
        GUI.Box(new Rect((width - w) * 0.5f, height - 104f - h, w, h), text, noticeStyle);
    }

    void DrawDialogue(GameState state)
    {
        var line = dialogue.Current;
        GUILayout.BeginVertical(cardStyle);
        GUILayout.Label(line.Speaker, subStyle);
        GUILayout.Label(line.Text, bodyStyle);
        GUILayout.EndVertical();
        GUILayout.Space(8);

        int? advance = DrawDialogueOptions(line, "Lanjut [Enter]", "Akhiri panggilan [Enter]");
        var app = state.GetApplication(interviewCompany);
        GUILayout.Space(10);
        GUILayout.Label("Bukti pada lamaran terkirim", subStyle);
        if (app.snapshotHasE1) GUILayout.Label(EvidenceE1, bodyStyle);
        if (app.snapshotHasE2) GUILayout.Label(EvidenceE2, bodyStyle);
        if (!app.snapshotHasE1 && !app.snapshotHasE2) GUILayout.Label("Tidak ada lampiran E1 atau E2.", mutedStyle);
        if (advance.HasValue) AdvanceDialogue(advance.Value);
    }

    int? DrawDialogueOptions(DialogueLine line, string nextLabel, string finishLabel)
    {
        if (line.ChoiceCount == 0)
        {
            bool isLast = dialogue.WillFinishOnAdvance;
            return GUILayout.Button(isLast ? finishLabel : nextLabel, buttonStyle, GUILayout.Height(29)) ? -1 : (int?)null;
        }

        int? choice = null;
        for (int i = 0; i < line.ChoiceCount; i++)
        {
            var style = dialogueChoice == i ? selectedChoiceStyle : dialogueChoiceStyle;
            if (GUILayout.Button($"[{i + 1}] {line.ChoiceAt(i).Text}", style, GUILayout.MinHeight(30)))
                choice = i;
        }
        return choice;
    }

    static string SectionLabel(string section)
    {
        switch (section)
        {
            case "Overview": return "Ringkasan";
            case "EvidenceBoard": return "Papan Bukti";
            case "Bathroom": return "Kamar Mandi";
            case "Bed": return "Kasur";
            case "BagAndOffers": return "Map Tawaran";
            case "Save": return "Simpanan";
            default: return section;
        }
    }

    void DrawFreeRoamControlPanel(GameLoopEngine engine, GameState state)
    {
        GUILayout.BeginHorizontal();
        foreach (string tab in GeneralTabs)
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
            DrawActivityButton(engine, new ActivityRequest(ActivityType.Shower));
        }
        else if (activeSection == "EvidenceBoard")
        {
            GUILayout.Label($"Bukti: E0 {(state.hasE0 ? "✓" : "–")}   ·   E1 {(state.hasE1 ? "✓" : "–")}   ·   E2 {(state.hasE2 ? "✓" : "–")}", bodyStyle);
            DrawActivityButton(engine, new ActivityRequest(ActivityType.ImproveUiExample));
            DrawActivityButton(engine, new ActivityRequest(ActivityType.CompleteTestNotes));
            GUILayout.Space(8);
            DrawCvDraftEditor(engine, state);
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

    const string EvidenceE1 = "E1 · Perbandingan label dan pesan kesalahan formulir sebelum dan sesudah diperbaiki.";
    const string EvidenceE2 = "E2 · Catatan untuk kolom kosong, format salah, dan data valid. Kesimpulan terbatas pada tiga kasus itu.";

    void DrawLaptopApp(GameLoopEngine engine, GameState state)
    {
        switch (laptopPage)
        {
            case LaptopPage.Applications:
                DrawJobPortalApp(engine, state);
                break;
            case LaptopPage.CvBuilder:
                DrawCvBuilderApp(engine, state);
                break;
            case LaptopPage.WorkMessages:
                GUILayout.Label("Kotak Masuk", headerStyle);
                GUILayout.Space(6);
                DrawWorkMessage(engine, state.GetApplication(CompanyId.StudioLajur));
                DrawWorkMessage(engine, state.GetApplication(CompanyId.KoperasiPijar));
                if (state.day >= 4)
                {
                    GUILayout.BeginVertical(cardStyle);
                    GUILayout.Label("Akses Prima  ·  Tawaran Langsung", subStyle);
                    GUILayout.Label("Kontrak meminta ijazah asli dan membatasi pengunduran diri.", bodyStyle);
                    GUILayout.EndVertical();
                    GUILayout.Space(6);
                }
                if (state.HasOffer(CompanyId.StudioLajur) || state.HasOffer(CompanyId.KoperasiPijar) || state.day >= 4)
                    DrawOfferChoiceSection(engine, state);
                if (state.day < 4 && state.applicationA.status == ApplicationStatus.NotSent &&
                    state.applicationB.status == ApplicationStatus.NotSent)
                    GUILayout.Label("Belum ada email masuk.", mutedStyle);
                break;
            case LaptopPage.Notes:
                DrawNotes(state);
                break;
        }
    }

    void DrawNotes(GameState state)
    {
        GUILayout.Label("Tujuan", subStyle);
        GUILayout.Label(ActivitySystem.GetObjectiveHint(state), bodyStyle);
        GUILayout.Space(10);
        GUILayout.Label("Bukti", subStyle);
        if (state.hasE0) GUILayout.Label("E0 · Contoh antarmuka dari proyek kuliah yang kukerjakan bersama tim.", bodyStyle);
        if (state.hasE1) GUILayout.Label(EvidenceE1, bodyStyle);
        if (state.hasE2) GUILayout.Label(EvidenceE2, bodyStyle);
    }

    void DrawWorkMessage(GameLoopEngine engine, ApplicationRecord app)
    {
        if (app.status == ApplicationStatus.NotSent) return;
        GUILayout.BeginVertical(cardStyle);
        GUILayout.Label(ActivitySystem.GetCompanyName(app.company), subStyle);
        switch (app.status)
        {
            case ApplicationStatus.Scheduled:
                GUILayout.Label($"Undangan interview · Hari {app.interviewDay}, {app.interviewHour:00}:00", bodyStyle);
                DrawActivityButton(engine, new ActivityRequest(ActivityType.AttendInterview, app.company));
                break;
            case ApplicationStatus.Rejected:
                GUILayout.Label(app.missedInterview ? "Interview terlewat" : "Lamaran tidak dilanjutkan", bodyStyle);
                foreach (string reason in app.rejectionReasons)
                    GUILayout.Label(reason, mutedStyle);
                break;
            case ApplicationStatus.Offered:
                GUILayout.Label("Perusahaan mengirim tawaran. Simpan sebagai pilihan sementara atau putuskan nanti.", bodyStyle);
                break;
        }
        GUILayout.EndVertical();
        GUILayout.Space(6);
    }

    void DrawActivityButton(GameLoopEngine engine, ActivityRequest req)
    {
        var preview = engine.GetPreview(req);
        GUILayout.BeginHorizontal();
        GUI.enabled = preview.Availability.IsAvailable;
        string warn = preview.Availability.WillMissScheduledInterview ? " [LEWAT INTERVIEW]" : "";
        if (preview.WillTriggerRift > 0) warn += $" [RETAKAN {preview.WillTriggerRift}]";
        string title = preview.Title;
        if (GUILayout.Button($"{title} ({preview.DurationHours}j){warn}", buttonStyle, GUILayout.MinWidth(230), GUILayout.ExpandWidth(false), GUILayout.Height(30)))
        {
            pendingRequest = req;
            scrollPos = Vector2.zero;
        }
        GUI.enabled = true;

        GUILayout.Space(8);
        string info = preview.Availability.IsAvailable
            ? $"{preview.StartHour:00}:00 → {preview.EndHour:00}:00  ·  Tenaga {preview.EnergyBefore}→{preview.EnergyAfter}  ·  Tekanan {preview.PressureBefore}→{preview.PressureAfter}"
            : preview.Availability.Reason;
        GUILayout.Label(info, preview.Availability.IsAvailable ? bodyStyle : mutedStyle, GUILayout.ExpandWidth(true));
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
        string status = app != null ? ApplicationLabel(app.status) : (state.day >= 4 ? "Tawaran langsung" : "Masuk Hari 4");
        GUILayout.BeginVertical(cardStyle);
        GUILayout.Label($"{ActivitySystem.GetCompanyName(company)}  ·  {status}  ·  Cek: {(ver.isVerified ? "Sudah" : "Belum")}", subStyle);
        DrawActivityButton(engine, new ActivityRequest(ActivityType.VerifyCompany, company));
        if (app != null)
        {
            DrawActivityButton(engine, new ActivityRequest(ActivityType.SubmitApplication, company));
        }
        GUILayout.EndVertical();
        GUILayout.Space(4);
    }

    static string ApplicationLabel(ApplicationStatus status)
    {
        switch (status)
        {
            case ApplicationStatus.Scheduled: return "Interview terjadwal";
            case ApplicationStatus.Rejected: return "Tidak dilanjutkan";
            case ApplicationStatus.Offered: return "Ada tawaran";
            default: return "Belum dikirim";
        }
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
        string title = preview.Title;
        GUILayout.Label($"{title} ({preview.DurationHours} jam)", headerStyle);
        GUILayout.Label($"{preview.Location}  ·  {preview.StartHour:00}:00 → {preview.EndHour:00}:00 ({preview.NextSession})", mutedStyle);
        GUILayout.Label($"Tenaga {preview.EnergyBefore} → {preview.EnergyAfter}   ·   Tekanan {preview.PressureBefore} → {preview.PressureAfter}", bodyStyle);

        if (preview.WillTriggerRift > 0)
            GUILayout.Box($"Tekanan naik ke batas Retakan {preview.WillTriggerRift}.", warnStyle);
        else if (preview.WillTriggerJeda)
            GUILayout.Box("Tekanan mencapai puncak — perlu ambil Jeda.", warnStyle);

        if (preview.Availability.WillMissScheduledInterview)
            GUILayout.Box($"Melewati interview {ActivitySystem.GetCompanyName(preview.Availability.MissedInterviewCompany)} jam 13:00 (lamaran gugur, +1 Tekanan).", warnStyle);

        if (!preview.Availability.IsAvailable)
            GUILayout.Box(preview.Availability.Reason, warnStyle);

        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        GUI.enabled = preview.Availability.IsAvailable;
        string confirm = req.Type == ActivityType.AttendInterview ? "Sambungkan" : "Jalankan";
        if (GUILayout.Button(confirm, buttonStyle, GUILayout.Height(32)))
            StartActivity(engine, state, req);
        GUI.enabled = true;
        if (GUILayout.Button("Batal", buttonStyle, GUILayout.Height(32)))
            BackFromConfirmation();
        GUILayout.EndHorizontal();
    }

    void StartActivity(GameLoopEngine engine, GameState state, ActivityRequest request)
    {
        pendingRequest = null;
        switch (request.Type)
        {
            case ActivityType.AttendInterview:
                interviewCompany = request.Company;
                var interview = NarrativeContent.StartInterview(state.GetApplication(request.Company));
                StartDialogue(interview.Session, () =>
                {
                    if (interview.TryGetAnswers(out var answers))
                        engine.ExecuteActivity(request, answers);
                });
                break;
            default:
                engine.ExecuteActivity(request);
                break;
        }
    }

    void DrawJedaView(GameLoopEngine engine)
    {
        GUILayout.Label("Tarik napas dulu.", headerStyle);
        GUILayout.Label("Tekanan memuncak. Ambil jeda sebentar (Tekanan turun ke 4, waktu tidak maju).", bodyStyle);
        GUILayout.Space(8);
        if (GUILayout.Button("Duduk sebentar", buttonStyle, GUILayout.Height(34)))
            engine.ResolveJeda();
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
