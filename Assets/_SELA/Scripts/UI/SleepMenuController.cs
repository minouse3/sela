using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using SELA.Core;

namespace SELA.UI
{
    public class SleepMenuController : MonoBehaviour
    {
        public static SleepMenuController Instance { get; private set; }
        public static bool IsOpen { get; private set; }
        public static bool IsSleeping { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            Instance = null;
            IsOpen = false;
            IsSleeping = false;
        }

        [Header("Panels & Fade")]
        [SerializeField] GameObject dialogRoot;
        [SerializeField] CanvasGroup fadeOverlay;
        [SerializeField] TMP_Text wakeCaptionText;

        [Header("Header & Info")]
        [SerializeField] TMP_Text currentTimeText;
        [SerializeField] TMP_Text warningBannerText;

        [Header("Sleep Option Buttons")]
        [SerializeField] UnityEngine.UI.Button noonButton;
        [SerializeField] TMP_Text noonDetailText;
        [SerializeField] UnityEngine.UI.Button nightButton;
        [SerializeField] TMP_Text nightDetailText;
        [SerializeField] UnityEngine.UI.Button morningButton;
        [SerializeField] TMP_Text morningLabelText;
        [SerializeField] TMP_Text morningDetailText;
        [SerializeField] UnityEngine.UI.Button cancelButton;

        [Header("Transition Timing")]
        [SerializeField] float fadeDuration = 0.4f;
        [SerializeField] float holdBlackDuration = 0.9f;

        [Header("Optional Audio Hooks")]
        [SerializeField] AudioSource uiAudioSource;
        [SerializeField] AudioClip clickClip;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            IsOpen = false;
            IsSleeping = false;

            if (noonButton) noonButton.onClick.AddListener(() => OnChooseSleep(SleepTarget.UntilNoon));
            if (nightButton) nightButton.onClick.AddListener(() => OnChooseSleep(SleepTarget.UntilNight));
            if (morningButton) morningButton.onClick.AddListener(() => OnChooseSleep(SleepTarget.UntilMorning));
            if (cancelButton) cancelButton.onClick.AddListener(CloseDialog);

