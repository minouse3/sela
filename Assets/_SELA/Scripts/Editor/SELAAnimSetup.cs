// Letakkan di Assets/Editor/. Menu: SELA > Setup Animasi (otomatis)...
// 1) Mengatur FBX SELA_Anim_Semua jadi Humanoid, nama klip bersih, loop, root bake.
// 2) (opsional) Membuat Animator Controller lengkap dengan parameter & transisi.
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class SELAAnimSetup : ScriptableWizard
{
    [Tooltip("Avatar karakter Anda (dari FBX karakter Humanoid). Kosongkan jika ingin memakai kerangka bawaan FBX animasi.")]
    public Avatar avatarKarakter;
    public bool buatAnimatorController = true;

    static readonly string[] Loop = { "BerdiriDiam", "Jalan", "Lari", "DudukDiam", "Tidur", "Jongkok", "Mengetik", "PegangBarang" };

    [MenuItem("SELA/Setup Animasi (otomatis)...")]
    static void Open() { DisplayWizard<SELAAnimSetup>("SELA Animasi", "Setup"); }

    static string Bersih(string n) { int i = n.LastIndexOf('|'); return i >= 0 ? n.Substring(i + 1) : n; }

    void OnWizardCreate()
    {
        var guids = AssetDatabase.FindAssets("SELA_Anim_Semua t:Model");
        if (guids.Length == 0) { Debug.LogError("SELA_Anim_Semua.fbx tidak ditemukan di project."); return; }
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        var imp = (ModelImporter)AssetImporter.GetAtPath(path);

        imp.animationType = ModelImporterAnimationType.Human;
        if (avatarKarakter != null) { imp.avatarSetup = ModelImporterAvatarSetup.CopyFromOther; imp.sourceAvatar = avatarKarakter; }
        else imp.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
        imp.importAnimation = true;
        imp.SaveAndReimport();

        imp = (ModelImporter)AssetImporter.GetAtPath(path);
        var clips = imp.defaultClipAnimations;
        foreach (var c in clips)
        {
            string nm = Bersih(c.name);
            c.name = nm;
            bool lp = Loop.Contains(nm);
            c.loopTime = lp; c.loopPose = lp;
            c.lockRootRotation = true; c.lockRootHeightY = true; c.lockRootPositionXZ = true;
            c.keepOriginalOrientation = true; c.keepOriginalPositionY = true; c.keepOriginalPositionXZ = true;
        }
        imp.clipAnimations = clips;
        imp.SaveAndReimport();
        Debug.Log("SELA: " + clips.Length + " klip animasi diatur (Humanoid).");

        if (buatAnimatorController) BuatController(path);
    }

    static void BuatController(string fbxPath)
    {
        var semua = new Dictionary<string, AnimationClip>();
        foreach (var c in AssetDatabase.LoadAllAssetsAtPath(fbxPath).OfType<AnimationClip>())
            if (!c.name.StartsWith("__preview__")) semua[Bersih(c.name)] = c;
        AnimationClip K(string n) { AnimationClip c; if (!semua.TryGetValue(n, out c)) Debug.LogWarning("Klip tidak ditemukan: " + n); return c; }

        string cpath = AssetDatabase.GenerateUniqueAssetPath(Path.GetDirectoryName(fbxPath).Replace("\\", "/") + "/Player_SELA.controller");
        var ctrl = AnimatorController.CreateAnimatorControllerAtPath(cpath);
        ctrl.AddParameter("Speed", AnimatorControllerParameterType.Float);
        foreach (var b in new[] { "IsSitting", "IsLying", "IsCrouching", "IsHolding", "IsTyping" }) ctrl.AddParameter(b, AnimatorControllerParameterType.Bool);
        ctrl.AddParameter("Interact", AnimatorControllerParameterType.Trigger);
        var sm = ctrl.layers[0].stateMachine;

        AnimatorState loco = ctrl.CreateBlendTreeInController("Locomotion", out BlendTree bt);
        bt.blendParameter = "Speed"; bt.useAutomaticThresholds = false;
        var idle = K("BerdiriDiam"); var walk = K("Jalan"); var run = K("Lari");
        if (idle) bt.AddChild(idle, 0f);
        if (walk) bt.AddChild(walk, 1f);
        if (run) bt.AddChild(run, 2f);
        sm.defaultState = loco;

        AnimatorState S(string n, string klip) { var s = sm.AddState(n); s.motion = K(klip); return s; }
        var sd = S("StandToSit", "BerdiriKeDuduk"); var si = S("SitIdle", "DudukDiam"); var su = S("SitToStand", "DudukKeBerdiri");
        var ty = S("Typing", "Mengetik");
        var ld = S("LieDown", "BerdiriKeTidur"); var sl = S("SleepIdle", "Tidur"); var gu = S("GetUp", "TidurKeBerdiri");
        var pu = S("PickUp", "AmbilBarang"); var ho = S("Holding", "PegangBarang"); var cr = S("Crouch", "Jongkok");

        T(loco, sd, false, "IsSitting", true); T(sd, si, true); T(si, su, false, "IsSitting", false); T(su, loco, true);
        T(si, ty, false, "IsTyping", true); T(ty, si, false, "IsTyping", false); T(ty, su, false, "IsSitting", false);
        T(loco, ld, false, "IsLying", true); T(ld, sl, true); T(sl, gu, false, "IsLying", false); T(gu, loco, true);
        T(loco, cr, false, "IsCrouching", true); T(cr, loco, false, "IsCrouching", false);
        T(loco, ho, false, "IsHolding", true); T(ho, loco, false, "IsHolding", false);
        TTrigger(loco, pu); T(pu, ho, true, "IsHolding", true); T(pu, loco, true, "IsHolding", false);

        EditorUtility.SetDirty(ctrl); AssetDatabase.SaveAssets();
        Selection.activeObject = ctrl;
        Debug.Log("SELA: Animator Controller dibuat di " + cpath);
    }

    static void T(AnimatorState a, AnimatorState b, bool exit, string param = null, bool nilai = true)
    {
        var t = a.AddTransition(b);
        t.hasExitTime = exit; t.exitTime = 0.9f; t.duration = 0.2f;
        if (param != null) t.AddCondition(nilai ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot, 0f, param);
    }

    static void TTrigger(AnimatorState a, AnimatorState b)
    {
        var t = a.AddTransition(b);
        t.hasExitTime = false; t.duration = 0.15f;
        t.AddCondition(AnimatorConditionMode.If, 0f, "Interact");
    }
}
