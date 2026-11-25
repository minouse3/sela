using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaptopInteraction : MonoBehaviour
{
    [Header("Referensi")]
    public Transform laptopView;
    public GameObject laptopUI;

    [Header("Pengaturan")]
    public bool requireSeated = true;
    public float moveTime = 0.5f;
    public float reach = 3f;            // jarak pandang untuk menekan E saat duduk

    FirstPersonController player;
    PlayerInteractor interactor;
    SeatInteraction seat;
    Camera cam;
    Vector3 savedPos;
    Quaternion savedRot;
    Coroutine routine;
    bool isOpen;

    // Dipanggil dari Interactable -> On Interact (tetap boleh dipakai)
    public void Open()
    {
        if (isOpen || !laptopView) return;

        seat = SeatInteraction.Current;
        if (requireSeated && (seat == null || seat.pose == SeatInteraction.Pose.Kasur)) return;

        player = Object.FindFirstObjectByType<FirstPersonController>();
        if (!player) return;
        cam = player.GetComponentInChildren<Camera>();
        if (!cam) return;
        interactor = player.GetComponent<PlayerInteractor>();

        savedPos = cam.transform.position;
        savedRot = cam.transform.rotation;

        if (seat) seat.lookLocked = true;
        if (interactor) interactor.enabled = false;

        player.SetTyping(true);

        isOpen = true;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(MoveCamera());
    }

    IEnumerator MoveCamera()
    {
        Vector3 p0 = cam.transform.position;
        Quaternion r0 = cam.transform.rotation;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, moveTime);
            float k = Mathf.SmoothStep(0f, 1f, t);
            cam.transform.position = Vector3.Lerp(p0, laptopView.position, k);
            cam.transform.rotation = Quaternion.Slerp(r0, laptopView.rotation, k);
            yield return null;
        }
        if (laptopUI) laptopUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (isOpen)
        {
            if (kb.escapeKey.wasPressedThisFrame) Close();
            return;
        }

        // Saat duduk: E pada laptop yang sedang dilihat
        if (kb.eKey.wasPressedThisFrame && LookingAtLaptop())
            Open();
    }

    bool LookingAtLaptop()
    {
        var s = SeatInteraction.Current;
        if (s == null || s.pose == SeatInteraction.Pose.Kasur || s.lookLocked) return false;

        var c = Camera.main;
        if (!c) return false;

        var hits = Physics.RaycastAll(c.transform.position, c.transform.forward, reach,
                                      ~0, QueryTriggerInteraction.Collide);
        foreach (var h in hits)
            if (h.collider.transform.IsChildOf(transform)) return true;
        return false;
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;

        if (routine != null) { StopCoroutine(routine); routine = null; }
        if (laptopUI) laptopUI.SetActive(false);

        if (cam)
        {
            cam.transform.position = savedPos;
            cam.transform.rotation = savedRot;
        }

        if (player) player.SetTyping(false);
        if (seat) seat.lookLocked = false;
        if (interactor) interactor.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}