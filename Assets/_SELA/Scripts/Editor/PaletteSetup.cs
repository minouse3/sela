// Menu: SELA > Setup Palette Material
// Membuat M_SELA_Palette dan memasangnya ke model SELA.
using UnityEditor;
using UnityEngine;

public static class PaletteSetup
{
    [MenuItem("SELA/Setup Palette Material")]
    static void Setup()
    {
        string[] tex = AssetDatabase.FindAssets("T_SELA_Palette t:Texture2D");
        if (tex.Length == 0) { Debug.LogError("T_SELA_Palette.png tidak ditemukan di project."); return; }
        string texPath = AssetDatabase.GUIDToAssetPath(tex[0]);
        var t = AssetImporter.GetAtPath(texPath) as TextureImporter;
        t.filterMode = FilterMode.Point; t.mipmapEnabled = false; t.sRGBTexture = true;
        t.SaveAndReimport();

        Shader sh = Shader.Find("Universal Render Pipeline/Lit");
        if (sh == null) sh = Shader.Find("Standard");
        var mat = new Material(sh);
        var tx = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
        mat.mainTexture = tx;
        if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tx);
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0f);
        if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0f);
        string matPath = System.IO.Path.GetDirectoryName(texPath).Replace("\\", "/") + "/M_SELA_Palette.mat";
        AssetDatabase.CreateAsset(mat, matPath);

        int n = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:Model", new[] { "Assets/_SELA/Art/Environments", "Assets/_SELA/Art/Props" }))
        {
            string p = AssetDatabase.GUIDToAssetPath(guid);
            var imp = AssetImporter.GetAtPath(p) as ModelImporter;
            if (imp == null) continue;
            imp.globalScale = 1f;
            imp.useFileScale = true;
            imp.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            imp.materialLocation = ModelImporterMaterialLocation.External;
            imp.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material), "M_SELA_Palette"), mat);
            imp.SaveAndReimport(); n++;
        }
        Debug.Log("SELA: material palet terpasang ke " + n + " model.");
    }
}
