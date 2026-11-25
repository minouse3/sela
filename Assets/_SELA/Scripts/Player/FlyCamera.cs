using UnityEngine;
using UnityEngine.InputSystem;

// FlyCamera murni berfungsi sebagai freecam bebas (tekan F).
// Tidak mengintervensi atau mengubah sistem animasi karakter.
[RequireComponent(typeof(Camera))]
public class FlyCamera : MonoBehaviour
{
    [SerializeField] float speed = 4f;
    [SerializeField] float fastMultiplier = 3f;
    [SerializeField] float sensitivity = 0.1f;

    FirstPersonController player;
    Transform originalParent;
    Vector3 originalPosition;
    Quaternion originalRotation;
    bool flying;
    public bool IsFlying => flying;
    bool playerWasEnabled;
    float yaw;
    float pitch;

    void Awake()
    {
        player = GetComponentInParent<FirstPersonController>();
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null || player == null) return;

        if (keyboard.fKey.wasPressedThisFrame)
        {
            if (flying) ExitFlyMode();
            else EnterFlyMode();
            return;
        }

        if (!flying) return;

        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Cursor.lockState != CursorLockMode.Locked) return;

        if (mouse != null)
        {
            Vector2 delta = mouse.delta.ReadValue() * sensitivity;
            yaw += delta.x;
            pitch = Mathf.Clamp(pitch - delta.y, -89f, 89f);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        Vector3 input = new Vector3(
            (keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0),
            (keyboard.eKey.isPressed ? 1 : 0) - (keyboard.qKey.isPressed ? 1 : 0),
            (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0));
        input = Vector3.ClampMagnitude(input, 1f);
        float multiplier = keyboard.leftShiftKey.isPressed ? fastMultiplier : 1f;
        transform.position += (transform.right * input.x + Vector3.up * input.y + transform.forward * input.z) * (speed * multiplier * Time.deltaTime);
    }

    void EnterFlyMode()
    {
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = Mathf.DeltaAngle(0f, angles.x);

        playerWasEnabled = player.enabled;
        player.enabled = false;

        var cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        transform.SetParent(null, true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        flying = true;
    }

    public void ExitFlyMode()
    {
        if (originalParent != null && gameObject.activeInHierarchy)
        {
            transform.SetParent(originalParent, false);
        }
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;

        if (player != null)
        {
            var cc = player.GetComponent<CharacterController>();
            if (cc && playerWasEnabled) cc.enabled = true;
            player.enabled = playerWasEnabled;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        flying = false;
    }

    void OnDisable()
    {
        if (flying)
        {
            flying = false;
            if (player != null)
            {
                var cc = player.GetComponent<CharacterController>();
                if (cc && playerWasEnabled) cc.enabled = true;
                player.enabled = playerWasEnabled;
            }
        }
    }
}
