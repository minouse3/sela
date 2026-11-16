using UnityEngine;
using UnityEngine.Events;

// Pasang di benda yang bisa ditekan E. Benda HARUS punya Collider.
public class Interactable : MonoBehaviour
{
    public string prompt = "Berinteraksi";
    public bool singleUse = false;
    public bool canInteract = true;
    public UnityEvent onInteract;

    public void Interact()
    {
        if (!canInteract) return;
        onInteract.Invoke();
        if (singleUse) canInteract = false;
    }
}
