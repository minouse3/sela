using UnityEngine;
using SELA.Core;

public partial class CoreLoopDebugUI
{
    void DrawEvidenceBoardView(GameLoopEngine engine, GameState state, float width, float height)
    {
        EnsureDeviceStyles();
        EnsureJobPortalStyles();

        float w = Mathf.Min(980f, width - 48f);
        float h = Mathf.Min(560f, height - 48f);
        var panel = new Rect((width - w) * 0.5f, (height - h) * 0.5f, w, h);

        // Wooden frame & warm cork/paper board surface
        DeviceFill(new Rect(panel.x + 5f, panel.y + 6f, panel.width, panel.height), DeviceShadow, 5f);
        DeviceFill(new Rect(panel.x - 3f, panel.y - 3f, panel.width + 6f, panel.height + 6f), DeviceChassis, 5f);
        DeviceFill(panel, DevicePaper, 3f);

        // Top header bar
        var headerBar = new Rect(panel.x, panel.y, panel.width, 58f);
        DeviceFill(headerBar, DeviceChassis);
        DeviceFill(new Rect(panel.x, headerBar.yMax - 3f, panel.width, 3f), PhoneClay);

        bool confirming = pendingRequest.HasValue;
        GUI.Label(new Rect(panel.x + 20f, panel.y + 8f, panel.width - 190f, 24f),
            confirming ? "PAPAN BUKTI  ·  KONFIRMASI" : "PAPAN BUKTI  ·  PORTOFOLIO RAKA", deviceLightLabel);
        GUI.Label(new Rect(panel.x + 20f, panel.y + 32f, panel.width - 190f, 18f),
            "Susun bukti kerja yang rapi dan bisa dipertanggungjawabkan saat interview.", calendarHeaderSubStyle);

        string backText = confirming ? "Kembali [Esc]" : "Tutup [Esc]";
        bool close = GUI.Button(new Rect(panel.xMax - 146f, panel.y + 14f, 128f, 30f), backText, buttonStyle);

        var bodyRect = new Rect(panel.x + 18f, panel.y + 70f, panel.width - 36f, panel.height - 86f);
        if (confirming)
        {
            GUILayout.BeginArea(bodyRect);
            GUILayout.BeginVertical(cardStyle);
            DrawConfirmationView(engine, state, pendingRequest.Value);
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        else
        {
            DrawEvidenceBoardCards(engine, state, bodyRect);
        }

        if (close) BackOrClose();
    }

    void DrawEvidenceBoardCards(GameLoopEngine engine, GameState state, Rect area)
    {
        float gap = 14f;
        float footerH = 74f;
        float cardsH = Mathf.Max(260f, area.height - footerH - 12f);
        float cardW = (area.width - gap * 2f) / 3f;

        var r0 = new Rect(area.x, area.y, cardW, cardsH);
        var r1 = new Rect(area.x + cardW + gap, area.y, cardW, cardsH);
        var r2 = new Rect(area.x + (cardW + gap) * 2f, area.y, cardW, cardsH);

        DrawEvidenceSlotCard(
            engine, state, r0,
            code: "BUKTI E0",
            title: "Proyek Kuliah Dasar",
            description: "Contoh antarmuka awal dari proyek kuliah yang dikerjakan bersama tim.",
            requirementHint: "Terpasang sejak awal dan otomatis dilampirkan di setiap CV.",
            isDone: state.hasE0,
            isAttached: true,
            canToggleAttach: false,
            activity: null,
            onToggleAttach: null);

        DrawEvidenceSlotCard(
            engine, state, r1,
            code: "BUKTI E1",
            title: "Perbandingan Antarmuka",
            description: "Perbandingan label dan pesan kesalahan formulir sebelum dan sesudah diperbaiki.",
            requirementHint: "Wajib dilampirkan untuk lamaran Studio Lajur & Koperasi Pijar.",
            isDone: state.hasE1,
            isAttached: state.cvDraft.attachE1,
            canToggleAttach: state.hasE1,
            activity: new ActivityRequest(ActivityType.ImproveUiExample),
            onToggleAttach: () => engine.UpdateCvDraft(state.cvDraft.claim1, state.cvDraft.claim2, !state.cvDraft.attachE1, state.cvDraft.attachE2));

        DrawEvidenceSlotCard(
            engine, state, r2,
            code: "BUKTI E2",
            title: "Catatan Pengujian Manual",
            description: "Catatan uji kolom kosong, format salah, dan data valid dengan batas kesimpulan yang jujur.",
            requirementHint: "Mulai tersedia Hari 3. Wajib dilampirkan untuk Koperasi Pijar.",
            isDone: state.hasE2,
            isAttached: state.cvDraft.attachE2,
            canToggleAttach: state.hasE2,
            activity: new ActivityRequest(ActivityType.CompleteTestNotes),
            onToggleAttach: () => engine.UpdateCvDraft(state.cvDraft.claim1, state.cvDraft.claim2, state.cvDraft.attachE1, !state.cvDraft.attachE2));

        // Footer bar: CV draft status summary
        var footerRect = new Rect(area.x, area.y + cardsH + 12f, area.width, footerH);
        var d = state.cvDraft;
        bool hasExag = d.claim1 == ClaimChoice.Exaggerated || d.claim2 == ClaimChoice.Exaggerated;
        DeviceFill(footerRect, hasExag ? PortalWarnBg : PortalSafeBg, 4f);
        GUILayout.BeginArea(new Rect(footerRect.x + 12f, footerRect.y + 8f, footerRect.width - 24f, footerRect.height - 16f));
        GUILayout.Label(
            $"Lampiran CV aktif:  E0 ✓   ·   E1 {(d.attachE1 ? "✓" : "–")}   ·   E2 {(d.attachE2 ? "✓" : "–")}",
            subStyle);
        GUILayout.Space(2);
        GUILayout.Label(
            hasExag
                ? "Ada klaim di CV yang melebihi bukti. Periksa di Laptop → CV Builder."
                : "Klaim CV sesuai bukti. Edit draf lengkap di Laptop → CV Builder.",
            portalSmallStyle);
        GUILayout.EndArea();
    }

    void DrawEvidenceSlotCard(
        GameLoopEngine engine,
        GameState state,
        Rect rect,
        string code,
        string title,
        string description,
        string requirementHint,
        bool isDone,
        bool isAttached,
        bool canToggleAttach,
        ActivityRequest? activity,
        System.Action onToggleAttach)
    {
        Color borderCol = isDone ? PortalSage : new Color32(176, 165, 142, 255);
        Color headerCol = isDone ? PhoneOlive : DesktopTile;
        Color bodyBg = isDone ? DeviceLight : PortalSurface;

        DeviceFill(new Rect(rect.x - 1f, rect.y - 1f, rect.width + 2f, rect.height + 2f), borderCol, 5f);
        DeviceFill(rect, bodyBg, 4f);
        DeviceFill(new Rect(rect.x, rect.y, rect.width, 34f), headerCol, 3f);

        // Pin tack dot + card code + status pill
        DeviceFill(new Rect(rect.x + 12f, rect.y + 13f, 8f, 8f), isDone ? DesktopGold : PhoneClay, 4f);
        GUI.Label(new Rect(rect.x + 26f, rect.y, 110f, 34f), code, deviceLightLabel);

        string badgeText = isDone ? "Terpasang ✓" : "Belum siap";
        var badgeRect = new Rect(rect.xMax - 102f, rect.y + 6f, 92f, 22f);
        DeviceFill(badgeRect, isDone ? PortalSafeBg : PortalBadgeBg, 4f);
        GUI.Label(badgeRect, badgeText, portalBadgeStyle);

        // Card inner content using top-level GUILayout.BeginArea (zero nesting, zero overlap)
        var inner = new Rect(rect.x + 12f, rect.y + 42f, rect.width - 24f, rect.height - 52f);
        GUILayout.BeginArea(inner);
        GUILayout.Label(title, portalRoleStyle);
        GUILayout.Space(6);
        GUILayout.Label(description, bodyStyle);
        GUILayout.Space(6);
        GUILayout.Label(requirementHint, portalSmallStyle);
        GUILayout.FlexibleSpace();

        if (isDone)
        {
            if (canToggleAttach && onToggleAttach != null)
            {
                string toggleText = isAttached ? "[✓] Terlampir di CV  (Lepas)" : "[  ] Lampirkan ke CV";
                if (GUILayout.Button(toggleText, isAttached ? selectedChoiceStyle : buttonStyle, GUILayout.Height(34)))
                    onToggleAttach();
            }
            else
            {
                GUILayout.Box("Otomatis terlampir di CV", portalSafeBoxStyle);
            }
        }
        else if (activity.HasValue)
        {
            var prev = engine.GetPreview(activity.Value);
            string info = prev.Availability.IsAvailable
                ? $"{prev.StartHour:00}:00 → {prev.EndHour:00}:00  ·  Tenaga {prev.EnergyBefore}→{prev.EnergyAfter}"
                : prev.Availability.Reason;
            GUILayout.Label(info, portalSmallStyle);
            GUILayout.Space(4);

            GUI.enabled = prev.Availability.IsAvailable;
            string shortAction = activity.Value.Type == ActivityType.ImproveUiExample ? "Rapikan Bukti E1" : "Susun Bukti E2";
            string btnLabel = $"{shortAction} ({prev.DurationHours} jam)";
            if (GUILayout.Button(btnLabel, portalPrimaryBtnStyle, GUILayout.Height(36)))
            {
                pendingRequest = activity.Value;
                scrollPos = Vector2.zero;
            }
            GUI.enabled = true;
        }
        GUILayout.EndArea();
    }
}
