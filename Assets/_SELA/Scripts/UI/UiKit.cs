using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Pabrik komponen UI. Semua layar dibangun dari sini supaya gaya, jarak, dan font konsisten (DESIGN 9).
    /// </summary>
    public static class UiKit
    {
        private static TMP_FontAsset _regular;
        private static TMP_FontAsset _bold;

        public static TMP_FontAsset RegularFont
        {
            get
            {
                if (_regular == null)
                {
                    _regular = LoadFont("UI/Fonts/Noto Sans Regular SDF", "UI/Fonts/NotoSans-Regular");
                }
                return _regular;
            }
        }

        public static TMP_FontAsset BoldFont
        {
            get
            {
                if (_bold == null)
                {
                    _bold = LoadFont("UI/Fonts/Noto Sans Bold SDF", "UI/Fonts/NotoSans-Bold");
                    if (!HasGlyphs(_bold))
                    {
                        _bold = RegularFont;
                    }
                }
                return _bold;
            }
        }

        /// <summary>
        /// Memuat font untuk UI. Aset font hasil pembuatan batch bisa-terbawa tanpa glif
        /// (tabel karakter kosong), jadi aset dicek dulu. Kalau kosong, font dibuat ulang dari
        /// file TTF saat runtime, dan kalau itu gagal dipakai font bawaan TMP.
        /// </summary>
        private static TMP_FontAsset LoadFont(string assetPath, string sourcePath)
        {
            TMP_FontAsset asset = Resources.Load<TMP_FontAsset>(assetPath);
            if (HasGlyphs(asset))
            {
                return asset;
            }

            Font source = Resources.Load<Font>(sourcePath);
            if (source != null)
            {
                try
                {
                    TMP_FontAsset runtime = TMP_FontAsset.CreateFontAsset(source, 90, 9,
                        UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA, 1024, 1024,
                        AtlasPopulationMode.Dynamic, true);
                    runtime.name = "SELA " + source.name;
                    if (HasGlyphs(runtime))
                    {
                        return runtime;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning("[SELA UI] font runtime gagal dibuat: " + e.Message);
                }
            }

            TMP_FontAsset fallback = TMP_Settings.defaultFontAsset;
            if (fallback == null)
            {
                Debug.LogError("[SELA UI] tidak ada font yang bisa dipakai untuk UI.");
            }
            return fallback;
        }

        private static bool HasGlyphs(TMP_FontAsset asset)
        {
            return asset != null && asset.characterTable != null && asset.characterTable.Count > 0;
        }

        public static RectTransform Rect(Transform parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rt = (RectTransform)go.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return rt;
        }

        public static RectTransform Box(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            RectTransform rt = Rect(parent, name);
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
            return rt;
        }

        /// <summary>Panel kertas dengan warna latar dan sudut potong imitate kertas (DESIGN 9.3).</summary>
        public static Image PaperPanel(Transform parent, string name, Color color)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = true;
            return image;
        }

        /// <summary>Panel yang menutupi seluruh area induknya, dipakai untuk latar gelap.</summary>
        public static RectTransform Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return rect;
        }

        /// <summary>
        /// Kolom dengan lebar persentase dari panel induknya. Pakai anchor, bukan offset piksel,
        /// supaya batas kolom ikut berubah kalau ukuran panel berubah.
        /// </summary>
        public static RectTransform Column(Transform parent, string name, float startFraction, float endFraction)
        {
            RectTransform rt = Rect(parent, name);
            rt.anchorMin = new Vector2(startFraction, 0f);
            rt.anchorMax = new Vector2(endFraction, 1f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return rt;
        }

        /// <summary>Garis pembatas vertikal di antara dua kolom.</summary>
        public static Image VerticalDivider(Transform parent, float fraction)
        {
            Image image = PaperPanel(parent, "Divider", UiTheme.HijauMeja);
            RectTransform rt = image.rectTransform;
            rt.anchorMin = new Vector2(fraction, 0f);
            rt.anchorMax = new Vector2(fraction, 1f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(2f, 0f);
            image.raycastTarget = false;
            return image;
        }

        public static TextMeshProUGUI Text(Transform parent, string content, float size, Color color,
            TextAlignmentOptions align = TextAlignmentOptions.TopLeft, bool bold = false)
        {
            GameObject go = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.text = content;
            tmp.font = bold ? BoldFont : RegularFont;
            tmp.fontSize = UiTheme.Scaled(size);
            tmp.color = color;
            tmp.alignment = align;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            tmp.overflowMode = TextOverflowModes.Overflow;
            tmp.raycastTarget = false;
            tmp.richText = false;
            return tmp;
        }

        /// <summary>Teks di dalam kotak yang menempel ke kotak, dipakai untuk daftar dan paragraf panel.</summary>
        public static TextMeshProUGUI TextIn(Transform parent, string content, float size, Color color,
            TextAlignmentOptions align = TextAlignmentOptions.TopLeft, bool bold = false)
        {
            TextMeshProUGUI tmp = Text(parent, content, size, color, align, bold);
            RectTransform rt = tmp.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(0f, 0f);
            rt.offsetMax = new Vector2(0f, 0f);
            return tmp;
        }

        /// <summary>
        /// Paragraf yang tinggi{-nya} diukur dari isi teks, jadi tidak pernah menimpa
        /// isi yang ada di bawahnya. Dipakai untuk semua teks panjang.
        /// </summary>
        public static TextMeshProUGUI Paragraph(Transform parent, string content, float size, Color color,
            TextAlignmentOptions align = TextAlignmentOptions.TopLeft, bool bold = false, float minHeight = 0f)
        {
            TextMeshProUGUI tmp = TextIn(parent, content, size, color, align, bold);
            tmp.overflowMode = TextOverflowModes.Overflow;
            TmpAutoHeight auto = tmp.gameObject.AddComponent<TmpAutoHeight>();
            auto.MinHeight = minHeight;
            auto.BottomPadding = 6f;
            return tmp;
        }

        public static Button Button(Transform parent, string label, Action onClick, Color background, Color textColor)
        {
            Image bg = PaperPanel(parent, "Button", background);
            Button button = bg.gameObject.AddComponent<Button>();
            button.targetGraphic = bg;

            TextMeshProUGUI tmp = TextIn(bg.transform, label, UiTheme.BodyFontSize, textColor, TextAlignmentOptions.Center, true);
            tmp.rectTransform.offsetMin = new Vector2(18f, 8f);
            tmp.rectTransform.offsetMax = new Vector2(-18f, -8f);

            // Label tombol selalu satu baris. Kalau ruangnya kurang, ukuran huruf dikecilkan
            // otomatis supaya tombol tidak pernah menulis label ke dua baris (DESIGN 9.3).
            tmp.textWrappingMode = TextWrappingModes.NoWrap;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 15f;
            tmp.fontSizeMax = UiTheme.Scaled(UiTheme.BodyFontSize);

            button.colors = new ColorBlock
            {
                normalColor = Color.white,
                highlightedColor = new Color(1f, 1f, 1f, 0.92f),
                pressedColor = new Color(0.86f, 0.86f, 0.86f, 1f),
                selectedColor = Color.white,
                disabledColor = new Color(0.55f, 0.55f, 0.55f, 0.65f),
                colorMultiplier = 1f,
                fadeDuration = 0.08f
            };

            if (onClick != null)
            {
                button.onClick.AddListener(() => onClick());
            }

            Height(button, 44f);
            MakeFocusable(tmp, UiTheme.KuningKusam);
            return button;
        }

        /// <summary>Pilihan jawaban panjang: teks dibungkus dan rata kiri, bukan label tombol pendek.</summary>
        public static Button Choice(Transform parent, string label, Action onClick, Color background,
            Color textColor, float height = 92f)
        {
            Button button = Button(parent, label, onClick, background, textColor);
            TextMeshProUGUI tmp = button.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.alignment = TextAlignmentOptions.MidlineLeft;
                tmp.textWrappingMode = TextWrappingModes.Normal;
                tmp.overflowMode = TextOverflowModes.Truncate;
                tmp.fontSize = UiTheme.Scaled(UiTheme.SmallFontSize);
                tmp.enableAutoSizing = true;
                tmp.fontSizeMin = 12f;
                tmp.fontSizeMax = UiTheme.Scaled(UiTheme.SmallFontSize);
                tmp.rectTransform.offsetMin = new Vector2(18f, 10f);
                tmp.rectTransform.offsetMax = new Vector2(-18f, -10f);
            }
            Height(button, height);
            return button;
        }

        /// <summary>Lebar minimum tombol mengikuti panjang label terpanjangnya.</summary>
        public static float LabelWidth(string label)
        {
            string longest = string.Empty;
            string[] lines = label.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > longest.Length)
                {
                    longest = lines[i];
                }
            }
            return Mathf.Max(96f, longest.Length * UiTheme.Scaled(UiTheme.BodyFontSize) * 0.58f + 40f);
        }

        /// <summary>
        /// Tombol daftar di kolom samping: label rata kiri, dua baris paling banyak,
        /// dan tinggi otomatis mengikuti isi. Dipakai di daftar perusahaan dan pesan.
        /// </summary>
        public static Button ListButton(Transform parent, string title, string subtitle, bool selected, Action onClick)
        {
            Image bg = PaperPanel(parent, "ListItem", selected ? UiTheme.KuningKusam : UiTheme.LatarSlot);
            Button button = bg.gameObject.AddComponent<Button>();
            button.targetGraphic = bg;

            RectTransform rt = bg.rectTransform;
            rt.sizeDelta = new Vector2(0f, 56f);

            TextMeshProUGUI head = TextIn(rt, title, UiTheme.BodyFontSize, UiTheme.TextOnPaper,
                TextAlignmentOptions.TopLeft, true);
            head.rectTransform.anchorMin = new Vector2(0f, 1f);
            head.rectTransform.anchorMax = new Vector2(1f, 1f);
            head.rectTransform.pivot = new Vector2(0.5f, 1f);
            head.rectTransform.offsetMin = new Vector2(14f, -30f);
            head.rectTransform.offsetMax = new Vector2(-14f, -6f);
            head.textWrappingMode = TextWrappingModes.NoWrap;
            head.enableAutoSizing = true;
            head.fontSizeMin = 13f;
            head.fontSizeMax = UiTheme.Scaled(UiTheme.BodyFontSize);

            if (!string.IsNullOrEmpty(subtitle))
            {
                TextMeshProUGUI sub = TextIn(rt, subtitle, UiTheme.SmallFontSize, UiTheme.TextOnPaper,
                    TextAlignmentOptions.TopLeft);
                sub.rectTransform.anchorMin = new Vector2(0f, 0f);
                sub.rectTransform.anchorMax = new Vector2(1f, 1f);
                sub.rectTransform.offsetMin = new Vector2(14f, 6f);
                sub.rectTransform.offsetMax = new Vector2(-14f, -30f);
                sub.textWrappingMode = TextWrappingModes.NoWrap;
                sub.enableAutoSizing = true;
                sub.fontSizeMin = 12f;
                sub.fontSizeMax = UiTheme.Scaled(UiTheme.SmallFontSize);
            }

            button.colors = new ColorBlock
            {
                normalColor = Color.white,
                highlightedColor = new Color(1f, 1f, 1f, 0.92f),
                pressedColor = new Color(0.86f, 0.86f, 0.86f, 1f),
                selectedColor = Color.white,
                disabledColor = new Color(0.55f, 0.55f, 0.55f, 0.65f),
                colorMultiplier = 1f,
                fadeDuration = 0.08f
            };

            if (onClick != null)
            {
                button.onClick.AddListener(() => onClick());
            }

            LayoutElement element = rt.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 56f;
            element.preferredHeight = 56f;
            MakeFocusable(head, UiTheme.KuningKusam);
            return button;
        }

        /// <summary>Memberi garis fokus tebal pada elemen yang sedang dipilih (DESIGN 9.1).</summary>
        public static void MakeFocusable(Graphic target, Color focusColor)
        {
            UiFocusRing ring = target.gameObject.AddComponent<UiFocusRing>();
            ring.Setup(focusColor);
        }

        /// <summary>Daftar tombol dengan tinggi tetap, untuk menu dan daftar tab.</summary>
        public static VerticalLayoutGroup VerticalStack(RectTransform parent, float spacing, RectOffset padding)
        {
            VerticalLayoutGroup group = parent.GetComponent<VerticalLayoutGroup>();
            if (group == null)
            {
                group = parent.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            group.spacing = spacing;
            group.padding = padding;
            group.childControlHeight = true;
            group.childControlWidth = true;
            group.childForceExpandHeight = false;
            group.childForceExpandWidth = true;
            group.childAlignment = TextAnchor.UpperLeft;
            return group;
        }

        /// <summary>Daftar tombol mendatar, dipakai untuk baris tombol footer.</summary>
        public static HorizontalLayoutGroup HorizontalStack(RectTransform parent, float spacing, RectOffset padding)
        {
            HorizontalLayoutGroup group = parent.GetComponent<HorizontalLayoutGroup>();
            if (group == null)
            {
                group = parent.gameObject.AddComponent<HorizontalLayoutGroup>();
            }
            group.spacing = spacing;
            group.padding = padding;
            group.childControlHeight = true;
            group.childControlWidth = true;
            group.childForceExpandHeight = true;
            group.childForceExpandWidth = true;
            group.childAlignment = TextAnchor.MiddleCenter;
            return group;
        }

        public static LayoutElement Height(Component target, float height)
        {
            LayoutElement element = target.GetComponent<LayoutElement>();
            if (element == null)
            {
                element = target.gameObject.AddComponent<LayoutElement>();
            }
            element.minHeight = height;
            element.preferredHeight = height;
            return element;
        }

        /// <summary>Lebar tetap untuk tombol yang diletakkan di baris mendatar.</summary>
        public static LayoutElement Width(Component target, float width)
        {
            LayoutElement element = target.GetComponent<LayoutElement>();
            if (element == null)
            {
                element = target.gameObject.AddComponent<LayoutElement>();
            }
            element.minWidth = width;
            element.preferredWidth = width;
            return element;
        }

        /// <summary>Area isi yang dapat digulir (DESIGN 9.4).</summary>
        public static ScrollRect Scroll(Transform parent, out RectTransform content)
        {
            GameObject scrollGo = new GameObject("Scroll", typeof(RectTransform), typeof(ScrollRect));
            scrollGo.transform.SetParent(parent, false);
            ScrollRect scroll = scrollGo.GetComponent<ScrollRect>();

            RectTransform scrollRect = (RectTransform)scrollGo.transform;
            scrollRect.anchorMin = Vector2.zero;
            scrollRect.anchorMax = Vector2.one;
            scrollRect.offsetMin = Vector2.zero;
            scrollRect.offsetMax = Vector2.zero;

            GameObject viewportGo = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D));
            viewportGo.transform.SetParent(scrollGo.transform, false);
            RectTransform viewport = (RectTransform)viewportGo.transform;
            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.offsetMin = Vector2.zero;
            // Sisakan ruang untuk batang gulir supaya teks tidak tertimpa (DESIGN 9.3).
            viewport.offsetMax = new Vector2(-14f, 0f);

            GameObject contentGo = new GameObject("Content", typeof(RectTransform));
            contentGo.transform.SetParent(viewport, false);
            content = (RectTransform)contentGo.transform;
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.offsetMin = new Vector2(0f, 0f);
            content.offsetMax = new Vector2(0f, 0f);

            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 40f;

            VerticalLayoutGroup group = content.gameObject.AddComponent<VerticalLayoutGroup>();
            group.spacing = 10f;
            group.padding = new RectOffset(0, 0, 0, 0);
            group.childControlHeight = true;
            group.childControlWidth = true;
            group.childForceExpandHeight = false;
            group.childForceExpandWidth = true;

            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            AddScrollbar(scroll, scrollGo.transform, content, viewport, scrollGo);

            return scroll;
        }

        /// <summary>
        /// Menempelkan scrollbar tipis di tepi kanan area isi. Batangnya digambar sendiri
        /// oleh ScrollBarDriver, jadi ScrollRect tidak ikut mengubah ukurannya.
        /// </summary>
        private static void AddScrollbar(ScrollRect scroll, Transform parent, RectTransform content,
            RectTransform viewport, GameObject scrollObject)
        {
            GameObject barGo = new GameObject("Scrollbar", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            barGo.transform.SetParent(parent, false);
            Image barImage = barGo.GetComponent<Image>();
            barImage.color = new Color(0f, 0f, 0f, 0f);
            barImage.raycastTarget = false;

            RectTransform barRect = (RectTransform)barGo.transform;
            barRect.anchorMin = new Vector2(1f, 0f);
            barRect.anchorMax = new Vector2(1f, 1f);
            barRect.pivot = new Vector2(1f, 0.5f);
            barRect.sizeDelta = new Vector2(6f, 0f);
            barRect.anchoredPosition = Vector2.zero;

            GameObject handleGo = new GameObject("Handle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            handleGo.transform.SetParent(barGo.transform, false);
            Image handleImage = handleGo.GetComponent<Image>();
            handleImage.color = UiTheme.HijauMeja;
            handleImage.raycastTarget = true;

            RectTransform handleRect = (RectTransform)handleGo.transform;
            handleRect.sizeDelta = new Vector2(6f, 60f);

            scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;

            // Sisakan ruang di kanan supaya teks tidak menempel pada scrollbar,
            // dan ruang di kiri supaya tidak menyentuh panel sebelahnya.
            content.offsetMin = new Vector2(14f, 0f);
            content.offsetMax = new Vector2(-10f, 0f);

            ScrollBarDriver driver = scrollObject.AddComponent<ScrollBarDriver>();
            driver.Track = barRect;
            driver.Thumb = handleRect;
        }

        /// <summary>
        /// Baris tombol aksi. Tombolnya memakai tinggi alaminya, tidak diregangkan
        /// setinggi baris, supaya tidak terlihat berlebihan.
        /// </summary>
        public static HorizontalLayoutGroup ActionRow(RectTransform parent, float spacing, RectOffset padding)
        {
            HorizontalLayoutGroup group = HorizontalStack(parent, spacing, padding);
            group.childForceExpandHeight = false;
            group.childAlignment = TextAnchor.MiddleLeft;
            return group;
        }

        /// <summary>Baris kosong sebagai pemisah di dalam daftar.</summary>
        public static RectTransform Spacer(Transform parent, float height)
        {
            RectTransform rt = Rect(parent, "Spacer");
            rt.sizeDelta = new Vector2(0f, height);
            LayoutElement element = rt.gameObject.AddComponent<LayoutElement>();
            element.minHeight = height;
            element.preferredHeight = height;
            return rt;
        }

        /// <summary>Baris pemisah tipis.</summary>
        public static Image Divider(Transform parent, Color color, float thickness = 2f)
        {
            GameObject go = new GameObject("Divider", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;

            RectTransform rt = (RectTransform)go.transform;
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(0f, thickness);
            rt.anchoredPosition = new Vector2(0f, -78f);

            LayoutElement element = go.AddComponent<LayoutElement>();
            element.minHeight = thickness;
            element.preferredHeight = thickness;
            return image;
        }

        /// <summary>Empat kotak Tenaga (DESIGN 9.4).</summary>
        public static RectTransform EnergyBoxes(Transform parent, int value, int max)
        {
            RectTransform row = Rect(parent, "Tenaga");
            row.anchorMin = new Vector2(0f, 0f);
            row.anchorMax = new Vector2(0f, 0f);
            row.pivot = new Vector2(0f, 0f);
            row.anchoredPosition = Vector2.zero;

            for (int i = 0; i < max; i++)
            {
                GameObject go = new GameObject("Box" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                go.transform.SetParent(row, false);
                RectTransform rt = (RectTransform)go.transform;
                rt.anchorMin = new Vector2(0f, 0.5f);
                rt.anchorMax = new Vector2(0f, 0.5f);
                rt.pivot = new Vector2(0f, 0.5f);
                rt.sizeDelta = new Vector2(22f, 22f);
                rt.anchoredPosition = new Vector2(i * 28f, 0f);
                Image image = go.GetComponent<Image>();
                image.color = i < value ? UiTheme.TanahTirai : UiTheme.LatarSlot;
                image.raycastTarget = false;
            }

            return row;
        }

        /// <summary>Enam garis Tekanan (DESIGN 9.4).</summary>
        public static RectTransform PressureBars(Transform parent, int value, int max)
        {
            RectTransform row = Rect(parent, "Tekanan");
            row.anchorMin = new Vector2(0f, 0f);
            row.anchorMax = new Vector2(0f, 0f);
            row.pivot = new Vector2(0f, 0f);
            row.anchoredPosition = Vector2.zero;

            for (int i = 0; i < max; i++)
            {
                GameObject go = new GameObject("Bar" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                go.transform.SetParent(row, false);
                RectTransform rt = (RectTransform)go.transform;
                rt.anchorMin = new Vector2(0f, 0.5f);
                rt.anchorMax = new Vector2(0f, 0.5f);
                rt.pivot = new Vector2(0f, 0.5f);
                rt.sizeDelta = new Vector2(6f, 26f);
                rt.anchoredPosition = new Vector2(i * 12f, 0f);
                Image image = go.GetComponent<Image>();
                image.color = i < value ? UiTheme.MerahPeringatan : UiTheme.LatarSlot;
                image.raycastTarget = false;
            }

            return row;
        }

        /// <summary>Kartu bukti dengan sudut terlipat, dipakai pada papan bukti dan CV.</summary>
        public static RectTransform EvidenceCard(Transform parent, string title, string body, bool owned)
        {
            Image bg = PaperPanel(parent, "Card", owned ? UiTheme.LatarKartu : UiTheme.LatarSlot);
            RectTransform rt = bg.rectTransform;
            rt.sizeDelta = new Vector2(0f, 150f);

            TextMeshProUGUI titleText = TextIn(rt, title, UiTheme.SectionFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft, true);
            titleText.rectTransform.anchorMin = new Vector2(0f, 1f);
            titleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            titleText.rectTransform.pivot = new Vector2(0.5f, 1f);
            titleText.rectTransform.offsetMin = new Vector2(16f, -46f);
            titleText.rectTransform.offsetMax = new Vector2(-16f, -12f);

            TextMeshProUGUI bodyText = TextIn(rt, body, UiTheme.SmallFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            bodyText.rectTransform.anchorMin = new Vector2(0f, 0f);
            bodyText.rectTransform.anchorMax = new Vector2(1f, 1f);
            bodyText.rectTransform.offsetMin = new Vector2(16f, 12f);
            bodyText.rectTransform.offsetMax = new Vector2(-16f, -50f);

            LayoutElement element = rt.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 150f;
            element.preferredHeight = 150f;
            return rt;
        }

        /// <summary>
        /// Kartu Meja Bukti yang bisa dipilih. Yang terpilih ditandai garis tebal dan label
        /// "Dipilih", bukan hanya perubahan warna (DESIGN 9.2).
        /// </summary>
        public static RectTransform SelectableCard(Transform parent, string title, string body,
            bool owned, bool selected, Action onClick, float height = 150f)
        {
            Image frame = PaperPanel(parent, "Card", selected ? UiTheme.ArangHijau : UiTheme.HijauMeja);
            RectTransform rt = frame.rectTransform;
            rt.sizeDelta = new Vector2(0f, height);

            Image inner = PaperPanel(rt, "Inner", owned ? UiTheme.LatarKartu : UiTheme.LatarSlot);
            RectTransform innerRect = inner.rectTransform;
            innerRect.anchorMin = Vector2.zero;
            innerRect.anchorMax = Vector2.one;
            innerRect.offsetMin = new Vector2(3f, 3f);
            innerRect.offsetMax = new Vector2(-3f, -3f);
            inner.raycastTarget = false;

            TextMeshProUGUI titleText = TextIn(innerRect, title, UiTheme.SectionFontSize, UiTheme.TextOnPaper,
                TextAlignmentOptions.TopLeft, true);
            titleText.rectTransform.anchorMin = new Vector2(0f, 1f);
            titleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            titleText.rectTransform.pivot = new Vector2(0.5f, 1f);
            titleText.rectTransform.offsetMin = new Vector2(16f, -46f);
            titleText.rectTransform.offsetMax = new Vector2(-16f, -12f);

            TextMeshProUGUI bodyText = TextIn(innerRect, body, UiTheme.SmallFontSize, UiTheme.TextOnPaper,
                TextAlignmentOptions.TopLeft);
            bodyText.rectTransform.anchorMin = Vector2.zero;
            bodyText.rectTransform.anchorMax = Vector2.one;
            bodyText.rectTransform.offsetMin = new Vector2(16f, 12f);
            bodyText.rectTransform.offsetMax = new Vector2(-16f, -50f);

            if (selected)
            {
                TextMeshProUGUI marker = TextIn(innerRect, "Dipilih", UiTheme.SmallFontSize, UiTheme.KuningKusam,
                    TextAlignmentOptions.TopRight, true);
                marker.rectTransform.anchorMin = new Vector2(1f, 1f);
                marker.rectTransform.anchorMax = new Vector2(1f, 1f);
                marker.rectTransform.pivot = new Vector2(1f, 1f);
                marker.rectTransform.sizeDelta = new Vector2(120f, 24f);
                marker.rectTransform.anchoredPosition = new Vector2(-16f, -10f);
            }

            Button button = frame.gameObject.AddComponent<Button>();
            button.targetGraphic = frame;
            if (onClick != null)
            {
                button.onClick.AddListener(() => onClick());
            }

            LayoutElement element = rt.gameObject.AddComponent<LayoutElement>();
            element.minHeight = height;
            element.preferredHeight = height;
            return rt;
        }

        /// <summary>Panel modal yang menutupi layar dan memblokir panel di belakangnya (DESIGN 9.4).</summary>
        public static RectTransform ModalLayer(Transform parent, string name)
        {
            Image bg = PaperPanel(parent, name, UiTheme.LatarGelap);
            RectTransform rt = bg.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return rt;
        }

        /// <summary>Judul bagian dengan garis pemisah.</summary>
        public static TextMeshProUGUI SectionTitle(Transform parent, string title, Color color)
        {
            TextMeshProUGUI tmp = Text(parent, title, UiTheme.SectionFontSize, color, TextAlignmentOptions.Left, true);
            LayoutElement element = tmp.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 34f;
            element.preferredHeight = 34f;
            return tmp;
        }

        /// <summary>Baris label dan nilai, dipakai pada pengaturan dan panel verifikasi.</summary>
        public static List<Button> Buttons(Transform parent, IEnumerable<string> labels, Action<int> onPick,
            Color background, Color textColor)
        {
            var result = new List<Button>();
            int index = 0;
            foreach (string label in labels)
            {
                int captured = index;
                Button button = Button(parent, label, () => onPick(captured), background, textColor);
                result.Add(button);
                index++;
            }
            return result;
        }
    }
}
