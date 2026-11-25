using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteractor : MonoBehaviour
{
    public Camera cam;
    public float range = 2f;
    public Key key = Key.E;
    public Text promptText;
    public TMP_Text promptTmpText;
    public GameObject promptContainer;
    public LayerMask mask = ~0;

    public static bool Blocked;

    readonly RaycastHit[] hits = new RaycastHit[32];
    InteractionOutline outline;
    PlayerCarry carry;
    LaptopInteraction laptop;
    Interactable target;
    string message;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Blocked = false;
    }

    void Awake()
    {
        Blocked = false;
        outline = GetComponent<InteractionOutline>();
        carry = GetComponent<PlayerCarry>();
        if (promptContainer != null) promptContainer.SetActive(false);
    }

    void Start()
    {
        laptop = Object.FindFirstObjectByType<LaptopInteraction>();
    }

    void OnDisable()
    {
        SetTarget(null);
        SetMessage(null);
    }

    void Update()
    {
        if (Blocked || !cam) { SetTarget(null); SetMessage(null); return; }

        if (SeatInteraction.Current != null)
        {
            SetTarget(null);
            if (SeatInteraction.Current.pose == SeatInteraction.Pose.Kasur)
            {
                SetMessage($"[{key}] Tidur   ·   [Q] Bangun");
            }
            else
            {
                bool canOpenLaptop = laptop != null && laptop.IsLookingAtLaptop;
                SetMessage(canOpenLaptop ? $"[{key}] Buka laptop   ·   [Q] Berdiri" : "[Q] Berdiri");
            }
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

        string promptLabel = null;
        bool blockedBySeat = false;
        if (found != null)
        {
            var lap = found.GetComponent<LaptopInteraction>();
            if (lap != null && lap.requireSeated && SeatInteraction.Current == null)
            {
                blockedBySeat = true;
                promptLabel = "Duduk dulu untuk buka laptop";
            }
            else
            {
                promptLabel = string.IsNullOrWhiteSpace(found.prompt) ? $"[{key}] Interaksi" : $"[{key}] {found.prompt}";
            }
        }

        if (carry != null && carry.Carrying)
        {
            string dropHint = $"[G] Letakkan {carry.CarriedItemName}";
            promptLabel = string.IsNullOrEmpty(promptLabel) ? dropHint : $"{promptLabel}   ·   {dropHint}";
        }

        SetMessage(promptLabel);
        if (found != null && !blockedBySeat && Keyboard.current != null && Keyboard.current[key].wasPressedThisFrame)
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

        bool hasMessage = !string.IsNullOrEmpty(value);
        if (promptContainer != null) promptContainer.SetActive(hasMessage);
        if (promptTmpText != null) promptTmpText.text = value ?? "";
        if (promptText != null) promptText.text = value ?? "";
    }
}
