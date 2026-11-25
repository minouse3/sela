using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using SELA.Core;

namespace SELA.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Scene Navigation")]
        [SerializeField] string gameplaySceneName = "Kos_Main";

        [Header("Primary Menu Buttons")]
        [SerializeField] UnityEngine.UI.Button newGameButton;
        [SerializeField] UnityEngine.UI.Button continueButton;
        [SerializeField] TMP_Text continueSubtitleText;
        [SerializeField] UnityEngine.UI.Button loadGameButton;
        [SerializeField] UnityEngine.UI.Button settingsButton;
        [SerializeField] UnityEngine.UI.Button creditsButton;
        [SerializeField] UnityEngine.UI.Button quitButton;

        [Header("Sub-Panels")]
        [SerializeField] GameObject loadPanel;
        [SerializeField] GameObject settingsPanel;
        [SerializeField] GameObject creditsPanel;
        [SerializeField] GameObject confirmNewGamePanel;
        [SerializeField] TMP_Text statusToastText;

        [Header("Load Panel Controls")]
        [SerializeField] TMP_Text autosaveInfoText;
        [SerializeField] UnityEngine.UI.Button loadAutosaveButton;
        [SerializeField] TMP_Text dayStartInfoText;
        [SerializeField] UnityEngine.UI.Button loadDayStartButton;
        [SerializeField] UnityEngine.UI.Button closeLoadButton;

        [Header("Confirm New Game Controls")]
        [SerializeField] UnityEngine.UI.Button confirmNewGameYesButton;
        [SerializeField] UnityEngine.UI.Button confirmNewGameCancelButton;

        [Header("Settings Controls")]
        [SerializeField] UnityEngine.UI.Slider masterVolumeSlider;
        [SerializeField] TMP_Text masterVolumeValue;
        [SerializeField] UnityEngine.UI.Slider ambienceVolumeSlider;
        [SerializeField] TMP_Text ambienceVolumeValue;
        [SerializeField] UnityEngine.UI.Slider effectsVolumeSlider;
        [SerializeField] TMP_Text effectsVolumeValue;
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

        [Header("Credits Controls")]
        [SerializeField] UnityEngine.UI.Button closeCreditsButton;

        [Header("Transition & Optional Audio Hooks")]
        [SerializeField] CanvasGroup fadeOverlay;
        [SerializeField] AudioSource uiAudioSource;
        [SerializeField] AudioClip clickClip;

        SaveSystem saveSystem;
        bool isTransitioning;

        void Awake()
        {
            saveSystem = new SaveSystem();
            GameSettings.ApplyAudio();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 1f;

            BindButtons();
            BindSettings();
        }

        void Start()
        {
            CloseAllSubPanels();
            RefreshSaveStateUI();
            SyncSettingsUI();

            if (fadeOverlay != null)
            {
                fadeOverlay.alpha = 1f;
                fadeOverlay.blocksRaycasts = false;
                StartCoroutine(FadeTo(0f, GameSettings.ReduceMotion ? 0f : 0.3f));
            }
        }

        void Update()
        {
            if (isTransitioning) return;
            var kb = Keyboard.current;
            if (kb != null && kb.escapeKey.wasPressedThisFrame)
            {
                if (HasOpenSubPanel())
                {
                    PlayClick();
                    CloseAllSubPanels();
                }
            }
        }

        void BindButtons()
        {
            if (newGameButton) newGameButton.onClick.AddListener(OnNewGameClicked);
            if (continueButton) continueButton.onClick.AddListener(OnContinueClicked);
            if (loadGameButton) loadGameButton.onClick.AddListener(OnOpenLoadPanel);
            if (settingsButton) settingsButton.onClick.AddListener(OnOpenSettingsPanel);
            if (creditsButton) creditsButton.onClick.AddListener(OnOpenCreditsPanel);
            if (quitButton) quitButton.onClick.AddListener(OnQuitClicked);

            if (loadAutosaveButton) loadAutosaveButton.onClick.AddListener(OnLoadAutosaveClicked);
            if (loadDayStartButton) loadDayStartButton.onClick.AddListener(OnLoadDayStartClicked);
            if (closeLoadButton) closeLoadButton.onClick.AddListener(OnCloseSubPanelsClicked);

            if (confirmNewGameYesButton) confirmNewGameYesButton.onClick.AddListener(OnConfirmNewGameYes);
            if (confirmNewGameCancelButton) confirmNewGameCancelButton.onClick.AddListener(OnCloseSubPanelsClicked);

            if (closeSettingsButton) closeSettingsButton.onClick.AddListener(OnCloseSettingsClicked);
            if (closeCreditsButton) closeCreditsButton.onClick.AddListener(OnCloseSubPanelsClicked);
        }

        void BindSettings()
        {
            if (masterVolumeSlider)
                masterVolumeSlider.onValueChanged.AddListener(v => { GameSettings.MasterVolume = v; UpdateSettingsLabels(); });
            if (ambienceVolumeSlider)
                ambienceVolumeSlider.onValueChanged.AddListener(v => { GameSettings.AmbienceVolume = v; UpdateSettingsLabels(); });
            if (effectsVolumeSlider)
                effectsVolumeSlider.onValueChanged.AddListener(v => { GameSettings.EffectsVolume = v; UpdateSettingsLabels(); });
            if (sensitivitySlider)
                sensitivitySlider.onValueChanged.AddListener(v => { GameSettings.MouseSensitivity = v; UpdateSettingsLabels(); });
            if (fovSlider)
                fovSlider.onValueChanged.AddListener(v => { GameSettings.FieldOfView = v; UpdateSettingsLabels(); });
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

        public void RefreshSaveStateUI()
        {
            var autoRes = saveSystem.LoadAutosave(allowFallbackToDayStart: false);
            var dayRes = saveSystem.LoadDayStartCheckpoint();

            bool canContinue = autoRes.Success || dayRes.Success;

            if (continueButton)
                continueButton.interactable = canContinue;

            if (continueSubtitleText)
            {
                if (autoRes.Success && autoRes.State != null)
                    continueSubtitleText.text = $"Hari {autoRes.State.day} · {autoRes.State.FormattedTime}";
                else if (dayRes.Success && dayRes.State != null)
                    continueSubtitleText.text = $"Hari {dayRes.State.day} · 08:00";
                else
                    continueSubtitleText.text = "Kosong";
            }

            if (autosaveInfoText)
            {
                autosaveInfoText.text = autoRes.Success && autoRes.State != null
                    ? $"Hari {autoRes.State.day} · {autoRes.State.FormattedTime} ({autoRes.State.session})  ·  Tenaga {autoRes.State.energy}/4"
                    : "Belum ada autosave.";
            }
            if (loadAutosaveButton)
                loadAutosaveButton.interactable = autoRes.Success;

            if (dayStartInfoText)
            {
                dayStartInfoText.text = dayRes.Success && dayRes.State != null
                    ? $"Hari {dayRes.State.day} · 08:00 (Pagi)"
                    : "Belum ada simpanan awal hari.";
            }
            if (loadDayStartButton)
                loadDayStartButton.interactable = dayRes.Success;
        }

        void SyncSettingsUI()
        {
            if (masterVolumeSlider) masterVolumeSlider.SetValueWithoutNotify(GameSettings.MasterVolume);
            if (ambienceVolumeSlider) ambienceVolumeSlider.SetValueWithoutNotify(GameSettings.AmbienceVolume);
            if (effectsVolumeSlider) effectsVolumeSlider.SetValueWithoutNotify(GameSettings.EffectsVolume);
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
            if (ambienceVolumeValue) ambienceVolumeValue.text = $"{Mathf.RoundToInt(GameSettings.AmbienceVolume * 100f)}%";
            if (effectsVolumeValue) effectsVolumeValue.text = $"{Mathf.RoundToInt(GameSettings.EffectsVolume * 100f)}%";
            if (sensitivityValue) sensitivityValue.text = $"{GameSettings.MouseSensitivity:0.00}";
            if (fovValue) fovValue.text = $"{Mathf.RoundToInt(GameSettings.FieldOfView)}°";
            if (textScaleValue)
            {
                int pct = Mathf.RoundToInt(GameSettings.TextScaleMultiplier * 100f);
                textScaleValue.text = $"{pct}%";
            }
        }

        // --- Public Button Handlers ---

        public void OnNewGameClicked()
        {
            PlayClick();
            if (saveSystem.HasAnySave() && confirmNewGamePanel != null)
            {
                ShowOnlyPanel(confirmNewGamePanel);
                return;
            }
            StartGameplay(BootLoadMode.NewGame);
        }

        public void OnConfirmNewGameYes()
        {
            PlayClick();
            StartGameplay(BootLoadMode.NewGame);
        }

        public void OnContinueClicked()
        {
            PlayClick();
            if (!saveSystem.HasAnySave())
            {
                SetToast("Belum ada simpanan yang dapat dilanjutkan.");
                return;
            }
            StartGameplay(BootLoadMode.ContinueAutosave);
        }

        public void OnOpenLoadPanel()
        {
            PlayClick();
            RefreshSaveStateUI();
            ShowOnlyPanel(loadPanel);
        }

        public void OnLoadAutosaveClicked()
        {
            PlayClick();
            StartGameplay(BootLoadMode.ContinueAutosave);
        }

        public void OnLoadDayStartClicked()
        {
            PlayClick();
            StartGameplay(BootLoadMode.LoadDayStart);
        }

        public void OnOpenSettingsPanel()
        {
            PlayClick();
            SyncSettingsUI();
            ShowOnlyPanel(settingsPanel);
        }

        public void OnCloseSettingsClicked()
        {
            PlayClick();
            GameSettings.Save();
            CloseAllSubPanels();
            SetToast("Pengaturan tersimpan.");
        }

        public void OnOpenCreditsPanel()
        {
            PlayClick();
            ShowOnlyPanel(creditsPanel);
        }

        public void OnCloseSubPanelsClicked()
        {
            PlayClick();
            CloseAllSubPanels();
        }

        public void OnQuitClicked()
        {
            PlayClick();
            GameSettings.Save();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // --- Helpers ---

        public void ShowOnlyPanel(GameObject panelToOpen)
        {
            CloseAllSubPanels();
            if (panelToOpen != null)
                panelToOpen.SetActive(true);
        }

        public void CloseAllSubPanels()
        {
            if (loadPanel) loadPanel.SetActive(false);
            if (settingsPanel) settingsPanel.SetActive(false);
            if (creditsPanel) creditsPanel.SetActive(false);
            if (confirmNewGamePanel) confirmNewGamePanel.SetActive(false);
        }

        bool HasOpenSubPanel()
        {
            return (loadPanel && loadPanel.activeSelf) ||
                   (settingsPanel && settingsPanel.activeSelf) ||
                   (creditsPanel && creditsPanel.activeSelf) ||
                   (confirmNewGamePanel && confirmNewGamePanel.activeSelf);
        }

        void SetToast(string msg)
        {
            if (statusToastText) statusToastText.text = msg;
        }

        void PlayClick()
        {
            if (uiAudioSource && clickClip)
                uiAudioSource.PlayOneShot(clickClip, GameSettings.EffectsVolume);
        }

        void StartGameplay(BootLoadMode mode)
        {
            if (isTransitioning) return;

            if (!Application.CanStreamedLevelBeLoaded(gameplaySceneName))
            {
                SetToast($"Scene '{gameplaySceneName}' belum terdaftar di Build Settings.");
                Debug.LogWarning($"[MainMenuController] Scene '{gameplaySceneName}' is not in Build Settings.");
                return;
            }

            isTransitioning = true;
            GameSettings.PendingBootMode = mode;
            StartCoroutine(TransitionAndLoadScene());
        }

        IEnumerator TransitionAndLoadScene()
        {
            if (fadeOverlay != null)
            {
                fadeOverlay.blocksRaycasts = true;
                yield return FadeTo(1f, GameSettings.ReduceMotion ? 0f : 0.3f);
            }
            SceneManager.LoadScene(gameplaySceneName);
        }

        IEnumerator FadeTo(float targetAlpha, float duration)
        {
            if (fadeOverlay == null) yield break;
            if (duration <= 0f)
            {
                fadeOverlay.alpha = targetAlpha;
                yield break;
            }

            float start = fadeOverlay.alpha;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / duration;
                fadeOverlay.alpha = Mathf.Lerp(start, targetAlpha, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
            fadeOverlay.alpha = targetAlpha;
        }
    }
}
