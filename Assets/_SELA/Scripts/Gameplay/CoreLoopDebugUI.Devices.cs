using UnityEngine;
using SELA.Core;
using SELA.Narrative;

public partial class CoreLoopDebugUI
{
    static readonly Color DevicePaper = new Color32(242, 232, 209, 255);
    static readonly Color DeviceLight = new Color32(250, 240, 219, 255);
    static readonly Color DeviceChassis = new Color32(48, 62, 56, 255);
    static readonly Color DeviceShadow = new Color32(27, 36, 33, 120);
    static readonly Color DesktopBlue = new Color32(89, 108, 128, 255);
    static readonly Color DesktopBar = new Color32(48, 65, 81, 255);
    static readonly Color DesktopTile = new Color32(59, 77, 94, 255);
    static readonly Color DesktopTileHover = new Color32(73, 93, 110, 255);
    static readonly Color DesktopGold = new Color32(194, 162, 98, 255);
    static readonly Color PhoneClay = new Color32(174, 116, 98, 255);
    static readonly Color PhoneOlive = new Color32(67, 84, 70, 255);
    static readonly Color PhoneSage = new Color32(190, 199, 171, 255);
    static readonly Color PhoneBlush = new Color32(225, 200, 180, 255);

    GUIStyle deviceLightLabel;
    GUIStyle deviceCenteredLabel;
    GUIStyle deviceClockStyle;
    GUIStyle deviceMessageStyle;
    GUIStyle calendarHeaderSubStyle;
    GUIStyle calendarPillStyle;
    readonly GUIContent deviceMeasureContent = new GUIContent();
    int deviceClockHour = -1;
    string deviceClockText;
    float phoneMessageWidth;
    float phoneGreetingHeight;
    float phoneMotherHeight;
    float phoneDitaHeight;

    void DrawLaptopDevice(GameLoopEngine engine, GameState state, float width, float height)
    {
        EnsureDeviceStyles();
        var shell = new Rect(0f, 0f, width, height);
        var screen = new Rect(8f, 10f, width - 16f, height - 20f);
        var desktop = new Rect(screen.x, screen.y, screen.width, screen.height - 42f);
        var taskbar = new Rect(screen.x, desktop.yMax, screen.width, 42f);
        var page = laptopPage;
        bool inDialogue = dialogue != null;
        bool confirming = pendingRequest.HasValue;
        bool appOpen = page != LaptopPage.Home || inDialogue || confirming;
        string appName = page != LaptopPage.Home ? DesktopAppName(page) : inDialogue ? "Percakapan" : "Konfirmasi";

        DeviceFill(shell, DeviceChassis);
        DeviceFill(new Rect(shell.center.x - 2f, 3f, 4f, 4f), DesktopBlue, 2f);
        DrawLaptopWallpaper(desktop);

        if (appOpen)
            DrawLaptopWindow(engine, state, desktop, page, appName, inDialogue, confirming);
        else
            DrawLaptopLauncher(desktop);

        DeviceFill(taskbar, DesktopBar);
        DeviceFill(new Rect(taskbar.x + 16f, taskbar.center.y - 4f, 8f, 8f), appOpen ? DesktopGold : DevicePaper, 2f);
        GUI.Label(new Rect(taskbar.x + 32f, taskbar.y, 200f, taskbar.height), appOpen ? appName : "Desktop", deviceLightLabel);
        float clockX = taskbar.xMax - (appOpen ? 76f : 252f);
        DeviceFill(new Rect(clockX - 10f, taskbar.y + 11f, 1f, 20f), DesktopBlue);
        GUI.Label(new Rect(clockX, taskbar.y, 62f, taskbar.height), DeviceTime(state), deviceLightLabel);
        if (!appOpen && GUI.Button(new Rect(taskbar.xMax - 172f, taskbar.y + 7f, 162f, 28f), "Tutup laptop [Esc]", buttonStyle))
            ClosePanel();
    }

