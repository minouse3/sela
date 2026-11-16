using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

// Pasang di Player. Raycast 2 m dari kamera sesuai GDD, tekan E untuk interaksi.
public class PlayerInteractor : MonoBehaviour
{
    public Camera cam;
    public float range = 2f;
    public Key key = Key.E;
    public TMP_Text promptText;           // teks kecil di tengah-bawah layar
    public LayerMask mask = ~0;

    public static bool Blocked;           // true saat sedang fokus (laptop, puzzle, dll.)

    void Update()
    {
        if (Blocked) { SetPrompt(null); return; }

        Interactable found = null;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit,
                            range, mask, QueryTriggerInteraction.Ignore))
            found = hit.collider.GetComponentInParent<Interactable>();

        if (found != null && !found.canInteract) found = null;
        SetPrompt(found);
        if (found != null && Keyboard.current != null && Keyboard.current[key].wasPressedThisFrame) found.Interact();
    }

    void SetPrompt(Interactable i)
    {
        if (promptText) promptText.text = i ? key + " · " + i.prompt : "";
    }
}
