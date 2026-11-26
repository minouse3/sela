using UnityEngine;
using SELA.Core;

public partial class CoreLoopDebugUI
{
    static readonly Color CvSheetPaper = new Color32(255, 253, 248, 255);
    static readonly Color CvSheetRule = new Color32(198, 190, 174, 255);
    static readonly Color CvSheetMuted = new Color32(95, 104, 98, 255);

    GUIStyle cvDocNameStyle;
    GUIStyle cvDocSectionStyle;
    GUIStyle cvDocBodyStyle;
    GUIStyle cvDocMetaStyle;

    void EnsureCvBuilderStyles()
    {
        if (cvDocNameStyle != null) return;
        cvDocNameStyle = LockTextColor(new GUIStyle(GUI.skin.label)
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            wordWrap = false
        }, InkColor);
        cvDocSectionStyle = LockTextColor(new GUIStyle(GUI.skin.label)
        {
            fontSize = 11,
            fontStyle = FontStyle.Bold,
            wordWrap = false
        }, DesktopBar);
        cvDocBodyStyle = LockTextColor(new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            wordWrap = true
        }, InkColor);
        cvDocMetaStyle = LockTextColor(new GUIStyle(GUI.skin.label)
        {
            fontSize = 11,
            wordWrap = true
        }, CvSheetMuted);
    }

    void DrawCvBuilderApp(GameLoopEngine engine, GameState state)
    {
        EnsureJobPortalStyles();
        EnsureCvBuilderStyles();

        var d = state.cvDraft;
        ClaimChoice nextC1 = d.claim1;
        ClaimChoice nextC2 = d.claim2;
        bool attE1 = d.attachE1;
        bool attE2 = d.attachE2;

        // Top bar
        GUILayout.BeginHorizontal();
        GUILayout.Label("CV Builder", portalTitleStyle);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Buka Lamaran →", portalPrimaryBtnStyle, GUILayout.Width(148), GUILayout.Height(30)))
            SetLaptopPage(LaptopPage.Applications);
        GUILayout.EndHorizontal();
        GUILayout.Space(8);

        // Split Screen: Left = Editor Controls, Right = Live A4 Paper CV Preview
        GUILayout.BeginHorizontal();

        // LEFT: Resume Editor
        GUILayout.BeginVertical(cardStyle, GUILayout.Width(390));
        GUILayout.Label("Isi Pengalaman", portalRoleStyle);
        GUILayout.Space(6);

        GUILayout.Label("Antarmuka (UI)", subStyle);
        if (GUILayout.Button(
            $"{(d.claim1 == ClaimChoice.Honest ? "●" : "○")}  Merapikan antarmuka formulir pada proyek kuliah bersama tim.",
            d.claim1 == ClaimChoice.Honest ? selectedChoiceStyle : dialogueChoiceStyle))
            nextC1 = ClaimChoice.Honest;
        if (GUILayout.Button(
            $"{(d.claim1 == ClaimChoice.Exaggerated ? "●" : "○")}  Memimpin perancangan sistem produksi berskala besar.",
            d.claim1 == ClaimChoice.Exaggerated ? selectedChoiceStyle : dialogueChoiceStyle))
            nextC1 = ClaimChoice.Exaggerated;

        GUILayout.Space(10);
        GUILayout.Label("Pengujian (QA)", subStyle);
        if (GUILayout.Button(
            $"{(d.claim2 == ClaimChoice.Honest ? "●" : "○")}  Menguji formulir secara manual (kosong, format salah, valid).",
            d.claim2 == ClaimChoice.Honest ? selectedChoiceStyle : dialogueChoiceStyle))
            nextC2 = ClaimChoice.Honest;
        if (GUILayout.Button(
            $"{(d.claim2 == ClaimChoice.Exaggerated ? "●" : "○")}  Menjamin seluruh aplikasi bebas bug di semua kondisi.",
            d.claim2 == ClaimChoice.Exaggerated ? selectedChoiceStyle : dialogueChoiceStyle))
            nextC2 = ClaimChoice.Exaggerated;

        GUILayout.Space(12);
        GUILayout.Label("Lampiran Portofolio", subStyle);
        GUILayout.Label("[✓]  E0 · Proyek Kuliah Dasar", bodyStyle);

        GUI.enabled = state.hasE1;
        string e1Text = state.hasE1
            ? $"{(attE1 ? "[✓]" : "[  ]")}  E1 · Perbandingan Antarmuka"
            : "[  ]  E1 · Belum tersedia (Papan Bukti)";
        if (GUILayout.Button(e1Text, attE1 ? selectedChoiceStyle : dialogueChoiceStyle))
            attE1 = !attE1;

        GUI.enabled = state.hasE2;
        string e2Text = state.hasE2
            ? $"{(attE2 ? "[✓]" : "[  ]")}  E2 · Catatan Pengujian Manual"
            : "[  ]  E2 · Belum tersedia (Hari 3)";
        if (GUILayout.Button(e2Text, attE2 ? selectedChoiceStyle : dialogueChoiceStyle))
            attE2 = !attE2;
        GUI.enabled = true;

        GUILayout.Space(10);
        GUILayout.Label("Perubahan tersimpan otomatis.", portalSmallStyle);
        GUILayout.EndVertical();

        GUILayout.Space(16);

        // RIGHT: Live A4 Resume Sheet Preview
        GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        DrawLiveCvSheetPreview(state, nextC1, nextC2, attE1, attE2);
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        if (nextC1 != d.claim1 || nextC2 != d.claim2 || attE1 != d.attachE1 || attE2 != d.attachE2)
            engine.UpdateCvDraft(nextC1, nextC2, attE1, attE2);
    }

    void DrawLiveCvSheetPreview(GameState state, ClaimChoice c1, ClaimChoice c2, bool attE1, bool attE2)
    {
        var sheetRect = GUILayoutUtility.GetRect(360f, 430f, GUILayout.ExpandWidth(true));

        // Crisp A4 paper sheet with drop shadow
        DeviceFill(new Rect(sheetRect.x + 4f, sheetRect.y + 5f, sheetRect.width, sheetRect.height), DeviceShadow, 3f);
        DeviceFill(new Rect(sheetRect.x - 1f, sheetRect.y - 1f, sheetRect.width + 2f, sheetRect.height + 2f), CvSheetRule, 3f);
        DeviceFill(sheetRect, CvSheetPaper, 2f);

        // Top subtle slate bar on resume header
        DeviceFill(new Rect(sheetRect.x, sheetRect.y, sheetRect.width, 6f), DesktopBar);

        float pad = 22f;
        float x = sheetRect.x + pad;
        float w = sheetRect.width - pad * 2f;
        float y = sheetRect.y + 18f;

        // Candidate Header
        GUI.Label(new Rect(x, y, w, 26f), "RAKA PRATAMA", cvDocNameStyle);
        y += 26f;
        GUI.Label(new Rect(x, y, w, 18f), "Junior Frontend & QA   ·   S1 Informatika   ·   raka.pratama@mail.id", cvDocMetaStyle);
        y += 24f;
        DeviceFill(new Rect(x, y, w, 1f), CvSheetRule);
        y += 12f;

        // Section: Ringkasan
        GUI.Label(new Rect(x, y, w, 16f), "RINGKASAN", cvDocSectionStyle);
        y += 18f;
        y += DrawCvWrappedLine(x, y, w, "Lulusan baru Informatika dengan fokus pada kerapian antarmuka formulir dan pengujian manual aplikasi web.", cvDocBodyStyle) + 12f;

        // Section: Pengalaman Proyek
        GUI.Label(new Rect(x, y, w, 16f), "PENGALAMAN PROYEK", cvDocSectionStyle);
        y += 18f;
        string exp1 = c1 == ClaimChoice.Honest
            ? "• Membantu merapikan antarmuka formulir pada proyek kuliah bersama tim."
            : "• Memimpin perancangan sistem produksi berskala besar.";
        string exp2 = c2 == ClaimChoice.Honest
            ? "• Menguji formulir secara manual pada skenario kosong, format salah, dan data valid."
            : "• Menjamin seluruh aplikasi bebas bug di semua kondisi.";
        y += DrawCvWrappedLine(x, y, w, exp1, cvDocBodyStyle) + 6f;
        y += DrawCvWrappedLine(x, y, w, exp2, cvDocBodyStyle) + 14f;

        // Section: Portofolio Terlampir
        GUI.Label(new Rect(x, y, w, 16f), "LAMPIRAN PORTOFOLIO", cvDocSectionStyle);
        y += 18f;
        y += DrawCvWrappedLine(x, y, w, "• E0 — Proyek Kuliah Dasar (Antarmuka Web)", cvDocBodyStyle) + 4f;
        if (state.hasE1 && attE1)
            y += DrawCvWrappedLine(x, y, w, "• E1 — Perbandingan Antarmuka Formulir", cvDocBodyStyle) + 4f;
        if (state.hasE2 && attE2)
            y += DrawCvWrappedLine(x, y, w, "• E2 — Catatan Pengujian Manual", cvDocBodyStyle) + 4f;
        if ((!state.hasE1 || !attE1) && (!state.hasE2 || !attE2))
            DrawCvWrappedLine(x, y, w, "Belum ada lampiran tambahan.", cvDocMetaStyle);

        // Section: Pendidikan (anchored cleanly near bottom of sheet)
        float eduY = Mathf.Max(y + 14f, sheetRect.yMax - 58f);
        DeviceFill(new Rect(x, eduY - 8f, w, 1f), CvSheetRule);
        GUI.Label(new Rect(x, eduY, w, 16f), "PENDIDIKAN", cvDocSectionStyle);
        GUI.Label(new Rect(x, eduY + 18f, w, 18f), "S1 Teknik Informatika  ·  Lulusan Baru", cvDocMetaStyle);
    }

    float DrawCvWrappedLine(float x, float y, float width, string text, GUIStyle style)
    {
        deviceMeasureContent.text = text;
        float h = style.CalcHeight(deviceMeasureContent, width);
        GUI.Label(new Rect(x, y, width, h + 2f), text, style);
        return h;
    }
}
