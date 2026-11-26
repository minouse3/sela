using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Alat bantu pengujian antarmuka saat game berjalan di Editor.
/// Dipakai lewat: unity command script run --file AgentScripts/UiTest.cs --entry UiTest.Click --args '["label"]'
/// Entry point: Click, Label, Screens, Dump, State.
/// </summary>
public static class UiTest
{
    /// <summary>Menekan tombol yang labelnya mengandung teks yang diberikan.</summary>
    public static string Click(string target)
    {
        Button found = null;
        foreach (Button b in ActiveButtons())
        {
            TextMeshProUGUI tmp = b.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp == null) { continue; }
            if (tmp.text == target || tmp.text.Contains(target))
            {
                found = b;
                break;
            }
        }

        if (found == null)
        {
            return "TIDAK ADA: " + target + " || aktif: " + Label();
        }

        found.onClick.Invoke();
        return "KLIK: " + target;
    }

    /// <summary>Daftar seluruh label tombol yang sedang aktif.</summary>
    public static string Label()
    {
        StringBuilder sb = new StringBuilder();
        foreach (Button b in ActiveButtons())
        {
            TextMeshProUGUI tmp = b.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) { sb.Append("'" + tmp.text + "' "); }
        }
        return sb.ToString();
    }

    /// <summary>Daftar layar yang sedang terbuka.</summary>
    public static string Screens()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) { return "(tidak ada canvas)"; }
        Transform root = canvas.transform.Find("Screens");
        if (root == null) { return "(tidak ada Screens)"; }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < root.childCount; i++)
        {
            Transform t = root.GetChild(i);
            if (t.gameObject.activeInHierarchy) { sb.Append(t.name + " | "); }
        }
        return sb.Length == 0 ? "(tidak ada layar terbuka)" : sb.ToString();
    }

    /// <summary>Posisi dan ukuran tiap elemen pada layar yang sedang terbuka.</summary>
    public static string Dump(string screenName)
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        Transform root = canvas.transform.Find("Screens");
        if (root == null) { return "(tidak ada Screens)"; }

        Transform target = root.Find(screenName);
        if (target == null)
        {
            return "(layar " + screenName + " tidak ada) || terbuka: " + Screens();
        }

        StringBuilder sb = new StringBuilder();
        sb.Append("layar " + screenName + " || " + Screen.width + "x" + Screen.height + " || ");
        foreach (Transform t in target.GetComponentsInChildren<Transform>(true))
        {
            RectTransform rt = t as RectTransform;
            if (rt == null) { continue; }

            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            Graphic graphic = t.GetComponent<Graphic>();
            TextMeshProUGUI tmp = t.GetComponent<TextMeshProUGUI>();

            string color = graphic != null ? ColorUtility.ToHtmlStringRGBA(graphic.color) : "-";
            string label = tmp != null ? tmp.text : (graphic != null ? graphic.GetType().Name : "");
            if (label.Length > 30) { label = label.Substring(0, 30); }

            sb.Append(t.name + "[" + color + "](" + (int)corners[0].x + "," + (int)corners[0].y + "-"
                + (int)corners[2].x + "," + (int)corners[2].y + ") '" + label + "' | ");
        }
        return sb.ToString();
    }

    /// <summary>Ringkasan state permainan yang sedang berjalan.</summary>
    public static string State()
    {
        SELA.Narrative.SelaState s = SELA.Narrative.SelaState.Current;
        StringBuilder sb = new StringBuilder();
        sb.Append("Hari " + s.Day + " " + SELA.Narrative.SelaContent.SessionLabel(s.Session)
            + " | Tenaga " + s.Energy + "/" + SELA.Narrative.SelaState.MaxEnergy
            + " Tekanan " + s.Pressure + "/" + SELA.Narrative.SelaState.MaxPressure
            + " | bukti: ");
        foreach (SELA.Narrative.EvidenceId id in s.Evidence) { sb.Append(id + " "); }
        sb.Append(" | lamaran: ");
        for (int i = 0; i < 3; i++) { sb.Append(s.StatusOf((SELA.Narrative.OfferId)i) + " "); }
        sb.Append(" | cvClaim " + s.CvClaim[0] + "," + s.CvClaim[1]);
        sb.Append(" | cvAttached ");
        foreach (SELA.Narrative.EvidenceId id in s.CvAttached) { sb.Append(id + " "); }
        sb.Append(" | verify ");
        for (int i = 0; i < 3; i++) { sb.Append(s.VerificationDone[i] + " "); }
        sb.Append(" | tutorialSeen " + s.TutorialSeen);
        return sb.ToString();
    }

    /// <summary>Memberi nilai bukti tertentu, untuk menguji panel yang butuh bukti ada.</summary>
    public static string GiveEvidence(string[] ids)
    {
        SELA.Narrative.SelaState s = SELA.Narrative.SelaState.Current;
        foreach (string raw in ids)
        {
            s.Evidence.Add((SELA.Narrative.EvidenceId)System.Enum.Parse(typeof(SELA.Narrative.EvidenceId), raw));
        }
        s.NotifyChanged();
        return State();
    }

    /// <summary>
    /// Membuka layar lewat metode publik SelaBootstrap, misalnya "OpenLaptop".
    /// Dipakai untuk menguji panel yang biasanya dibuka dari trigger benda 3D.
    /// </summary>
    public static string Open(string methodName)
    {
        SELA.UI.SelaBootstrap boot = SELA.UI.SelaBootstrap.Instance;
        if (boot == null) { return "(SelaBootstrap tidak ada)"; }

        System.Reflection.MethodInfo[] methods = typeof(SELA.UI.SelaBootstrap).GetMethods(
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        foreach (System.Reflection.MethodInfo m in methods)
        {
            if (m.Name != methodName) { continue; }
            if (m.GetParameters().Length != 0) { continue; }
            m.Invoke(boot, null);
            return "BUKA: " + methodName;
        }
        return "(metode " + methodName + " tidak ada)";
    }

    private static List<Button> ActiveButtons()
    {
        List<Button> result = new List<Button>();
        foreach (Button b in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            if (b.gameObject.activeInHierarchy) { result.Add(b); }
        }
        return result;
    }
}
