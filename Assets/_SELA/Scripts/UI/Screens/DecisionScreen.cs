using System.Collections.Generic;
using SELA.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Panel keputusan akhir: memilih surat yang benar-benar diperoleh, satu catatan batas, lalu mengunci (DESIGN 9.13).
    /// </summary>
    public class DecisionScreen : UiScreen
    {
        private UiSheet _sheet;
        private RectTransform _content;
        private bool _riskAcknowledged;

        protected override string ScreenName
        {
            get { return "Decision"; }
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, SelaContent.Ui.DecisionTitle, new Vector2(880f, 600f), true);
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

            TextMeshProUGUI current = UiKit.TextIn(_content,
                SelaContent.Ui.DecisionCurrent + ": " + ChoiceLabel(state.OfferChoice),
                UiTheme.BodyFontSize, UiTheme.ArangHijau, TextAlignmentOptions.TopLeft);
            UiKit.Height(current, 34f);

            if (state.OfferChoice != OfferChoice.TidakAda)
            {
                Button withdraw = UiKit.Button(_content, SelaContent.Ui.DecisionWithdraw, () =>
                {
                    state.OfferChoice = OfferChoice.TidakAda;
                    state.DecisionNote = null;
                    state.NotifyChanged();
                    Refresh();
                }, UiTheme.LatarSlot, UiTheme.TextOnPaper);
                UiKit.Height(withdraw, 40f);
            }

            UiKit.Divider(_content, UiTheme.HijauMeja);

            OfferId[] offers = { OfferId.A, OfferId.B, OfferId.C };
            foreach (OfferId offer in offers)
            {
                bool obtained = state.StatusOf(offer) == ApplicationStatus.Ditawari;
                bool isCurrent = state.OfferChoice == (OfferChoice)((int)offer + 1);

                string label = SelaContent.OfferName(offer) + "\n" +
                               (obtained ? SelaContent.OfferTitle(offer) : SelaContent.Ui.DecisionNotObtained);

                Button button = UiKit.Button(_content, label, () => Choose(offer),
                    isCurrent ? UiTheme.KuningKusam : (obtained ? UiTheme.LatarKartu : UiTheme.LatarSlot),
                    UiTheme.TextOnPaper);
                UiKit.Height(button, 58f);
                button.interactable = obtained;
            }

            Button home = UiKit.Button(_content, SelaContent.Ui.DecisionHome + "\n" +
                                             SelaContent.OfferTitle(OfferId.C),
                () => ChooseHome(), state.OfferChoice == OfferChoice.TidakAda ? UiTheme.KuningKusam : UiTheme.LatarKartu,
                UiTheme.TextOnPaper);
            UiKit.Height(home, 58f);

            UiKit.Divider(_content, UiTheme.HijauMeja);

            if (state.OfferChoice == OfferChoice.C)
            {
                TextMeshProUGUI warning = UiKit.TextIn(_content, SelaContent.Ui.DecisionRiskWarning,
                    UiTheme.BodyFontSize, UiTheme.MerahPeringatan, TextAlignmentOptions.TopLeft);
                UiKit.Height(warning, 70f);

                Button acknowledge = UiKit.Button(_content, "Pilih sementara meski berisiko", () =>
                {
                    _riskAcknowledged = true;
                    state.DecisionNote = "Menguasai syarat berisiko: penyerahan dokumen asli dan pembatasan pengunduran diri";
                    state.NotifyChanged();
                    Refresh();
                }, _riskAcknowledged ? UiTheme.MerahPeringatan : UiTheme.LatarSlot, UiTheme.TextOnPaper);
                UiKit.Height(acknowledge, 44f);

                Button backRead = UiKit.Button(_content, "Kembali membaca", Router.CloseTop, UiTheme.LatarSlot, UiTheme.TextOnPaper);
                UiKit.Height(backRead, 40f);
            }
            else
            {
                BuildNoteChoice(state);
            }

            Button finalize = UiKit.Button(_content, SelaContent.Ui.DecisionFinalize, ConfirmDecision,
                UiTheme.BiruTinta, UiTheme.TextOnInk);
            UiKit.Height(finalize, 48f);
            finalize.interactable = state.OfferChoice == OfferChoice.C
                ? _riskAcknowledged
                : state.OfferChoice != OfferChoice.TidakAda && !string.IsNullOrEmpty(state.DecisionNote);
        }

        private void BuildNoteChoice(SelaState state)
        {
            UiKit.SectionTitle(_content, "Catatan batas", UiTheme.ArangHijau);

            string[] notes;
            if (state.OfferChoice == OfferChoice.A)
            {
                notes = new[] { SelaContent.Ui.DecisionNoteLajur, SelaContent.Ui.DecisionNotePijar };
            }
            else if (state.OfferChoice == OfferChoice.B)
            {
                notes = new[] { SelaContent.Ui.DecisionNoteLajur, SelaContent.Ui.DecisionNotePijar };
            }
            else
            {
                notes = new[] { SelaContent.Ui.DecisionNoteHomeA, SelaContent.Ui.DecisionNoteHomeB };
            }

            foreach (string note in notes)
            {
                bool selected = state.DecisionNote == note;
                Button button = UiKit.Button(_content, note, () =>
                {
                    state.DecisionNote = note;
                    state.NotifyChanged();
                    Refresh();
                }, selected ? UiTheme.KuningKusam : UiTheme.LatarSlot, UiTheme.TextOnPaper);
                UiKit.Height(button, 44f);
            }
        }

        private void Choose(OfferId offer)
        {
            SelaState state = SelaState.Current;
            OfferChoice choice = (OfferChoice)((int)offer + 1);
            if (state.OfferChoice != choice)
            {
                state.DecisionNote = null;
            }
            state.OfferChoice = choice;
            state.NotifyChanged();
            Refresh();
        }

        private void ChooseHome()
        {
            SelaState state = SelaState.Current;
            if (state.OfferChoice != OfferChoice.TidakAda)
            {
                state.DecisionNote = null;
            }
            state.OfferChoice = OfferChoice.TidakAda;
            state.NotifyChanged();
            Refresh();
        }

        private void ConfirmDecision()
        {
            SelaState state = SelaState.Current;
            state.SessionResolved = true;
            state.NotifyChanged();
            SelaSaveStore.Write(state);
            SelaBootstrap.Instance.OpenEpilog();
        }

        private static string ChoiceLabel(OfferChoice choice)
        {
            switch (choice)
            {
                case OfferChoice.A: return SelaContent.OfferName(OfferId.A);
                case OfferChoice.B: return SelaContent.OfferName(OfferId.B);
                case OfferChoice.C: return SelaContent.OfferName(OfferId.C);
                default: return "Belum ada pilihan";
            }
        }
    }

    /// <summary>
    /// Epilog dan refleksi sesuai pilihan kontrak (GDD 3.12).
    /// </summary>
    public class EpilogScreen : UiScreen
    {
        private UiSheet _sheet;
        private RectTransform _content;

        protected override string ScreenName
        {
            get { return "Epilog"; }
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, "Penutup", new Vector2(880f, 600f), false);
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

            UiKit.SectionTitle(_content, "Ringkasan", UiTheme.ArangHijau);

            List<string> summary = new List<string>();
            summary.Add("Keputusan: " + (state.OfferChoice == OfferChoice.TidakAda
                ? SelaContent.Ui.DecisionHome
                : SelaContent.OfferName((OfferId)((int)state.OfferChoice - 1))));
            if (!string.IsNullOrEmpty(state.DecisionNote))
            {
                summary.Add("Catatan batas: " + state.DecisionNote);
            }
            summary.Add("Bukti yang dipakai: " + string.Join(", ", EvidenceNames(state)));
            summary.Add("Aspek yang diperiksa: " + VerifiedAspectCount(state) + " dari 12");

            TextMeshProUGUI summaryText = UiKit.TextIn(_content, string.Join("\n", summary.ToArray()),
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            UiKit.Height(summaryText, 150f);

            UiKit.Divider(_content, UiTheme.HijauMeja);

            TextMeshProUGUI epilog = UiKit.TextIn(_content,
                SelaContent.Endings.Epilog(state.OfferChoice, state.BestEvidence()),
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            UiKit.Height(epilog, 330f);

            if (state.OfferChoice == OfferChoice.TidakAda)
            {
                TextMeshProUGUI progress = UiKit.TextIn(_content,
                    SelaContent.Endings.ProgressInsert(state.BestEvidence()),
                    UiTheme.SmallFontSize, UiTheme.HijauMeja, TextAlignmentOptions.TopLeft);
                UiKit.Height(progress, 40f);
            }

            Button help = UiKit.Button(_content, SelaContent.Ui.HelpTitle, SelaBootstrap.Instance.OpenHelp,
                UiTheme.LatarSlot, UiTheme.TextOnPaper);
            UiKit.Height(help, 44f);

            Button back = UiKit.Button(_content, SelaContent.Ui.Close, SelaBootstrap.Instance.ReturnToMenu,
                UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Height(back, 44f);
        }

        private static List<string> EvidenceNames(SelaState state)
        {
            List<string> names = new List<string>();
            foreach (EvidenceId id in state.Evidence)
            {
                names.Add(SelaContent.Evidence.SlotLabel(id));
            }
            if (names.Count == 0)
            {
                names.Add("belum ada");
            }
            return names;
        }

        private static int VerifiedAspectCount(SelaState state)
        {
            int count = 0;
            foreach (VerificationMark mark in state.Verification)
            {
                if (mark != VerificationMark.Belum)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
