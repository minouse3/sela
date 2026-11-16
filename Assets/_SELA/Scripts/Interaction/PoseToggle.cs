using System.Collections;
using UnityEngine;

// Animasi buka/tutup tanpa Animator: pintu, laci, tutup laptop, daun jendela, dll.
// Cara atur pose: lihat tutorial (menu klik-kanan pada komponen: "Simpan pose ...").
public class PoseToggle : MonoBehaviour
{
    public Transform target;
    public Vector3 closedPos, closedEuler;
    public Vector3 openPos, openEuler;
    public float duration = 0.6f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool startOpen = false;

    public bool IsOpen { get; private set; }
    float t;                               // 0 = tertutup, 1 = terbuka
    Coroutine co;

    void Reset()
    {
        target = transform;
        closedPos = openPos = transform.localPosition;
        closedEuler = openEuler = transform.localEulerAngles;
    }

    void Awake()
    {
        if (!target) target = transform;
        IsOpen = startOpen;
        t = startOpen ? 1f : 0f;
        Apply(t);
    }

    public void Toggle() { Set(!IsOpen); }
    public void Open()   { Set(true); }
    public void Close()  { Set(false); }

    public void Set(bool open)
    {
        IsOpen = open;
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(Run(open ? 1f : 0f));
    }

    IEnumerator Run(float to)
    {
        float from = t, e = 0f;
        while (e < duration)
        {
            e += Time.deltaTime;
            t = Mathf.Lerp(from, to, curve.Evaluate(Mathf.Clamp01(e / duration)));
            Apply(t);
            yield return null;
        }
        t = to; Apply(t);
    }

    void Apply(float k)
    {
        target.localPosition = Vector3.Lerp(closedPos, openPos, k);
        target.localRotation = Quaternion.Slerp(Quaternion.Euler(closedEuler), Quaternion.Euler(openEuler), k);
    }

    [ContextMenu("Simpan pose sekarang sebagai TERTUTUP")]
    void SaveClosed()
    {
        var tr = target ? target : transform;
        closedPos = tr.localPosition; closedEuler = tr.localEulerAngles;
    }

    [ContextMenu("Simpan pose sekarang sebagai TERBUKA")]
    void SaveOpen()
    {
        var tr = target ? target : transform;
        openPos = tr.localPosition; openEuler = tr.localEulerAngles;
    }
}
