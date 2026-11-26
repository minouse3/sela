using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using SELA.Core;

namespace SELA.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        public static PauseMenuController Instance { get; private set; }
        public static bool IsPaused { get; private set; }
        internal bool IsLogOpen { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            Instance = null;
            IsPaused = false;
        }

        [Header("Navigation")]
        [SerializeField] string mainMenuSceneName = "MainMenu";

        [Header("Panels")]
        [SerializeField] GameObject rootOverlay;
        [SerializeField] GameObject mainPausePanel;
        [SerializeField] GameObject settingsPanel;

        [Header("Main Pause Controls")]
        [SerializeField] TMP_Text statusSubtitleText;
        [SerializeField] UnityEngine.UI.Button resumeButton;
        [SerializeField] UnityEngine.UI.Button restartDayButton;
        [SerializeField] UnityEngine.UI.Button riftHelpButton;
        [SerializeField] UnityEngine.UI.Button settingsButton;
        [SerializeField] UnityEngine.UI.Button mainMenuButton;
        [SerializeField] UnityEngine.UI.Button quitButton;

        [Header("Settings Controls")]
        [SerializeField] UnityEngine.UI.Slider masterVolumeSlider;
        [SerializeField] TMP_Text masterVolumeValue;
        [SerializeField] UnityEngine.UI.Slider sensitivitySlider;
        [SerializeField] TMP_Text sensitivityValue;
        [SerializeField] UnityEngine.UI.Slider fovSlider;
        [SerializeField] TMP_Text fovValue;
        [SerializeField] UnityEngine.UI.Toggle invertYToggle;
        [SerializeField] UnityEngine.UI.Toggle reduceMotionToggle;
        [SerializeField] UnityEngine.UI.Toggle lowAtmosphereToggle;
        [SerializeField] UnityEngine.UI.Button textScaleButton;
        [SerializeField] TMP_Text textScaleValue;
        [SerializeField] UnityEngine.UI.Button closeSettingsButton;

        [Header("Optional Audio Hooks")]
        [SerializeField] AudioSource uiAudioSource;
        [SerializeField] AudioClip clickClip;

        bool wasBlockedBeforePause;
        CursorLockMode prevCursorLock;
        bool prevCursorVisible;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            IsPaused = false;
            Time.timeScale = 1f;

            BindButtons();
            CreateLogButton();
            BindSettings();

            if (rootOverlay != null) rootOverlay.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (mainPausePanel != null) mainPausePanel.SetActive(true);
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                IsPaused = false;
                Time.timeScale = 1f;
            }
        }

        void Update()
        {
            var kb = Keyboard.current;
            if (kb == null || !kb.escapeKey.wasPressedThisFrame) return;

            if (IsPaused)
            {
                if (IsLogOpen)
                {
                    CloseLog();
                    return;
                }
                PlayClick();
                if (settingsPanel != null && settingsPanel.activeSelf)
                {
                    CloseSettings();
                }
                else
                {
                    Resume();
                }
                return;
            }

            // Let active gameplay overlays (Sleep, Laptop, or CoreLoop panel) consume Escape first.
            if (SleepMenuController.IsOpen || SleepMenuController.IsSleeping ||
                LaptopInteraction.AnyOpen || CoreLoopDebugUI.IsPanelOpen)
                return;

            Pause();
        }

        void BindButtons()
        {
            if (resumeButton) resumeButton.onClick.AddListener(OnResumeClicked);
            if (restartDayButton) restartDayButton.onClick.AddListener(OnRestartDayClicked);
            if (riftHelpButton) riftHelpButton.onClick.AddListener(OnRiftHelpClicked);
            if (settingsButton) settingsButton.onClick.AddListener(OnOpenSettingsClicked);
            if (mainMenuButton) mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            if (quitButton) quitButton.onClick.AddListener(OnQuitClicked);
            if (closeSettingsButton) closeSettingsButton.onClick.AddListener(OnCloseSettingsClicked);
        }

        void CreateLogButton()
        {
            if (!settingsButton || !mainPausePanel) return;
            var logButton = Instantiate(settingsButton, settingsButton.transform.parent, false);
            logButton.name = "DialogueLogButton";
            logButton.transform.SetSiblingIndex(settingsButton.transform.GetSiblingIndex() + 1);
            logButton.GetComponentInChildren<TMP_Text>(true).text = "Log";
            logButton.onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            logButton.onClick.AddListener(OpenLog);
        }

        void OpenLog()
        {
            PlayClick();
            IsLogOpen = true;
            mainPausePanel.SetActive(false);
        }

        internal void CloseLog()
        {
            PlayClick();
            IsLogOpen = false;
            mainPausePanel.SetActive(true);
        }

        void BindSettings()
        {
            if (masterVolumeSlider)
                masterVolumeSlider.onValueChanged.AddListener(v => { GameSettings.MasterVolume = v; UpdateSettingsLabels(); });
            if (sensitivitySlider)
                sensitivitySlider.onValueChanged.AddListener(v => { GameSettings.MouseSensitivity = v; UpdateSettingsLabels(); });
            if (fovSlider)
                fovSlider.onValueChanged.AddListener(v =>
                {
                    GameSettings.FieldOfView = v;
                    ApplyLiveCameraFov();
                    UpdateSettingsLabels();
                });
            if (invertYToggle)
                invertYToggle.onValueChanged.AddListener(v => GameSettings.InvertY = v);
            if (reduceMotionToggle)
                reduceMotionToggle.onValueChanged.AddListener(v => GameSettings.ReduceMotion = v);
            if (lowAtmosphereToggle)
                lowAtmosphereToggle.onValueChanged.AddListener(v => GameSettings.LowAtmosphere = v);
            if (textScaleButton)
                textScaleButton.onClick.AddListener(() =>
                {
                    PlayClick();
                    GameSettings.TextScaleIndex = (GameSettings.TextScaleIndex + 1) % 3;
                    UpdateSettingsLabels();
                });
        }

        public void Pause()
        {
            if (IsPaused) return;

            wasBlockedBeforePause = PlayerInteractor.Blocked;
            prevCursorLock = Cursor.lockState;
            prevCursorVisible = Cursor.visible;

            IsPaused = true;
            PlayerInteractor.Blocked = true;
            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (rootOverlay != null) rootOverlay.SetActive(true);
            if (mainPausePanel != null) mainPausePanel.SetActive(true);
            if (settingsPanel != null) settingsPanel.SetActive(false);

            RefreshDynamicState();
            SyncSettingsUI();
        }

        public void Resume()
        {
            if (!IsPaused) return;

            GameSettings.Save();
            IsLogOpen = false;
            IsPaused = false;
            Time.timeScale = 1f;

            if (rootOverlay != null) rootOverlay.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);

            PlayerInteractor.Blocked = wasBlockedBeforePause;
            Cursor.lockState = wasBlockedBeforePause ? prevCursorLock : CursorLockMode.Locked;
            Cursor.visible = wasBlockedBeforePause ? prevCursorVisible : false;
        }

        public void OpenSettings()
        {
            SyncSettingsUI();
            if (mainPausePanel != null) mainPausePanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(true);
        }

        public void CloseSettings()
        {
            GameSettings.Save();
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (mainPausePanel != null) mainPausePanel.SetActive(true);
            RefreshDynamicState();
        }

        void RefreshDynamicState()
        {
            var ctrl = GameLoopController.Instance;
            var state = ctrl != null ? ctrl.State : null;

            if (statusSubtitleText != null)
            {
                statusSubtitleText.text = state != null
                    ? $"Hari {state.day}  ·  {state.FormattedTime}  ·  {state.session}"
                    : "Kamar 07";
            }

            if (restartDayButton != null)
            {
                bool hasCheckpoint = ctrl != null && ctrl.Engine != null && ctrl.Engine.Save.HasDayStartCheckpoint();
                restartDayButton.interactable = hasCheckpoint;
            }

            if (riftHelpButton != null)
            {
                bool inRift = state != null && state.phase == GamePhase.InRift;
                riftHelpButton.gameObject.SetActive(inRift);
            }
        }

        void SyncSettingsUI()
        {
            if (masterVolumeSlider) masterVolumeSlider.SetValueWithoutNotify(GameSettings.MasterVolume);
            if (sensitivitySlider) sensitivitySlider.SetValueWithoutNotify(GameSettings.MouseSensitivity);
            if (fovSlider) fovSlider.SetValueWithoutNotify(GameSettings.FieldOfView);
            if (invertYToggle) invertYToggle.SetIsOnWithoutNotify(GameSettings.InvertY);
            if (reduceMotionToggle) reduceMotionToggle.SetIsOnWithoutNotify(GameSettings.ReduceMotion);
            if (lowAtmosphereToggle) lowAtmosphereToggle.SetIsOnWithoutNotify(GameSettings.LowAtmosphere);
            UpdateSettingsLabels();
        }

        void UpdateSettingsLabels()
        {
            if (masterVolumeValue) masterVolumeValue.text = $"{Mathf.RoundToInt(GameSettings.MasterVolume * 100f)}%";
            if (sensitivityValue) sensitivityValue.text = $"{GameSettings.MouseSensitivity:0.00}";
            if (fovValue) fovValue.text = $"{Mathf.RoundToInt(GameSettings.FieldOfView)}°";
            if (textScaleValue) textScaleValue.text = $"{Mathf.RoundToInt(GameSettings.TextScaleMultiplier * 100f)}%";
        }

        void ApplyLiveCameraFov()
        {
            var cam = Camera.main;
            if (cam != null) cam.fieldOfView = GameSettings.FieldOfView;
        }

        // --- Button Callbacks ---

        void OnResumeClicked()
        {
            PlayClick();
            Resume();
        }

        void OnRestartDayClicked()
        {
            PlayClick();
            var ctrl = GameLoopController.Instance;
            if (ctrl != null && ctrl.Engine != null)
                ctrl.Engine.LoadDayStartCheckpoint();
            Resume();
        }

        void OnRiftHelpClicked()
        {
            PlayClick();
            var ctrl = GameLoopController.Instance;
            if (ctrl != null && ctrl.Engine != null)
                ctrl.Engine.ResolveRift(skippedViaHelp: true);
            Resume();
        }

        void OnOpenSettingsClicked()
        {
            PlayClick();
            OpenSettings();
        }

        void OnCloseSettingsClicked()
        {
            PlayClick();
            CloseSettings();
        }

        void OnMainMenuClicked()
        {
            PlayClick();
            GameSettings.Save();
            IsPaused = false;
            Time.timeScale = 1f;
            PlayerInteractor.Blocked = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (Application.CanStreamedLevelBeLoaded(mainMenuSceneName))
                SceneManager.LoadScene(mainMenuSceneName);
        }

        void OnQuitClicked()
        {
            PlayClick();
            GameSettings.Save();
            Time.timeScale = 1f;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        void PlayClick()
        {
            if (uiAudioSource && clickClip)
                uiAudioSource.PlayOneShot(clickClip, GameSettings.EffectsVolume);
        }
    }
}
