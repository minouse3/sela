using System;
using UnityEngine;
using SELA.Core;

/// <summary>
/// Scene host for SELA's pure GameLoopEngine.
/// Also applies simple Pagi / Siang / Malam directional & ambient lighting cues.
/// </summary>
public class GameLoopController : MonoBehaviour
{
    public static GameLoopController Instance { get; private set; }

    [Header("Startup")]
    [SerializeField] bool autoLoadSaveOnStart = false;

    [Header("Optional Session Lighting")]
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
        Engine = new GameLoopEngine();
        Engine.OnStateChanged += HandleStateChanged;
    }

    void Start()
    {
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

        if (autoLoadSaveOnStart && Engine.Save.HasAnySave())
            Engine.LoadAutosave(true);
        else
            Engine.StartNewGame(saveInitialCheckpoints: true);
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
        if (state == null || mainDirectionalLight == null) return;

        switch (state.session)
        {
            case SessionType.Pagi:
                mainDirectionalLight.color = morningLightColor;
                mainDirectionalLight.intensity = morningIntensity;
                break;
            case SessionType.Siang:
                mainDirectionalLight.color = noonLightColor;
                mainDirectionalLight.intensity = noonIntensity;
                break;
            case SessionType.Malam:
                mainDirectionalLight.color = nightLightColor;
                mainDirectionalLight.intensity = nightIntensity;
                break;
        }
    }
}
