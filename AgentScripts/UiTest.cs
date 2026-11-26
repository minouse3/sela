using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Alat bantu pengujian antarmuka saat game berjalan di Editor.
/// Dipakai lewat: unity command script run --file AgentScripts/UiTest.cs --entry UiTest.Click --args '["label"]'
/// Entry point: Click, Label, Buttons, Screens, Dump, State, GiveEvidence, Open, OpenArg,
/// SetDay, SetSession, SaveReload.
/// </summary>
public static class UiTest
{
    /// <summary>
    /// Menekan tombol aktif yang labelnya cocok. Kecocokan persis diutamakan; kalau tidak ada,
    /// dipakai satu-satunya label yang mengandung teks itu. Tombol terkunci tidak ditekan,
    /// supaya uji tidak lolos di panel yang sebenarnya memblokir pemain.
    /// </summary>
    public static string Click(string target)
    {
        Button exact = null;
        Button exactBlocked = null;
        Button partial = null;
        Button partialBlocked = null;
        int partialCount = 0;

        foreach (Button b in ActiveButtons())
        {
            TextMeshProUGUI tmp = b.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp == null) { continue; }
            if (tmp.text == target)
            {
                // Panel belakang bisa punya label sama; dahulukan yang benar-benar bisa diklik.
                if (IsHittable(b)) { exact = b; break; }
                if (exactBlocked == null) { exactBlocked = b; }
                continue;
            }

            if (tmp.text.Contains(target))
            {
                partialCount++;
                if (IsHittable(b))
                {
                    if (partial == null) { partial = b; }
                }
                else if (partialBlocked == null)
                {
                    partialBlocked = b;
                }
            }
        }

        Button found = exact != null ? exact
            : partial != null ? partial
            : exactBlocked != null ? exactBlocked
            : partialBlocked;
        if (found == null)
        {
            return "TIDAK ADA: " + target + " || aktif: " + Label();
        }

        if (!found.interactable)
        {
            return "TERKUNCI: " + target + " || aktif: " + Label();
        }

        if (!IsHittable(found))
        {
            return "TERTUTUP/DI LUAR AREA: " + target + " || aktif: " + Label();
        }

