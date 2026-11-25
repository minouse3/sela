using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InteractionOutline : MonoBehaviour
{
    [SerializeField] Color color = new Color(0.96f, 0.84f, 0.48f, 0.94f);
    [SerializeField, Min(0f)] float width = 1.0f;

    sealed class OutlineRenderer
    {
        public Renderer renderer;
        public MeshFilter filter;
        public SkinnedMeshRenderer skinned;
        public Mesh outlineMesh;
        public bool ownsMesh;
    }

    readonly List<OutlineRenderer> renderers = new List<OutlineRenderer>();
    readonly Dictionary<Mesh, Mesh> smoothMeshCache = new Dictionary<Mesh, Mesh>();
    Interactable target;
    Material material;

    public void SetTarget(Interactable next)
    {
        if (target == next && (next != null || renderers.Count == 0)) return;
        ClearTarget();
        if (!isActiveAndEnabled || !next) return;

        target = next;
        foreach (var r in target.GetComponentsInChildren<Renderer>(true))
        {
            if (r.GetComponentInParent<Interactable>() != target) continue;
            var skinned = r as SkinnedMeshRenderer;
            var filter = r is MeshRenderer ? r.GetComponent<MeshFilter>() : null;
            if (!skinned && !filter) continue;

            if (skinned)
            {
                renderers.Add(new OutlineRenderer
                {
                    renderer = r,
                    skinned = skinned,
                    outlineMesh = new Mesh { name = "Interaction Outline Skinned" },
                    ownsMesh = true
                });
            }
            else if (filter.sharedMesh != null)
            {
                renderers.Add(new OutlineRenderer
                {
                    renderer = r,
                    filter = filter,
                    outlineMesh = GetOrBuildSmoothMesh(filter.sharedMesh),
                    ownsMesh = false
                });
            }
        }
    }

    void LateUpdate()
    {
        if (!target)
        {
            if (renderers.Count != 0) ClearTarget();
            return;
        }
        if (!target.isActiveAndEnabled) return;

        if (!material)
        {
            var shader = Resources.Load<Shader>("InteractionOutline");
            if (!shader) return;
            material = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
        }

        material.SetColor("_OutlineColor", color);
        material.SetFloat("_OutlineWidth", width);

        foreach (var entry in renderers)
        {
            var source = entry.renderer;
            if (!source || !source.enabled || source.forceRenderingOff ||
                !source.gameObject.activeInHierarchy ||
                source.shadowCastingMode == ShadowCastingMode.ShadowsOnly)
                continue;

            Mesh mesh = entry.outlineMesh;
            if (entry.skinned)
            {
                if (!entry.skinned.sharedMesh) continue;
                entry.skinned.BakeMesh(mesh);
                ApplySmoothNormalsInPlace(mesh);
            }
            if (!mesh) continue;

            var bounds = source.bounds;
            bounds.Expand(0.05f);
            var renderParams = new RenderParams(material)
            {
                layer = source.gameObject.layer,
                renderingLayerMask = source.renderingLayerMask,
                worldBounds = bounds,
                shadowCastingMode = ShadowCastingMode.Off,
                receiveShadows = false,
                lightProbeUsage = LightProbeUsage.Off
            };

            for (int i = 0; i < mesh.subMeshCount; i++)
                Graphics.RenderMesh(renderParams, mesh, i, source.localToWorldMatrix);
        }
    }

    void OnDisable()
    {
        ClearTarget();
        foreach (var kv in smoothMeshCache)
            if (kv.Value != null) Destroy(kv.Value);
        smoothMeshCache.Clear();
        if (material) Destroy(material);
        material = null;
    }

    void ClearTarget()
    {
        target = null;
        foreach (var entry in renderers)
            if (entry.ownsMesh && entry.outlineMesh != null)
                Destroy(entry.outlineMesh);
        renderers.Clear();
    }

    Mesh GetOrBuildSmoothMesh(Mesh src)
    {
        if (smoothMeshCache.TryGetValue(src, out Mesh cached) && cached != null)
            return cached;

        if (!src.isReadable)
            return src;

        var clone = Instantiate(src);
        clone.name = src.name + "_SmoothOutline";
        clone.hideFlags = HideFlags.HideAndDontSave;
        ApplySmoothNormalsInPlace(clone);
        smoothMeshCache[src] = clone;
        return clone;
    }

    static void ApplySmoothNormalsInPlace(Mesh mesh)
    {
        var verts = mesh.vertices;
        var normals = mesh.normals;
        if (verts == null || normals == null || verts.Length != normals.Length || verts.Length == 0)
            return;

        var accum = new Dictionary<Vector3Int, Vector3>(verts.Length);
        for (int i = 0; i < verts.Length; i++)
        {
            var key = Quantize(verts[i]);
            if (accum.TryGetValue(key, out Vector3 sum))
                accum[key] = sum + normals[i];
            else
                accum[key] = normals[i];
        }

        var smooth = new Vector3[verts.Length];
        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 avg = accum[Quantize(verts[i])];
            smooth[i] = avg.sqrMagnitude > 1e-6f ? avg.normalized : normals[i];
        }
        mesh.normals = smooth;
    }

    static Vector3Int Quantize(Vector3 v)
    {
        return new Vector3Int(
            Mathf.RoundToInt(v.x * 1000f),
            Mathf.RoundToInt(v.y * 1000f),
            Mathf.RoundToInt(v.z * 1000f));
    }
}
