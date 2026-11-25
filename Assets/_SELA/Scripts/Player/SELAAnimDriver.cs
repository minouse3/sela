using System.Collections;
using UnityEngine;

public class SELAAnimDriver : MonoBehaviour
{
    public Animator animator;
    public float walkSpeed = 2.5f;      // samakan dengan FirstPersonController
    public float smooth = 10f;
    public float pickUpDelay = 0.6f;    // jeda sebelum IsHolding menyala

    Vector3 last;
    float v;

    void Start() { last = transform.position; }

    void Update()
    {
        if (animator == null) return;
        Vector3 d = transform.position - last; d.y = 0f; last = transform.position;
        float s = Time.deltaTime > 0f ? d.magnitude / Time.deltaTime : 0f;
        if (s > 8f) s = 0f;             // lompatan teleport/snap diabaikan
        v = Mathf.Lerp(v, s / walkSpeed, Time.deltaTime * smooth);
        animator.SetFloat("Speed", Mathf.Clamp(v, 0f, 2f));   // 1 = jalan, 2 = lari
    }

    public void SetSitting(bool b)  { if (animator) animator.SetBool("IsSitting", b); }
    public void SetTyping(bool b)   { if (animator) animator.SetBool("IsTyping", b); }
    public void SetLying(bool b)    { if (animator) animator.SetBool("IsLying", b); }
    public void SetCrouch(bool b)   { if (animator) animator.SetBool("IsCrouching", b); }
    public void SetHolding(bool b)  { if (animator) animator.SetBool("IsHolding", b); }

    // Ambil benda: mainkan AmbilBarang dulu, baru PegangBarang
    public void PickUp()
    {
        if (!animator) return;
        animator.SetTrigger("Interact");
        StartCoroutine(HoldAfterDelay());
    }

    IEnumerator HoldAfterDelay()
    {
        yield return new WaitForSeconds(pickUpDelay);
        animator.SetBool("IsHolding", true);
    }

    public void Drop() { SetHolding(false); }

    public void PutDown()
    {
        if (!animator) return;
        animator.SetTrigger("Place");
        StartCoroutine(ReleaseHold());
    }

    IEnumerator ReleaseHold()
    {
        yield return new WaitForSeconds(0.15f);   // beri waktu transisi ke PickUp dimulai
        animator.SetBool("IsHolding", false);
    }
}