    void DrawLaptopLauncher(Rect desktop)
    {
        float x = desktop.x + 30f;
        float y = desktop.y + 30f;
        if (DrawLaptopTile(new Rect(x, y, 196f, 58f), LaptopPage.Applications))
            SetLaptopPage(LaptopPage.Applications);
        if (DrawLaptopTile(new Rect(x, y + 70f, 196f, 58f), LaptopPage.CvBuilder))
            SetLaptopPage(LaptopPage.CvBuilder);
        if (DrawLaptopTile(new Rect(x, y + 140f, 196f, 58f), LaptopPage.WorkMessages))
            SetLaptopPage(LaptopPage.WorkMessages);
        if (DrawLaptopTile(new Rect(x, y + 210f, 196f, 58f), LaptopPage.Notes))
            SetLaptopPage(LaptopPage.Notes);
    }

    bool DrawLaptopTile(Rect rect, LaptopPage page)
    {
        DeviceFill(rect, rect.Contains(Event.current.mousePosition) ? DesktopTileHover : DesktopTile, 5f);
        DrawDesktopIcon(new Rect(rect.x + 12f, rect.y + 12f, 34f, 34f), page);
        GUI.Label(new Rect(rect.x + 56f, rect.y, rect.width - 62f, rect.height), DesktopAppName(page), deviceLightLabel);
        return GUI.Button(rect, GUIContent.none, GUIStyle.none);
    }

    void DrawLaptopWindow(GameLoopEngine engine, GameState state, Rect desktop, LaptopPage page, string appName, bool inDialogue, bool confirming)
    {
        var window = new Rect(desktop.x + 8f, desktop.y + 8f, desktop.width - 16f, desktop.height - 16f);
        DeviceFill(new Rect(window.x + 3f, window.y + 3f, window.width, window.height), DeviceShadow, 5f);
        DeviceFill(window, DevicePaper, 5f);
        DeviceFill(new Rect(window.x, window.y, window.width, 38f), DesktopTile, 4f);
        if (page != LaptopPage.Home)
            DrawDesktopIcon(new Rect(window.x + 10f, window.y + 6f, 26f, 26f), page);
        GUI.Label(new Rect(window.x + 44f, window.y, window.width - 210f, 38f), appName, deviceLightLabel);
        string backLabel = inDialogue ? "Batal [Esc]" : confirming ? "Kembali [Esc]" : "Desktop [Esc]";
        bool back = GUI.Button(new Rect(window.xMax - 152f, window.y + 5f, 144f, 28f), backLabel, buttonStyle);

        GUILayout.BeginArea(new Rect(window.x + 16f, window.y + 48f, window.width - 32f, window.height - 58f));
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        if (inDialogue)
            DrawDialogue(state);
        else if (confirming)
            DrawConfirmationView(engine, state, pendingRequest.Value);
        else
            DrawLaptopApp(engine, state);
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        if (back) BackOrClose();
    }

    static string DesktopAppName(LaptopPage page)
    {
        switch (page)
        {
            case LaptopPage.Applications: return "Lamaran";
            case LaptopPage.CvBuilder: return "CV Builder";
            case LaptopPage.WorkMessages: return "Email";
            case LaptopPage.Notes: return "Catatan";
            default: return "Desktop";
        }
    }

    static void DrawDesktopIcon(Rect rect, LaptopPage page)
    {
        var center = rect.center;
        switch (page)
        {
            case LaptopPage.Applications:
                DeviceFill(rect, DesktopGold, 4f);
                DeviceFill(new Rect(center.x - 10f, center.y - 8f, 9f, 6f), DesktopBar, 1f);
                DeviceFill(new Rect(center.x - 10f, center.y - 4f, 20f, 12f), DesktopBar, 2f);
                DeviceFill(new Rect(center.x - 8f, center.y - 1f, 16f, 7f), DevicePaper, 1f);
                break;
            case LaptopPage.CvBuilder:
                DeviceFill(rect, PhoneBlush, 4f);
                DeviceFill(new Rect(center.x - 8f, center.y - 10f, 16f, 20f), DeviceLight, 2f);
                DeviceFill(new Rect(center.x - 6f, center.y - 7f, 6f, 5f), DesktopBar, 1f);
                DeviceFill(new Rect(center.x + 2f, center.y - 6f, 4f, 2f), DesktopBlue);
                DeviceFill(new Rect(center.x - 6f, center.y + 1f, 12f, 2f), DesktopBar);
                DeviceFill(new Rect(center.x - 6f, center.y + 5f, 9f, 2f), DesktopBlue);
                break;
            case LaptopPage.WorkMessages:
                DeviceFill(rect, DevicePaper, 4f);
                DeviceFill(new Rect(center.x - 10f, center.y - 7f, 20f, 14f), DesktopTile, 2f);
                DeviceLine(new Vector2(center.x - 8f, center.y - 5f), new Vector2(center.x, center.y + 1f), DevicePaper);
                DeviceLine(new Vector2(center.x, center.y + 1f), new Vector2(center.x + 8f, center.y - 5f), DevicePaper);
                break;
            case LaptopPage.Notes:
                DeviceFill(rect, PhoneSage, 4f);
                DeviceFill(new Rect(center.x - 7f, center.y - 9f, 15f, 19f), DesktopBar, 1f);
                DeviceFill(new Rect(center.x - 4f, center.y - 9f, 12f, 18f), DevicePaper, 1f);
                DeviceFill(new Rect(center.x + 3f, center.y - 9f, 5f, 5f), DesktopGold);
                DeviceFill(new Rect(center.x - 2f, center.y, 7f, 1f), DesktopBlue);
                DeviceFill(new Rect(center.x - 2f, center.y + 4f, 7f, 1f), DesktopBlue);
                break;
        }
    }

