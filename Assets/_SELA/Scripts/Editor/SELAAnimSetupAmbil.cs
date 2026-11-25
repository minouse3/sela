using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class SELAAnimSetupAmbil
{
    static string Bersih(string n) { int i = n.LastIndexOf('|'); return i >= 0 ? n.Substring(i + 1) : n; }

    [MenuItem("SELA/Setup Animasi Ambil-Pegang-Taruh")]
    static void Run()
    {
    var guids = AssetDatabase.FindAssets("SELA_Anim_AmbilPegangTaruh t:Model");
    if (guids.Length == 0) { Debug.LogError("SELA_Anim_AmbilPegangTaruh.fbx tidak ditemukan."); return; }
    string path = AssetDatabase.GUIDToAssetPath(guids[0]);

    var imp = (ModelImporter)AssetImporter.GetAtPath(path);
    imp.animationType = ModelImporterAnimationType.Human;
    imp.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
    imp.importAnimation = true;
    imp.SaveAndReimport();

    imp = (ModelImporter)AssetImporter.GetAtPath(path);

    // Pakai default; kalau kosong, buat sendiri dari daftar take di FBX
    var clips = imp.defaultClipAnimations;
    Debug.Log("SELA: defaultClipAnimations=" + clips.Length + ", takes=" + imp.importedTakeInfos.Length);
    if (clips.Length == 0)
    {
        var list = new System.Collections.Generic.List<ModelImporterClipAnimation>();
        foreach (var tk in imp.importedTakeInfos)
        {
            if (tk.stopTime <= tk.startTime) continue;
            var c = new ModelImporterClipAnimation();
            c.takeName = tk.name;
            c.name = tk.defaultClipName;
            c.firstFrame = Mathf.Round(tk.startTime * tk.sampleRate);
            c.lastFrame = Mathf.Round(tk.stopTime * tk.sampleRate);
            list.Add(c);
        }
        clips = list.ToArray();
    }
    if (clips.Length == 0)
    {
        Debug.LogError("SELA: FBX tidak punya take yang terbaca. Kirim screenshot Console.");
        return;
    }

    foreach (var c in clips)
    {
        c.name = Bersih(c.name);
        bool loop = c.name.Contains("Pegang");
        c.loopTime = loop; c.loopPose = loop;
        c.lockRootRotation = true; c.lockRootHeightY = true; c.lockRootPositionXZ = true;
        c.keepOriginalOrientation = true; c.keepOriginalPositionY = true; c.keepOriginalPositionXZ = true;
    }
    imp.clipAnimations = clips;
    imp.SaveAndReimport();
    Debug.Log("SELA: " + clips.Length + " klip diatur: " + string.Join(", ", clips.Select(c => c.name)));

    PasangKeController(path);
    }
    
    static void PasangKeController(string fbxPath)
    {
        var klip = AssetDatabase.LoadAllAssetsAtPath(fbxPath).OfType<AnimationClip>()
            .Where(c => !c.name.StartsWith("__preview__")).ToDictionary(c => Bersih(c.name));
        AnimationClip K(string n) { klip.TryGetValue(n, out var c); if (!c) Debug.LogWarning("Klip tidak ditemukan: " + n); return c; }

        var cg = AssetDatabase.FindAssets("Player_SELA t:AnimatorController");
        if (cg.Length == 0) { Debug.LogError("Player_SELA.controller tidak ditemukan."); return; }
        var ctrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(cg[0]));

        if (!ctrl.parameters.Any(p => p.name == "Place"))
            ctrl.AddParameter("Place", AnimatorControllerParameterType.Trigger);

        var sm = ctrl.layers[0].stateMachine;
        AnimatorState Cari(string n) => sm.states.Select(s => s.state).FirstOrDefault(s => s.name == n);

        var loco = Cari("Locomotion");
        var pu = Cari("PickUp");
        var ho = Cari("Holding");
        var put = Cari("PutDown") ?? sm.AddState("PutDown");
        if (!loco || !pu || !ho) { Debug.LogError("State Locomotion/PickUp/Holding tidak ada di Player_SELA."); return; }

        pu.motion = K("AmbilBarangMeja");
        ho.motion = K("PegangBarangTelapak");
        put.motion = K("TaruhBarangMeja");

        if (!ho.transitions.Any(t => t.destinationState == put))
        {
            var t = ho.AddTransition(put);
            t.hasExitTime = false; t.duration = 0.15f;
            t.AddCondition(AnimatorConditionMode.If, 0f, "Place");
            // pindahkan ke prioritas teratas
            var list = ho.transitions.ToList();
            list.Remove(t); list.Insert(0, t);
            ho.transitions = list.ToArray();
        }
        if (!put.transitions.Any(t => t.destinationState == loco))
        {
            var t = put.AddTransition(loco);
            t.hasExitTime = true; t.exitTime = 0.9f; t.duration = 0.2f;
        }

        EditorUtility.SetDirty(ctrl);
        AssetDatabase.SaveAssets();
        Selection.activeObject = ctrl;
        Debug.Log("SELA: Player_SELA diperbarui (PickUp, Holding, PutDown).");
    }
}