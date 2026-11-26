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
        float w = Mathf.Min(850f, width - 80f);
        float h = Mathf.Min(500f, height - 56f);
        var shell = new Rect((width - w) * 0.5f, (height - h) * 0.5f, w, h);
        var screen = new Rect(shell.x + 10f, shell.y + 13f, w - 20f, h - 29f);
        var desktop = new Rect(screen.x, screen.y, screen.width, screen.height - 38f);
        var taskbar = new Rect(screen.x, desktop.yMax, screen.width, 38f);
        var page = laptopPage;
        bool inDialogue = dialogue != null;
        bool confirming = pendingRequest.HasValue;
        bool appOpen = page != LaptopPage.Home || inDialogue || confirming;
        string appName = page != LaptopPage.Home ? DesktopAppName(page) : inDialogue ? "Percakapan" : "Konfirmasi";

        DeviceFill(new Rect(shell.x + 3f, shell.y + 5f, w, h + 4f), DeviceShadow, 12f);
        DeviceFill(shell, DeviceChassis, 10f);
        DeviceFill(new Rect(shell.center.x - 2f, shell.y + 4f, 4f, 4f), DesktopBlue, 2f);
        DeviceFill(new Rect(shell.x - 6f, shell.yMax - 9f, w + 12f, 12f), DeviceChassis, 5f);
        DeviceFill(new Rect(shell.center.x - 34f, shell.yMax - 9f, 68f, 3f), DesktopBlue, 1f);
        DrawLaptopWallpaper(desktop);

        if (appOpen)
            DrawLaptopWindow(engine, state, desktop, page, appName, inDialogue, confirming);
        else
            DrawLaptopLauncher(desktop);

        DeviceFill(taskbar, DesktopBar);
        DeviceFill(new Rect(taskbar.x + 14f, taskbar.center.y - 4f, 8f, 8f), appOpen ? DesktopGold : DevicePaper, 2f);
        GUI.Label(new Rect(taskbar.x + 30f, taskbar.y, 150f, taskbar.height), appOpen ? appName : "Desktop", deviceLightLabel);
        float clockX = taskbar.xMax - (appOpen ? 62f : 208f);
        DeviceFill(new Rect(clockX - 8f, taskbar.y + 10f, 1f, 18f), DesktopBlue);
        GUI.Label(new Rect(clockX, taskbar.y, 54f, taskbar.height), DeviceTime(state), deviceLightLabel);
        if (!appOpen && GUI.Button(new Rect(taskbar.xMax - 148f, taskbar.y + 5f, 141f, 28f), "Tutup laptop [Esc]", buttonStyle))
            ClosePanel();
    }

    void DrawLaptopLauncher(Rect desktop)
    {
        float x = desktop.x + 22f;
        float y = desktop.y + Mathf.Max(12f, (desktop.height - 184f) * 0.5f);
        if (DrawLaptopTile(new Rect(x, y, 157f, 54f), LaptopPage.Applications))
            SetLaptopPage(LaptopPage.Applications);
        if (DrawLaptopTile(new Rect(x, y + 65f, 157f, 54f), LaptopPage.WorkMessages))
            SetLaptopPage(LaptopPage.WorkMessages);
        if (DrawLaptopTile(new Rect(x, y + 130f, 157f, 54f), LaptopPage.Notes))
            SetLaptopPage(LaptopPage.Notes);
    }

    bool DrawLaptopTile(Rect rect, LaptopPage page)
    {
        DeviceFill(rect, rect.Contains(Event.current.mousePosition) ? DesktopTileHover : DesktopTile, 5f);
        DrawDesktopIcon(new Rect(rect.x + 10f, rect.y + 10f, 34f, 34f), page);
        GUI.Label(new Rect(rect.x + 54f, rect.y, rect.width - 60f, rect.height), DesktopAppName(page), deviceLightLabel);
        return GUI.Button(rect, GUIContent.none, GUIStyle.none);
    }

    void DrawLaptopWindow(GameLoopEngine engine, GameState state, Rect desktop, LaptopPage page, string appName, bool inDialogue, bool confirming)
    {
        var window = new Rect(desktop.x + 12f, desktop.y + 10f, desktop.width - 24f, desktop.height - 20f);
        DeviceFill(new Rect(window.x + 3f, window.y + 3f, window.width, window.height), DeviceShadow, 5f);
        DeviceFill(window, DevicePaper, 5f);
        DeviceFill(new Rect(window.x, window.y, window.width, 36f), DesktopTile, 4f);
        if (page != LaptopPage.Home)
            DrawDesktopIcon(new Rect(window.x + 8f, window.y + 5f, 26f, 26f), page);
        GUI.Label(new Rect(window.x + 42f, window.y, window.width - 176f, 36f), appName, deviceLightLabel);
        string backLabel = inDialogue ? "Batal [Esc]" : confirming ? "Kembali [Esc]" : "Desktop [Esc]";
        bool back = GUI.Button(new Rect(window.xMax - 127f, window.y + 4f, 120f, 28f), backLabel, buttonStyle);

        GUILayout.BeginArea(new Rect(window.x + 12f, window.y + 44f, window.width - 24f, window.height - 52f));
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
            case LaptopPage.WorkMessages: return "Pesan Kerja";
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

    void EnsureDeviceStyles()
    {
        if (deviceLightLabel != null) return;
        deviceLightLabel = new GUIStyle(subStyle)
        {
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = DeviceLight }
        };
        deviceCenteredLabel = new GUIStyle(subStyle) { alignment = TextAnchor.MiddleCenter };
        deviceClockStyle = new GUIStyle(deviceLightLabel) { fontSize = 36 };
        deviceMessageStyle = new GUIStyle(bodyStyle)
        {
            alignment = TextAnchor.UpperLeft,
            padding = new RectOffset(),
            richText = false
        };
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