    static void DrawLaptopWallpaper(Rect rect)
    {
        if (Event.current.type != EventType.Repaint) return;
        var dusk = new Color32(103, 125, 140, 255);
        var roof = new Color32(77, 98, 116, 255);
        var sun = new Color32(177, 163, 129, 255);
        float disk = rect.height * 0.22f;
        DeviceFill(rect, DesktopBlue);
        DeviceFill(new Rect(rect.x + rect.width * 0.70f, rect.y + rect.height * 0.15f, disk, disk), sun, disk * 0.5f);
        DeviceFill(new Rect(rect.x, rect.y + rect.height * 0.53f, rect.width, rect.height * 0.47f), dusk);
        DeviceFill(new Rect(rect.x + rect.width * 0.49f, rect.y + rect.height * 0.68f, rect.width * 0.51f, rect.height * 0.32f), roof);
        DeviceFill(new Rect(rect.x + rect.width * 0.57f, rect.y + rect.height * 0.58f, rect.width * 0.16f, rect.height * 0.12f), roof);
        DeviceFill(new Rect(rect.x + rect.width * 0.76f, rect.y + rect.height * 0.72f, rect.width * 0.24f, rect.height * 0.28f), DesktopTile);
        DeviceFill(new Rect(rect.x + rect.width * 0.82f, rect.y + rect.height * 0.64f, rect.width * 0.18f, rect.height * 0.10f), DesktopTile);
        for (int i = 0; i < 3; i++)
            DeviceFill(new Rect(rect.x + rect.width * (0.84f + 0.05f * i), rect.y + rect.height * 0.78f, 6f, 12f), dusk);
    }

    void DrawPhoneDevice(GameState state, float width, float height)
    {
        EnsureDeviceStyles();
        float h = Mathf.Min(510f, height - 44f);
        float w = Mathf.Min(h * 0.62f, width - 48f);
        float x = Mathf.Min(width * 0.63f - w * 0.5f, width - w - 24f);
        var shell = new Rect(x, (height - h) * 0.5f, w, h);
        var screen = new Rect(shell.x + 7f, shell.y + 16f, w - 14f, h - 28f);
        var page = phonePage;
        bool home = page == PhonePage.Home;

        DeviceFill(new Rect(shell.x + 4f, shell.y + 6f, w, h), DeviceShadow, 26f);
        DeviceFill(shell, PhoneClay, 24f);
        DeviceFill(new Rect(shell.xMax - 1f, shell.y + 67f, 3f, 24f), PhoneClay, 1f);
        DeviceFill(new Rect(shell.x + 3f, shell.y + 3f, w - 6f, h - 6f), DeviceChassis, 21f);
        DeviceFill(new Rect(shell.center.x - 14f, shell.y + 8f, 28f, 3f), DesktopBlue, 2f);
        DeviceFill(screen, home ? PhoneOlive : DevicePaper, 15f);

        DeviceFill(new Rect(screen.xMax - 35f, screen.y + 11f, 19f, 10f), home ? DevicePaper : PhoneOlive, 2f);
        DeviceFill(new Rect(screen.xMax - 33f, screen.y + 13f, 15f, 6f), home ? PhoneOlive : DevicePaper, 1f);
        DeviceFill(new Rect(screen.xMax - 15f, screen.y + 14f, 2f, 4f), home ? DevicePaper : PhoneOlive, 1f);
        if (!home)
            GUI.Label(new Rect(screen.x + 13f, screen.y + 3f, 64f, 26f), DeviceTime(state), bodyStyle);

        var content = new Rect(screen.x + 13f, screen.y + 35f, screen.width - 26f, screen.height - 81f);
        switch (page)
        {
            case PhonePage.Home:
                DrawPhoneLauncher(content, DeviceTime(state));
                break;
            case PhonePage.Inbox:
                DrawPhoneInbox(content);
                break;
            case PhonePage.Mother:
            case PhonePage.Dita:
                DrawPhoneThread(content, page, state.day >= 5);
                break;
        }

        string backLabel = home ? "Tutup [Esc]" : page == PhonePage.Inbox ? "Beranda [Esc]" : "Kotak masuk [Esc]";
        if (GUI.Button(new Rect(screen.x + 13f, screen.yMax - 36f, screen.width - 26f, 27f), backLabel, buttonStyle))
        {
            if (home) ClosePanel();
            else BackOrClose();
        }
    }

