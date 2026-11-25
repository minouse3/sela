using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// Pasang di Player. Raycast 2 m dari kamera, tekan E untuk interaksi.
public class PlayerInteractor : MonoBehaviour
{
    public Camera cam;
    public float range = 2f;
    public Key key = Key.E;
    public Text promptText;               // teks di bagian bawah layar
    public LayerMask mask = ~0;

    public static bool Blocked;           // true saat sedang fokus (laptop, puzzle, dll.)
    readonly RaycastHit[] hits = new RaycastHit[32];
    InteractionOutline outline;
    Interactable target;
    string message;

    void Awake()
    {
        outline = GetComponent<InteractionOutline>();
    }

    void OnDisable()
    {
        SetTarget(null);
        SetMessage(null);
    }

    void Update()
    {
        if (Blocked || !cam) { SetTarget(null); SetMessage(null); return; }

        // Saat duduk, tampilkan prompt E · Berdiri (sesuai standar tombol E di GDD §3.9).
        if (SeatInteraction.Current != null)
        {
            SetTarget(null);
            SetMessage($"{key} · Berdiri");
            return;
        }

        Interactable found = null;
        int count = Physics.RaycastNonAlloc(cam.transform.position, cam.transform.forward, hits,
                                           range, mask, QueryTriggerInteraction.Ignore);
        float closest = float.PositiveInfinity;
        for (int n = 0; n < count; n++)
        {
            var hit = hits[n];
            if (hit.collider.transform.IsChildOf(transform) || hit.distance >= closest) continue;
            closest = hit.distance;
            found = hit.collider.GetComponentInParent<Interactable>();
        }

        if (found != null && !found.canInteract) found = null;
        SetTarget(found);
        string promptLabel = found != null
            ? (string.IsNullOrWhiteSpace(found.prompt) ? $"Press {key} to Interact" : $"{key} · {found.prompt}")
            : null;
        SetMessage(promptLabel);
        if (found != null && Keyboard.current != null && Keyboard.current[key].wasPressedThisFrame)
        {
            found.Interact();
            if (!found || !found.canInteract || Blocked) { SetTarget(null); SetMessage(null); }
        }
    }

    void SetTarget(Interactable value)
    {
        if (ReferenceEquals(target, value)) return;
        target = value;
        if (outline) outline.SetTarget(value);
    }

    void SetMessage(string value)
    {
        if (message == value) return;
        message = value;
        if (promptText) promptText.text = value ?? "";
    }
}
