using System.Collections.Generic;
using SELA.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Panel pengaturan: Audio, Kamera, dan Keterbacaan dalam satu area scroll (DESIGN 9.15).
    /// </summary>
    public class SettingsScreen : UiScreen
    {
        private UiSheet _sheet;
        private readonly Dictionary<string, TextMeshProUGUI> _values = new Dictionary<string, TextMeshProUGUI>();
        private readonly List<Button> _textScaleButtons = new List<Button>();
        private readonly List<Button> _atmosphereButtons = new List<Button>();
        private TextMeshProUGUI _invertLabel;
        private TextMeshProUGUI _motionLabel;

        protected override string ScreenName
        {
            get { return "Settings"; }
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, SelaContent.Ui.SettingsTitle, new Vector2(720f, 600f), true);

            RectTransform content;
            UiKit.Scroll(_sheet.Content, out content);
            UiKit.VerticalStack(content, 8f, new RectOffset(0, 0, 0, 0));

            UiKit.SectionTitle(content, SelaContent.Ui.SettingsAudio, UiTheme.ArangHijau);
            SliderRow(content, SelaContent.Ui.VolumeMaster, () => UiSettings.MasterVolume,
                v => UiSettings.MasterVolume = v);
            SliderRow(content, SelaContent.Ui.VolumeAmbience, () => UiSettings.AmbienceVolume,
                v => UiSettings.AmbienceVolume = v);
            SliderRow(content, SelaContent.Ui.VolumeEffects, () => UiSettings.EffectsVolume,
                v => UiSettings.EffectsVolume = v);

            UiKit.Divider(content, UiTheme.HijauMeja);

            UiKit.SectionTitle(content, SelaContent.Ui.SettingsCamera, UiTheme.ArangHijau);
            SliderRow(content, SelaContent.Ui.Sensitivity, () => UiSettings.Sensitivity,
                v => UiSettings.Sensitivity = v);
            StepperRow(content, SelaContent.Ui.FieldOfView, () => UiSettings.FieldOfView, 60f, 90f, 5f,
                v => UiSettings.FieldOfView = v);
            ToggleRow(content, SelaContent.Ui.InvertY, () => UiSettings.InvertY, v => UiSettings.InvertY = v,
                text => _invertLabel = text);
            ToggleRow(content, SelaContent.Ui.ReduceMotion, () => UiSettings.ReduceMotion, v => UiSettings.ReduceMotion = v,
                text => _motionLabel = text);

            UiKit.Divider(content, UiTheme.HijauMeja);

            UiKit.SectionTitle(content, SelaContent.Ui.SettingsReading, UiTheme.ArangHijau);
            ChoiceRow(content, SelaContent.Ui.TextSize,
                new[] { SelaContent.Ui.TextScale100, SelaContent.Ui.TextScale125, SelaContent.Ui.TextScale150 },
                () => UiSettings.TextScaleIndex,
                index => { UiSettings.TextScaleIndex = index; UiSettings.NotifyChanged(); },
                _textScaleButtons);
            ChoiceRow(content, SelaContent.Ui.Atmosphere,
                new[] { SelaContent.Ui.AtmosphereNormal, SelaContent.Ui.AtmosphereLow },
                () => UiSettings.AtmosphereLow ? 1 : 0,
                index => { UiSettings.AtmosphereLow = index == 1; UiSettings.NotifyChanged(); },
                _atmosphereButtons);
        }

        private void SliderRow(Transform parent, string label, System.Func<float> getter, System.Action<float> setter)
        {
            RectTransform row = UiKit.Rect(parent, "Row");
            row.sizeDelta = new Vector2(0f, 40f);
            UiKit.Height(row, 40f);

            TextMeshProUGUI name = UiKit.TextIn(row, label, UiTheme.BodyFontSize, UiTheme.TextOnPaper,
                TextAlignmentOptions.Left);
            name.rectTransform.anchorMin = new Vector2(0f, 0f);
            name.rectTransform.anchorMax = new Vector2(0.4f, 1f);
            name.rectTransform.offsetMin = new Vector2(0f, 0f);
            name.rectTransform.offsetMax = new Vector2(0f, 0f);

            Slider slider = row.gameObject.AddComponent<Slider>();
            RectTransform fill = UiKit.Rect(row, "Fill");
            fill.anchorMin = new Vector2(0.42f, 0.35f);
            fill.anchorMax = new Vector2(0.9f, 0.65f);
            fill.offsetMin = Vector2.zero;
            fill.offsetMax = Vector2.zero;

            Image fillImage = UiKit.PaperPanel(fill, "FillImage", UiTheme.HijauMeja);
            fillImage.raycastTarget = false;

            slider.fillRect = fillImage.rectTransform;
            slider.targetGraphic = fillImage;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = getter();
            slider.onValueChanged.AddListener(v => setter(v));

            TextMeshProUGUI value = UiKit.TextIn(row, Mathf.RoundToInt(getter() * 100f) + "%", UiTheme.SmallFontSize,
                UiTheme.TextOnPaper, TextAlignmentOptions.Right);
            value.rectTransform.anchorMin = new Vector2(0.92f, 0f);
            value.rectTransform.anchorMax = new Vector2(1f, 1f);

            _values[label] = value;
        }

        private void StepperRow(Transform parent, string label, System.Func<float> getter, float min, float max, float step,
            System.Action<float> setter)
        {
            RectTransform row = UiKit.Rect(parent, "Row");
            row.sizeDelta = new Vector2(0f, 40f);
            UiKit.Height(row, 40f);

            TextMeshProUGUI name = UiKit.TextIn(row, label, UiTheme.BodyFontSize, UiTheme.TextOnPaper,
                TextAlignmentOptions.Left);
            name.rectTransform.anchorMin = new Vector2(0f, 0f);
            name.rectTransform.anchorMax = new Vector2(0.4f, 1f);

            Button minus = UiKit.Button(row, "-", () => setter(Mathf.Clamp(getter() - step, min, max)),
                UiTheme.HijauMeja, UiTheme.TextOnPaper);
            RectTransform minusRect = minus.GetComponent<RectTransform>();
            minusRect.anchorMin = new Vector2(0.45f, 0.2f);
            minusRect.anchorMax = new Vector2(0.45f, 0.8f);
            minusRect.pivot = new Vector2(0f, 0.5f);
            minusRect.sizeDelta = new Vector2(40f, 0f);

            TextMeshProUGUI value = UiKit.TextIn(row, getter().ToString("0"), UiTheme.BodyFontSize, UiTheme.TextOnPaper,
                TextAlignmentOptions.Center);
            value.rectTransform.anchorMin = new Vector2(0.52f, 0f);
            value.rectTransform.anchorMax = new Vector2(0.78f, 1f);

            Button plus = UiKit.Button(row, "+", () => setter(Mathf.Clamp(getter() + step, min, max)),
                UiTheme.HijauMeja, UiTheme.TextOnPaper);
            RectTransform plusRect = plus.GetComponent<RectTransform>();
            plusRect.anchorMin = new Vector2(0.82f, 0.2f);
            plusRect.anchorMax = new Vector2(0.82f, 0.8f);
            plusRect.pivot = new Vector2(0f, 0.5f);
            plusRect.sizeDelta = new Vector2(40f, 0f);

            _values[label] = value;
        }

        private void ToggleRow(Transform parent, string label, System.Func<bool> getter, System.Action<bool> setter,
            System.Action<TextMeshProUGUI> capture)
        {
            RectTransform row = UiKit.Rect(parent, "Row");
            row.sizeDelta = new Vector2(0f, 40f);
            UiKit.Height(row, 40f);

            TextMeshProUGUI name = UiKit.TextIn(row, label, UiTheme.BodyFontSize, UiTheme.TextOnPaper,
                TextAlignmentOptions.Left);
            name.rectTransform.anchorMin = new Vector2(0f, 0f);
            name.rectTransform.anchorMax = new Vector2(0.4f, 1f);

            TextMeshProUGUI value = UiKit.TextIn(row, string.Empty, UiTheme.BodyFontSize, UiTheme.TextOnPaper,
                TextAlignmentOptions.Center);
            value.rectTransform.anchorMin = new Vector2(0.45f, 0f);
            value.rectTransform.anchorMax = new Vector2(0.95f, 1f);
            capture(value);

            Button toggle = UiKit.Button(row, "Ubah", () => setter(!getter()), UiTheme.HijauMeja, UiTheme.TextOnPaper);
            RectTransform toggleRect = toggle.GetComponent<RectTransform>();
            toggleRect.anchorMin = new Vector2(0.97f, 0.2f);
            toggleRect.anchorMax = new Vector2(0.97f, 0.8f);
            toggleRect.pivot = new Vector2(1f, 0.5f);
            toggleRect.sizeDelta = new Vector2(80f, 0f);
        }

        private void ChoiceRow(Transform parent, string label, string[] options, System.Func<int> getter,
            System.Action<int> setter, List<Button> buttons)
        {
            RectTransform row = UiKit.Rect(parent, "Row");
            row.sizeDelta = new Vector2(0f, 40f);
            UiKit.Height(row, 40f);

            TextMeshProUGUI name = UiKit.TextIn(row, label, UiTheme.BodyFontSize, UiTheme.TextOnPaper,
                TextAlignmentOptions.Left);
            name.rectTransform.anchorMin = new Vector2(0f, 0f);
            name.rectTransform.anchorMax = new Vector2(0.3f, 1f);

            float x = 0.32f;
            for (int i = 0; i < options.Length; i++)
            {
                int captured = i;
                Button button = UiKit.Button(row, options[i], () => setter(captured), UiTheme.LatarSlot, UiTheme.TextOnPaper);
                RectTransform rect = button.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(x, 0.15f);
                rect.anchorMax = new Vector2(x, 0.85f);
                rect.pivot = new Vector2(0f, 0.5f);
                rect.sizeDelta = new Vector2(140f, 0f);
                x += 0.16f;
                buttons.Add(button);
            }
        }

        public override void Refresh()
        {
            if (_invertLabel != null)
            {
                _invertLabel.text = UiSettings.InvertY ? SelaContent.Ui.On : SelaContent.Ui.Off;
            }
            if (_motionLabel != null)
            {
                _motionLabel.text = UiSettings.ReduceMotion ? SelaContent.Ui.On : SelaContent.Ui.Off;
            }
            Highlight(_textScaleButtons, UiSettings.TextScaleIndex);
            Highlight(_atmosphereButtons, UiSettings.AtmosphereLow ? 1 : 0);
        }

        private static void Highlight(List<Button> buttons, int active)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Image image = buttons[i].targetGraphic as Image;
                if (image != null)
                {
                    image.color = i == active ? UiTheme.KuningKusam : UiTheme.LatarSlot;
                }
            }
        }
    }

    /// <summary>Halaman baca: Credits dan Sumber (DESIGN 9.15).</summary>
    public class CreditsScreen : UiScreen
    {
        protected override string ScreenName
        {
            get { return "Credits"; }
        }

        protected override void Build()
        {
            UiSheet sheet = new UiSheet(Root, SelaContent.Ui.CreditsTitle, new Vector2(720f, 600f), true);
            RectTransform content;
            UiKit.Scroll(sheet.Content, out content);
            UiKit.VerticalStack(content, 8f, new RectOffset(0, 0, 0, 0));

            TextMeshProUGUI body = UiKit.TextIn(content, SelaContent.Endings.Credits(), UiTheme.BodyFontSize,
                UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            UiKit.Height(body, 420f);

            Button back = UiKit.Button(sheet.EnsureFooter(), SelaContent.Ui.Back, Router.CloseTop,
                UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Height(back, 46f);
        }
    }

    /// <summary>Halaman refleksi dan bantuan (GDD 3.12).</summary>
    public class HelpScreen : UiScreen
    {
        protected override string ScreenName
        {
            get { return "Help"; }
        }

        protected override void Build()
        {
            UiSheet sheet = new UiSheet(Root, SelaContent.Ui.HelpTitle, new Vector2(720f, 600f), true);
            RectTransform content;
            UiKit.Scroll(sheet.Content, out content);
            UiKit.VerticalStack(content, 8f, new RectOffset(0, 0, 0, 0));

            TextMeshProUGUI body = UiKit.TextIn(content, SelaContent.Endings.Reflection(), UiTheme.BodyFontSize,
                UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            UiKit.Height(body, 420f);

            Button back = UiKit.Button(sheet.EnsureFooter(), SelaContent.Ui.Back, Router.CloseTop,
                UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Height(back, 46f);
        }
    }

    /// <summary>Informasi konten sebelum permainan dimulai (DESIGN 9.15).</summary>
    public class ContentInfoScreen : UiScreen
    {
        protected override string ScreenName
        {
            get { return "ContentInfo"; }
        }

        protected override void Build()
        {
            UiSheet sheet = new UiSheet(Root, SelaContent.Ui.ContentInfoTitle, new Vector2(720f, 600f), false);
            RectTransform content;
            UiKit.Scroll(sheet.Content, out content);
            UiKit.VerticalStack(content, 10f, new RectOffset(0, 0, 0, 0));

            TextMeshProUGUI body = UiKit.TextIn(content, SelaContent.Endings.ContentInfo(), UiTheme.BodyFontSize,
                UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            UiKit.Height(body, 380f);

            // Tiga pilihan tetap terlihat di bawah area scroll: Pengaturan, Kembali ke menu, Lanjut (DESIGN 9.15).
            RectTransform footer = sheet.EnsureFooter(64f);
            UiKit.HorizontalStack(footer, 12f, new RectOffset(0, 0, 0, 0));

            UiKit.Button(footer, SelaContent.Ui.SettingsTitle, SelaBootstrap.Instance.OpenSettingsFromInfo,
                UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Button(footer, SelaContent.Ui.ContentInfoBackToMenu, SelaBootstrap.Instance.ReturnToMenu,
                UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Button(footer, SelaContent.Ui.ContentInfoContinue, SelaBootstrap.Instance.ContinueAfterInfo,
                UiTheme.BiruTinta, UiTheme.TextOnInk);
        }
    }
}