    void DrawPhoneLauncher(Rect content, string time)
    {
        GUI.Label(new Rect(content.x + 4f, content.y + 8f, content.width - 8f, 49f), time, deviceClockStyle);
        DeviceFill(new Rect(content.x + 4f, content.y + 67f, content.width - 8f, 1f), PhoneSage);
        var tile = new Rect(content.x + 4f, content.y + 83f, 82f, 71f);
        DeviceFill(tile, tile.Contains(Event.current.mousePosition) ? DeviceLight : PhoneSage, 9f);
        float x = tile.center.x - 13f;
        float y = tile.y + 14f;
        DeviceFill(new Rect(x, y, 26f, 18f), PhoneOlive, 5f);
        DeviceFill(new Rect(x + 5f, y + 14f, 5f, 7f), PhoneOlive, 2f);
        DeviceFill(new Rect(x + 6f, y + 6f, 14f, 2f), DevicePaper, 1f);
        DeviceFill(new Rect(x + 6f, y + 11f, 10f, 2f), DevicePaper, 1f);
        GUI.Label(new Rect(tile.x, tile.y + 39f, tile.width, 25f), "Pesan", deviceCenteredLabel);
        if (GUI.Button(tile, GUIContent.none, GUIStyle.none))
            SetPhonePage(PhonePage.Inbox);
    }

    void DrawPhoneInbox(Rect content)
    {
        GUI.Label(new Rect(content.x, content.y, content.width, 26f), "Kotak masuk", headerStyle);
        if (DrawPhoneInboxRow(new Rect(content.x, content.y + 34f, content.width, 53f), "I", NarrativeContent.MotherGreeting.Speaker, PhoneBlush))
            SetPhonePage(PhonePage.Mother);
        if (DrawPhoneInboxRow(new Rect(content.x, content.y + 94f, content.width, 53f), "D", NarrativeContent.DitaMessage.Speaker, PhoneSage))
            SetPhonePage(PhonePage.Dita);
    }

    bool DrawPhoneInboxRow(Rect rect, string initial, string sender, Color accent)
    {
        DeviceFill(rect, rect.Contains(Event.current.mousePosition) ? accent : DeviceLight, 8f);
        var avatar = new Rect(rect.x + 9f, rect.y + 11f, 31f, 31f);
        DeviceFill(avatar, accent, 15f);
        GUI.Label(avatar, initial, deviceCenteredLabel);
        GUI.Label(new Rect(rect.x + 49f, rect.y + 15f, rect.width - 76f, 24f), sender, subStyle);
        GUI.Label(new Rect(rect.xMax - 24f, rect.y, 20f, rect.height), "›", deviceCenteredLabel);
        return GUI.Button(rect, GUIContent.none, GUIStyle.none);
    }

