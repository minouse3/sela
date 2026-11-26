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

    void OnDisable()
    {
        SetTarget(null);
        SetMessage(null);
    }

    void Update()
    {
        if (Blocked || !cam) { SetTarget(null); SetMessage(null); return; }

        var kb = Keyboard.current;
        var seat = SeatInteraction.Current;
        if (seat != null && seat.pose == SeatInteraction.Pose.Kasur)
        {
            SetTarget(null);
            SetMessage($"[{key}] Tidur   ·   [Q] Bangun");
            return;
        }

        var found = FindTarget();
        var laptop = found != null ? found.GetComponent<LaptopInteraction>() : null;
        if (seat != null && laptop == null) found = null;
        SetTarget(seat == null ? found : null);

        bool phoneInHand = carry != null && carry.HasPhoneInHand;
        if (phoneInHand && kb != null && kb.pKey.wasPressedThisFrame &&
            GameLoopController.Instance is GameLoopController loop &&
            loop.TryGetComponent<CoreLoopDebugUI>(out var panel) && panel.OpenPhone(carry))
        {
            SetTarget(null);
            SetMessage(null);
            return;
        }

        bool requiresSeat = laptop != null && laptop.requireSeated && seat == null;
        string promptLabel = null;
        if (requiresSeat)
            promptLabel = "Duduk dulu untuk buka laptop";
        else if (found != null)
            promptLabel = $"[{key}] {(string.IsNullOrWhiteSpace(found.prompt) ? "Interaksi" : found.prompt)}";
        if (phoneInHand) AppendHint(ref promptLabel, "[P] Cek ponsel");
        if (seat != null)
            AppendHint(ref promptLabel, "[Q] Berdiri");
        else if (carry != null && carry.Carrying)
            AppendHint(ref promptLabel, $"[G] Letakkan {carry.CarriedItemName}");
        SetMessage(promptLabel);

        if (found == null || kb == null) return;
        if (!requiresSeat && kb[key].wasPressedThisFrame)
        {
            found.Interact();
            if (!found || !found.canInteract || Blocked) { SetTarget(null); SetMessage(null); }
        }
    }

    Interactable FindTarget()
    {
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
        return found != null && found.canInteract ? found : null;
    }

    static void AppendHint(ref string current, string hint)
    {
        current = string.IsNullOrEmpty(current) ? hint : $"{current}   ·   {hint}";
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
