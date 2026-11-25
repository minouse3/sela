using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarry : MonoBehaviour
{
    public SELAAnimDriver driver;
    public Camera cam;
    public float placeRange = 2.5f;
    public float attachDelay = 0.5f;   // kapan barang menempel ke tangan
    public float placeDelay = 0.5f;    // kapan barang lepas dari tangan

    Transform hold;
    Pickupable item;
    bool busy;

    public bool Carrying => item != null;

    void Awake()
    {
        if (!driver) driver = GetComponent<SELAAnimDriver>();
        if (!cam) cam = GetComponentInChildren<Camera>();
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
        if (busy || item != null || !p) return;
        StartCoroutine(PickRoutine(p));
    }

    IEnumerator PickRoutine(Pickupable p)
    {
        busy = true;
        driver.PickUp();
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
        if (Keyboard.current.gKey.wasPressedThisFrame) StartCoroutine(PlaceRoutine());
    }

    IEnumerator PlaceRoutine()
    {
        // cari permukaan datar di depan kamera
        var ray = new Ray(cam.transform.position, cam.transform.forward);
        var hits = Physics.RaycastAll(ray, placeRange, ~0, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        bool found = false;
        RaycastHit target = default;
        foreach (var h in hits)
        {
            if (h.collider.transform.IsChildOf(transform)) continue;       // abaikan badan pemain
            if (h.collider.transform.IsChildOf(item.transform)) continue;
            target = h; found = h.normal.y > 0.5f; break;                  // harus permukaan datar
        }
        if (!found) yield break;

        busy = true;
        driver.PutDown();
        yield return new WaitForSeconds(placeDelay);

        var p = item;
        p.transform.SetParent(null, true);
        p.transform.position = target.point + Vector3.up * p.placeYOffset;
        p.transform.rotation = Quaternion.Euler(0f, p.transform.eulerAngles.y, 0f);
        SetPhysics(p, true);
        item = null;
        busy = false;
    }

    static void SetPhysics(Pickupable p, bool on)
    {
        foreach (var c in p.GetComponentsInChildren<Collider>()) c.enabled = on;
        var rb = p.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = !on;
    }
}