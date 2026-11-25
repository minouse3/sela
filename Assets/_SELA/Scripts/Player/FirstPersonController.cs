using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using SELA.UI;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Pergerakan")]
    [SerializeField] float walkSpeed = 2.5f;
    [SerializeField] float runSpeed = 5.5f;
    [SerializeField] float mouseSensitivity = 0.1f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float eyeForwardClearance = 0.18f;

    [Header("Jongkok & Kamera")]
    [SerializeField] float crouchCameraDrop = 0.5f;   // fallback turun pandangan jika bone mata tidak tersedia
    [SerializeField] float cameraSmooth = 12f;        // kehalusan kamera mengikuti kepala saat jongkok/lari

    [Header("Animasi SELA")]
    [SerializeField] SELAAnimDriver driver;

    CharacterController controller;
    Animator animator;
    Camera playerCamera;
    FlyCamera flyCamera;
    Transform leftEyeBone;
    Transform rightEyeBone;
    Transform headBone;
    float pitch;
    float verticalVelocity;
    bool isSeated;
    bool isCrouching;
    bool isRunning;
    Vector3 standingCamLocalPos;
    float crouchOffset;

    public bool IsSeated => isSeated;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.applyRootMotion = false;
            leftEyeBone = animator.GetBoneTransform(HumanBodyBones.LeftEye);
            rightEyeBone = animator.GetBoneTransform(HumanBodyBones.RightEye);
            headBone = animator.GetBoneTransform(HumanBodyBones.Head);
        }

        if (driver == null) driver = GetComponent<SELAAnimDriver>() ?? GetComponentInChildren<SELAAnimDriver>();

        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            flyCamera = playerCamera.GetComponent<FlyCamera>();
            pitch = Mathf.DeltaAngle(0f, playerCamera.transform.localEulerAngles.x);
        }
        else
        {
            Debug.LogError("FirstPersonController requires a child camera.", this);
            enabled = false;
        }
    }

    bool TryGetEyesLocalPosition(out Vector3 eyesLocal)
    {
        if (leftEyeBone && rightEyeBone)
        {
            Vector3 eyesWorld = (leftEyeBone.position + rightEyeBone.position) * 0.5f;
            eyesLocal = transform.InverseTransformPoint(eyesWorld);
            return true;
        }
        if (headBone)
        {
            eyesLocal = transform.InverseTransformPoint(headBone.position);
            eyesLocal.y += 0.08f;
            return true;
        }
        eyesLocal = Vector3.zero;
        return false;
    }

    void AlignCameraToEyes()
    {
        if (!playerCamera || !TryGetEyesLocalPosition(out Vector3 local)) return;
        local.x = 0f;
        local.z += eyeForwardClearance;
        playerCamera.transform.localPosition = local;
    }

    void Start()
    {
        GameSettings.ApplyAudio();
        if (playerCamera != null)
            playerCamera.fieldOfView = GameSettings.FieldOfView;
        AlignCameraToEyes();
        standingCamLocalPos = playerCamera.transform.localPosition;
        SetCursorLocked(true);
    }

    void OnDisable()
    {
        SetCursorLocked(false);
    }

    void Update()
    {
        if (!controller.enabled || isSeated || PlayerInteractor.Blocked) return;

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        if (PauseMenuController.IsPaused) return;
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame && PauseMenuController.Instance == null)
            SetCursorLocked(false);
        else if (Cursor.lockState != CursorLockMode.Locked && mouse != null && mouse.leftButton.wasPressedThisFrame)
            SetCursorLocked(true);

        if (Cursor.lockState == CursorLockMode.Locked && mouse != null)
        {
            float sens = GameSettings.MouseSensitivity * (mouseSensitivity > 0f ? mouseSensitivity / 0.1f : 1f);
            float ySign = GameSettings.InvertY ? -1f : 1f;
            Vector2 look = mouse.delta.ReadValue() * sens;
            transform.Rotate(Vector3.up * look.x);
            pitch = Mathf.Clamp(pitch - look.y * ySign, -89f, 89f);
            playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        // Input WASD
        Vector2 input = Vector2.zero;
        isRunning = false;

        if (keyboard != null && Cursor.lockState == CursorLockMode.Locked)
        {
            input.x = (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f);
            input.y = (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f);
            input = Vector2.ClampMagnitude(input, 1f);

            // Toggle jongkok (C) hanya saat kursor terkunci
            if (keyboard.cKey.wasPressedThisFrame)
            {
                isCrouching = !isCrouching;
                if (driver) driver.SetCrouch(isCrouching);
            }

            if (keyboard.leftShiftKey.isPressed && input.sqrMagnitude > 0.01f && !isCrouching)
                isRunning = true;
        }

        // Gravitasi
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;

        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        if (isCrouching) currentSpeed = walkSpeed * 0.5f;

        Vector3 movement = (transform.right * input.x + transform.forward * input.y) * currentSpeed;
        movement.y = verticalVelocity;
        controller.Move(movement * Time.deltaTime);
    }

    // Ikuti posisi mata/kepala secara halus agar kamera tidak menembus kepala/rambut saat lari atau jongkok
    void LateUpdate()
    {
        if (!playerCamera || isSeated || (flyCamera && flyCamera.IsFlying)) return;

        Vector3 targetLocal = standingCamLocalPos;
        if (TryGetEyesLocalPosition(out Vector3 eyesLocal))
        {
            // Dorong kamera maju di depan kepala saat animasi condong ke depan (lari / jongkok)
            targetLocal.z = Mathf.Max(standingCamLocalPos.z, eyesLocal.z + eyeForwardClearance);

            // Ikuti tinggi mata aktual saat jongkok atau lari; jaga tetap stabil saat berdiri/jalan
            if (isCrouching || isRunning)
                targetLocal.y = eyesLocal.y;
        }
        else
        {
            float targetDrop = isCrouching ? crouchCameraDrop : 0f;
            crouchOffset = Mathf.Lerp(crouchOffset, targetDrop, Time.deltaTime * cameraSmooth);
            targetLocal.y = standingCamLocalPos.y - crouchOffset;
        }

        playerCamera.transform.localPosition = Vector3.Lerp(
            playerCamera.transform.localPosition,
            targetLocal,
            Time.deltaTime * cameraSmooth);
    }

    // ---------- Pose (dipanggil SeatInteraction) ----------

    void CancelCrouch()
    {
        isCrouching = false;
        crouchOffset = 0f;
        if (driver) driver.SetCrouch(false);
    }

    public void SetSeated(bool seated)
    {
        if (seated)
        {
            StopAllCoroutines();
            CancelCrouch();
            isSeated = true;
            if (driver) driver.SetSitting(true);
        }
        else
        {
            if (driver)
            {
                driver.SetTyping(false);
                driver.SetSitting(false);
            }
            StartCoroutine(UnlockAfter(1f));
        }
    }

    public void SetLying(bool lying)
    {
        if (lying)
        {
            StopAllCoroutines();
            CancelCrouch();
            isSeated = true;
            if (driver) driver.SetLying(true);
        }
        else
        {
            if (driver) driver.SetLying(false);
            StartCoroutine(UnlockAfter(1.2f));
        }
    }

    public void SetTyping(bool typing)
    {
        if (driver) driver.SetTyping(typing);
    }

    public void FinishStandUp()
    {
        StopAllCoroutines();
        isSeated = false;
    }

    IEnumerator UnlockAfter(float t)
    {
        yield return new WaitForSeconds(t);
        isSeated = false;
    }

    public void SetAnimationState(int stateCode) { }

    public static void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}