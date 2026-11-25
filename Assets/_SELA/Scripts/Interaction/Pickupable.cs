using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Pickupable : MonoBehaviour
{
    public string itemName = "Hp";
    public float placeYOffset = 0f;   // naikkan jika barang tenggelam di permukaan

    // Dipanggil dari Interactable -> On Interact
    public void Take()
    {
        var carry = Object.FindFirstObjectByType<PlayerCarry>();
        if (carry) carry.PickUp(this);
    }
}