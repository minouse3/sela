using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using SELA.UI;

public class SeatInteraction : MonoBehaviour
{
    public enum Pose { Duduk, Laptop, Kasur }

    public static SeatInteraction Current { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Current = null;
    }

    [Header("Pose")]
    public Pose pose = Pose.Duduk;
    public float moveTime = 1f;
    public float standUpTime = 1.5f;     // durasi transisi bangun mengikuti mata (sesuaikan dengan durasi klip TidurKeBerdiri)

    [Header("Titik")]
    public Transform seatView;
    public Transform standingPosition;
    public Transform bodyPoint;          // opsional, prioritas di atas Body Offset

    [Header("Dipakai jika Body Point kosong (lokal terhadap seatView)")]
    public Vector3 bodyOffset = new Vector3(0f, -1.1f, 0f);

    [Header("Kamera")]
    public float cameraForward = 0.22f;  // jarak bebas ke depan saat duduk, ke atas saat berbaring
    public bool followEyes = false;      // nyalakan nanti kalau mau kamera ikut mata

    [HideInInspector] public bool lookLocked;   // dikunci LaptopInteraction

    FirstPersonController player;
    PlayerInteractor interactor;
    Camera playerCamera;
    Transform leftEye, rightEye;
    Coroutine moveRoutine;
    float seatYaw, yaw, pitch;
    bool seated, settled, standingUp, lyingSettled;
    Animator animator;

    Vector3 TargetPos() => bodyPoint ? bodyPoint.position : seatView.TransformPoint(bodyOffset);

    Quaternion TargetRot() => Quaternion.Euler(0f, bodyPoint ? bodyPoint.eulerAngles.y : seatView.eulerAngles.y, 0f);

    public void Sit()
    {
        if (seated || standingUp || Current != null || PlayerInteractor.Blocked) return;
        player = Object.FindFirstObjectByType<FirstPersonController>();
        if (!player || !seatView || !standingPosition) return;
        playerCamera = player.GetComponentInChildren<Camera>();
        if (!playerCamera) return;
        animator = player.GetComponentInChildren<Animator>();
        leftEye = animator ? animator.GetBoneTransform(HumanBodyBones.LeftEye) : null;
        rightEye = animator ? animator.GetBoneTransform(HumanBodyBones.RightEye) : null;
        interactor = player.GetComponent<PlayerInteractor>();

        player.enabled = false;

        var cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        ResetLook();

        settled = false;
        standingUp = false;
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveToSeat(TargetPos(), TargetRot()));

        if (pose == Pose.Kasur) player.SetLying(true);
        else player.SetSeated(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        seated = true;
        lookLocked = false;
        Current = this;
    }

