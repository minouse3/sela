using UnityEngine;

namespace SELA.UI
{
    /// <summary>
    /// Palet, tipografi, dan jarak layar sesuai DESIGN 2 dan DESIGN 9.
    /// </summary>
    public static class UiTheme
    {
        public static readonly Color KertasBeras = new Color32(0xE9, 0xDD, 0xC4, 0xFF);
        public static readonly Color HijauMeja = new Color32(0x71, 0x85, 0x79, 0xFF);
        public static readonly Color TanahTirai = new Color32(0xAE, 0x74, 0x62, 0xFF);
        public static readonly Color BiruTinta = new Color32(0x59, 0x6C, 0x80, 0xFF);
        public static readonly Color ArangHijau = new Color32(0x30, 0x3E, 0x38, 0xFF);
        public static readonly Color KuningKusam = new Color32(0xC2, 0xA2, 0x62, 0xFF);

        /// <summary>Redup dipakai hanya untuk peringatan kontrak, selalu diikuti teks Berisiko (DESIGN 2).</summary>
        public static readonly Color MerahPeringatan = new Color32(0xA3, 0x40, 0x38, 0xFF);

        public static readonly Color LatarGelap = new Color32(0x1C, 0x24, 0x21, 0xE6);
        public static readonly Color LatarPanel = new Color32(0xE9, 0xDD, 0xC4, 0xF2);
        public static readonly Color LatarKartu = new Color32(0xF2, 0xE9, 0xD8, 0xFF);
        public static readonly Color LatarSlot = new Color32(0xCF, 0xC1, 0xA6, 0xFF);

        public const float ReferenceWidth = 1280f;
        public const float ReferenceHeight = 720f;

        /// <summary>Margin aman HUD minimal 32 px pada kanvas acuan 1280 x 720 (DESIGN 9.4).</summary>
        public const float HudMargin = 32f;

        /// <summary>Margin luar panel fokus 48 px (DESIGN 9.4).</summary>
        public const float PanelMargin = 48f;

        public const float PanelWidth = ReferenceWidth - PanelMargin * 2f;
        public const float PanelHeight = ReferenceHeight - PanelMargin * 2f;

        public const float BodyFontSize = 23f;
        public const float SmallFontSize = 19f;

        /// <summary>Ukuran teks untuk dokumen panjang, supaya seluruh lowongan terbaca tanpa banyak menggulir.</summary>
        public const float DocumentFontSize = 19f;

        public const float TitleFontSize = 36f;
        public const float SectionFontSize = 26f;

        public const float FocusOutlineWidth = 3f;
        public const float CornerRadius = 0f;

        /// <summary>Skala ukuran teks mengikuti pilihan keterbacaan (DESIGN 9.15).</summary>
        public static float TextScale
        {
            get { return UiSettings.TextScale; }
        }

        public static float Scaled(float size)
        {
            return size * TextScale;
        }

        public static Color TextOnPaper
        {
            get { return ArangHijau; }
        }

        public static Color TextOnInk
        {
            get { return KertasBeras; }
        }
    }
}
