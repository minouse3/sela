using UnityEngine;
using UnityEngine.Rendering;
using SELA.Core;

namespace SELA.Gameplay
{
    /// <summary>
    /// Continuously drives SELA's skybox, sun/moon arc, directional light, fog, ambient trilight,
    /// exterior clouds, stars/moon, yard lantern, neighbor windows, and indoor lamps from the
    /// exact game clock hour (06:00 - 23:00), rather than snapping between only 3 presets.
    /// </summary>
    public class EnvironmentTimeController : MonoBehaviour
    {
        [Header("Time Preview / Current Hour")]
        [Range(6f, 23f)]
        [SerializeField] float currentHour = 8f;
        [SerializeField] float transitionDuration = 0.65f;

        [Header("Core Lighting & Skybox")]
        [SerializeField] Light sunDirectionalLight;
        [SerializeField] Material proceduralSkyMaterial;

        [Header("Exterior Time-of-Day Props")]
        [SerializeField] GameObject dayCloudsGroup;
        [SerializeField] GameObject nightSkyGroup;
        [SerializeField] GameObject nightLitWindowsGroup;
        [SerializeField] Light exteriorYardLight;
        [SerializeField] float yardLightBaseIntensity = 3.2f;
        [SerializeField] Renderer exteriorYardBulbRenderer;
        [SerializeField] Renderer[] cloudRenderers;
        [SerializeField] Renderer[] nightWindowRenderers;

        [Header("Indoor Room Lights")]
        [SerializeField] Light[] indoorPointLights;
        [SerializeField] float[] indoorBaseIntensities;

        struct TimeKeyframe
        {
            public float hour;
            public Vector2 sunEuler;        // x = pitch/altitude, y = yaw/azimuth
            public Color lightColor;
            public float lightIntensity;
            public float shadowStrength;
            public float sunSize;
            public float sunConvergence;
            public float atmosphereThickness;
            public Color skyTint;
            public Color groundColor;
            public float skyExposure;
            public Color ambientSky;
            public Color ambientEquator;
            public Color ambientGround;
            public Color fogColor;
            public float fogDensity;
            public Color cloudTint;
            public float cloudsVisible;     // > 0.2 shows clouds
            public float nightFactor;       // 0 = day, 1 = night (moon, stars, yard lamp, neighbor windows)
            public float indoorLampScale;
        }

