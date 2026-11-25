using System;
using UnityEngine;
using SELA.Core;
using SELA.Gameplay;
using SELA.UI;

/// <summary>
/// Scene host for SELA's pure GameLoopEngine.
/// Also applies Pagi / Siang / Malam skybox, directional, ambient & exterior lighting cues.
/// </summary>
public class GameLoopController : MonoBehaviour
{
    public static GameLoopController Instance { get; private set; }

    [Header("Startup")]
    [SerializeField] bool autoLoadSaveOnStart = false;

    [Header("Optional Session Lighting")]
    [SerializeField] EnvironmentTimeController environmentTimeController;
    [SerializeField] Light mainDirectionalLight;
    [SerializeField] Color morningLightColor = new Color(1.00f, 0.92f, 0.78f);
    [SerializeField] float morningIntensity = 1.6f;
    [SerializeField] Color noonLightColor = new Color(0.98f, 0.98f, 0.96f);
    [SerializeField] float noonIntensity = 2.0f;
    [SerializeField] Color nightLightColor = new Color(0.55f, 0.65f, 0.85f);
    [SerializeField] float nightIntensity = 0.45f;

    public GameLoopEngine Engine { get; private set; }
    public GameState State => Engine?.State;

    public event Action<string> OnWorldFocusRequested;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        GameSettings.ApplyAudio();
        Engine = new GameLoopEngine();
        Engine.OnStateChanged += HandleStateChanged;
    }

    void Start()
    {
        if (environmentTimeController == null)
            environmentTimeController = GetComponent<EnvironmentTimeController>() ?? FindFirstObjectByType<EnvironmentTimeController>();

        if (mainDirectionalLight == null)
        {
            foreach (var l in FindObjectsByType<Light>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                if (l.type == LightType.Directional)
                {
                    mainDirectionalLight = l;
                    break;
                }
            }
        }

        BootLoadMode boot = GameSettings.PendingBootMode;
        GameSettings.PendingBootMode = BootLoadMode.Default;

        if (boot == BootLoadMode.NewGame)
            Engine.StartNewGame(saveInitialCheckpoints: true);
        else if (boot == BootLoadMode.ContinueAutosave && Engine.Save.HasAnySave())
            Engine.LoadAutosave(true);
        else if (boot == BootLoadMode.LoadDayStart && Engine.Save.HasDayStartCheckpoint())
            Engine.LoadDayStartCheckpoint();
        else if (autoLoadSaveOnStart && Engine.Save.HasAnySave())
            Engine.LoadAutosave(true);
        else
            Engine.StartNewGame(saveInitialCheckpoints: !Engine.Save.HasAnySave());
    }

    void OnDestroy()
    {
        if (Engine != null)
            Engine.OnStateChanged -= HandleStateChanged;
        if (Instance == this)
            Instance = null;
    }

    public void RequestWorldFocus(string stationId)
    {
        OnWorldFocusRequested?.Invoke(stationId);
    }

    void HandleStateChanged(GameState state)
    {
        ApplySessionLighting(state);
    }

    void ApplySessionLighting(GameState state)
    {
        if (state == null) return;

        if (environmentTimeController != null)
        {
            environmentTimeController.ApplyHour(state.currentHour);
            return;
        }

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;

        switch (state.session)
        {
            case SessionType.Pagi:
                if (mainDirectionalLight != null)
                {
                    mainDirectionalLight.color = morningLightColor;
                    mainDirectionalLight.intensity = morningIntensity;
                    mainDirectionalLight.transform.rotation = Quaternion.Euler(36f, 238f, 0f);
                }
                RenderSettings.ambientSkyColor = new Color(0.46f, 0.50f, 0.54f, 1f);
                RenderSettings.ambientEquatorColor = new Color(0.42f, 0.39f, 0.35f, 1f);
                RenderSettings.ambientGroundColor = new Color(0.24f, 0.21f, 0.18f, 1f);
                break;
            case SessionType.Siang:
                if (mainDirectionalLight != null)
                {
                    mainDirectionalLight.color = noonLightColor;
                    mainDirectionalLight.intensity = noonIntensity;
                    mainDirectionalLight.transform.rotation = Quaternion.Euler(52f, 225f, 0f);
                }
                RenderSettings.ambientSkyColor = new Color(0.54f, 0.57f, 0.60f, 1f);
                RenderSettings.ambientEquatorColor = new Color(0.46f, 0.44f, 0.40f, 1f);
                RenderSettings.ambientGroundColor = new Color(0.27f, 0.24f, 0.21f, 1f);
                break;
            case SessionType.Malam:
                if (mainDirectionalLight != null)
                {
                    mainDirectionalLight.color = nightLightColor;
                    mainDirectionalLight.intensity = nightIntensity;
                    mainDirectionalLight.transform.rotation = Quaternion.Euler(28f, 250f, 0f);
                }
                RenderSettings.ambientSkyColor = new Color(0.16f, 0.20f, 0.28f, 1f);
                RenderSettings.ambientEquatorColor = new Color(0.20f, 0.19f, 0.21f, 1f);
                RenderSettings.ambientGroundColor = new Color(0.12f, 0.11f, 0.11f, 1f);
                break;
        }
    }
}