    IEnumerator MoveToSeat(Vector3 targetPos, Quaternion targetRot)
    {
        Vector3 startPos = player.transform.position;
        Quaternion startRot = player.transform.rotation;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, moveTime);
            float k = Mathf.SmoothStep(0f, 1f, t);
            player.transform.position = Vector3.Lerp(startPos, targetPos, k);
            player.transform.rotation = Quaternion.Slerp(startRot, targetRot, k);
            yield return null;
        }
        settled = true;
    }

    void OnDisable()
    {
        if (Current == this) Current = null;
    }

    void Update()
    {
        if (!seated || lookLocked || standingUp || Keyboard.current == null) return;
        var flyCamera = playerCamera ? playerCamera.GetComponent<FlyCamera>() : null;
        if (flyCamera && flyCamera.IsFlying) return;

        var mouse = Mouse.current;
        if (mouse != null && Cursor.lockState == CursorLockMode.Locked)
        {
            float sens = GameSettings.MouseSensitivity;
            float ySign = GameSettings.InvertY ? -1f : 1f;
            var delta = mouse.delta.ReadValue() * sens;
            ApplyLook(delta.x, -delta.y * ySign);
        }
        if (PlayerInteractor.Blocked) return;

        if (Keyboard.current.qKey.wasPressedThisFrame)
            StandUp();
    }

    void ResetLook()
    {
        seatYaw = yaw = seatView.eulerAngles.y;
        pitch = Mathf.DeltaAngle(0f, seatView.eulerAngles.x);
        lyingSettled = pose != Pose.Kasur;
        if (lyingSettled) ApplyLook(0f, 0f);
    }

    void ApplyLook(float yawDelta, float pitchDelta)
    {
        yaw += yawDelta;
        pitch += pitchDelta;
        if (pose != Pose.Kasur) { pitch = Mathf.Clamp(pitch, -70f, 70f); return; }
        if (!lyingSettled) return;   // biarkan bebas saat animasi berbaring masih berjalan
        yaw = Mathf.Clamp(yaw, seatYaw - 40f, seatYaw + 40f);
        pitch = Mathf.Clamp(pitch, -65f, -15f);
    }

    bool LyingIdleSettled()
    {
        if (!animator) return true;
        if (animator.IsInTransition(0)) return false;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Tidur")) return true;
        foreach (var clip in animator.GetCurrentAnimatorClipInfo(0))
            if (clip.clip && clip.clip.name == "Tidur") return true;
        return false;
    }

    void LateUpdate()
    {
        if ((!seated && !standingUp) || !player) return;

        if (settled && !standingUp) player.transform.position = TargetPos();

        if (pose == Pose.Kasur && !lyingSettled && !standingUp && animator && LyingIdleSettled())
        {
            lyingSettled = true;
            yaw = Mathf.Clamp(yaw, seatYaw - 40f, seatYaw + 40f);
            pitch = Mathf.Clamp(pitch, -65f, -15f);
        }

        if (lookLocked || !playerCamera) return;   // laptop terbuka: kamera diatur LaptopInteraction

        var flyCamera = playerCamera.GetComponent<FlyCamera>();
        if (flyCamera && flyCamera.IsFlying) return;   // freecam aktif: jangan timpa posisi kamera

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        bool lying = pose == Pose.Kasur;
        Vector3 clearanceAxis = lying ? Vector3.up : rot * Vector3.forward;
        Vector3 origin = seatView.position;
        float clearance = cameraForward;
        if (followEyes && leftEye && rightEye)
        {
            origin = (leftEye.position + rightEye.position) * 0.5f;
            if (!lying) clearance = 0.12f;
        }

        playerCamera.transform.SetPositionAndRotation(origin + clearanceAxis * clearance, rot);
    }

    public void StandUp()
    {
        if (!seated || lookLocked || standingUp) return;
        var flyCamera = playerCamera ? playerCamera.GetComponent<FlyCamera>() : null;
        if (flyCamera && flyCamera.IsFlying) return;

        if (moveRoutine != null) { StopCoroutine(moveRoutine); moveRoutine = null; }
        settled = false;
        standingUp = true;

        if (player != null)
        {
            if (pose == Pose.Kasur) player.SetLying(false);
            else player.SetSeated(false);

            moveRoutine = StartCoroutine(StandUpRoutine());
        }
    }

    IEnumerator StandUpRoutine()
    {
        Vector3 startPos = player.transform.position;
        Quaternion startRot = player.transform.rotation;
        Quaternion targetRot = Quaternion.Euler(0f, standingPosition.eulerAngles.y, 0f);

        float durasi = followEyes ? standUpTime : 0.2f;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, durasi);
            float k = Mathf.SmoothStep(0f, 1f, t);

            // Geser badan ke Standing Position secara halus sembari kamera terus mengikuti mata di LateUpdate
            player.transform.position = Vector3.Lerp(startPos, standingPosition.position, k);
            player.transform.rotation = Quaternion.Slerp(startRot, targetRot, k);
            yield return null;
        }
        var controller = player.GetComponent<CharacterController>();
        if (controller) controller.enabled = false;
        player.transform.position = standingPosition.position;
        if (controller) controller.enabled = true;

        if (playerCamera != null)
            player.ResetStandingView();

        player.FinishStandUp();
        player.enabled = true;
        if (interactor) interactor.enabled = true;

        standingUp = false;
        seated = false;
        moveRoutine = null;
        if (Current == this) Current = null;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDrawGizmos()
    {
        if (seatView)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(seatView.position, 0.08f);
            Gizmos.DrawRay(seatView.position, seatView.forward * 0.4f);
        }
        if (bodyPoint)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(bodyPoint.position, 0.1f);
            Gizmos.DrawRay(bodyPoint.position, bodyPoint.forward * 0.5f);
        }
    }
}
