using System.Collections.Generic;
using SELA.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Overlay Catatan: Bukti, Jadwal, Fakta ruang, dan Riwayat hasil (DESIGN 9.11).
    /// </summary>
    public class NotesScreen : UiScreen
    {
        private UiSheet _sheet;
        private RectTransform _content;

        protected override string ScreenName
        {
            get { return "Notes"; }
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, SelaContent.Ui.NotesTitle, new Vector2(760f, 600f), true);
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

            UiKit.SectionTitle(_content, SelaContent.Ui.NotesEvidence, UiTheme.ArangHijau);
            foreach (EvidenceId id in new[] { EvidenceId.E0, EvidenceId.E1, EvidenceId.E2 })
            {
                bool owned = state.HasEvidence(id);
                string body = owned
                    ? SelaContent.Evidence.CardText(id)
                    : SelaContent.Evidence.MissingText(id);
                RectTransform card = UiKit.EvidenceCard(_content, SelaContent.Evidence.SlotLabel(id), body, owned);
                UiKit.Height(card, 150f);
            }

            UiKit.Divider(_content, UiTheme.HijauMeja);
            UiKit.SectionTitle(_content, SelaContent.Ui.NotesSchedule, UiTheme.ArangHijau);
            TextMeshProUGUI schedule = UiKit.Paragraph(_content, ScheduleText(state), UiTheme.BodyFontSize,
                UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            UiKit.Divider(_content, UiTheme.HijauMeja);
            UiKit.SectionTitle(_content, SelaContent.Ui.NotesRoomFacts, UiTheme.ArangHijau);
            TextMeshProUGUI facts = UiKit.Paragraph(_content, RoomFactsText(state), UiTheme.BodyFontSize,
                UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            UiKit.Divider(_content, UiTheme.HijauMeja);
            UiKit.SectionTitle(_content, SelaContent.Ui.NotesHistory, UiTheme.ArangHijau);
            TextMeshProUGUI history = UiKit.Paragraph(_content, HistoryText(state), UiTheme.BodyFontSize,
                UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            UiKit.Divider(_content, UiTheme.HijauMeja);
            UiKit.SectionTitle(_content, "Cara_MAIN", UiTheme.ArangHijau);
            TextMeshProUGUI guide = UiKit.Paragraph(_content, GuideText(), UiTheme.SmallFontSize,
                UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            Button reopen = UiKit.Button(_content, "Buka pengenalan lagi", () => SelaBootstrap.Instance.OpenTutorial(),
                UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Height(reopen, 44f);

            Button board = UiKit.Button(_content, "Buka Meja Bukti", () => SelaBootstrap.Instance.OpenEvidenceBoard(),
                UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Height(board, 44f);
        }

        /// <summary>Ringkasan pengenalan, tetap terbaca di Catatan (DESIGN 9.15).</summary>
        private static string GuideText()
        {
            List<string> lines = new List<string>();
            for (int step = 0; step < SelaContent.Tutorial.StepCount; step++)
            {
                lines.Add(SelaContent.Tutorial.StepTitle(step) + ": " + SelaContent.Tutorial.StepBody(step));
            }
            return string.Join("\n", lines.ToArray());
        }

        private static string ScheduleText(SelaState state)
        {
            List<string> lines = new List<string>();
            lines.Add("Hari " + state.Day + " - " + SelaContent.SessionLabel(state.Session) +
                      (state.SessionResolved ? " (sesi selesai)" : string.Empty));
            lines.Add("Bus berangkat pada pagi hari kedelapan. Tiket tersedia di dalam tas.");
            lines.Add("Interview selalu berlangsung di sesi siang. Undangan terlewat menambah tekanan satu.");
            return string.Join("\n", lines.ToArray());
        }

        private static string RoomFactsText(SelaState state)
        {
            List<string> lines = new List<string>();
            lines.Add("Nomor kamar 07 terlihat pada pintu dan pada gantungan kunci.");
            lines.Add("Kartu proyek E0 sudah ada sejak awal, sebagai anggota tim proyek.");
            lines.Add("Nomor 07 membedakan alamat kamar dari label hasil, misalnya DITERIMA atau TERLAMBAT.");
            lines.Add("Pesan Ibu tentang tiket masuk pada pagi hari keenam.");
            return string.Join("\n", lines.ToArray());
        }

        private static string HistoryText(SelaState state)
        {
            List<string> lines = new List<string>();
            lines.Add("Bukti yang sudah dimiliki: " + string.Join(", ", Names(state)));
            lines.Add("Lamaran A: " + SelaContent.Offers.StatusLabel(state.StatusOf(OfferId.A), OfferId.A, state.Day));
            lines.Add("Lamaran B: " + SelaContent.Offers.StatusLabel(state.StatusOf(OfferId.B), OfferId.B, state.Day));
            lines.Add("Pilihan kontrak: " + ChoiceLabel(state.OfferChoice));
            if (!string.IsNullOrEmpty(state.DecisionNote))
            {
                lines.Add("Catatan batas: " + state.DecisionNote);
            }
            return string.Join("\n", lines.ToArray());
        }

        private static List<string> Names(SelaState state)
        {
            List<string> names = new List<string>();
            foreach (EvidenceId id in state.Evidence)
            {
                names.Add(SelaContent.Evidence.SlotLabel(id));
            }
            return names;
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
}
