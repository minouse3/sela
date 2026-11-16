using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InteractionOutline : MonoBehaviour
{
    [SerializeField] Color color = new Color(1f, 0.85f, 0.2f, 1f);
    [SerializeField, Min(0f)] float width = 0.015f;

    sealed class OutlineRenderer
    {
        public Renderer renderer;
        public MeshFilter filter;
        public SkinnedMeshRenderer skinned;
        public Mesh bakedMesh;
    }

    readonly List<OutlineRenderer> renderers = new List<OutlineRenderer>();
    Interactable target;
    Material material;

    public void SetTarget(Interactable next)
    {
        if (target == next && (next != null || renderers.Count == 0)) return;
        ClearTarget();
        if (!isActiveAndEnabled || !next) return;

        target = next;
        foreach (var renderer in target.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer.GetComponentInParent<Interactable>() != target) continue;
            var skinned = renderer as SkinnedMeshRenderer;
            var filter = renderer is MeshRenderer ? renderer.GetComponent<MeshFilter>() : null;
            if (!skinned && !filter) continue;
            renderers.Add(new OutlineRenderer
            {
                renderer = renderer,
                filter = filter,
                skinned = skinned,
                bakedMesh = skinned ? new Mesh { name = "Interaction Outline Snapshot" } : null
            });
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
            if (!source || !source.enabled || source.forceRenderingOff || !source.gameObject.activeInHierarchy || source.shadowCastingMode == ShadowCastingMode.ShadowsOnly) continue;
            Mesh mesh;
            if (entry.skinned)
            {
                if (!entry.skinned.sharedMesh) continue;
                entry.skinned.BakeMesh(entry.bakedMesh);
                mesh = entry.bakedMesh;
            }
            else
            {
                mesh = entry.filter ? entry.filter.sharedMesh : null;
            }
            if (!mesh) continue;

            var bounds = source.bounds;
            bounds.Expand(width * 2f);
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
        if (material) Destroy(material);
        material = null;
    }

    void ClearTarget()
    {
        target = null;
        foreach (var entry in renderers)
            if (entry.bakedMesh) Destroy(entry.bakedMesh);
        renderers.Clear();
    }
}
