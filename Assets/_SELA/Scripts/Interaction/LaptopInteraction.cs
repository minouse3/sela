using System.Collections;
using UnityEngine;

public class LaptopInteraction : MonoBehaviour
{
    [Header("Referensi")]
    public Transform laptopView;

    [Header("Pengaturan")]
    public bool requireSeated = true;
    public float moveTime = 0.5f;

    FirstPersonController player;
    CoreLoopDebugUI panel;
    SeatInteraction seat;
    Camera cam;
    Vector3 savedPos;
    Quaternion savedRot;
    Coroutine routine;
    bool isOpen;

    public static bool AnyOpen { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        AnyOpen = false;
    }

    public void Open()
    {
        if (AnyOpen || !laptopView || PlayerInteractor.Blocked) return;

        var loop = GameLoopController.Instance;
        if (loop == null || loop.State.phase != SELA.Core.GamePhase.FreeRoam ||
            !loop.TryGetComponent(out panel) || !panel.isActiveAndEnabled) return;

        seat = SeatInteraction.Current;
        if (requireSeated && (seat == null || seat.pose == SeatInteraction.Pose.Kasur)) return;

        player = Object.FindFirstObjectByType<FirstPersonController>();
        if (!player) return;
        cam = player.GetComponentInChildren<Camera>();
        if (!cam) return;
        var fly = cam.GetComponent<FlyCamera>();
        if (fly && fly.IsFlying) return;
        if (!panel.BeginLaptop(this)) return;

        savedPos = cam.transform.position;
        savedRot = cam.transform.rotation;

        if (seat) seat.lookLocked = true;
        player.SetTyping(true);

        isOpen = true;
        AnyOpen = true;
        PlayerInteractor.Blocked = true;
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
        routine = null;
        panel.ShowLaptop(this);
    }

    void OnDisable()
    {
        if (isOpen) Close();
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        AnyOpen = false;

        if (routine != null) { StopCoroutine(routine); routine = null; }

        if (cam)
        {
            cam.transform.position = savedPos;
            cam.transform.rotation = savedRot;
        }

        if (player) player.SetTyping(false);
        if (seat) seat.lookLocked = false;
        if (panel) panel.OnLaptopClosed(this);
    }
}