            if (dialogRoot) dialogRoot.SetActive(false);
            if (fadeOverlay)
            {
                fadeOverlay.alpha = 0f;
                fadeOverlay.blocksRaycasts = false;
                fadeOverlay.gameObject.SetActive(false);
            }
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                IsOpen = false;
                IsSleeping = false;
            }
        }

        void Update()
        {
            if (IsSleeping || PauseMenuController.IsPaused) return;

            var kb = Keyboard.current;
            if (kb == null) return;

            if (IsOpen)
            {
                if (kb.escapeKey.wasPressedThisFrame)
                {
                    PlayClick();
                    CloseDialog();
                }
                return;
            }

            // While lying on the bed (Pose.Kasur), press E to open Sleep options.
            var seat = SeatInteraction.Current;
            if (seat != null && seat.pose == SeatInteraction.Pose.Kasur &&
                !PlayerInteractor.Blocked && !CoreLoopDebugUI.IsPanelOpen && !LaptopInteraction.AnyOpen)
            {
                var cam = Camera.main;
                var fly = cam != null ? cam.GetComponent<FlyCamera>() : null;
                if (fly != null && fly.IsFlying) return;

                if (kb.eKey.wasPressedThisFrame)
                    OpenDialog();
            }
        }

        public void OpenDialog()
        {
            if (IsOpen || IsSleeping) return;
            var ctrl = GameLoopController.Instance;
            if (ctrl == null || ctrl.State == null || ctrl.State.phase != GamePhase.FreeRoam) return;

            IsOpen = true;
            PlayerInteractor.Blocked = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            RefreshOptions();
            if (dialogRoot) dialogRoot.SetActive(true);
        }

        public void CloseDialog()
        {
            if (!IsOpen) return;
            IsOpen = false;
            if (dialogRoot) dialogRoot.SetActive(false);

            if (!IsSleeping && !PauseMenuController.IsPaused)
            {
                PlayerInteractor.Blocked = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        void RefreshOptions()
        {
            var ctrl = GameLoopController.Instance;
            if (ctrl == null || ctrl.Engine == null || ctrl.State == null) return;

            var state = ctrl.State;
            var engine = ctrl.Engine;

            if (currentTimeText)
                currentTimeText.text = $"Hari {state.day}  ·  {state.FormattedTime} ({state.session})";

            var pNoon = engine.GetSleepPreview(SleepTarget.UntilNoon);
            var pNight = engine.GetSleepPreview(SleepTarget.UntilNight);
            var pMorning = engine.GetSleepPreview(SleepTarget.UntilMorning);

            ApplyOption(noonButton, null, noonDetailText, pNoon, "Siang  ·  12:00");
            ApplyOption(nightButton, null, nightDetailText, pNight, "Malam  ·  18:00");
            ApplyOption(morningButton, morningLabelText, morningDetailText, pMorning,
                pMorning.OpensFinalDecision ? "Tutup Hari 7" : $"Pagi  ·  Hari {pMorning.TargetDay} (08:00)");

            if (warningBannerText)
            {
                bool hasInterviewToday = state.HasScheduledInterviewOn(state.day, out CompanyId scheduled) &&
                                         state.currentHour <= GameState.InterviewHour;
                warningBannerText.gameObject.SetActive(hasInterviewToday);
                if (hasInterviewToday)
                    warningBannerText.text = $"Ada interview {ActivitySystem.GetCompanyName(scheduled)} jam 13:00. Tidur melewatinya bikin lamaran gugur.";
            }
        }

        static void ApplyOption(UnityEngine.UI.Button btn, TMP_Text labelTxt, TMP_Text detailTxt, SleepPreview p, string label)
        {
            if (btn) btn.interactable = p.IsAvailable;
            if (labelTxt) labelTxt.text = label;
            if (!detailTxt) return;

            if (!p.IsAvailable)
            {
                detailTxt.text = p.UnavailableReason;
            }
            else if (p.OpensFinalDecision)
            {
                detailTxt.text = "Masuk ke keputusan akhir";
            }
            else
            {
                string miss = p.WillMissScheduledInterview ? "  ·  Lewatkan interview!" : "";
                detailTxt.text = $"Tenaga {p.EnergyBefore}→{p.EnergyAfter}  ·  Tekanan {p.PressureBefore}→{p.PressureAfter}{miss}";
            }
        }

        void OnChooseSleep(SleepTarget target)
        {
            if (IsSleeping) return;
            var ctrl = GameLoopController.Instance;
            if (ctrl == null || ctrl.Engine == null) return;

            var preview = ctrl.Engine.GetSleepPreview(target);
            if (!preview.IsAvailable) return;

            PlayClick();
            IsOpen = false;
            if (dialogRoot) dialogRoot.SetActive(false);
            StartCoroutine(SleepRoutine(target));
        }

        IEnumerator SleepRoutine(SleepTarget target)
        {
            IsSleeping = true;
            PlayerInteractor.Blocked = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            float fadeTime = GameSettings.ReduceMotion ? 0.05f : fadeDuration;
            float holdTime = GameSettings.ReduceMotion ? 0.35f : holdBlackDuration;

            if (fadeOverlay)
            {
                fadeOverlay.gameObject.SetActive(true);
                fadeOverlay.blocksRaycasts = true;
                if (wakeCaptionText) wakeCaptionText.text = "";
                yield return FadeAlpha(0f, 1f, fadeTime);
            }

            var res = GameLoopController.Instance.Engine.SleepUntil(target);

            if (wakeCaptionText && res.Executed)
                wakeCaptionText.text = res.WakeCaption;

            yield return new WaitForSecondsRealtime(holdTime);

            // Stand up smoothly from bed if currently lying on it (unless FinalDecision opened).
            var seat = SeatInteraction.Current;
            if (seat != null && seat.pose == SeatInteraction.Pose.Kasur && !res.OpenedFinalDecision)
                seat.StandUp();

            if (fadeOverlay)
            {
                yield return FadeAlpha(1f, 0f, fadeTime);
                fadeOverlay.blocksRaycasts = false;
                fadeOverlay.gameObject.SetActive(false);
            }

            IsSleeping = false;

            // If FinalDecision / Rift / Jeda opened, CoreLoopDebugUI takes over cursor/blocking.
            bool stillInFreeRoam = GameLoopController.Instance != null &&
                                   GameLoopController.Instance.State != null &&
                                   GameLoopController.Instance.State.phase == GamePhase.FreeRoam;
            if (stillInFreeRoam && !CoreLoopDebugUI.IsPanelOpen && !PauseMenuController.IsPaused)
            {
                PlayerInteractor.Blocked = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        IEnumerator FadeAlpha(float from, float to, float duration)
        {
            if (fadeOverlay == null) yield break;
            if (duration <= 0f)
            {
                fadeOverlay.alpha = to;
                yield break;
            }
            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / duration;
                fadeOverlay.alpha = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
            fadeOverlay.alpha = to;
        }

        void PlayClick()
        {
            if (uiAudioSource && clickClip)
                uiAudioSource.PlayOneShot(clickClip, GameSettings.EffectsVolume);
        }
    }
}
