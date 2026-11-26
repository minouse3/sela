using System.Collections.Generic;
using SELA.Gameplay;
using SELA.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Panel draf CV: dua baris klaim, bukti yang dipasang, dan peringatan klaim berlebihan (DESIGN 9.8).
    /// </summary>
    public class CvScreen : UiScreen
    {
        private UiSheet _sheet;
        private RectTransform _content;

        protected override string ScreenName
        {
            get { return "CV"; }
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, "Draf CV", new Vector2(880f, 600f), true);
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

            for (int line = 0; line < 2; line++)
            {
                UiKit.SectionTitle(_content, SelaContent.Cv.ClaimLines[line], UiTheme.ArangHijau);

                string[] options = SelaContent.Cv.ClaimOptions(line);
                for (int option = 0; option < options.Length; option++)
                {
                    int capturedLine = line;
                    int capturedOption = option;
                    bool selected = state.CvClaim[line] == option;
                    bool excessive = SelaContent.Cv.IsExcessive(line, option);

                    string label = options[option] + (excessive ? "\n(" + SelaContent.Cv.UnsupportedNote() + ")" : string.Empty);
                    Color background = selected
                        ? (excessive ? UiTheme.MerahPeringatan : UiTheme.KuningKusam)
                        : UiTheme.LatarSlot;

                    Button button = UiKit.Button(_content, label,
                        () => { state.CvClaim[capturedLine] = capturedOption; state.NotifyChanged(); Refresh(); },
                        background, UiTheme.TextOnPaper);
                    UiKit.Height(button, excessive ? 66f : 46f);
                }

                // Klaim tanpa dukungan ditulis apa adanya, tanpa ikon gembok (DESIGN 9.2).
                string support = SelaContent.Cv.ClaimSupportSummary(line, state.CvClaim[line], state.CvAttached);
                bool supported = !support.Contains(SelaContent.Cv.UnsupportedNote());
                TextMeshProUGUI supportText = UiKit.TextIn(_content, support, UiTheme.SmallFontSize,
                    supported ? UiTheme.HijauMeja : UiTheme.MerahPeringatan, TextAlignmentOptions.TopLeft);
                UiKit.Height(supportText, 32f);
            }

            UiKit.Divider(_content, UiTheme.HijauMeja);
            UiKit.SectionTitle(_content, SelaContent.Ui.NotesEvidence, UiTheme.ArangHijau);

            foreach (EvidenceId id in new[] { EvidenceId.E0, EvidenceId.E1, EvidenceId.E2 })
            {
                bool owned = state.HasEvidence(id);
                bool attached = state.IsAttached(id);
                string label = SelaContent.Evidence.SlotLabel(id) + " - " +
                               SelaContent.Cv.EvidenceSlotState(owned, attached);

                Button button = UiKit.Button(_content, label,
                    () => ToggleAttach(id, owned),
                    attached ? UiTheme.KuningKusam : UiTheme.LatarSlot, UiTheme.TextOnPaper);
                UiKit.Height(button, 44f);
            }

            Button back = UiKit.Button(_content, SelaContent.Ui.Back, Router.CloseTop, UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Height(back, 44f);
        }

        private void ToggleAttach(EvidenceId id, bool owned)
        {
            if (!owned)
            {
                return;
            }

            SelaState state = SelaState.Current;
            if (state.IsAttached(id))
            {
                state.CvAttached.Remove(id);
            }
            else
            {
                state.CvAttached.Add(id);
            }
            state.NotifyChanged();
            Refresh();
        }
    }

    /// <summary>
    /// Panel sebelum pengiriman lamaran: perusahaan, klaim, bukti, jadwal, dan biaya (DESIGN 9.8).
    /// </summary>
    public class ApplicationSnapshotScreen : UiScreen
    {
        private OfferId _offer = OfferId.A;
        private UiSheet _sheet;
        private RectTransform _content;

        protected override string ScreenName
        {
            get { return "ApplicationSnapshot"; }
        }

        public void SetOffer(OfferId offer)
        {
            _offer = offer;
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, "Kirim lamaran", new Vector2(880f, 600f), true);
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

            UiKit.SectionTitle(_content, SelaContent.OfferName(_offer), UiTheme.ArangHijau);

            for (int line = 0; line < 2; line++)
            {
                string claim = SelaContent.Cv.ClaimOptions(line)[state.CvClaim[line]];
                TextMeshProUGUI text = UiKit.TextIn(_content, SelaContent.Cv.ClaimLines[line] + ": " + claim,
                    UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
                UiKit.Height(text, 34f);
            }

            UiKit.Divider(_content, UiTheme.HijauMeja);
            UiKit.SectionTitle(_content, "Bukti yang ikut dikirim", UiTheme.ArangHijau);

            List<string> missing = new List<string>();
            foreach (EvidenceId id in new[] { EvidenceId.E0, EvidenceId.E1, EvidenceId.E2 })
            {
                bool attached = state.IsAttached(id);
                TextMeshProUGUI text = UiKit.TextIn(_content,
                    SelaContent.Evidence.SlotLabel(id) + ": " + (attached ? "terpasang" : "tidak ikut"),
                    UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
                UiKit.Height(text, 30f);

                if (Required(id) && !attached)
                {
                    missing.Add(SelaContent.Evidence.SlotLabel(id));
                }
            }

            if (missing.Count > 0)
            {
                TextMeshProUGUI warning = UiKit.TextIn(_content, SelaContent.Cv.IncompleteEvidenceWarning(),
                    UiTheme.BodyFontSize, UiTheme.MerahPeringatan, TextAlignmentOptions.TopLeft);
                UiKit.Height(warning, 70f);
            }
            else if (state.CvAttached.Count == 0)
            {
                TextMeshProUGUI warning = UiKit.TextIn(_content, SelaContent.Cv.NoEvidenceWarning(),
                    UiTheme.BodyFontSize, UiTheme.MerahPeringatan, TextAlignmentOptions.TopLeft);
                UiKit.Height(warning, 70f);
            }

            UiKit.Divider(_content, UiTheme.HijauMeja);
            TextMeshProUGUI schedule = UiKit.TextIn(_content,
                "Interview besok siang memerlukan Tenaga 1.\n" +
                "Biaya: Tenaga -1, Tekanan +1, satu sesi.",
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            UiKit.Height(schedule, 60f);

            Button send = UiKit.Button(_content, SelaContent.Ui.SendApplication, Send,
                UiTheme.BiruTinta, UiTheme.TextOnInk);
            UiKit.Height(send, 48f);

            Button cancel = UiKit.Button(_content, SelaContent.Ui.Cancel, Router.CloseTop, UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Height(cancel, 44f);
        }

        private bool Required(EvidenceId id)
        {
            if (_offer == OfferId.A)
            {
                return id == EvidenceId.E1;
            }
            return id == EvidenceId.E1 || id == EvidenceId.E2;
        }

        private void Send()
        {
            SelaState state = SelaState.Current;
            SelaSessionFlow.Resolve(state, SelaActivity.KirimLamaran);
            state.RecordSentApplication(_offer, state.CvClaim, state.CvAttached);
            state.ApplicationOpened[(int)_offer] = true;
            state.NotifyChanged();
            SelaSaveStore.Write(state);
            Router.Close(this);
            SelaBootstrap.Instance.AdvanceSessionAfterActivity();
        }
    }
}