        TextMeshProUGUI label = found.GetComponentInChildren<TextMeshProUGUI>();
        found.onClick.Invoke();
        string note = exact != null ? "KLIK" : "KLIK(sebagian, " + partialCount + " kandidat)";
        return note + ": " + (label != null ? label.text : target);
    }

    /// <summary>
    /// Menggeser scroll di dalam satu layar. Nilai: "top", "bottom", atau 0..1.
    /// Dipakai supaya tombol yang berada di bawah lipatan benar-benar terlihat dulu
    /// sebelum ditekan, bukan ditekan secara buta.
    /// </summary>
    public static string Scroll(string screenName, string value)
    {
        Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) { return "(tidak ada canvas)"; }
        Transform root = canvas.transform.Find("Screens");
        Transform target = root != null ? root.Find(screenName) : null;
        if (target == null) { return "(layar " + screenName + " tidak ada) || terbuka: " + Screens(); }

        float position;
        if (value == "top") { position = 1f; }
        else if (value == "bottom") { position = 0f; }
        else if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out position))
        {
            return "(posisi '" + value + "' bukan angka)";
        }

        ScrollRect[] scrolls = target.GetComponentsInChildren<ScrollRect>(true);
        int changed = 0;
        foreach (ScrollRect scroll in scrolls)
        {
            if (!scroll.gameObject.activeInHierarchy) { continue; }
            scroll.verticalNormalizedPosition = Mathf.Clamp01(position);
            changed++;
        }

        Canvas.ForceUpdateCanvases();
        return "SCROLL " + screenName + " -> " + position + " (" + changed + " scroll)";
    }

    /// <summary>Benar bila tombol benar-benar kena klik, bukan tertutup panel lain atau terpotong.</summary>
    private static bool IsHittable(Button button)
    {
        RectTransform rect = button.transform as RectTransform;
        if (rect == null) { return true; }

        Canvas canvas = button.GetComponentInParent<Canvas>();
        GraphicRaycaster raycaster = canvas != null ? canvas.GetComponent<GraphicRaycaster>() : null;
        if (raycaster == null || EventSystem.current == null) { return true; }

        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        Vector2 point = (corners[0] + corners[2]) * 0.5f;
        if (point.x < 0f || point.y < 0f || point.x > Screen.width || point.y > Screen.height)
        {
            return false;
        }

        PointerEventData data = new PointerEventData(EventSystem.current) { position = point };
        List<RaycastResult> hits = new List<RaycastResult>();
        raycaster.Raycast(data, hits);
        if (hits.Count == 0) { return false; }

        Transform top = hits[0].gameObject.transform;
        return top == button.transform || top.IsChildOf(button.transform);
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

    /// <summary>Daftar label tombol beserta status kuncinya, dipakai untuk bukti uji.</summary>
    public static string Buttons()
    {
        StringBuilder sb = new StringBuilder();
        foreach (Button b in ActiveButtons())
        {
            TextMeshProUGUI tmp = b.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp == null) { continue; }
            sb.Append("'" + tmp.text + "'" + (b.interactable ? "" : "(terkunci)") + " ");
        }
        return sb.ToString();
    }

    /// <summary>Daftar layar yang sedang terbuka.</summary>
    public static string Screens()
    {
        Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
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
        Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
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

    /// <summary>Teks penuh tiap label pada layar terbuka, untuk memeriksa naskah.</summary>
    public static string Texts(string screenName)
    {
        Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
        Transform root = canvas.transform.Find("Screens");
        if (root == null) { return "(tidak ada Screens)"; }

        Transform target = root.Find(screenName);
        if (target == null)
        {
            return "(layar " + screenName + " tidak ada) || terbuka: " + Screens();
        }

        StringBuilder sb = new StringBuilder();
        TextMeshProUGUI[] labels = target.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI label in labels)
        {
            if (string.IsNullOrEmpty(label.text)) { continue; }
            sb.Append("[" + label.name + "] ");
            sb.Append(label.text.Replace("\n", " / "));
            sb.Append(" || ");
        }
        return sb.Length == 0 ? "(tidak ada teks)" : sb.ToString();
    }

    /// <summary>Ringkasan state permainan yang sedang berjalan.</summary>
    public static string State()
    {
        SELA.Narrative.SelaState s = SELA.Narrative.SelaState.Current;
        StringBuilder sb = new StringBuilder();
        sb.Append("Hari " + s.Day + " " + SELA.Narrative.SelaContent.SessionLabel(s.Session)
            + " (sesiSelesai " + s.SessionResolved + ")"
            + " | Tenaga " + s.Energy + "/" + SELA.Narrative.SelaState.MaxEnergy
            + " Tekanan " + s.Pressure + "/" + SELA.Narrative.SelaState.MaxPressure
            + " | bukti: ");
        foreach (SELA.Narrative.EvidenceId id in s.Evidence) { sb.Append(id + " "); }
        sb.Append(" | lamaran: ");
        for (int i = 0; i < 3; i++) { sb.Append(s.StatusOf((SELA.Narrative.OfferId)i) + " "); }
        sb.Append(" | dikirimHari ");
        for (int i = 0; i < 3; i++) { sb.Append(s.ApplicationSentDay[i] + " "); }
        sb.Append(" | skor ");
        for (int i = 0; i < 3; i++) { sb.Append(s.ApplicationInterviewScore[i] + " "); }
        sb.Append(" | cvClaim " + s.CvClaim[0] + "," + s.CvClaim[1]);
        sb.Append(" | cvAttached ");
        foreach (SELA.Narrative.EvidenceId id in s.CvAttached) { sb.Append(id + " "); }
        sb.Append(" | verify ");
        for (int i = 0; i < 3; i++) { sb.Append(s.VerificationDone[i] + " "); }
        sb.Append(" | retakan ");
        for (int i = 0; i < 3; i++) { sb.Append(s.RiftSolved[i] + " "); }
        sb.Append(" | pilihan " + s.OfferChoice);
        sb.Append(" | catatan '" + (s.DecisionNote ?? string.Empty) + "'");
        sb.Append(" | pesanDibaca " + s.ReadMessages.Count);
        sb.Append(" | tutorialSeen " + s.TutorialSeen);
        return sb.ToString();
    }

    /// <summary>Memberi nilai bukti tertentu, untuk menguji panel yang butuh bukti ada.</summary>
    public static string GiveEvidence(string[] ids)
    {
        SELA.Narrative.SelaState s = SELA.Narrative.SelaState.Current;
        foreach (string raw in ids)
        {
            SELA.Narrative.EvidenceId id = (SELA.Narrative.EvidenceId)Enum.Parse(
                typeof(SELA.Narrative.EvidenceId), raw);
            if (!s.Evidence.Contains(id)) { s.Evidence.Add(id); }
        }
        s.NotifyChanged();
        return State();
    }

    /// <summary>
    /// Membuka layar lewat metode publik SelaBootstrap, misalnya "OpenLaptop".
    /// Hanya metode tanpa parameter; untuk metode berparameter pakai OpenArg.
    /// </summary>
    public static string Open(string methodName)
    {
        SELA.UI.SelaBootstrap boot = SELA.UI.SelaBootstrap.Instance;
        if (boot == null) { return "(SelaBootstrap tidak ada)"; }

        MethodInfo[] methods = typeof(SELA.UI.SelaBootstrap).GetMethods(
            BindingFlags.Instance | BindingFlags.Public);
        foreach (MethodInfo m in methods)
        {
            if (m.Name != methodName) { continue; }
            if (m.GetParameters().Length != 0) { continue; }
            m.Invoke(boot, null);
            return "BUKA: " + methodName;
        }
        return "(metode " + methodName + " tidak ada atau butuh argumen)";
    }

    /// <summary>
    /// Membuka layar lewat metode publik SelaBootstrap yang punya satu parameter,
    /// misalnya OpenArg("OpenVerification", "A") atau OpenArg("OpenRift", "1").
    /// </summary>
    public static string OpenArg(string methodName, string arg)
    {
        SELA.UI.SelaBootstrap boot = SELA.UI.SelaBootstrap.Instance;
        if (boot == null) { return "(SelaBootstrap tidak ada)"; }

        MethodInfo[] methods = typeof(SELA.UI.SelaBootstrap).GetMethods(
            BindingFlags.Instance | BindingFlags.Public);
        foreach (MethodInfo m in methods)
        {
            if (m.Name != methodName) { continue; }
            ParameterInfo[] parameters = m.GetParameters();
            if (parameters.Length != 1) { continue; }

            object value;
            if (!TryConvert(arg, parameters[0].ParameterType, out value))
            {
                return "(argumen '" + arg + "' tidak cocok untuk " + methodName + "("
                    + parameters[0].ParameterType.Name + "))";
            }

            m.Invoke(boot, new[] { value });
            return "BUKA: " + methodName + "(" + arg + ")";
        }
        return "(metode " + methodName + " dengan satu parameter tidak ada)";
    }

    /// <summary>Menutup panel teratas, sama seperti menekan Esc. Hanya untuk alat uji.</summary>
    public static string CloseTop()
    {
        SELA.UI.UiRouter router = SELA.UI.UiRouter.Instance;
        if (router == null) { return "(UiRouter tidak ada)"; }
        string before = Screens();
        router.CloseTop();
        return "TUTUP TERATAS: " + before + " -> " + Screens();
    }

    /// <summary>Memindahkan hari tanpa tidur, untuk menguji undangan interview hari berikutnya.</summary>
    public static string SetDay(string value)
    {
        int day;
        if (!int.TryParse(value, out day))
        {
            return "(hari '" + value + "' bukan angka)";
        }

        SELA.Narrative.SelaState s = SELA.Narrative.SelaState.Current;
        s.Day = Mathf.Max(1, day);
        s.NotifyChanged();
        return State();
    }

    /// <summary>Memindahkan sesi, misalnya SetSession("Siang").</summary>
    public static string SetSession(string value)
    {
        SELA.Narrative.SelaState s = SELA.Narrative.SelaState.Current;
        try
        {
            s.Session = (SELA.Narrative.SelaSession)Enum.Parse(
                typeof(SELA.Narrative.SelaSession), value, true);
        }
        catch (ArgumentException)
        {
            return "(sesi '" + value + "' tidak dikenal)";
        }

        s.SessionResolved = false;
        s.NotifyChanged();
        return State();
    }

    /// <summary>Menulis simpanan, lalu membacanya kembali seperti setelah aplikasi dibuka ulang.</summary>
    public static string SaveReload()
    {
        SELA.Narrative.SelaState current = SELA.Narrative.SelaState.Current;
        string before = State();
        SELA.Narrative.SelaSaveStore.Write(current);
        SELA.Narrative.SelaState loaded = SELA.Narrative.SelaSaveStore.Read();
        if (loaded == null)
        {
            return "GAGAL MEMBACA SIMPANAN || sebelum: " + before;
        }

        SELA.Narrative.SelaState.Adopt(loaded);
        return "SEBELUM: " + before + "\nSESUDAH: " + State();
    }

    private static bool TryConvert(string raw, Type type, out object value)
    {
        value = null;
        if (type == typeof(string))
        {
            value = raw;
            return true;
        }

        if (type == typeof(int))
        {
            int number;
            if (!int.TryParse(raw, out number)) { return false; }
            value = number;
            return true;
        }

        if (type.IsEnum)
        {
            try
            {
                value = Enum.Parse(type, raw, true);
                return true;
            }
            catch (ArgumentException)
            {
                int number;
                if (!int.TryParse(raw, out number)) { return false; }
                value = Enum.ToObject(type, number);
                return true;
            }
        }

        return false;
    }

    private static List<Button> ActiveButtons()
    {
        List<Button> result = new List<Button>();
        foreach (Button b in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
        {
            if (b.gameObject.activeInHierarchy) { result.Add(b); }
        }
        return result;
    }
}
