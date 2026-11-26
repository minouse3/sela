using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarry : MonoBehaviour
{
    public SELAAnimDriver driver;
    public Camera cam;
    public float placeRange = 2.5f;
    public float attachDelay = 0.5f;
    public float placeDelay = 0.5f;

    Transform hold;
    Pickupable item;
    FlyCamera flyCamera;
    bool busy;
    readonly RaycastHit[] placementHits = new RaycastHit[32];
    readonly Collider[] placementOverlap = new Collider[16];
    readonly List<Renderer> itemRenderers = new List<Renderer>();
    readonly List<Collider> itemColliders = new List<Collider>();

    public bool Carrying => item != null;
    public string CarriedItemName => item != null ? item.itemName : "";
    internal bool HasPhoneInHand => !busy && item != null && item.IsPhone;

    void Awake()
    {
        if (!driver) driver = GetComponent<SELAAnimDriver>() ?? GetComponentInChildren<SELAAnimDriver>();
        if (!cam) cam = GetComponentInChildren<Camera>();
        if (cam) flyCamera = cam.GetComponent<FlyCamera>();
        var anim = GetComponentInChildren<Animator>();
        if (anim)
        {
            foreach (var t in anim.GetComponentsInChildren<Transform>(true))
                if (t.name == "HoldPoint") { hold = t; break; }
            if (!hold) hold = anim.GetBoneTransform(HumanBodyBones.RightHand);
        }
    }

    public void PickUp(Pickupable p)
    {
        if (busy || item != null || !p || PlayerInteractor.Blocked || SeatInteraction.Current != null) return;
        if (flyCamera != null && flyCamera.IsFlying) return;
        StartCoroutine(PickRoutine(p));
    }

    IEnumerator PickRoutine(Pickupable p)
    {
        busy = true;
        if (driver) driver.PickUp();
        yield return new WaitForSeconds(attachDelay);

        SetPhysics(p, false);
        p.transform.SetParent(hold, false);
        p.transform.localPosition = Vector3.zero;
        p.transform.localRotation = Quaternion.identity;
        item = p;
        busy = false;
    }

    void Update()
    {
        if (item == null || busy || Keyboard.current == null) return;
        if (PlayerInteractor.Blocked || SeatInteraction.Current != null) return;
        if (flyCamera != null && flyCamera.IsFlying) return;
        if (Cursor.lockState != CursorLockMode.Locked) return;
        if (Keyboard.current.gKey.wasPressedThisFrame) StartCoroutine(PlaceRoutine());
    }

    IEnumerator PlaceRoutine()
    {
        var ray = new Ray(cam.transform.position, cam.transform.forward);
        int count = Physics.RaycastNonAlloc(ray, placementHits, placeRange, ~0, QueryTriggerInteraction.Ignore);
        if (count == placementHits.Length) yield break;

        float nearest = float.PositiveInfinity;
        RaycastHit target = default;
        for (int i = 0; i < count; i++)
        {
            var h = placementHits[i];
            if (h.distance >= nearest || h.collider.transform.IsChildOf(transform) ||
                h.collider.transform.IsChildOf(item.transform)) continue;
            string hitName = h.collider.gameObject.name;
            if (hitName.Contains("Tirai") || hitName.Contains("Jendela")) continue;
            target = h;
            nearest = h.distance;
        }
        if (!target.collider || target.normal.y <= 0.65f) yield break;

        busy = true;
        if (driver) driver.PutDown();
        yield return new WaitForSeconds(placeDelay);

        if (target.collider)
            TryPlace(target.point, Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f));
        busy = false;
    }

    bool TryPlace(Vector3 surface, Quaternion rotation)
    {
        var p = item;
        var t = p.transform;
        var parent = t.parent;
        var localPosition = t.localPosition;
        var localRotation = t.localRotation;
        var localScale = t.localScale;
        t.SetParent(null, true);
        t.SetPositionAndRotation(surface, rotation);
        t.position += Vector3.up * (p.placeYOffset - ComputeMeshBottomOffset(p.gameObject));

        var box = p.GetComponent<BoxCollider>();
        var center = t.TransformPoint(box.center);
        var half = Vector3.Scale(box.size, t.lossyScale) * 0.5f;
        // Exclude the support plane, not the rest of the new pose.
        float top = center.y + half.y;
        float bottom = Mathf.Max(center.y - half.y, surface.y + 0.002f);
        half.y = (top - bottom) * 0.5f;
        center.y = (top + bottom) * 0.5f;
        var physics = p.gameObject.scene.GetPhysicsScene();

        bool blocked = half.y > 0f && OverlapsPlacement(physics, center, half, rotation, p.transform);
        if (blocked)
        {
            var narrow = new Vector3(half.x * 0.72f, half.y, half.z * 0.72f);
            blocked = OverlapsPlacement(physics, center, narrow, rotation, p.transform);
            if (!blocked)
                half = narrow;
            else
            {
                blocked = FindPlacementOffset(physics, ref center, ref t, half, rotation, p.transform, 0.55f) ||
                          FindPlacementOffset(physics, ref center, ref t, half, rotation, p.transform, 0.35f);
            }
        }

        if (blocked)
        {
            t.SetParent(parent, false);
            t.SetLocalPositionAndRotation(localPosition, localRotation);
            t.localScale = localScale;
            return false;
        }

        SetPhysics(p, true);
        item = null;
        return true;
    }

    bool FindPlacementOffset(PhysicsScene physics, ref Vector3 center, ref Transform t, Vector3 half,
                             Quaternion rotation, Transform placed, float footprint)
    {
        var probe = new Vector3(half.x * footprint, half.y, half.z * footprint);
        for (int ring = 1; ring <= 6; ring++)
        {
            float radius = ring * 0.05f;
            for (int step = 0; step < 8; step++)
            {
                float angle = step * Mathf.PI * 0.25f;
                var offset = rotation * new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
                var candidate = center + offset;
                if (OverlapsPlacement(physics, candidate, probe, rotation, placed)) continue;
                t.position += offset;
                center = candidate;
                return false;
            }
        }
        return true;
    }

    bool OverlapsPlacement(PhysicsScene physics, Vector3 center, Vector3 half, Quaternion rotation, Transform placed)
    {
        int count = physics.OverlapBox(center, half, placementOverlap, rotation, ~0, QueryTriggerInteraction.Ignore);
        if (count == placementOverlap.Length) return true;
        for (int i = 0; i < count; i++)
        {
            var other = placementOverlap[i];
            if (!other) continue;
            if (other.transform == transform || other.transform.IsChildOf(transform)) continue; // the player's own body
            if (other.transform == placed || other.transform.IsChildOf(placed)) continue;       // the carried item
            return true;
        }
        return false;
    }

    float ComputeMeshBottomOffset(GameObject go)
    {
        go.GetComponentsInChildren(itemRenderers);
        bool has = false;
        float minY = 0f;
        foreach (var r in itemRenderers)
        {
            if (!r.enabled) continue;
            if (!has) { minY = r.bounds.min.y; has = true; }
            else if (r.bounds.min.y < minY) minY = r.bounds.min.y;
        }
        itemRenderers.Clear();
        return has ? (minY - go.transform.position.y) : 0f;
    }

    void SetPhysics(Pickupable p, bool on)
    {
        p.GetComponentsInChildren(itemColliders);
        foreach (var c in itemColliders) c.enabled = on;
        itemColliders.Clear();
        var rb = p.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = !on;
            if (on)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
