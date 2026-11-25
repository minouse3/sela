using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Pergerakan")]
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float runSpeed = 5.5f;
    [SerializeField] float mouseSensitivity = 0.1f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float eyeForwardClearance = 0.18f;

    [Header("Jongkok")]
    [SerializeField] float crouchCameraDrop = 0.5f;   // seberapa turun pandangan (meter)
    [SerializeField] float crouchSmooth = 8f;

    [Header("Animasi SELA")]
    [SerializeField] SELAAnimDriver driver;

    CharacterController controller;
    Animator animator;
    Camera playerCamera;
    float pitch;
    float verticalVelocity;
    bool isSeated;
    bool isCrouching;
    float standingCamY;      // tinggi kamera saat berdiri
    float crouchOffset;      // 0 = berdiri, crouchCameraDrop = jongkok penuh

    public bool IsSeated => isSeated;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.applyRootMotion = false;

        if (driver == null) driver = GetComponent<SELAAnimDriver>();

        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            pitch = Mathf.DeltaAngle(0f, playerCamera.transform.localEulerAngles.x);
        }
        else
        {
            Debug.LogError("FirstPersonController requires a child camera.", this);
            enabled = false;
        }
    }

    void AlignCameraToEyes()
    {
        if (!animator || !playerCamera) return;
        var left = animator.GetBoneTransform(HumanBodyBones.LeftEye);
        var right = animator.GetBoneTransform(HumanBodyBones.RightEye);
        if (!left || !right) return;

        Vector3 eyes = (left.position + right.position) * 0.5f;
        Vector3 local = transform.InverseTransformPoint(eyes);
        local.z += eyeForwardClearance;
        playerCamera.transform.localPosition = local;
    }

    void Start()
    {
        AlignCameraToEyes();
        standingCamY = playerCamera.transform.localPosition.y;
        SetCursorLocked(true);
    }

    void OnDisable()
    {
        SetCursorLocked(false);
    }

    void Update()
    {
        if (!controller.enabled || isSeated) return;

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            SetCursorLocked(false);
        else if (Cursor.lockState != CursorLockMode.Locked && mouse != null && mouse.leftButton.wasPressedThisFrame)
            SetCursorLocked(true);

        if (Cursor.lockState == CursorLockMode.Locked && mouse != null)
        {
            Vector2 look = mouse.delta.ReadValue() * mouseSensitivity;
            transform.Rotate(Vector3.up * look.x);
            pitch = Mathf.Clamp(pitch - look.y, -89f, 89f);
            playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        // Input WASD
        Vector2 input = Vector2.zero;
        bool isRunning = false;

        if (keyboard != null && Cursor.lockState == CursorLockMode.Locked)
        {
            input.x = (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f);
            input.y = (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f);
            input = Vector2.ClampMagnitude(input, 1f);

            if (keyboard.leftShiftKey.isPressed && input.sqrMagnitude > 0.01f)
                isRunning = true;
        }

        // Toggle jongkok (C)
        if (keyboard != null && keyboard.cKey.wasPressedThisFrame)
        {
            isCrouching = !isCrouching;
            if (driver) driver.SetCrouch(isCrouching);
        }
        if (isCrouching && isRunning) isRunning = false;   // tidak lari saat jongkok

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

    // Pandangan turun halus saat jongkok
    void LateUpdate()
    {
        if (!playerCamera || isSeated) return;
        float target = isCrouching ? crouchCameraDrop : 0f;
        crouchOffset = Mathf.Lerp(crouchOffset, target, Time.deltaTime * crouchSmooth);
        var p = playerCamera.transform.localPosition;
        p.y = standingCamY - crouchOffset;
        playerCamera.transform.localPosition = p;
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