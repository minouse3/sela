using SELA.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// HUD saat eksplorasi: Hari/Sesi kiri atas, Tenaga dan Tekanan kiri bawah, prompt bawah tengah (DESIGN 9.4).
    /// </summary>
    public class HudScreen : UiScreen
    {
        private TextMeshProUGUI _daySession;
        private TextMeshProUGUI _energyValue;
        private TextMeshProUGUI _pressureValue;
        private TextMeshProUGUI _prompt;
        private TextMeshProUGUI _caption;
        private RectTransform _energyBoxes;
        private RectTransform _pressureBars;

        public override bool IsModal
        {
            get { return false; }
        }

        protected override string ScreenName
        {
            get { return "HUD"; }
        }

        protected override void Build()
        {
            // Label hari/sesi di kiri atas. Pivot di kiri atas supaya margin aman 32 px
            // dihitung dari tepi, bukan dari tengah kotak (DESIGN 9.4).
            _daySession = PaperLabel(new Vector2(UiTheme.HudMargin, -UiTheme.HudMargin), new Vector2(320f, 46f),
                new Vector2(0f, 1f), SelaContent.Ui.HudDaySession(1, SelaSession.Pagi), new Vector2(0f, 1f));

            RectTransform stats = UiKit.Box(Root, "Stats", new Vector2(0f, 0f), new Vector2(0f, 0f),
                new Vector2(UiTheme.HudMargin, UiTheme.HudMargin), new Vector2(UiTheme.HudMargin + 320f, UiTheme.HudMargin + 130f));

            TextMeshProUGUI energyLabel = UiKit.Text(stats, SelaContent.Ui.EnergyLabel, UiTheme.SmallFontSize,
                UiTheme.KertasBeras, TextAlignmentOptions.Left, true);
            RectTransform energyLabelRect = energyLabel.rectTransform;
            energyLabelRect.anchorMin = new Vector2(0f, 1f);
            energyLabelRect.anchorMax = new Vector2(0.3f, 1f);
            energyLabelRect.pivot = new Vector2(0f, 1f);
            energyLabelRect.anchoredPosition = Vector2.zero;
            energyLabelRect.sizeDelta = new Vector2(120f, 24f);

            _energyValue = UiKit.Text(stats, "3/4", UiTheme.SmallFontSize, UiTheme.KertasBeras,
                TextAlignmentOptions.Left);
            RectTransform energyValueRect = _energyValue.rectTransform;
            energyValueRect.anchorMin = new Vector2(0.3f, 1f);
            energyValueRect.anchorMax = new Vector2(0.5f, 1f);
            energyValueRect.pivot = new Vector2(0f, 1f);
            energyValueRect.anchoredPosition = Vector2.zero;
            energyValueRect.sizeDelta = new Vector2(60f, 24f);

            _energyBoxes = UiKit.EnergyBoxes(stats, 3, SelaState.MaxEnergy);
            RectTransform energyBoxRect = _energyBoxes;
            energyBoxRect.anchorMin = new Vector2(0f, 1f);
            energyBoxRect.anchorMax = new Vector2(0f, 1f);
            energyBoxRect.pivot = new Vector2(0f, 1f);
            energyBoxRect.anchoredPosition = new Vector2(0f, -32f);

            TextMeshProUGUI pressureLabel = UiKit.Text(stats, SelaContent.Ui.PressureLabel, UiTheme.SmallFontSize,
                UiTheme.KertasBeras, TextAlignmentOptions.Left, true);
            RectTransform pressureLabelRect = pressureLabel.rectTransform;
            pressureLabelRect.anchorMin = new Vector2(0f, 1f);
            pressureLabelRect.anchorMax = new Vector2(0.3f, 1f);
            pressureLabelRect.pivot = new Vector2(0f, 1f);
            pressureLabelRect.anchoredPosition = new Vector2(0f, -66f);
            pressureLabelRect.sizeDelta = new Vector2(120f, 24f);

            _pressureValue = UiKit.Text(stats, "1/6", UiTheme.SmallFontSize, UiTheme.KertasBeras,
                TextAlignmentOptions.Left);
            RectTransform pressureValueRect = _pressureValue.rectTransform;
            pressureValueRect.anchorMin = new Vector2(0.3f, 1f);
            pressureValueRect.anchorMax = new Vector2(0.5f, 1f);
            pressureValueRect.pivot = new Vector2(0f, 1f);
            pressureValueRect.anchoredPosition = new Vector2(0f, -66f);
            pressureValueRect.sizeDelta = new Vector2(60f, 24f);

            _pressureBars = UiKit.PressureBars(stats, 1, SelaState.MaxPressure);
            RectTransform pressureBarRect = _pressureBars;
            pressureBarRect.anchorMin = new Vector2(0f, 1f);
            pressureBarRect.anchorMax = new Vector2(0f, 1f);
            pressureBarRect.pivot = new Vector2(0f, 1f);
            pressureBarRect.anchoredPosition = new Vector2(0f, -98f);

            TextMeshProUGUI captionNote = UiKit.Text(stats, SelaContent.Ui.PressureCaption, 14f,
                UiTheme.LatarSlot, TextAlignmentOptions.Left);
            RectTransform captionNoteRect = captionNote.rectTransform;
            captionNoteRect.anchorMin = new Vector2(0f, 0f);
            captionNoteRect.anchorMax = new Vector2(1f, 0f);
            captionNoteRect.pivot = new Vector2(0f, 0f);
            captionNoteRect.anchoredPosition = Vector2.zero;
            captionNoteRect.sizeDelta = new Vector2(320f, 18f);

            _caption = PaperLabel(new Vector2(0f, 92f), new Vector2(640f, 34f), new Vector2(0.5f, 0f), string.Empty,
                new Vector2(0.5f, 0.5f));
            _caption.rectTransform.anchoredPosition = new Vector2(0f, 96f);

            _prompt = PaperLabel(new Vector2(0f, 44f), new Vector2(640f, 44f), new Vector2(0.5f, 0f), string.Empty,
                new Vector2(0.5f, 0.5f));
            _prompt.rectTransform.anchoredPosition = new Vector2(0f, 48f);

            SetLabelVisible(_caption, false);
            SetLabelVisible(_prompt, false);
        }

        private TextMeshProUGUI PaperLabel(Vector2 position, Vector2 size, Vector2 anchor, string content,
            Vector2 pivot)
        {
            Image bg = UiKit.PaperPanel(Root, "Label", UiTheme.LatarPanel);
            RectTransform rect = bg.rectTransform;
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            bg.raycastTarget = false;

            TextMeshProUGUI tmp = UiKit.TextIn(bg.transform, content, UiTheme.BodyFontSize, UiTheme.TextOnPaper,
                TextAlignmentOptions.Center, true);
            tmp.rectTransform.offsetMin = new Vector2(12f, 4f);
            tmp.rectTransform.offsetMax = new Vector2(-12f, -4f);
            return tmp;
        }

        public override void Refresh()
        {
            SelaState state = SelaState.Current;
            _daySession.text = SelaContent.Ui.HudDaySession(state.Day, state.Session);
            _energyValue.text = state.Energy + "/" + SelaState.MaxEnergy;
            _pressureValue.text = state.Pressure + "/" + SelaState.MaxPressure;
            RedrawMeters();
        }

        private void RedrawMeters()
        {
            SelaState state = SelaState.Current;
            for (int i = 0; i < _energyBoxes.childCount; i++)
            {
                Image image = _energyBoxes.GetChild(i).GetComponent<Image>();
                image.color = i < state.Energy ? UiTheme.TanahTirai : UiTheme.LatarSlot;
            }
            for (int i = 0; i < _pressureBars.childCount; i++)
            {
                Image image = _pressureBars.GetChild(i).GetComponent<Image>();
                image.color = i < state.Pressure ? UiTheme.MerahPeringatan : UiTheme.LatarSlot;
            }
        }

        /// <summary>Prompt interaksi ditulis di bawah tengah (DESIGN 9.4).</summary>
        public void SetPrompt(string message)
        {
            _prompt.text = message;
            SetLabelVisible(_prompt, !string.IsNullOrEmpty(message));
        }

        /// <summary>Caption audio berada di atas prompt (DESIGN 9.12).</summary>
        public void SetCaption(string message)
        {
            _caption.text = message;
            SetLabelVisible(_caption, !string.IsNullOrEmpty(message));
        }

        /// <summary>Kotak kosong tidak digambar, supaya layar tetap bersih.</summary>
        private static void SetLabelVisible(TextMeshProUGUI label, bool visible)
        {
            // Jangan pakai GetComponentInParent<Image>(): saat Build() root layar masih nonaktif,
            // pencarian default melewati induk nonaktif dan latar kosong tetap tampil (DESIGN 9.4).
            Transform parent = label.transform.parent;
            Image background = parent != null ? parent.GetComponent<Image>() : null;
            if (background != null)
            {
                background.enabled = visible;
            }
            label.enabled = visible;
        }
    }
}
