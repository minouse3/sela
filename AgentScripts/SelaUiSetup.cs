using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

// Dijalankan lewat: unity command script run --file AgentScripts/SelaUiSetup.cs --entry SelaUiSetup.Run
// Idempotent: boleh dijalankan berkali-kali tanpa merusak hasil sebelumnya.
public static class SelaUiSetup
{
    private const string TmpPackageRelative = "Library/PackageCache/com.unity.ugui@c0ef89c047f6/Package Resources/TMP Essential Resources.unitypackage";
    private const string FontFolder = "Assets/_SELA/UI/Fonts";
    private const string RuntimeFontFolder = "Assets/_SELA/Resources/UI/Fonts";
    private const string PlayableScene = "Assets/_SELA/Scenes/Test/KosV2_PlayerTest.unity";

    public static void Run()
    {
        int steps = 0;
        try
        {
            if (EnsureTmpEssentials())
            {
                steps++;
            }

            if (EnsureFontAssets())
            {
                steps++;
            }

            if (EnsureSceneSetup())
            {
                steps++;
            }

            if (EnsureBuildSettings())
            {
                steps++;
            }

            Debug.Log("[SelaUiSetup] selesai, " + steps + " langkah dijalankan.");
        }
        catch (Exception e)
        {
            Debug.LogError("[SelaUiSetup] gagal: " + e);
        }
    }

    private static bool EnsureTmpEssentials()
    {
        if (AssetDatabase.LoadAssetAtPath<TMPro.TMP_Settings>("Assets/TextMesh Pro/Resources/TMP Settings.asset") != null)
        {
            Debug.Log("[SelaUiSetup] TMP Essential Resources sudah ada.");
            return false;
        }

        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string packagePath = Path.Combine(projectRoot, TmpPackageRelative.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(packagePath))
        {
            string[] found = Directory.GetFiles(projectRoot, "TMP Essential Resources.unitypackage",
                SearchOption.AllDirectories);
            if (found.Length == 0)
            {
                Debug.LogError("[SelaUiSetup] TMP Essential Resources tidak ditemukan.");
                return false;
            }
            packagePath = found[0];
        }

        AssetDatabase.ImportPackage(packagePath, false);
        AssetDatabase.Refresh();
        Debug.Log("[SelaUiSetup] TMP Essential Resources diimpor dari " + packagePath);
        return true;
    }

    private static bool EnsureFontAssets()
    {
        EnsureFolder(RuntimeFontFolder);

        bool created = false;
        created |= CreateFontAsset("NotoSans-Regular.ttf", "Noto Sans Regular SDF");
        created |= CreateFontAsset("NotoSans-Bold.ttf", "Noto Sans Bold SDF");
        return created;
    }

    private static bool CreateFontAsset(string ttfName, string assetName)
    {
        string ttfPath = FontFolder + "/" + ttfName;
        string assetPath = RuntimeFontFolder + "/" + assetName + ".asset";

        if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath) != null)
        {
            return false;
        }

        Font font = AssetDatabase.LoadAssetAtPath<Font>(ttfPath);
        if (font == null)
        {
            Debug.LogError("[SelaUiSetup] font sumber tidak ditemukan: " + ttfPath);
            return false;
        }

        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(font, 90, 9,
            UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA, 1024, 1024,
            AtlasPopulationMode.Static, true);
        if (fontAsset == null)
        {
            Debug.LogError("[SelaUiSetup] gagal membuat font asset untuk " + ttfName);
            return false;
        }

        fontAsset.name = assetName;
        fontAsset.TryAddCharacters(Characters);

        AssetDatabase.CreateAsset(fontAsset, assetPath);

        // Material dan atlas texture harus ikut disimpan sebagai sub-asset, kalau tidak
        // referensi m_AtlasTextures hilang setelah domain reload.
        if (fontAsset.material != null)
        {
            AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
        }
        if (fontAsset.atlasTextures != null)
        {
            for (int i = 0; i < fontAsset.atlasTextures.Length; i++)
            {
                fontAsset.atlasTextures[i].name = assetName + " Atlas";
                AssetDatabase.AddObjectToAsset(fontAsset.atlasTextures[i], fontAsset);
            }
        }

        EditorUtility.SetDirty(fontAsset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[SelaUiSetup] font asset dibuat: " + assetPath);
        return true;
    }

    /// <summary>Huruf latin dasar, angka, tanda baca, dan huruf beraksen yang dipakai teks game.</summary>
    private static readonly string Characters =
        " ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789" +
        "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~" +
        "·—–’“”…°±" +
        "àáâäçèéêëìíîïñòóôöùúûü";

    private static bool EnsureSceneSetup()
    {
        Scene scene = EditorSceneManager.OpenScene(PlayableScene, OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError("[SelaUiSetup] scene tidak bisa dibuka: " + PlayableScene);
            return false;
        }

        bool changed = false;

        SELA.UI.SelaBootstrap bootstrap = UnityEngine.Object.FindFirstObjectByType<SELA.UI.SelaBootstrap>();
        if (bootstrap == null)
        {
            GameObject go = new GameObject("SELA UI Bootstrap");
            bootstrap = go.AddComponent<SELA.UI.SelaBootstrap>();
            changed = true;
            Debug.Log("[SelaUiSetup] SelaBootstrap ditambahkan ke scene.");
        }

        if (UnityEngine.Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem));
            changed = true;
        }

        if (changed)
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        return changed;
    }

    private static bool EnsureBuildSettings()
    {
        var scenes = EditorBuildSettings.scenes.ToList();
        foreach (EditorBuildSettingsScene s in scenes)
        {
            if (s.path == PlayableScene)
            {
                return false;
            }
        }

        scenes.Insert(0, new EditorBuildSettingsScene(PlayableScene, true));
        EditorBuildSettings.scenes = scenes.ToArray();
        Debug.Log("[SelaUiSetup] scene playable ditambahkan ke Build Settings.");
        return true;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            return;
        }

        string[] parts = path.Split('/');
        string current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, parts[i]);
            }
            current = next;
        }
    }
}