        // Continuous keyframe curve across the entire day (06:00 Dawn -> 08:00 Pagi -> 10:00 Late Morning
        // -> 12:00 Siang -> 15:00 Afternoon -> 17:00 Golden Hour Senja -> 18:00 Twilight -> 19:30+ Malam)
        static readonly TimeKeyframe[] Keyframes = new TimeKeyframe[]
        {
            // 06:00 — Subuh / Early Dawn
            new TimeKeyframe
            {
                hour = 6f,
                sunEuler = new Vector2(8f, 268f),
                lightColor = new Color(0.95f, 0.72f, 0.62f),
                lightIntensity = 0.90f,
                shadowStrength = 0.65f,
                sunSize = 0.06f,
                sunConvergence = 4.0f,
                atmosphereThickness = 1.65f,
                skyTint = new Color(0.45f, 0.48f, 0.68f),
                groundColor = new Color(0.32f, 0.27f, 0.28f),
                skyExposure = 0.92f,
                ambientSky = new Color(0.30f, 0.33f, 0.44f),
                ambientEquator = new Color(0.38f, 0.31f, 0.30f),
                ambientGround = new Color(0.18f, 0.16f, 0.16f),
                fogColor = new Color(0.76f, 0.65f, 0.62f),
                fogDensity = 0.0085f,
                cloudTint = new Color(0.95f, 0.76f, 0.70f),
                cloudsVisible = 1f,
                nightFactor = 0.35f,
                indoorLampScale = 0.85f
            },
            // 08:00 — Pagi (Warm golden morning streaming straight through East window)
            new TimeKeyframe
            {
                hour = 8f,
                sunEuler = new Vector2(22f, 258f),
                lightColor = new Color(1.00f, 0.89f, 0.72f),
                lightIntensity = 1.75f,
                shadowStrength = 0.80f,
                sunSize = 0.055f,
                sunConvergence = 4.5f,
                atmosphereThickness = 1.42f,
                skyTint = new Color(0.60f, 0.72f, 0.86f),
                groundColor = new Color(0.50f, 0.43f, 0.35f),
                skyExposure = 1.24f,
                ambientSky = new Color(0.48f, 0.52f, 0.57f),
                ambientEquator = new Color(0.46f, 0.41f, 0.35f),
                ambientGround = new Color(0.25f, 0.21f, 0.18f),
                fogColor = new Color(0.89f, 0.83f, 0.73f),
                fogDensity = 0.0055f,
                cloudTint = new Color(1.00f, 0.93f, 0.84f),
                cloudsVisible = 1f,
                nightFactor = 0f,
                indoorLampScale = 0.50f
            },
            // 10:00 — Pagi Menjelang Siang (Higher bright sun, fresher sky)
            new TimeKeyframe
            {
                hour = 10f,
                sunEuler = new Vector2(44f, 245f),
                lightColor = new Color(1.00f, 0.96f, 0.88f),
                lightIntensity = 2.00f,
                shadowStrength = 0.80f,
                sunSize = 0.045f,
                sunConvergence = 5.5f,
                atmosphereThickness = 1.05f,
                skyTint = new Color(0.44f, 0.68f, 0.93f),
                groundColor = new Color(0.46f, 0.47f, 0.39f),
                skyExposure = 1.32f,
                ambientSky = new Color(0.52f, 0.57f, 0.62f),
                ambientEquator = new Color(0.47f, 0.45f, 0.41f),
                ambientGround = new Color(0.27f, 0.24f, 0.21f),
                fogColor = new Color(0.85f, 0.89f, 0.92f),
                fogDensity = 0.0040f,
                cloudTint = new Color(0.99f, 0.98f, 0.95f),
                cloudsVisible = 1f,
                nightFactor = 0f,
                indoorLampScale = 0.30f
            },
            // 12:00 — Siang / Midday (Crisp tropical daylight)
            new TimeKeyframe
            {
                hour = 12f,
                sunEuler = new Vector2(64f, 224f),
                lightColor = new Color(1.00f, 0.99f, 0.95f),
                lightIntensity = 2.15f,
                shadowStrength = 0.78f,
                sunSize = 0.040f,
                sunConvergence = 6.0f,
                atmosphereThickness = 0.86f,
                skyTint = new Color(0.35f, 0.65f, 0.96f),
                groundColor = new Color(0.42f, 0.48f, 0.40f),
                skyExposure = 1.35f,
                ambientSky = new Color(0.55f, 0.60f, 0.65f),
                ambientEquator = new Color(0.48f, 0.47f, 0.44f),
                ambientGround = new Color(0.28f, 0.26f, 0.23f),
                fogColor = new Color(0.82f, 0.90f, 0.96f),
                fogDensity = 0.0032f,
                cloudTint = new Color(0.98f, 0.99f, 1.00f),
                cloudsVisible = 1f,
                nightFactor = 0f,
                indoorLampScale = 0.22f
            },
            // 15:00 — Sore Awal (Warming afternoon light)
            new TimeKeyframe
            {
                hour = 15f,
                sunEuler = new Vector2(40f, 206f),
                lightColor = new Color(1.00f, 0.91f, 0.76f),
                lightIntensity = 1.85f,
                shadowStrength = 0.78f,
                sunSize = 0.048f,
                sunConvergence = 5.0f,
                atmosphereThickness = 1.25f,
                skyTint = new Color(0.50f, 0.67f, 0.86f),
                groundColor = new Color(0.48f, 0.42f, 0.34f),
                skyExposure = 1.24f,
                ambientSky = new Color(0.48f, 0.51f, 0.55f),
                ambientEquator = new Color(0.48f, 0.42f, 0.35f),
                ambientGround = new Color(0.26f, 0.22f, 0.18f),
                fogColor = new Color(0.89f, 0.82f, 0.72f),
                fogDensity = 0.0048f,
                cloudTint = new Color(1.00f, 0.91f, 0.80f),
                cloudsVisible = 1f,
                nightFactor = 0f,
                indoorLampScale = 0.40f
            },
            // 17:00 — Senja / Golden Hour Sunset (Warm terracotta & amber glow!)
            new TimeKeyframe
            {
                hour = 17f,
                sunEuler = new Vector2(14f, 194f),
                lightColor = new Color(1.00f, 0.68f, 0.44f),
                lightIntensity = 1.35f,
                shadowStrength = 0.74f,
                sunSize = 0.065f,
                sunConvergence = 3.8f,
                atmosphereThickness = 1.85f,
                skyTint = new Color(0.76f, 0.49f, 0.44f),
                groundColor = new Color(0.42f, 0.30f, 0.26f),
                skyExposure = 1.05f,
                ambientSky = new Color(0.44f, 0.35f, 0.38f),
                ambientEquator = new Color(0.50f, 0.36f, 0.28f),
                ambientGround = new Color(0.22f, 0.17f, 0.15f),
                fogColor = new Color(0.84f, 0.59f, 0.47f),
                fogDensity = 0.0078f,
                cloudTint = new Color(0.98f, 0.70f, 0.55f),
                cloudsVisible = 1f,
                nightFactor = 0.25f,
                indoorLampScale = 0.85f
            },
            // 18:00 — Maghrib / Twilight Dusk (Deep plum-indigo sky, warm lamps turning ON)
            new TimeKeyframe
            {
                hour = 18f,
                sunEuler = new Vector2(22f, 250f),
                lightColor = new Color(0.62f, 0.62f, 0.86f),
                lightIntensity = 0.52f,
                shadowStrength = 0.66f,
                sunSize = 0.036f,
                sunConvergence = 7.0f,
                atmosphereThickness = 0.55f,
                skyTint = new Color(0.14f, 0.16f, 0.32f),
                groundColor = new Color(0.10f, 0.09f, 0.13f),
                skyExposure = 0.70f,
                ambientSky = new Color(0.19f, 0.21f, 0.32f),
                ambientEquator = new Color(0.24f, 0.20f, 0.24f),
                ambientGround = new Color(0.11f, 0.10f, 0.11f),
                fogColor = new Color(0.17f, 0.18f, 0.28f),
                fogDensity = 0.0105f,
                cloudTint = new Color(0.34f, 0.30f, 0.42f),
                cloudsVisible = 0.4f,
                nightFactor = 0.80f,
                indoorLampScale = 1.18f
            },
            // 20:00 — Malam (Deep indigo starlit night + warm cozy indoor sanctuary)
            new TimeKeyframe
            {
                hour = 20f,
                sunEuler = new Vector2(28f, 255f),
                lightColor = new Color(0.46f, 0.58f, 0.86f),
                lightIntensity = 0.34f,
                shadowStrength = 0.62f,
                sunSize = 0.032f,
                sunConvergence = 8.0f,
                atmosphereThickness = 0.24f,
                skyTint = new Color(0.04f, 0.07f, 0.16f),
                groundColor = new Color(0.03f, 0.04f, 0.07f),
                skyExposure = 0.54f,
                ambientSky = new Color(0.11f, 0.14f, 0.22f),
                ambientEquator = new Color(0.14f, 0.13f, 0.16f),
                ambientGround = new Color(0.08f, 0.07f, 0.08f),
                fogColor = new Color(0.06f, 0.09f, 0.16f),
                fogDensity = 0.012f,
                cloudTint = new Color(0.15f, 0.18f, 0.26f),
                cloudsVisible = 0f,
                nightFactor = 1f,
                indoorLampScale = 1.32f
            },
            // 22:00 — Larut Malam (Deepest night before sleep)
            new TimeKeyframe
            {
                hour = 22f,
                sunEuler = new Vector2(34f, 258f),
                lightColor = new Color(0.38f, 0.50f, 0.78f),
                lightIntensity = 0.26f,
                shadowStrength = 0.58f,
                sunSize = 0.030f,
                sunConvergence = 8.5f,
                atmosphereThickness = 0.18f,
                skyTint = new Color(0.025f, 0.045f, 0.11f),
                groundColor = new Color(0.02f, 0.025f, 0.05f),
                skyExposure = 0.45f,
                ambientSky = new Color(0.08f, 0.10f, 0.17f),
                ambientEquator = new Color(0.11f, 0.10f, 0.13f),
                ambientGround = new Color(0.06f, 0.06f, 0.07f),
                fogColor = new Color(0.04f, 0.06f, 0.12f),
                fogDensity = 0.0135f,
                cloudTint = new Color(0.10f, 0.12f, 0.18f),
                cloudsVisible = 0f,
                nightFactor = 1f,
                indoorLampScale = 1.38f
            }
        };

