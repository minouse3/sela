using UnityEngine;
using UnityEngine.InputSystem;

public class SeatInteraction : MonoBehaviour
{
    public static SeatInteraction Current { get; private set; }

    public Transform seatView;
    public Transform standingPosition;
    FirstPersonController player;
    Camera playerCamera;
    Vector3 savedCameraPosition;
    Quaternion savedCameraRotation;
    Animator animator;
    float yaw;
    float pitch;
    bool seated;
    int satFrame;

    public void Sit()
    {
        if (seated) return;
        player = Object.FindFirstObjectByType<FirstPersonController>();
        if (!player || !seatView || !standingPosition) return;
        playerCamera = player.GetComponentInChildren<Camera>();
        if (!playerCamera) return;
        savedCameraPosition = playerCamera.transform.localPosition;
        savedCameraRotation = playerCamera.transform.localRotation;
        animator = player.GetComponentInChildren<Animator>();
        player.enabled = false;
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = seatView.position - Vector3.up * 1.1f;
        player.transform.rotation = seatView.rotation;
        playerCamera.transform.position = seatView.position + seatView.forward * 0.22f;
        playerCamera.transform.rotation = seatView.rotation;
        yaw = player.transform.eulerAngles.y;
        pitch = 0f;
        if (animator) animator.SetInteger("Animation", 5);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        seated = true;
        satFrame = Time.frameCount;
        Current = this;
    }

    void OnDisable()
    {
        if (Current == this) Current = null;
    }

    void Update()
    {
        if (!seated || Keyboard.current == null) return;
        var flyCamera = playerCamera ? playerCamera.GetComponent<FlyCamera>() : null;
        if (flyCamera && flyCamera.IsFlying) return;
        var mouse = Mouse.current;
        if (mouse != null && Cursor.lockState == CursorLockMode.Locked)
        {
            var delta = mouse.delta.ReadValue() * 0.1f;
            yaw += delta.x;
            pitch = Mathf.Clamp(pitch - delta.y, -70f, 70f);
            playerCamera.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        if (Time.frameCount == satFrame) return;
        if (!Keyboard.current.eKey.wasPressedThisFrame && !Keyboard.current.escapeKey.wasPressedThisFrame) return;
        var controller = player.GetComponent<CharacterController>();
        controller.enabled = false;
        player.transform.position = standingPosition.position;
        controller.enabled = true;
        playerCamera.transform.localPosition = savedCameraPosition;
        playerCamera.transform.localRotation = savedCameraRotation;
        if (animator) animator.SetInteger("Animation", 0);
        seated = false;
        if (Current == this) Current = null;
        player.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
