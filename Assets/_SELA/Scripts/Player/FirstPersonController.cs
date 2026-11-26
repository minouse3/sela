using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float mouseSensitivity = 0.1f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float eyeForwardClearance = 0.18f;
    [SerializeField] bool invertY = false;

    CharacterController controller;
    Animator animator;
    Camera playerCamera;
    float pitch;
    float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.applyRootMotion = false;
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
        if (!animator) return;
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
        SetCursorLocked(true);
    }

    void OnDisable()
    {
        SetCursorLocked(false);
    }

    void Update()
    {
        if (!controller.enabled) return;

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        // WASD: gerak; mouse: lihat; Esc: lepas kursor; klik kiri: kunci lagi.
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            SetCursorLocked(false);
        else if (Cursor.lockState != CursorLockMode.Locked && mouse != null && mouse.leftButton.wasPressedThisFrame)
            SetCursorLocked(true);

        if (Cursor.lockState == CursorLockMode.Locked && mouse != null)
        {
            Vector2 look = mouse.delta.ReadValue() * mouseSensitivity;
            transform.Rotate(Vector3.up * look.x);
            pitch = Mathf.Clamp(pitch + (invertY ? look.y : -look.y), -89f, 89f);
            playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        Vector2 input = Vector2.zero;
        if (keyboard != null && Cursor.lockState == CursorLockMode.Locked)
        {
            input.x = (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f);
            input.y = (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f);
            input = Vector2.ClampMagnitude(input, 1f);
        }

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 movement = (transform.right * input.x + transform.forward * input.y) * moveSpeed;
        movement.y = verticalVelocity;
        Vector3 previousPosition = transform.position;
        controller.Move(movement * Time.deltaTime);
        if (animator != null)
        {
            Vector3 displacement = transform.position - previousPosition;
            displacement.y = 0f;
            animator.SetInteger("Animation", displacement.sqrMagnitude > 0.00000001f ? 1 : 0);
        }
    }

    static void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
