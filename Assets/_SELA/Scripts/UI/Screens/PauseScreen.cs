using SELA.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Panel jeda: Lanjutkan, Pengaturan, Ulangi dari checkpoint, Kembali ke menu, Keluar (DESIGN 9.14).
    /// Selama retakan berlangsung, Petunjuk dan Selesaikan adegan ditambahkan di panel yang sama.
    /// </summary>
    public class PauseScreen : UiScreen
    {
        private UiSheet _sheet;
        private RectTransform _list;

        protected override string ScreenName
        {
            get { return "Pause"; }
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, "Pause", new Vector2(540f, 640f), false);

            RectTransform scrollContent;
            UiKit.Scroll(_sheet.Content, out scrollContent);
            _list = scrollContent;
            UiKit.VerticalStack(_list, 12f, new RectOffset(0, 0, 0, 0));
        }

        public override void Refresh()
        {
            for (int i = _list.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(_list.GetChild(i).gameObject);
            }

            SelaState state = SelaState.Current;
            string[] items = SelaContent.Ui.PauseItems;

            UiKit.Button(_list, items[0], Router.CloseTop, UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Button(_list, items[1], SelaBootstrap.Instance.OpenSettingsFromPause, UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Button(_list, items[2], SelaBootstrap.Instance.RestartDay, UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Button(_list, items[3], SelaBootstrap.Instance.ReturnToMenu, UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Button(_list, items[4], QuitGame, UiTheme.HijauMeja, UiTheme.TextOnPaper);

            // Selama retakan, panel yang sama bertambah Petunjuk dan Selesaikan adegan (DESIGN 9.14).
            if (SelaBootstrap.Instance != null && SelaBootstrap.Instance.ActiveRift >= 0)
            {
                UiKit.Button(_list, SelaContent.Ui.RiftHint, SelaBootstrap.Instance.ShowRiftHint,
                    UiTheme.BiruTinta, UiTheme.TextOnInk);
                UiKit.Button(_list, SelaContent.Ui.RiftSkip, ConfirmSkipScene, UiTheme.BiruTinta, UiTheme.TextOnInk);
            }

            // Label dibuat ulang setiap Refresh karena daftar dibersihkan di atas.
            string last = SelaContent.Ui.ExitCheckpoint + ": " +
                          SelaContent.DaySessionLabel(state.Day, state.Session);
            SelaState dayStart = SelaSaveStore.ReadDayStart();
            string dayLine = SelaContent.Ui.ExitDayStart + ": " +
                             SelaContent.DaySessionLabel(dayStart != null ? dayStart.Day : state.Day,
                                 SelaSession.Pagi);

            TextMeshProUGUI checkpointInfo = UiKit.Text(_list, last + "\n" + dayLine + "\n" +
                SelaContent.Ui.ExitWarning, UiTheme.SmallFontSize, UiTheme.ArangHijau,
                TextAlignmentOptions.TopLeft);
            UiKit.Height(checkpointInfo, 72f);

            TextMeshProUGUI saveInfo = UiKit.Text(_list,
                SelaSaveStore.LastWriteSucceeded ? SelaContent.Ui.SaveStored : SelaContent.Ui.SaveFailed,
                UiTheme.SmallFontSize, UiTheme.HijauMeja, TextAlignmentOptions.TopLeft);
            UiKit.Height(saveInfo, 30f);
        }

        private void ConfirmSkipScene()
        {
            SelaBootstrap.Instance.AskConfirm(
                SelaContent.Ui.RiftSkip,
                "Adegan diselesaikan lebih awal. Hasil ceritanya sama dan tidak memakai sesi.",
                SelaBootstrap.Instance.CompleteRiftScene);
        }

        private void QuitGame()
        {
            SelaBootstrap.Instance.AskConfirm(
                SelaContent.Ui.PauseItems[4],
                SelaContent.Ui.ExitConfirm + " " + SelaContent.Ui.ExitWarning,
                () => UnityEngine.Application.Quit());
        }
    }
}
