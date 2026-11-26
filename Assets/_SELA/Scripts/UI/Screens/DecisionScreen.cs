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
        private RectTransform _footer;
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

            // Tombol finalisasi tetap di tempat, tidak ikut tergulir (DESIGN 9.13).
            _footer = _sheet.EnsureFooter(58f);
            UiKit.HorizontalStack(_footer, 12f, new RectOffset(0, 0, 0, 0));
        }

        public override void Refresh()
        {
            for (int i = _content.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(_content.GetChild(i).gameObject);
            }
            for (int i = _footer.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(_footer.GetChild(i).gameObject);
            }

            SelaState state = SelaState.Current;

            TextMeshProUGUI current = UiKit.Paragraph(_content,
                SelaContent.Ui.DecisionCurrent + ": " + ChoiceLabel(state.OfferChoice),
                UiTheme.BodyFontSize, UiTheme.ArangHijau, TextAlignmentOptions.TopLeft);

            if (state.OfferChoice != OfferChoice.TidakAda)
            {
                Button withdraw = UiKit.Button(_content, SelaContent.Ui.DecisionWithdraw, () =>
                {
                    // GDD 3.11: penarikan tawaran yang sudah dipilih memakai konfirmasi.
                    SelaBootstrap.Instance.AskConfirm(SelaContent.Ui.DecisionWithdraw,
                        SelaContent.Ui.DecisionWithdrawBody, () =>
                        {
                            state.OfferChoice = OfferChoice.TidakAda;
                            state.DecisionNote = null;
                            state.NotifyChanged();
                            SelaSaveStore.Write(state);
                            Refresh();
                        });
                }, UiTheme.LatarSlot, UiTheme.TextOnPaper);
                UiKit.Height(withdraw, 40f);
            }

            UiKit.Divider(_content, UiTheme.HijauMeja);

            OfferId[] offers = { OfferId.A, OfferId.B, OfferId.C };
            foreach (OfferId offer in offers)
            {
                // GDD 3.6: A/B didapat lewat interview, sedangkan C adalah tawaran langsung
                // yang berlaku sejak pesannya datang (hari 4) sampai malam hari 7.
                bool obtained = offer == OfferId.C
                    ? state.Day >= 4
                    : state.StatusOf(offer) == ApplicationStatus.Ditawari;
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
                                             SelaContent.Ui.DecisionHomeSub,
                () => ChooseHome(), state.OfferChoice == OfferChoice.TidakAda ? UiTheme.KuningKusam : UiTheme.LatarKartu,
                UiTheme.TextOnPaper);
            UiKit.Height(home, 58f);

            UiKit.Divider(_content, UiTheme.HijauMeja);

            if (state.OfferChoice == OfferChoice.C)
            {
                TextMeshProUGUI warning = UiKit.Paragraph(_content, SelaContent.Ui.DecisionRiskWarning,
                    UiTheme.BodyFontSize, UiTheme.MerahPeringatan, TextAlignmentOptions.TopLeft);

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

            Button finalize = UiKit.Button(_footer, SelaContent.Ui.DecisionFinalize, RequestDecision,
                UiTheme.BiruTinta, UiTheme.TextOnInk);

            // GDD 3.11: keputusan dikunci hanya setelah sesi malam hari ketujuh selesai.
            // Pulang selalu sah (GDD 3.12), jadi catatan batas yang menjadi syaratnya, bukan tawaran.
            bool unlocked = state.Day >= 7 && state.Session == SelaSession.Malam && state.SessionResolved;
            bool ready = state.OfferChoice == OfferChoice.C
                ? _riskAcknowledged
                : !string.IsNullOrEmpty(state.DecisionNote);
            finalize.interactable = unlocked && ready;

            if (!unlocked)
            {
                UiKit.Paragraph(_content, SelaContent.Ui.DecisionLocked, UiTheme.SmallFontSize,
                    UiTheme.HijauMeja, TextAlignmentOptions.TopLeft);
            }
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
                    SelaSaveStore.Write(state);
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
            SelaSaveStore.Write(state);
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
            SelaSaveStore.Write(state);
            Refresh();
        }

        /// <summary>Konfirmasi terakhir dengan label spesifik sebelum ending ditutup (DESIGN 9.13).</summary>
        private void RequestDecision()
        {
            SelaState state = SelaState.Current;
            if (state.Day < 7 || state.Session != SelaSession.Malam || !state.SessionResolved)
            {
                return;
            }

            string body = state.OfferChoice == OfferChoice.C
                ? SelaContent.Ui.DecisionRiskWarning + "\n" + SelaContent.Ui.DecisionConfirmBody
                : SelaContent.Ui.DecisionConfirmBody;
            SelaBootstrap.Instance.AskConfirm(
                SelaContent.Endings.DecisionConfirmLabel(state.OfferChoice), body,
                SelaContent.Endings.DecisionConfirmLabel(state.OfferChoice), FinalizeDecision);
        }

        private void FinalizeDecision()
        {
            SelaState state = SelaState.Current;
            if (state.Day < 7 || state.Session != SelaSession.Malam || !state.SessionResolved)
            {
                return;
            }

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
        private RectTransform _footer;

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

            // Epilog panjang; tombol baca bantuan dan tutup harus tetap terjangkau (DESIGN 9.13).
            _footer = _sheet.EnsureFooter(56f);
            UiKit.HorizontalStack(_footer, 12f, new RectOffset(0, 0, 0, 0));

            Button help = UiKit.Button(_footer, SelaContent.Ui.HelpTitle, SelaBootstrap.Instance.OpenHelp,
                UiTheme.LatarSlot, UiTheme.TextOnPaper);

            Button back = UiKit.Button(_footer, SelaContent.Ui.Close, SelaBootstrap.Instance.ReturnToMenu,
                UiTheme.HijauMeja, UiTheme.TextOnPaper);
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

            TextMeshProUGUI summaryText = UiKit.Paragraph(_content, string.Join("\n", summary.ToArray()),
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            UiKit.Divider(_content, UiTheme.HijauMeja);

            TextMeshProUGUI epilog = UiKit.Paragraph(_content,
                SelaContent.Endings.Epilog(state.OfferChoice, state.BestEvidence()),
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            if (state.OfferChoice == OfferChoice.TidakAda)
            {
                TextMeshProUGUI progress = UiKit.Paragraph(_content,
                    SelaContent.Endings.ProgressInsert(state.BestEvidence()),
                    UiTheme.SmallFontSize, UiTheme.HijauMeja, TextAlignmentOptions.TopLeft);
            }
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
