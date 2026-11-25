using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SELA.Core;

namespace SELA.UI
{
    public class GameplayHudController : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] GameObject hudRoot;

        [Header("Top-Left Day & Objective Note")]
        [SerializeField] TMP_Text dayTimeText;
        [SerializeField] TMP_Text objectiveText;

        [Header("Bottom-Left Stat Card")]
        [SerializeField] Image[] energySlots;
        [SerializeField] Image[] pressureSlots;

        [Header("Cat Poster Palette")]
        [SerializeField] Color energyFilledColor = new Color(0.682f, 0.455f, 0.384f, 1f);
        [SerializeField] Color pressureFilledColor = new Color(0.769f, 0.361f, 0.243f, 1f);
        [SerializeField] Color slotEmptyColor = new Color(0.812f, 0.776f, 0.706f, 1f);

        bool subscribed;

        void Start()
        {
            TrySubscribe();
        }

        void OnDestroy()
        {
            if (subscribed && GameLoopController.Instance != null && GameLoopController.Instance.Engine != null)
                GameLoopController.Instance.Engine.OnStateChanged -= Refresh;
        }

        void Update()
        {
            if (!subscribed)
                TrySubscribe();

            if (hudRoot != null)
            {
                bool shouldShow = !PauseMenuController.IsPaused &&
                                  !SleepMenuController.IsOpen &&
                                  !SleepMenuController.IsSleeping &&
                                  !CoreLoopDebugUI.IsPanelOpen &&
                                  !LaptopInteraction.AnyOpen;
                if (hudRoot.activeSelf != shouldShow)
                    hudRoot.SetActive(shouldShow);
            }
        }

        void TrySubscribe()
        {
            if (GameLoopController.Instance == null || GameLoopController.Instance.Engine == null) return;
            GameLoopController.Instance.Engine.OnStateChanged -= Refresh;
            GameLoopController.Instance.Engine.OnStateChanged += Refresh;
            subscribed = true;
            Refresh(GameLoopController.Instance.State);
        }

        public void Refresh(GameState state)
        {
            if (state == null) return;

            if (dayTimeText != null)
                dayTimeText.text = $"Hari {state.day}  ·  {state.FormattedTime}  ·  {state.session}";

            if (objectiveText != null)
                objectiveText.text = ActivitySystem.GetHudObjectiveHint(state);

            if (energySlots != null)
            {
                for (int i = 0; i < energySlots.Length; i++)
                {
                    if (energySlots[i] != null)
                        energySlots[i].color = i < state.energy ? energyFilledColor : slotEmptyColor;
                }
            }

            if (pressureSlots != null)
            {
                for (int i = 0; i < pressureSlots.Length; i++)
                {
                    if (pressureSlots[i] != null)
                        pressureSlots[i].color = i < state.pressure ? pressureFilledColor : slotEmptyColor;
                }
            }
        }
    }
}
