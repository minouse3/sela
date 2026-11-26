using System.Collections.Generic;
using SELA.Gameplay;
using SELA.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Panel verifikasi: empat aspek syarat, penilaian pemain, pernyataan awal, dan jawaban klarifikasi (DESIGN 9.9).
    /// </summary>
    public class VerificationScreen : UiScreen
    {
        private OfferId _offer = OfferId.A;
        private UiSheet _sheet;
        private RectTransform _content;

        protected override string ScreenName
        {
            get { return "Verification"; }
        }

        public void SetOffer(OfferId offer)
        {
            _offer = offer;
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, SelaContent.Ui.VerificationTitle, new Vector2(960f, 600f), true);
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
            OfferAspect[] aspects = SelaContent.Offers.Aspects(_offer);

            UiKit.SectionTitle(_content, SelaContent.OfferName(_offer), UiTheme.ArangHijau);

            if (!state.VerificationDone[(int)_offer])
            {
                TextMeshProUGUI intro = UiKit.Paragraph(_content,
                    "Periksa empat aspek berikut. Penandaanmu tidak memakai sesi. " +
                    "Satu sesi dipakai untuk membaca ringkasan dan jawaban klarifikasi.",
                    UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            }
            else
            {
                TextMeshProUGUI intro = UiKit.Paragraph(_content,
                    SelaContent.Offers.VerificationQuestion(_offer),
                    UiTheme.BodyFontSize, UiTheme.ArangHijau, TextAlignmentOptions.TopLeft);
            }

            for (int i = 0; i < aspects.Length; i++)
            {
                int aspectIndex = i;
                OfferAspect aspect = aspects[i];

                UiKit.SectionTitle(_content, aspect.Name, UiTheme.HijauMeja);

                RectTransform marks = UiKit.Rect(_content, "Marks");
                UiKit.VerticalStack(marks, 6f, new RectOffset(0, 0, 0, 0));

                string[] markLabels =
                {
                    SelaContent.Ui.MarkUnset,
                    SelaContent.Ui.MarkClear,
                    SelaContent.Ui.MarkAsk,
                    SelaContent.Ui.MarkRisky
                };

                VerificationMark current = state.MarkOf(_offer, aspectIndex);

                Button row = UiKit.Button(marks, string.Empty, null,
                    current == VerificationMark.Belum ? UiTheme.LatarSlot : UiTheme.LatarKartu, UiTheme.TextOnPaper);
                RectTransform rowRect = row.GetComponent<RectTransform>();
                rowRect.sizeDelta = new Vector2(0f, 44f);

                TextMeshProUGUI rowText = UiKit.TextIn(row.transform,
                    "Penilaianmu: " + markLabels[(int)current], UiTheme.SmallFontSize, UiTheme.TextOnPaper,
                    TextAlignmentOptions.Left);
                rowText.rectTransform.offsetMin = new Vector2(12f, 4f);
                rowText.rectTransform.offsetMax = new Vector2(-12f, -4f);

                for (int m = 1; m < markLabels.Length; m++)
                {
                    int capturedMark = m;
                    Button button = UiKit.Button(marks, markLabels[m], () =>
                    {
                        state.SetMark(_offer, aspectIndex, (VerificationMark)capturedMark);
                        state.NotifyChanged();
                        Refresh();
                    }, current == (VerificationMark)m ? UiTheme.KuningKusam : UiTheme.LatarSlot, UiTheme.TextOnPaper);
                    UiKit.Height(button, 40f);
                }

                if (state.VerificationDone[(int)_offer])
                {
                    TextMeshProUGUI initial = UiKit.Paragraph(_content,
                        SelaContent.Ui.VerificationInitial + ": " + aspect.Initial,
                        UiTheme.SmallFontSize, UiTheme.ArangHijau, TextAlignmentOptions.TopLeft);

                    TextMeshProUGUI answer = UiKit.Paragraph(_content,
                        SelaContent.Ui.VerificationClarification + ": " + aspect.Clarification,
                        UiTheme.SmallFontSize, UiTheme.ArangHijau, TextAlignmentOptions.TopLeft);
                }
            }

            if (!state.VerificationDone[(int)_offer])
            {
                Button run = UiKit.Button(_content, SelaContent.Ui.VerifyCompany + " (" + SelaContent.Ui.OneSession + ")",
                    RunVerification, UiTheme.BiruTinta, UiTheme.TextOnInk);
                UiKit.Height(run, 48f);
            }
            else
            {
                Button back = UiKit.Button(_content, SelaContent.Ui.Back, Router.CloseTop, UiTheme.HijauMeja, UiTheme.TextOnPaper);
                UiKit.Height(back, 44f);
            }
        }

        private void RunVerification()
        {
            SelaState state = SelaState.Current;
            state.VerificationDone[(int)_offer] = true;
            SelaSessionFlow.Resolve(state, SelaActivity.PeriksaPerusahaan);
            state.NotifyChanged();
            SelaSaveStore.Write(state);
            Refresh();
            SelaBootstrap.Instance.FinishActivityEvents();
        }
    }
}
