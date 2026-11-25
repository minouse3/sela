using System.Collections;
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

    public bool Carrying => item != null;
    public string CarriedItemName => item != null ? item.itemName : "";

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
        var hits = Physics.RaycastAll(ray, placeRange, ~0, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        bool found = false;
        RaycastHit target = default;
        foreach (var h in hits)
        {
            if (h.collider.transform.IsChildOf(transform)) continue;
            if (h.collider.transform.IsChildOf(item.transform)) continue;
            string hitName = h.collider.gameObject.name;
            if (hitName.Contains("Tirai") || hitName.Contains("Jendela")) continue;
            target = h;
            found = h.normal.y > 0.65f;
            break;
        }
        if (!found) yield break;

        busy = true;
        if (driver) driver.PutDown();
        yield return new WaitForSeconds(placeDelay);

        var p = item;
        p.transform.SetParent(null, true);
        p.transform.rotation = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f);
        p.transform.position = target.point;

        // Snap item's actual visual mesh bottom flush onto the placement surface
        float bottomOffset = ComputeMeshBottomOffset(p.gameObject);
        p.transform.position = new Vector3(
            target.point.x,
            target.point.y - bottomOffset + p.placeYOffset,
            target.point.z);

        SetPhysics(p, true);
        item = null;
        busy = false;
    }

    static float ComputeMeshBottomOffset(GameObject go)
    {
        var renderers = go.GetComponentsInChildren<Renderer>();
        bool has = false;
        float minY = 0f;
        foreach (var r in renderers)
        {
            if (!r.enabled) continue;
            if (!has) { minY = r.bounds.min.y; has = true; }
            else if (r.bounds.min.y < minY) minY = r.bounds.min.y;
        }
        return has ? (minY - go.transform.position.y) : 0f;
    }

    static void SetPhysics(Pickupable p, bool on)
    {
        foreach (var c in p.GetComponentsInChildren<Collider>()) c.enabled = on;
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
