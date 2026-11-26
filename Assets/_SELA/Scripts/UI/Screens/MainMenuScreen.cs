using SELA.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Menu utama: Mulai baru, Lanjutkan, Pengaturan, Credits dan Sumber, Keluar (DESIGN 9.14).
    /// Judul berada di bidang kosong kiri dan daftar menu tepat di bawahnya (DESIGN 9.3).
    /// </summary>
    public class MainMenuScreen : UiScreen
    {
        private Button _continueButton;
        private TextMeshProUGUI _continueReason;
        private TextMeshProUGUI _checkpointInfo;

        protected override string ScreenName
        {
            get { return "MainMenu"; }
        }

        protected override void Build()
        {
            // Latar gelap tipis supaya komposisi meja dan tirai di belakang tetap terbaca.
            UiKit.Stretch(UiKit.PaperPanel(Root, "Scrim", new Color(0.11f, 0.14f, 0.13f, 0.55f)).rectTransform);

            // Satu kolom di kiri. Tingginya pas di kanvas acuan 1280 x 720 supaya tidak ada
            // bagian menu yang keluar layar.
            RectTransform column = UiKit.Rect(Root, "Column");
            column.anchorMin = new Vector2(0f, 1f);
            column.anchorMax = new Vector2(0f, 1f);
            column.pivot = new Vector2(0f, 1f);
            column.anchoredPosition = new Vector2(UiTheme.PanelMargin, -40f);
            column.sizeDelta = new Vector2(560f, 640f);

            TextMeshProUGUI title = UiKit.Text(column, SelaContent.GameTitle, 80f, UiTheme.KertasBeras,
                TextAlignmentOptions.Left, true);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(0f, 1f);
            titleRect.pivot = new Vector2(0f, 1f);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = new Vector2(560f, 96f);

            TextMeshProUGUI tagline = UiKit.Text(column, SelaContent.Tagline, UiTheme.BodyFontSize, UiTheme.KertasBeras,
                TextAlignmentOptions.TopLeft);
            RectTransform tagRect = tagline.rectTransform;
            tagRect.anchorMin = new Vector2(0f, 1f);
            tagRect.anchorMax = new Vector2(0f, 1f);
            tagRect.pivot = new Vector2(0f, 1f);
            tagRect.anchoredPosition = new Vector2(0f, -104f);
            tagRect.sizeDelta = new Vector2(560f, 64f);

            RectTransform list = UiKit.Rect(column, "Menu");
            list.anchorMin = new Vector2(0f, 1f);
            list.anchorMax = new Vector2(0f, 1f);
            list.pivot = new Vector2(0f, 1f);
            list.anchoredPosition = new Vector2(0f, -186f);
            list.sizeDelta = new Vector2(560f, 330f);
            UiKit.VerticalStack(list, 10f, new RectOffset(0, 0, 0, 0));

            string[] items = SelaContent.Ui.MainMenuItems;

            UiKit.Button(list, items[0], StartNewGame, UiTheme.HijauMeja, UiTheme.TextOnPaper);

            _continueButton = UiKit.Button(list, items[1], ContinueGame, UiTheme.HijauMeja, UiTheme.TextOnPaper);

            // Alasan melanjutkan ditulis tepat di bawah tombolnya, jadi tidak menutupi daftar.
            _continueReason = UiKit.Text(list, string.Empty, UiTheme.SmallFontSize, UiTheme.KuningKusam,
                TextAlignmentOptions.TopLeft);
            UiKit.Height(_continueReason, 30f);

            UiKit.Button(list, items[2], OpenSettings, UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Button(list, items[3], OpenCredits, UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Button(list, items[4], QuitGame, UiTheme.HijauMeja, UiTheme.TextOnPaper);

            // Status simpanan diletakkan di kaki layar, terpisah dari daftar tombol.
            _checkpointInfo = UiKit.Text(Root, string.Empty, UiTheme.SmallFontSize, UiTheme.KertasBeras,
                TextAlignmentOptions.BottomLeft);
            RectTransform infoRect = _checkpointInfo.rectTransform;
            infoRect.anchorMin = new Vector2(0f, 0f);
            infoRect.anchorMax = new Vector2(0f, 0f);
            infoRect.pivot = new Vector2(0f, 0f);
            infoRect.anchoredPosition = new Vector2(UiTheme.PanelMargin, 28f);
            infoRect.sizeDelta = new Vector2(700f, 44f);
        }

        public override void Refresh()
        {
            bool hasSave = SelaSaveStore.Exists();
            _continueButton.interactable = hasSave;

            // Isi diambil dari berkas simpanan yang benar-benar ada, bukan dari state yang sedang berjalan.
            SelaState saved = hasSave ? SelaSaveStore.Read() : null;
            if (saved == null)
            {
                _continueReason.text = SelaContent.Ui.NoSaveReason;
                _continueReason.color = UiTheme.MerahPeringatan;
                _checkpointInfo.text = string.Empty;
                return;
            }

            string where = "Hari " + saved.Day + " - " + SelaContent.SessionLabel(saved.Session);
            _continueReason.text = SelaContent.Ui.ResumeReason(where);
            _continueReason.color = UiTheme.KuningKusam;
            _checkpointInfo.text = SelaContent.Ui.ExitCheckpoint + ": " + where;
        }

        private void StartNewGame()
        {
            // Mulai baru menimpa progres, jadi minta konfirmasi dulu (DESIGN 9.14).
            SelaBootstrap.Instance.AskConfirm(
                SelaContent.Ui.MainMenuItems[0],
                SelaContent.Ui.NewGameWarning,
                () =>
                {
                    SelaState.Reset();
                    SelaSaveStore.Clear();
                    SelaBootstrap.Instance.OnNewGameStarted();
                    Router.Close(this);
                });
        }

        private void ContinueGame()
        {
            SelaState loaded = SelaSaveStore.Read();
            if (loaded == null)
            {
                Refresh();
                return;
            }

            SelaState.Adopt(loaded);
            SelaBootstrap.Instance.OnGameResumed();
            Router.Close(this);
        }

        private void OpenSettings()
        {
            SelaBootstrap.Instance.OpenSettingsFromMenu();
        }

        private void OpenCredits()
        {
            SelaBootstrap.Instance.OpenCreditsFromMenu();
        }

        private void QuitGame()
        {
            SelaBootstrap.Instance.AskConfirm(
                SelaContent.Ui.PauseItems[4],
                SelaContent.Ui.ExitConfirm,
                () => UnityEngine.Application.Quit());
        }
    }
}
