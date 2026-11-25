using UnityEngine;

namespace SELA.UI
{
    public enum BootLoadMode
    {
        Default = 0,
        NewGame = 1,
        ContinueAutosave = 2,
        LoadDayStart = 3
    }

    /// <summary>
    /// Passes menu launch intent (New Game / Continue / Load Day Start) across scene loads
    /// and stores persistent user settings separately from game saves (GDD §3.13 - §3.14).
    /// </summary>
    public static class GameSettings
    {
        public static BootLoadMode PendingBootMode = BootLoadMode.Default;

        const string KeyMasterVol = "SELA_MasterVol";
        const string KeyAmbienceVol = "SELA_AmbienceVol";
        const string KeyEffectsVol = "SELA_EffectsVol";
        const string KeySensitivity = "SELA_Sensitivity";
        const string KeyInvertY = "SELA_InvertY";
        const string KeyFov = "SELA_Fov";
        const string KeyReduceMotion = "SELA_ReduceMotion";
        const string KeyTextScaleIndex = "SELA_TextScaleIndex"; // 0 = 100%, 1 = 125%, 2 = 150%
        const string KeyLowAtmosphere = "SELA_LowAtmosphere";

        public static float MasterVolume
        {
            get => PlayerPrefs.GetFloat(KeyMasterVol, 0.85f);
            set { PlayerPrefs.SetFloat(KeyMasterVol, Mathf.Clamp01(value)); AudioListener.volume = Mathf.Clamp01(value); }
        }

        public static float AmbienceVolume
        {
            get => PlayerPrefs.GetFloat(KeyAmbienceVol, 0.8f);
            set => PlayerPrefs.SetFloat(KeyAmbienceVol, Mathf.Clamp01(value));
        }

        public static float EffectsVolume
        {
            get => PlayerPrefs.GetFloat(KeyEffectsVol, 0.9f);
            set => PlayerPrefs.SetFloat(KeyEffectsVol, Mathf.Clamp01(value));
        }

        public static float MouseSensitivity
        {
            get => PlayerPrefs.GetFloat(KeySensitivity, 0.1f);
            set => PlayerPrefs.SetFloat(KeySensitivity, Mathf.Clamp(value, 0.03f, 0.35f));
        }

        public static bool InvertY
        {
            get => PlayerPrefs.GetInt(KeyInvertY, 0) == 1;
            set => PlayerPrefs.SetInt(KeyInvertY, value ? 1 : 0);
        }

        public static float FieldOfView
        {
            get => PlayerPrefs.GetFloat(KeyFov, 70f);
            set => PlayerPrefs.SetFloat(KeyFov, Mathf.Clamp(value, 60f, 90f));
        }

        public static bool ReduceMotion
        {
            get => PlayerPrefs.GetInt(KeyReduceMotion, 0) == 1;
            set => PlayerPrefs.SetInt(KeyReduceMotion, value ? 1 : 0);
        }

        /// <summary>0 = 100%, 1 = 125%, 2 = 150%.</summary>
        public static int TextScaleIndex
        {
            get => Mathf.Clamp(PlayerPrefs.GetInt(KeyTextScaleIndex, 0), 0, 2);
            set => PlayerPrefs.SetInt(KeyTextScaleIndex, Mathf.Clamp(value, 0, 2));
        }

        public static float TextScaleMultiplier => TextScaleIndex == 2 ? 1.5f : (TextScaleIndex == 1 ? 1.25f : 1.0f);

        public static bool LowAtmosphere
        {
            get => PlayerPrefs.GetInt(KeyLowAtmosphere, 0) == 1;
            set => PlayerPrefs.SetInt(KeyLowAtmosphere, value ? 1 : 0);
        }

        public static void ApplyAudio()
        {
            AudioListener.volume = MasterVolume;
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }
    }
}