    void DrawPhoneThread(Rect content, PhonePage page, bool hasLaterMessage)
    {
        bool mother = page == PhonePage.Mother;
        var first = mother ? NarrativeContent.MotherGreeting : NarrativeContent.DitaMessage;
        GUI.Label(new Rect(content.x, content.y, content.width, 25f), first.Speaker, headerStyle);
        DeviceFill(new Rect(content.x, content.y + 31f, content.width, 1f), mother ? PhoneBlush : PhoneSage);
        var view = new Rect(content.x, content.y + 42f, content.width, content.height - 42f);
        float scrollGutter = GUI.skin.verticalScrollbar.fixedWidth + GUI.skin.verticalScrollbar.margin.left;
        float bubbleWidth = view.width - scrollGutter - 15f;
        if (phoneMessageWidth != bubbleWidth)
        {
            phoneMessageWidth = bubbleWidth;
            phoneGreetingHeight = PhoneBubbleHeight(NarrativeContent.MotherGreeting, bubbleWidth);
            phoneMotherHeight = PhoneBubbleHeight(NarrativeContent.MotherMessage, bubbleWidth);
            phoneDitaHeight = PhoneBubbleHeight(NarrativeContent.DitaMessage, bubbleWidth);
        }

        float firstHeight = mother ? phoneGreetingHeight : phoneDitaHeight;
        bool showLater = mother && hasLaterMessage;
        float totalHeight = firstHeight + (showLater ? 9f + phoneMotherHeight : 0f);
        scrollPos = GUI.BeginScrollView(view, scrollPos, new Rect(0f, 0f, view.width - scrollGutter, totalHeight));
        Color bubble = mother ? PhoneBlush : PhoneSage;
        DrawPhoneBubble(new Rect(5f, 0f, bubbleWidth, firstHeight), first.Text, bubble);
        if (showLater)
            DrawPhoneBubble(new Rect(5f, firstHeight + 9f, bubbleWidth, phoneMotherHeight), NarrativeContent.MotherMessage.Text, bubble);
        GUI.EndScrollView();
    }

    float PhoneBubbleHeight(DialogueLine line, float width)
    {
        deviceMeasureContent.text = line.Text;
        return deviceMessageStyle.CalcHeight(deviceMeasureContent, width - 20f) + 16f;
    }

    void DrawPhoneBubble(Rect rect, string text, Color color)
    {
        DeviceFill(rect, color, 9f);
        DeviceFill(new Rect(rect.x - 4f, rect.y + 12f, 9f, 6f), color, 3f);
        GUI.Label(new Rect(rect.x + 10f, rect.y + 8f, rect.width - 20f, rect.height - 16f), text, deviceMessageStyle);
    }

    void DrawCalendarView(GameState state, float width, float height)
    {
        EnsureDeviceStyles();
        EnsureJobPortalStyles();

        float w = Mathf.Min(1040f, width - 36f);
        float h = Mathf.Min(580f, height - 36f);
        var panel = new Rect((width - w) * 0.5f, (height - h) * 0.5f, w, h);

        // Paper wall-calendar backing (matching SELA paper UI)
        DeviceFill(new Rect(panel.x + 5f, panel.y + 6f, panel.width, panel.height), DeviceShadow, 4f);
        DeviceFill(new Rect(panel.x - 2f, panel.y - 2f, panel.width + 4f, panel.height + 4f), DeviceChassis, 4f);
        DeviceFill(panel, DevicePaper, 3f);

        // Top header bar
        var headerBar = new Rect(panel.x, panel.y, panel.width, 56f);
        DeviceFill(headerBar, DeviceChassis);
        DeviceFill(new Rect(panel.x, headerBar.yMax - 3f, panel.width, 3f), PhoneClay);

        GUI.Label(new Rect(panel.x + 20f, panel.y + 8f, panel.width - 180f, 24f),
            "KALENDER KAMAR 07  ·  7 HARI TERAKHIR", deviceLightLabel);
        GUI.Label(new Rect(panel.x + 20f, panel.y + 32f, panel.width - 180f, 18f),
            $"Hari {state.day} dari {GameState.MaxDay}  ·  {state.FormattedTime} ({state.session})",
            calendarHeaderSubStyle);

        bool close = GUI.Button(new Rect(panel.xMax - 142f, panel.y + 13f, 124f, 28f), "Tutup [Esc]", buttonStyle);

        // 4 x 2 Grid (Days 1-7 + Today's Summary Card)
        float padX = 16f;
        float padTop = 68f;
        float gap = 10f;
        float gridW = panel.width - padX * 2f;
        float gridH = panel.height - padTop - 16f;
        float cellW = (gridW - gap * 3f) / 4f;
        float cellH = (gridH - gap) / 2f;

        for (int day = 1; day <= GameState.MaxDay; day++)
        {
            int idx = day - 1;
            int col = idx % 4;
            int row = idx / 4;
            var cell = new Rect(panel.x + padX + col * (cellW + gap), panel.y + padTop + row * (cellH + gap), cellW, cellH);
            DrawCalendarDayCard(cell, state, day);
        }

        // 8th slot (bottom-right): Summary & Focus Card
        var summaryCell = new Rect(panel.x + padX + 3f * (cellW + gap), panel.y + padTop + (cellH + gap), cellW, cellH);
        DrawCalendarSummaryCard(summaryCell, state);

        if (close) ClosePanel();
    }