        Material runtimeSkyMaterial;
        float displayedHour = -1f;
        float targetHour = 8f;
        float startHour = 8f;
        float transitionElapsed = 1f;
        bool isTransitioning;

        public float CurrentHour => currentHour;

        void Awake()
        {
            CacheIndoorLightsIfNeeded();
            EnsureRuntimeSkyMaterial();
        }

        void OnDestroy()
        {
            if (runtimeSkyMaterial != null)
                Destroy(runtimeSkyMaterial);
        }

        void OnValidate()
        {
            if (!Application.isPlaying && isActiveAndEnabled)
                ApplyHourImmediate(currentHour);
        }

        void Update()
        {
            if (!isTransitioning) return;

            transitionElapsed += Time.unscaledDeltaTime / Mathf.Max(0.01f, transitionDuration);
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(transitionElapsed));
            float h = Mathf.Lerp(startHour, targetHour, t);
            ApplyHourImmediate(h);

            if (transitionElapsed >= 1f)
            {
                isTransitioning = false;
                ApplyHourImmediate(targetHour);
            }
        }

        /// <summary>
        /// Sets the environment to match an exact game hour (e.g. 8, 10, 12, 15, 17, 18, 20, 22).
        /// Smoothly interpolates during Play Mode when moving forward within the same day,
        /// or snaps immediately on day reset / sleep / initial load.
        /// </summary>
        public void ApplyHour(float hour, bool instant = false)
        {
            hour = Mathf.Clamp(hour, 6f, 23f);
            currentHour = hour;

            if (instant || !Application.isPlaying || displayedHour < 0f ||
                hour < displayedHour || SELA.UI.GameSettings.ReduceMotion || transitionDuration <= 0.01f)
            {
                isTransitioning = false;
                ApplyHourImmediate(hour);
                return;
            }

            if (Mathf.Approximately(hour, targetHour) && !isTransitioning)
                return;

            startHour = displayedHour;
            targetHour = hour;
            transitionElapsed = 0f;
            isTransitioning = true;
        }

