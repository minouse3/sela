using System;
using UnityEngine;

namespace SELA.UI
{
    /// <summary>
    /// Pengaturan disimpan terpisah dari simpanan permainan (GDD 3.13).
    /// </summary>
    public static class UiSettings
    {
        private const string KeyMaster = "sela.vol.master";
        private const string KeyAmbience = "sela.vol.ambience";
        private const string KeyEffects = "sela.vol.effects";
        private const string KeySensitivity = "sela.cam.sensitivity";
        private const string KeyInvertY = "sela.cam.invertY";
        private const string KeyFov = "sela.cam.fov";
        private const string KeyReduceMotion = "sela.motion.reduce";
        private const string KeyTextScale = "sela.read.textScale";
        private const string KeyAtmosphere = "sela.read.atmosphereLow";

        public static float MasterVolume
        {
            get { return PlayerPrefs.GetFloat(KeyMaster, 0.8f); }
            set { PlayerPrefs.SetFloat(KeyMaster, Mathf.Clamp01(value)); }
        }

        public static float AmbienceVolume
        {
            get { return PlayerPrefs.GetFloat(KeyAmbience, 0.8f); }
            set { PlayerPrefs.SetFloat(KeyAmbience, Mathf.Clamp01(value)); }
        }

        public static float EffectsVolume
        {
            get { return PlayerPrefs.GetFloat(KeyEffects, 0.8f); }
            set { PlayerPrefs.SetFloat(KeyEffects, Mathf.Clamp01(value)); }
        }

        public static float Sensitivity
        {
            get { return PlayerPrefs.GetFloat(KeySensitivity, 1f); }
            set { PlayerPrefs.SetFloat(KeySensitivity, Mathf.Clamp(value, 0.2f, 3f)); }
        }

        public static bool InvertY
        {
            get { return PlayerPrefs.GetInt(KeyInvertY, 0) == 1; }
            set { PlayerPrefs.SetInt(KeyInvertY, value ? 1 : 0); }
        }

        public static float FieldOfView
        {
            get { return PlayerPrefs.GetFloat(KeyFov, 70f); }
            set { PlayerPrefs.SetFloat(KeyFov, Mathf.Clamp(value, 60f, 90f)); }
        }

        public static bool ReduceMotion
        {
            get { return PlayerPrefs.GetInt(KeyReduceMotion, 0) == 1; }
            set { PlayerPrefs.SetInt(KeyReduceMotion, value ? 1 : 0); }
        }

        /// <summary>0 = 100 persen, 1 = 125 persen, 2 = 150 persen (DESIGN 9.15).</summary>
        public static int TextScaleIndex
        {
            get { return PlayerPrefs.GetInt(KeyTextScale, 0); }
            set { PlayerPrefs.SetInt(KeyTextScale, Mathf.Clamp(value, 0, 2)); }
        }

        public static float TextScale
        {
            get
            {
                switch (TextScaleIndex)
                {
                    case 1: return 1.25f;
                    case 2: return 1.5f;
                    default: return 1f;
                }
            }
        }

        public static bool AtmosphereLow
        {
            get { return PlayerPrefs.GetInt(KeyAtmosphere, 0) == 1; }
            set { PlayerPrefs.SetInt(KeyAtmosphere, value ? 1 : 0); }
        }

        public static event Action Changed;

        public static void NotifyChanged()
        {
            PlayerPrefs.Save();
            if (Changed != null)
            {
                Changed();
            }
        }

        public static void ResetToDefault()
        {
            PlayerPrefs.DeleteKey(KeyMaster);
            PlayerPrefs.DeleteKey(KeyAmbience);
            PlayerPrefs.DeleteKey(KeyEffects);
            PlayerPrefs.DeleteKey(KeySensitivity);
            PlayerPrefs.DeleteKey(KeyInvertY);
            PlayerPrefs.DeleteKey(KeyFov);
            PlayerPrefs.DeleteKey(KeyReduceMotion);
            PlayerPrefs.DeleteKey(KeyTextScale);
            PlayerPrefs.DeleteKey(KeyAtmosphere);
            PlayerPrefs.Save();
            NotifyChanged();
        }
    }
}