    void DrawCalendarDayCard(Rect rect, GameState state, int day)
    {
        bool isPast = day < state.day;
        bool isToday = day == state.day;
        bool isDeadline = day == GameState.MaxDay;

        Color borderCol = isToday ? PhoneClay : isDeadline ? DesktopGold : new Color32(180, 170, 148, 255);
        Color bgCol = isToday ? DeviceLight : isPast ? new Color32(230, 221, 200, 255) : PortalSurface;
        Color topCol = isToday ? PhoneClay : isPast ? new Color32(158, 152, 138, 255) : isDeadline ? DesktopBar : DesktopTile;

        DeviceFill(new Rect(rect.x - 1f, rect.y - 1f, rect.width + 2f, rect.height + 2f), borderCol, 4f);
        DeviceFill(rect, bgCol, 3f);
        DeviceFill(new Rect(rect.x, rect.y, rect.width, 30f), topCol, 2f);

        GUI.Label(new Rect(rect.x + 10f, rect.y, 76f, 30f), $"HARI {day}", deviceLightLabel);

        string tag = isToday ? "HARI INI" : isPast ? "Selesai" : isDeadline ? "Batas Kos" : "";
        if (!string.IsNullOrEmpty(tag))
        {
            var tagRect = new Rect(rect.xMax - 84f, rect.y + 5f, 76f, 20f);
            DeviceFill(tagRect, isToday ? DeviceLight : isDeadline ? DesktopGold : DevicePaper, 3f);
            GUI.Label(tagRect, tag, portalBadgeStyle);
        }

        // Auto-flowing GUILayout body prevents any overlap or vertical clipping
        var inner = new Rect(rect.x + 10f, rect.y + 36f, rect.width - 20f, rect.height - 44f);
        GUILayout.BeginArea(inner);
        DrawCalendarInterviewBadge(state.applicationA, day);
        DrawCalendarInterviewBadge(state.applicationB, day);
        GUILayout.Label(GetCalendarDayNote(day), isPast ? portalSmallStyle : deviceMessageStyle);
        GUILayout.EndArea();
    }

    void DrawCalendarInterviewBadge(ApplicationRecord app, int day)
    {
        if (app == null || app.status != ApplicationStatus.Scheduled || app.interviewDay != day)
            return;

        string label = $"{app.interviewHour:00}:00  Interview\n{ActivitySystem.GetCompanyName(app.company)}";
        var pill = GUILayoutUtility.GetRect(new GUIContent(label), calendarPillStyle, GUILayout.ExpandWidth(true));
        DeviceFill(pill, DesktopGold, 4f);
        GUI.Label(pill, label, calendarPillStyle);
        GUILayout.Space(6);
    }

    static string GetCalendarDayNote(int day)
    {
        switch (day)
        {
            case 1: return "• Awal pekan di Kamar 07\n• Studio Lajur dibuka\n• Rapikan bukti E1 di papan";
            case 2: return "• Studio Lajur masih dibuka\n• Cek syarat kontrak di laptop";
            case 3: return "• Koperasi Pijar & E2 dibuka\n• Tutup Studio Lajur (22:00)";
            case 4: return "• Tawaran langsung masuk\n• Keputusan akhir terbuka";
            case 5: return "• Tutup Koperasi Pijar (22:00)\n• Pesan lanjutan dari Ibu";
            case 6: return "• Persiapan terakhir sebelum batas kos";
            case 7: return "• Batas sewa kos & tiket pulang\n• Kunci keputusan akhir";
            default: return "";
        }
    }