        /// <summary>
        /// Convenience wrapper when only a SessionType is passed.
        /// </summary>
        public void ApplySession(SessionType session, bool force = false)
        {
            float hour = session == SessionType.Pagi ? 8f : (session == SessionType.Siang ? 13f : 20f);
            ApplyHour(hour, instant: force);
        }

        void ApplyHourImmediate(float hour)
        {
            hour = Mathf.Clamp(hour, 6f, 23f);
            displayedHour = hour;
            currentHour = hour;

            CacheIndoorLightsIfNeeded();
            EnsureRuntimeSkyMaterial();

            if (sunDirectionalLight == null)
            {
                foreach (var l in FindObjectsByType<Light>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                {
                    if (l.type == LightType.Directional) { sunDirectionalLight = l; break; }
                }
            }

            TimeKeyframe k = EvaluateKeyframes(hour);

            if (sunDirectionalLight != null)
            {
                RenderSettings.sun = sunDirectionalLight;
                sunDirectionalLight.transform.rotation = Quaternion.Euler(k.sunEuler.x, k.sunEuler.y, 0f);
                sunDirectionalLight.color = k.lightColor;
                sunDirectionalLight.intensity = k.lightIntensity;
                sunDirectionalLight.shadowStrength = k.shadowStrength;
            }

            SetSkyProperties(k.sunSize, k.sunConvergence, k.atmosphereThickness, k.skyTint, k.groundColor, k.skyExposure);

            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = k.ambientSky;
            RenderSettings.ambientEquatorColor = k.ambientEquator;
            RenderSettings.ambientGroundColor = k.ambientGround;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = k.fogColor;
            RenderSettings.fogDensity = k.fogDensity;

            ApplyExteriorState(k);
            ScaleIndoorLights(k.indoorLampScale);

            if (!Application.isPlaying)
                DynamicGI.UpdateEnvironment();
        }

        static TimeKeyframe EvaluateKeyframes(float hour)
        {
            if (hour <= Keyframes[0].hour) return Keyframes[0];
            int last = Keyframes.Length - 1;
            if (hour >= Keyframes[last].hour) return Keyframes[last];

            for (int i = 0; i < last; i++)
            {
                var a = Keyframes[i];
                var b = Keyframes[i + 1];
                if (hour >= a.hour && hour <= b.hour)
                {
                    float t = Mathf.InverseLerp(a.hour, b.hour, hour);
                    return LerpKeyframe(a, b, t);
                }
            }
            return Keyframes[0];
        }

        static TimeKeyframe LerpKeyframe(TimeKeyframe a, TimeKeyframe b, float t)
        {
            return new TimeKeyframe
            {
                hour = Mathf.Lerp(a.hour, b.hour, t),
                sunEuler = new Vector2(
                    Mathf.LerpAngle(a.sunEuler.x, b.sunEuler.x, t),
                    Mathf.LerpAngle(a.sunEuler.y, b.sunEuler.y, t)),
                lightColor = Color.Lerp(a.lightColor, b.lightColor, t),
                lightIntensity = Mathf.Lerp(a.lightIntensity, b.lightIntensity, t),
                shadowStrength = Mathf.Lerp(a.shadowStrength, b.shadowStrength, t),
                sunSize = Mathf.Lerp(a.sunSize, b.sunSize, t),
                sunConvergence = Mathf.Lerp(a.sunConvergence, b.sunConvergence, t),
                atmosphereThickness = Mathf.Lerp(a.atmosphereThickness, b.atmosphereThickness, t),
                skyTint = Color.Lerp(a.skyTint, b.skyTint, t),
                groundColor = Color.Lerp(a.groundColor, b.groundColor, t),
                skyExposure = Mathf.Lerp(a.skyExposure, b.skyExposure, t),
                ambientSky = Color.Lerp(a.ambientSky, b.ambientSky, t),
                ambientEquator = Color.Lerp(a.ambientEquator, b.ambientEquator, t),
                ambientGround = Color.Lerp(a.ambientGround, b.ambientGround, t),
                fogColor = Color.Lerp(a.fogColor, b.fogColor, t),
                fogDensity = Mathf.Lerp(a.fogDensity, b.fogDensity, t),
                cloudTint = Color.Lerp(a.cloudTint, b.cloudTint, t),
                cloudsVisible = Mathf.Lerp(a.cloudsVisible, b.cloudsVisible, t),
                nightFactor = Mathf.Lerp(a.nightFactor, b.nightFactor, t),
                indoorLampScale = Mathf.Lerp(a.indoorLampScale, b.indoorLampScale, t)
            };
        }

        void ApplyExteriorState(TimeKeyframe k)
        {
            bool showClouds = k.cloudsVisible > 0.25f;
            bool showNightSky = k.nightFactor > 0.45f;
            bool showNightWindows = k.nightFactor > 0.22f;

            if (dayCloudsGroup != null) dayCloudsGroup.SetActive(showClouds);
            if (nightSkyGroup != null) nightSkyGroup.SetActive(showNightSky);
            if (nightLitWindowsGroup != null) nightLitWindowsGroup.SetActive(showNightWindows);

            if (exteriorYardLight != null)
            {
                exteriorYardLight.enabled = k.nightFactor > 0.15f;
                exteriorYardLight.intensity = yardLightBaseIntensity * Mathf.Clamp01(k.nightFactor);
            }

            if (exteriorYardBulbRenderer != null && exteriorYardBulbRenderer.sharedMaterial != null)
            {
                var mat = Application.isPlaying ? exteriorYardBulbRenderer.material : exteriorYardBulbRenderer.sharedMaterial;
                if (mat.HasProperty("_EmissionColor"))
                    mat.SetColor("_EmissionColor", new Color(1.0f, 0.82f, 0.48f) * (2.6f * k.nightFactor));
            }

            if (showNightWindows && nightWindowRenderers != null)
            {
                Color em = new Color(1.0f, 0.80f, 0.42f) * (2.0f * Mathf.Clamp01(k.nightFactor * 1.15f));
                foreach (var r in nightWindowRenderers)
                {
                    if (r == null) continue;
                    var mat = Application.isPlaying ? r.material : r.sharedMaterial;
                    if (mat != null && mat.HasProperty("_EmissionColor"))
                        mat.SetColor("_EmissionColor", em);
                }
            }

            if (showClouds && cloudRenderers != null)
            {
                foreach (var r in cloudRenderers)
                {
                    if (r == null) continue;
                    var mat = Application.isPlaying ? r.material : r.sharedMaterial;
                    if (mat != null && mat.HasProperty("_BaseColor"))
                        mat.SetColor("_BaseColor", k.cloudTint);
                }
            }
        }

        void CacheIndoorLightsIfNeeded()
        {
            if (indoorPointLights != null && indoorPointLights.Length > 0 &&
                indoorBaseIntensities != null && indoorBaseIntensities.Length == indoorPointLights.Length)
                return;

            var list = new System.Collections.Generic.List<Light>();
            foreach (var l in FindObjectsByType<Light>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                if (l.type != LightType.Point) continue;
                if (exteriorYardLight != null && l == exteriorYardLight) continue;
                list.Add(l);
            }
            indoorPointLights = list.ToArray();
            indoorBaseIntensities = new float[indoorPointLights.Length];
            for (int i = 0; i < indoorPointLights.Length; i++)
                indoorBaseIntensities[i] = indoorPointLights[i].intensity;
        }

        void EnsureRuntimeSkyMaterial()
        {
            if (Application.isPlaying)
            {
                if (runtimeSkyMaterial == null && proceduralSkyMaterial != null)
                {
                    runtimeSkyMaterial = new Material(proceduralSkyMaterial) { name = "M_SELA_Sky_Runtime" };
                    RenderSettings.skybox = runtimeSkyMaterial;
                }
            }
            else if (proceduralSkyMaterial != null)
            {
                RenderSettings.skybox = proceduralSkyMaterial;
            }
        }

        Material ActiveSkyMaterial => Application.isPlaying && runtimeSkyMaterial != null
            ? runtimeSkyMaterial
            : proceduralSkyMaterial;

        void SetSkyProperties(float sunSize, float sunConvergence, float atmosphereThickness, Color skyTint, Color groundColor, float exposure)
        {
            var sky = ActiveSkyMaterial;
            if (sky == null) return;
            if (sky.HasProperty("_SunSize")) sky.SetFloat("_SunSize", sunSize);
            if (sky.HasProperty("_SunSizeConvergence")) sky.SetFloat("_SunSizeConvergence", sunConvergence);
            if (sky.HasProperty("_AtmosphereThickness")) sky.SetFloat("_AtmosphereThickness", atmosphereThickness);
            if (sky.HasProperty("_SkyTint")) sky.SetColor("_SkyTint", skyTint);
            if (sky.HasProperty("_GroundColor")) sky.SetColor("_GroundColor", groundColor);
            if (sky.HasProperty("_Exposure")) sky.SetFloat("_Exposure", exposure);
        }

        void ScaleIndoorLights(float multiplier)
        {
            if (indoorPointLights == null || indoorBaseIntensities == null) return;
            int count = Mathf.Min(indoorPointLights.Length, indoorBaseIntensities.Length);
            for (int i = 0; i < count; i++)
            {
                if (indoorPointLights[i] != null)
                    indoorPointLights[i].intensity = indoorBaseIntensities[i] * multiplier;
            }
        }

        [ContextMenu("Preview 08:00 (Pagi)")]      void Preview08() => ApplyHour(8f, true);
        [ContextMenu("Preview 10:00 (Pagi Terang)")] void Preview10() => ApplyHour(10f, true);
        [ContextMenu("Preview 12:00 (Siang)")]     void Preview12() => ApplyHour(12f, true);
        [ContextMenu("Preview 15:00 (Sore Awal)")] void Preview15() => ApplyHour(15f, true);
        [ContextMenu("Preview 17:00 (Senja)")]     void Preview17() => ApplyHour(17f, true);
        [ContextMenu("Preview 18:00 (Maghrib)")]   void Preview18() => ApplyHour(18f, true);
        [ContextMenu("Preview 20:00 (Malam)")]     void Preview20() => ApplyHour(20f, true);
        [ContextMenu("Preview 22:00 (Larut)")]     void Preview22() => ApplyHour(22f, true);
    }
}
