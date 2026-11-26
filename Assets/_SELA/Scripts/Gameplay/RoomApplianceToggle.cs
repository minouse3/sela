using UnityEngine;

namespace SELA.Gameplay
{
    [RequireComponent(typeof(Interactable))]
    public class RoomApplianceToggle : MonoBehaviour
    {
        public enum ApplianceKind
        {
            CeilingLight,
            DeskLamp,
            Fan
        }

        [SerializeField] ApplianceKind kind = ApplianceKind.CeilingLight;
        [SerializeField] bool startOn = true;
        [SerializeField] Light[] targetLights;
        [SerializeField] Transform[] switchLevers;
        [SerializeField] Spin fanSpinner;
        [SerializeField] string promptWhenOn = "Matikan lampu";
        [SerializeField] string promptWhenOff = "Nyalakan lampu";

        Interactable interactable;
        bool isOn;

        void Awake()
        {
            interactable = GetComponent<Interactable>();
            isOn = startOn;
            ApplyState();
        }

        public void Toggle()
        {
            isOn = !isOn;
            ApplyState();
        }

        void ApplyState()
        {
            if (targetLights != null)
            {
                for (int i = 0; i < targetLights.Length; i++)
                {
                    if (targetLights[i] != null)
                        targetLights[i].enabled = isOn;
                }
            }

            if (switchLevers != null)
            {
                float angle = isOn ? 14f : -14f;
                for (int i = 0; i < switchLevers.Length; i++)
                {
                    if (switchLevers[i] != null)
                        switchLevers[i].localRotation = Quaternion.Euler(angle, 0f, 0f);
                }
            }

            if (kind == ApplianceKind.Fan && fanSpinner != null)
                fanSpinner.running = isOn;

            if (interactable != null)
                interactable.prompt = isOn ? promptWhenOn : promptWhenOff;
        }
    }
}