    void DrawCalendarSummaryCard(Rect rect, GameState state)
    {
        DeviceFill(new Rect(rect.x - 1f, rect.y - 1f, rect.width + 2f, rect.height + 2f), DesktopTile, 4f);
        DeviceFill(rect, PortalSidebar, 3f);
        DeviceFill(new Rect(rect.x, rect.y, rect.width, 30f), DesktopBar, 2f);
        GUI.Label(new Rect(rect.x + 10f, rect.y, rect.width - 20f, 30f), "RINGKASAN", deviceLightLabel);

        int daysLeft = Mathf.Max(0, GameState.MaxDay - state.day);
        string countdown = daysLeft == 0 ? "Hari terakhir di kos." : $"Sisa {daysLeft} hari sebelum batas kos.";
        string hint = ActivitySystem.GetObjectiveHint(state);

        string offerLabel = FormatOfferChoiceLabel(state.offerChoice);

        var inner = new Rect(rect.x + 10f, rect.y + 36f, rect.width - 20f, rect.height - 44f);
        GUILayout.BeginArea(inner);
        GUILayout.Label(countdown, subStyle);
        GUILayout.Space(4);
        GUILayout.Label(hint, deviceMessageStyle);
        GUILayout.FlexibleSpace();
        string footerText = $"Pilihan: {offerLabel}";
        var footerRect = GUILayoutUtility.GetRect(new GUIContent(footerText), calendarPillStyle, GUILayout.ExpandWidth(true));
        DeviceFill(footerRect, DevicePaper, 3f);
        GUI.Label(footerRect, footerText, calendarPillStyle);
        GUILayout.EndArea();
    }

    static string FormatOfferChoiceLabel(OfferChoice choice)
    {
        switch (choice)
        {
            case OfferChoice.StudioLajur: return "Studio Lajur";
            case OfferChoice.KoperasiPijar: return "Koperasi Pijar";
            case OfferChoice.AksesPrima: return "Akses Prima";
            default: return "Pulang";
        }
    }

    void EnsureDeviceStyles()
    {
        if (deviceLightLabel != null) return;
        deviceLightLabel = LockTextColor(new GUIStyle(subStyle)
        {
            alignment = TextAnchor.MiddleLeft
        }, DeviceLight);
        deviceCenteredLabel = LockTextColor(new GUIStyle(subStyle) { alignment = TextAnchor.MiddleCenter }, InkColor);
        deviceClockStyle = LockTextColor(new GUIStyle(deviceLightLabel) { fontSize = 36 }, DeviceLight);
        deviceMessageStyle = LockTextColor(new GUIStyle(bodyStyle)
        {
            alignment = TextAnchor.UpperLeft,
            padding = new RectOffset(),
            richText = false
        }, InkColor);
        calendarHeaderSubStyle = LockTextColor(new GUIStyle(mutedStyle), DevicePaper);
        calendarPillStyle = LockTextColor(new GUIStyle(GUI.skin.label)
        {
            fontSize = 11,
            fontStyle = FontStyle.Bold,
            wordWrap = true,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(6, 6, 4, 4)
        }, InkColor);
    }

    string DeviceTime(GameState state)
    {
        if (deviceClockHour != state.currentHour)
        {
            deviceClockHour = state.currentHour;
            deviceClockText = state.FormattedTime;
        }
        return deviceClockText;
    }

    static void DeviceFill(Rect rect, Color color, float radius = 0f)
    {
        if (Event.current.type == EventType.Repaint)
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0f, color, 0f, radius);
    }

    static void DeviceLine(Vector2 start, Vector2 end, Color color)
    {
        if (Event.current.type != EventType.Repaint) return;
        var prior = GUI.matrix;
        var delta = end - start;
        var angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        GUI.matrix = prior * Matrix4x4.TRS(start, Quaternion.Euler(0f, 0f, angle), Vector3.one);
        DeviceFill(new Rect(0f, 0f, delta.magnitude, 1.5f), color);
        GUI.matrix = prior;
    }
}
