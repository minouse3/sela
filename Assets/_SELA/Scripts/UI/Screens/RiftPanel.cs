using SELA.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Panel Retakan Ruang: benda yang dibawa, petunjuk, dan penyelesaian adegan (DESIGN 9.12).
    /// </summary>
    public class RiftPanel : UiScreen
    {
        private int _rift = 1;
        private bool _hintShown;
        private UiSheet _sheet;
        private RectTransform _content;

        protected override string ScreenName
        {
            get { return "Rift"; }
        }

        public void SetRift(int rift)
        {
            _rift = Mathf.Clamp(rift, 1, SelaContent.Rifts.Count);
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, "Retakan Ruang", new Vector2(680f, 520f), false);
            RectTransform scrollContent;
            UiKit.Scroll(_sheet.Content, out scrollContent);
            _content = scrollContent;
        }

        public override void Refresh()
        {
            for (int i = _content.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(_content.GetChild(i).gameObject);
            }

            SelaState state = SelaState.Current;
            bool hasOffer = state.OfferChoice != OfferChoice.TidakAda ||
                            state.StatusOf(OfferId.A) != ApplicationStatus.BelumDikirim ||
                            state.StatusOf(OfferId.B) != ApplicationStatus.BelumDikirim;
            bool hasE1 = state.HasEvidence(EvidenceId.E1);

            UiKit.SectionTitle(_content, "Adegan " + _rift, UiTheme.ArangHijau);

            TextMeshProUGUI opening = UiKit.TextIn(_content,
                SelaContent.Rifts.Opening(_rift, hasOffer, hasE1),
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            UiKit.Height(opening, 80f);

            TextMeshProUGUI carried = UiKit.TextIn(_content,
                "Dibawa: " + SelaContent.Rifts.PuzzleObjectName(_rift),
                UiTheme.BodyFontSize, UiTheme.HijauMeja, TextAlignmentOptions.TopLeft);
            UiKit.Height(carried, 34f);

            UiKit.Button(_content, SelaContent.Ui.RiftHint, ShowHint, UiTheme.LatarSlot, UiTheme.TextOnPaper);

            if (_hintShown)
            {
                TextMeshProUGUI hint = UiKit.TextIn(_content, SelaContent.Rifts.Hint(_rift),
                    UiTheme.BodyFontSize, UiTheme.HijauMeja, TextAlignmentOptions.TopLeft);
                UiKit.Height(hint, 90f);
            }

            Button skip = UiKit.Button(_content, SelaContent.Ui.RiftSkip, Skip,
                UiTheme.LatarSlot, UiTheme.TextOnPaper);
            UiKit.Height(skip, 44f);

            if (state.RiftSolved[_rift - 1])
            {
                TextMeshProUGUI solved = UiKit.TextIn(_content, SelaContent.Rifts.Solved(_rift),
                    UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
                UiKit.Height(solved, 100f);
            }
        }

        private void ShowHint()
        {
            _hintShown = true;
            Refresh();
        }

        private void Skip()
        {
            // "Selesaikan adegan" selalu meminta konfirmasi dulu (DESIGN 9.12).
            SelaBootstrap.Instance.AskConfirm(
                SelaContent.Ui.RiftSkip,
                "Adegan diselesaikan lebih awal. Hasil ceritanya sama dan tidak memakai sesi.",
                SelaBootstrap.Instance.CompleteRiftScene);
        }
    }
}
