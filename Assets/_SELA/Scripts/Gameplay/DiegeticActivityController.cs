using System.Collections;
using UnityEngine;
using TMPro;
using SELA.Core;
using SELA.UI;

namespace SELA.Gameplay
{
    public class DiegeticActivityController : MonoBehaviour
    {
        public static DiegeticActivityController Instance { get; private set; }
        public static bool IsBusy { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            Instance = null;
            IsBusy = false;
        }

        [Header("Transition Overlay")]
        [SerializeField] CanvasGroup fadeOverlay;
        [SerializeField] TMP_Text captionText;
        [SerializeField] float fadeDuration = 0.35f;
        [SerializeField] float holdDuration = 0.85f;

        [Header("Optional World Effects")]
        [SerializeField] ParticleSystem showerMistEffect;
        [SerializeField] Transform windowLookTarget;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            IsBusy = false;

            if (fadeOverlay != null)
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
                IsBusy = false;
            }
        }

        public void TriggerShower()
        {
            TryRunDiegeticActivity(
                new ActivityRequest(ActivityType.Shower),
                "Mandi  ·  1 jam\nKepala terasa lebih segar.",
                playShowerMist: true,
                lookOutWindow: false);
        }

        public void TriggerTidyDesk()
        {
            TryRunDiegeticActivity(
                new ActivityRequest(ActivityType.TidyDesk),
                "Merapikan meja  ·  1 jam\nKertas-kertas tersusun lagi.",
                playShowerMist: false,
                lookOutWindow: false);
        }

        public void TriggerSitWithoutScreen()
        {
            TryRunDiegeticActivity(
                new ActivityRequest(ActivityType.SitWithoutScreen),
                "Duduk tanpa layar  ·  1 jam\nSebentar menatap keluar jendela.",
                playShowerMist: false,
                lookOutWindow: true);
        }

        void TryRunDiegeticActivity(ActivityRequest req, string caption, bool playShowerMist, bool lookOutWindow)
        {
            if (IsBusy || PauseMenuController.IsPaused || SleepMenuController.IsOpen || SleepMenuController.IsSleeping)
                return;

            var ctrl = GameLoopController.Instance;
            if (ctrl == null || ctrl.Engine == null || ctrl.State == null) return;

            var preview = ctrl.Engine.GetPreview(req);
            if (!preview.Availability.IsAvailable)
            {
                ShowFeedbackThought(preview.Availability.Reason);
                return;
            }

            // If this activity would cause the player to miss today's 13:00 interview, let CoreLoopDebugUI show the confirmation warning.
            if (preview.Availability.WillMissScheduledInterview || preview.WillTriggerRift > 0 || preview.WillTriggerJeda)
            {
                var panel = ctrl.GetComponent<CoreLoopDebugUI>();
                if (panel != null)
                {
                    panel.RequestConfirmation(req);
                    return;
                }
            }

            StartCoroutine(RunRoutine(req, caption, playShowerMist, lookOutWindow));
        }

        IEnumerator RunRoutine(ActivityRequest req, string caption, bool playShowerMist, bool lookOutWindow)
        {
            IsBusy = true;
            PlayerInteractor.Blocked = true;

            float fadeTime = GameSettings.ReduceMotion ? 0.05f : fadeDuration;
            float holdTime = GameSettings.ReduceMotion ? 0.35f : holdDuration;

            if (playShowerMist && showerMistEffect != null)
                showerMistEffect.Play();

            if (lookOutWindow && windowLookTarget != null && Camera.main != null && !GameSettings.ReduceMotion)
            {
                var cam = Camera.main.transform;
                Quaternion startRot = cam.rotation;
                Quaternion targetRot = Quaternion.LookRotation((windowLookTarget.position - cam.position).normalized);
                float lt = 0f;
                while (lt < 1f)
                {
                    lt += Time.unscaledDeltaTime / 0.45f;
                    cam.rotation = Quaternion.Slerp(startRot, targetRot, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(lt)));
                    yield return null;
                }
            }

            if (fadeOverlay != null)
            {
                fadeOverlay.gameObject.SetActive(true);
                fadeOverlay.blocksRaycasts = true;
                if (captionText != null) captionText.text = "";
                yield return FadeAlpha(0f, 1f, fadeTime);
            }

            var ctrl = GameLoopController.Instance;
            if (ctrl != null && ctrl.Engine != null)
                ctrl.Engine.ExecuteActivity(req);

            if (captionText != null)
                captionText.text = caption;

            yield return new WaitForSecondsRealtime(holdTime);

            if (fadeOverlay != null)
            {
                yield return FadeAlpha(1f, 0f, fadeTime);
                fadeOverlay.blocksRaycasts = false;
                fadeOverlay.gameObject.SetActive(false);
            }

            if (playShowerMist && showerMistEffect != null)
                showerMistEffect.Stop();

            IsBusy = false;

            bool freeRoam = ctrl != null && ctrl.State != null && ctrl.State.phase == GamePhase.FreeRoam;
            if (freeRoam && !CoreLoopDebugUI.IsPanelOpen && !PauseMenuController.IsPaused)
            {
                PlayerInteractor.Blocked = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        void ShowFeedbackThought(string text)
        {
            var ctrl = GameLoopController.Instance;
            if (ctrl == null) return;
            var ui = ctrl.GetComponent<CoreLoopDebugUI>();
            if (ui != null) ui.ShowNoticeThought(text);
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
    }
}
