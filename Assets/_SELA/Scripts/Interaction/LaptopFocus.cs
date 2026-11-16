using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// Efek "masuk ke laptop": kamera bergerak ke depan layar, FOV menyempit,
// lalu UI laptop (CanvasGroup) muncul. Esc untuk keluar.
public class LaptopFocus : MonoBehaviour
{
    [Header("Kamera")]
    public Camera cam;
    public Transform focusPoint;          // empty di depan layar (lihat tutorial)
    public float moveTime = 0.8f;
    public float focusFov = 40f;

    [Header("Laptop")]
    public PoseToggle lid;                // PoseToggle pada SM_Laptop_Screen (opsional)
    public CanvasGroup screenUI;          // panel UI laptop (alpha 0 saat awal)
    public float fadeTime = 0.45f;

    [Header("Matikan saat fokus (script gerak & mouse look)")]
    public Behaviour[] disableWhileFocused;

    public UnityEvent onEnter, onExit;

    Vector3 savedPos; Quaternion savedRot; float savedFov;
    bool focused, busy;

    void Awake() { SetUI(0f, false); }

    public void Enter() { if (!focused && !busy) StartCoroutine(EnterRoutine()); }
    public void Exit()  { if (focused && !busy)  StartCoroutine(ExitRoutine()); }

    void Update()
    {
        if (focused && !busy && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) Exit();
    }

    IEnumerator EnterRoutine()
    {
        busy = true;
        PlayerInteractor.Blocked = true;
        foreach (var b in disableWhileFocused) if (b) b.enabled = false;

        savedPos = cam.transform.position; savedRot = cam.transform.rotation; savedFov = cam.fieldOfView;
        if (lid) lid.Open();

        yield return MoveCam(focusPoint.position, focusPoint.rotation, focusFov);

        Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
        yield return Fade(0f, 1f);
        SetUI(1f, true);

        focused = true; busy = false;
        onEnter.Invoke();
    }

    IEnumerator ExitRoutine()
    {
        busy = true;
        SetUI(screenUI.alpha, false);
        yield return Fade(1f, 0f);

        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
        yield return MoveCam(savedPos, savedRot, savedFov);

        foreach (var b in disableWhileFocused) if (b) b.enabled = true;
        PlayerInteractor.Blocked = false;
        focused = false; busy = false;
        onExit.Invoke();
    }

    IEnumerator MoveCam(Vector3 p, Quaternion r, float fov)
    {
        Vector3 p0 = cam.transform.position; Quaternion r0 = cam.transform.rotation; float f0 = cam.fieldOfView;
        for (float e = 0; e < moveTime; e += Time.deltaTime)
        {
            float k = Mathf.SmoothStep(0, 1, e / moveTime);
            cam.transform.SetPositionAndRotation(Vector3.Lerp(p0, p, k), Quaternion.Slerp(r0, r, k));
            cam.fieldOfView = Mathf.Lerp(f0, fov, k);
            yield return null;
        }
        cam.transform.SetPositionAndRotation(p, r);
        cam.fieldOfView = fov;
    }

    IEnumerator Fade(float from, float to)
    {
        for (float e = 0; e < fadeTime; e += Time.deltaTime)
        {
            float v = Mathf.Lerp(from, to, Mathf.SmoothStep(0, 1, e / fadeTime));
            screenUI.alpha = v;
            screenUI.transform.localScale = Vector3.one * Mathf.Lerp(0.92f, 1f, v);   // efek "zoom masuk"
            yield return null;
        }
        screenUI.alpha = to;
        screenUI.transform.localScale = Vector3.one * (to > 0.5f ? 1f : 0.92f);
    }

    void SetUI(float alpha, bool interactive)
    {
        if (!screenUI) return;
        screenUI.alpha = alpha;
        screenUI.interactable = interactive;
        screenUI.blocksRaycasts = interactive;
    }
}
