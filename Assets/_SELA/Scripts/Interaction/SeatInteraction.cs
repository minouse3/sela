using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SeatInteraction : MonoBehaviour
{
    public enum Pose { Duduk, Laptop, Kasur }

    public static SeatInteraction Current { get; private set; }

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
    public float cameraForward = 0.22f;  // maju dari SeatView
    public bool followEyes = false;      // nyalakan nanti kalau mau kamera ikut mata

    [HideInInspector] public bool lookLocked;   // dikunci LaptopInteraction

    FirstPersonController player;
    PlayerInteractor interactor;
    Camera playerCamera;
    Animator anim;
    Vector3 savedCameraPosition;
    Quaternion savedCameraRotation;
    Coroutine moveRoutine;
    float yaw, pitch;
    bool seated, settled, standingUp;

    Vector3 TargetPos() => bodyPoint ? bodyPoint.position : seatView.TransformPoint(bodyOffset);

    Quaternion TargetRot() => Quaternion.Euler(0f, bodyPoint ? bodyPoint.eulerAngles.y : seatView.eulerAngles.y, 0f);

    public void Sit()
    {
        if (seated || standingUp) return;
        player = Object.FindFirstObjectByType<FirstPersonController>();
        if (!player || !seatView || !standingPosition) return;
        playerCamera = player.GetComponentInChildren<Camera>();
        if (!playerCamera) return;
        anim = player.GetComponentInChildren<Animator>();
        interactor = player.GetComponent<PlayerInteractor>();

        savedCameraPosition = playerCamera.transform.localPosition;
        savedCameraRotation = playerCamera.transform.localRotation;

        player.enabled = false;
        if (pose == Pose.Kasur && interactor) interactor.enabled = false;

        var cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        yaw = seatView.eulerAngles.y;
        pitch = Mathf.DeltaAngle(0f, seatView.eulerAngles.x);

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
            var delta = mouse.delta.ReadValue() * 0.1f;
            yaw += delta.x;
            pitch = Mathf.Clamp(pitch - delta.y, -70f, 70f);
        }
        if (PlayerInteractor.Blocked) return;

        if (Keyboard.current.qKey.wasPressedThisFrame)
            StandUp();
    }

    void LateUpdate()
    {
        if ((!seated && !standingUp) || !player) return;

        if (settled && !standingUp) player.transform.position = TargetPos();

        if (lookLocked || !playerCamera) return;   // laptop terbuka: kamera diatur LaptopInteraction

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 camPos = seatView.position + rot * Vector3.forward * cameraForward;

        if (followEyes && anim)
        {
            var l = anim.GetBoneTransform(HumanBodyBones.LeftEye);
            var r = anim.GetBoneTransform(HumanBodyBones.RightEye);
            if (l && r) camPos = (l.position + r.position) * 0.5f + rot * Vector3.forward * 0.12f;
        }

        playerCamera.transform.SetPositionAndRotation(camPos, rot);
    }

    public void StandUp()
    {
        if (!seated || lookLocked || standingUp) return;

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
        {
            playerCamera.transform.localPosition = savedCameraPosition;
            playerCamera.transform.localRotation = savedCameraRotation;
        }

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
