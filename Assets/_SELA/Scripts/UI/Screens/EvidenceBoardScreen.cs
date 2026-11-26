using System.Collections.Generic;
using SELA.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Meja Bukti: papan tempat kartu bukti dipilih lalu dipasang ke penjepit.
    /// Yang terpilih ditandai garis tebal dan label "Dipilih" (DESIGN 9.2).
    /// </summary>
    public class EvidenceBoardScreen : UiScreen
    {
        private static readonly EvidenceId[] Slots = { EvidenceId.E0, EvidenceId.E1, EvidenceId.E2 };

        private EvidenceId _selected = EvidenceId.E0;

        private UiSheet _sheet;
        private RectTransform _cards;
        private RectTransform _detail;

        protected override string ScreenName
        {
            get { return "EvidenceBoard"; }
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, "Meja Bukti", new Vector2(960f, 600f), true);

            RectTransform body = UiKit.Box(_sheet.Content, "Body", Vector2.zero, Vector2.one,
                new Vector2(0f, 0f), new Vector2(0f, -54f));

            _cards = UiKit.Box(body, "Cards", Vector2.zero, Vector2.one,
                new Vector2(0f, 0f), new Vector2(0.42f, 0f));
            UiKit.VerticalStack(_cards, 10f, new RectOffset(0, 0, 0, 0));

            _detail = UiKit.Box(body, "Detail", Vector2.zero, Vector2.one,
                new Vector2(0.42f, 0f), new Vector2(0f, 0f));
        }

        public override void Refresh()
        {
            for (int i = _cards.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(_cards.GetChild(i).gameObject);
            }
            for (int i = _detail.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(_detail.GetChild(i).gameObject);
            }

            SelaState state = SelaState.Current;

            for (int i = 0; i < Slots.Length; i++)
            {
                EvidenceId id = Slots[i];
                bool owned = state.HasEvidence(id);
                string body = owned ? SelaContent.Evidence.CardText(id) : SelaContent.Evidence.MissingText(id);

                UiKit.SelectableCard(_cards, SelaContent.Evidence.SlotLabel(id), body, owned, id == _selected,
                    () =>
                    {
                        _selected = id;
                        Refresh();
                    }, 132f);
            }

            BuildDetail(state);
        }

        private void BuildDetail(SelaState state)
        {
            bool owned = state.HasEvidence(_selected);
            bool attached = state.IsAttached(_selected);

            UiKit.SectionTitle(_detail, SelaContent.Evidence.SlotLabel(_selected), UiTheme.ArangHijau);

            RectTransform scroll;
            UiKit.Scroll(_detail, out scroll);
            UiKit.VerticalStack(scroll, 10f, new RectOffset(0, 0, 0, 0));

            TextMeshProUGUI body = UiKit.TextIn(scroll,
                owned ? SelaContent.Evidence.CardText(_selected) : SelaContent.Evidence.MissingText(_selected),
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            UiKit.Height(body, 170f);

            UiKit.Divider(scroll, UiTheme.HijauMeja);

            UiKit.SectionTitle(scroll, "Hubungan dengan klaim CV", UiTheme.ArangHijau);
            for (int line = 0; line < SelaContent.Cv.ClaimLines.Length; line++)
            {
                string claim = SelaContent.Cv.ClaimOptions(line)[state.CvClaim[line]];
                TextMeshProUGUI claimText = UiKit.TextIn(scroll,
                    SelaContent.Cv.ClaimLines[line] + ": " + claim,
                    UiTheme.SmallFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
                UiKit.Height(claimText, 46f);

                string support = SelaContent.Cv.SupportsClaim(line, _selected)
                    ? "Bukti ini bisa mendukung kalimat ini."
                    : SelaContent.Cv.UnsupportedNote();
                TextMeshProUGUI supportText = UiKit.TextIn(scroll, support,
                    UiTheme.SmallFontSize,
                    SelaContent.Cv.SupportsClaim(line, _selected) ? UiTheme.HijauMeja : UiTheme.MerahPeringatan,
                    TextAlignmentOptions.TopLeft);
                UiKit.Height(supportText, 34f);
            }

            UiKit.Divider(scroll, UiTheme.HijauMeja);

            if (!owned)
            {
                TextMeshProUGUI note = UiKit.TextIn(scroll,
                    "Kartu ini belum ada, jadi belum bisa dipasang ke penjepit.",
                    UiTheme.BodyFontSize, UiTheme.MerahPeringatan, TextAlignmentOptions.TopLeft);
                UiKit.Height(note, 40f);
            }
            else
            {
                Button toggle = UiKit.Button(scroll,
                    attached ? "Lepas dari penjepit" : "Pasang ke penjepit",
                    () =>
                    {
                        if (attached)
                        {
                            state.CvAttached.Remove(_selected);
                        }
                        else
                        {
                            state.CvAttached.Add(_selected);
                        }
                        state.NotifyChanged();
                        SelaSaveStore.Write(state);
                        Refresh();
                    },
                    attached ? UiTheme.HijauMeja : UiTheme.BiruTinta,
                    attached ? UiTheme.TextOnPaper : UiTheme.TextOnInk);
                UiKit.Height(toggle, 46f);
            }

            UiKit.Button(scroll, SelaContent.Ui.Back, Router.CloseTop, UiTheme.HijauMeja, UiTheme.TextOnPaper);
        }
    }
}
