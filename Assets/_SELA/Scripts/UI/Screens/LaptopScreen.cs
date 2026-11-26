using System.Collections.Generic;
using SELA.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Layar laptop dengan tiga tab: Lamaran, Pesan, dan Catatan (DESIGN 9.2).
    /// </summary>
    public class LaptopScreen : UiScreen
    {
        private const int TabApplications = 0;
        private const int TabMessages = 1;
        private const int TabNotes = 2;

        private UiSheet _sheet;
        private RectTransform _tabRow;
        private RectTransform _leftList;
        private RectTransform _right;
        private readonly List<Button> _tabButtons = new List<Button>();
        private int _tab = TabApplications;
        private int _selectedCompany;
        private int _selectedMessage;

        protected override string ScreenName
        {
            get { return "Laptop"; }
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, "Laptop", new Vector2(1080f, 600f), true);

            _tabRow = UiKit.Box(_sheet.Content, "Tabs", new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0f, -46f), new Vector2(0f, 0f));

            string[] labels = { SelaContent.Ui.TabApplications, SelaContent.Ui.TabMessages, SelaContent.Ui.TabNotes };
            for (int i = 0; i < labels.Length; i++)
            {
                int captured = i;
                Button button = UiKit.Button(_tabRow, labels[i], () => SelectTab(captured), UiTheme.LatarSlot, UiTheme.TextOnPaper);
                RectTransform rect = button.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(i * 0.2f, 0f);
                rect.anchorMax = new Vector2((i + 1) * 0.2f, 1f);
                rect.offsetMin = new Vector2(4f, 0f);
                rect.offsetMax = new Vector2(-4f, 0f);
                _tabButtons.Add(button);
            }

            RectTransform body = UiKit.Box(_sheet.Content, "Body", Vector2.zero, Vector2.one,
                new Vector2(0f, 0f), new Vector2(0f, -54f));

            Image divider = UiKit.PaperPanel(body, "Divider", UiTheme.HijauMeja);
            RectTransform dividerRect = divider.rectTransform;
            dividerRect.anchorMin = new Vector2(0.3f, 0f);
            dividerRect.anchorMax = new Vector2(0.3f, 1f);
            dividerRect.offsetMin = new Vector2(-1f, 0f);
            dividerRect.offsetMax = new Vector2(1f, 0f);
            divider.raycastTarget = false;

            _leftList = UiKit.Box(body, "List", Vector2.zero, Vector2.one,
                new Vector2(0f, 0f), new Vector2(0.3f, 0f));
            UiKit.VerticalStack(_leftList, 8f, new RectOffset(0, 0, 0, 0));

            _right = UiKit.Box(body, "Detail", Vector2.zero, Vector2.one,
                new Vector2(0.3f, 0f), new Vector2(0f, 0f));
        }

        public override void Refresh()
        {
            for (int i = 0; i < _tabButtons.Count; i++)
            {
                Image image = _tabButtons[i].targetGraphic as Image;
                if (image != null)
                {
                    image.color = i == _tab ? UiTheme.BiruTinta : UiTheme.LatarSlot;
                }
                TextMeshProUGUI label = _tabButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                {
                    label.color = i == _tab ? UiTheme.TextOnInk : UiTheme.TextOnPaper;
                }
            }

            Clear(_leftList);
            Clear(_right);

            switch (_tab)
            {
                case TabApplications: BuildApplications(); break;
                case TabMessages: BuildMessages(); break;
                default: BuildNotes(); break;
            }
        }

        private static void Clear(RectTransform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(parent.GetChild(i).gameObject);
            }
        }

        private void SelectTab(int index)
        {
            _tab = index;
            Refresh();
        }

        /// <summary>Menampilkan tab pesan, dipakai panel ponsel.</summary>
        public void ShowMessages()
        {
            _tab = TabMessages;
        }

        private void BuildApplications()
        {
            OfferId[] offers = { OfferId.A, OfferId.B, OfferId.C };
            for (int i = 0; i < offers.Length; i++)
            {
                int captured = i;
                OfferId offer = offers[i];
                string statusText = SelaContent.Offers.StatusLabel(SelaState.Current.StatusOf(offer),
                    SelaState.Current.ApplicationOpened[i]);
                Button button = UiKit.Button(_leftList, SelaContent.OfferName(offer) + "\n" + statusText,
                    () => { _selectedCompany = captured; Refresh(); },
                    _selectedCompany == i ? UiTheme.KuningKusam : UiTheme.LatarSlot, UiTheme.TextOnPaper);
                UiKit.Height(button, 62f);
            }

            OfferId selected = offers[Mathf.Clamp(_selectedCompany, 0, 2)];
            RectTransform content;
            UiKit.Scroll(_right, out content);
            UiKit.VerticalStack(content, 10f, new RectOffset(0, 0, 0, 0));

            UiKit.SectionTitle(content, SelaContent.OfferName(selected) + " - " + SelaContent.OfferTitle(selected),
                UiTheme.ArangHijau);

            TextMeshProUGUI document = UiKit.TextIn(content, SelaContent.Offers.Document(selected), UiTheme.BodyFontSize,
                UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            UiKit.Height(document, 220f);

            SelaState state = SelaState.Current;
            bool opened = state.ApplicationOpened[(int)selected];
            bool verified = state.VerificationDone[(int)selected];
            bool isOfficial = selected == OfferId.A || selected == OfferId.B;

            TextMeshProUGUI status = UiKit.TextIn(content,
                "Status: " + SelaContent.Offers.StatusLabel(state.StatusOf(selected), opened),
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            UiKit.Height(status, 30f);

            if (isOfficial)
            {
                Button cv = UiKit.Button(content, SelaContent.Ui.OpenCv, () => SelaBootstrap.Instance.OpenCv(),
                    UiTheme.HijauMeja, UiTheme.TextOnPaper);
                UiKit.Height(cv, 44f);
            }

            if (verified)
            {
                Button read = UiKit.Button(content, SelaContent.Ui.ReadVerification, () => SelaBootstrap.Instance.OpenVerification(selected),
                    UiTheme.HijauMeja, UiTheme.TextOnPaper);
                UiKit.Height(read, 44f);
            }
            else if (opened)
            {
                Button verify = UiKit.Button(content, SelaContent.Ui.VerifyCompany, () => SelaBootstrap.Instance.OpenVerification(selected),
                    UiTheme.BiruTinta, UiTheme.TextOnInk);
                UiKit.Height(verify, 44f);
            }

            if (isOfficial)
            {
                string blocked = SelaContent.Offers.SendBlockedReason(state.StatusOf(selected), false);
                Button send = UiKit.Button(content, SelaContent.Ui.SendApplication,
                    () => SelaBootstrap.Instance.OpenApplicationSnapshot(selected),
                    UiTheme.BiruTinta, UiTheme.TextOnInk);
                UiKit.Height(send, 44f);

                if (blocked != null)
                {
                    TextMeshProUGUI reason = UiKit.TextIn(content, blocked, UiTheme.SmallFontSize,
                        UiTheme.MerahPeringatan, TextAlignmentOptions.TopLeft);
                    UiKit.Height(reason, 44f);
                    send.interactable = false;
                }
            }

            // Tombol interview hanya ada pada sesi siang hari undangan (DESIGN 9.11).
            OfferId? invitation = state.InterviewOfferToday();
            if (invitation.HasValue && invitation.Value == selected && state.Session == SelaSession.Siang)
            {
                Button interview = UiKit.Button(content, SelaContent.Ui.FollowInterview,
                    () => SelaBootstrap.Instance.OpenInterview(selected),
                    UiTheme.BiruTinta, UiTheme.TextOnInk);
                UiKit.Height(interview, 44f);

                TextMeshProUGUI note = UiKit.TextIn(content,
                    "Undangan hari ini siang. Interview perlu Tenaga 1, dan memakai satu sesi.",
                    UiTheme.SmallFontSize, UiTheme.HijauMeja, TextAlignmentOptions.TopLeft);
                UiKit.Height(note, 44f);
            }

            if (!string.IsNullOrEmpty(state.ApplicationOutcome[(int)selected]))
            {
                TextMeshProUGUI outcome = UiKit.TextIn(content, state.ApplicationOutcome[(int)selected],
                    UiTheme.SmallFontSize, UiTheme.ArangHijau, TextAlignmentOptions.TopLeft);
                UiKit.Height(outcome, 80f);
            }
        }

        private void BuildMessages()
        {
            List<Message> messages = AllMessages();
            for (int i = 0; i < messages.Count; i++)
            {
                int captured = i;
                Message message = messages[i];
                bool unread = !SelaState.Current.ReadMessages.Contains(i);
                string label = (unread ? "Baru - " : string.Empty) + SelaContent.SenderLabel(message.Sender) +
                               "\n" + message.Title;
                Button button = UiKit.Button(_leftList, label, () => { _selectedMessage = captured; Refresh(); },
                    _selectedMessage == i ? UiTheme.KuningKusam : UiTheme.LatarSlot, UiTheme.TextOnPaper);
                UiKit.Height(button, 62f);
            }

            if (messages.Count == 0)
            {
                TextMeshProUGUI empty = UiKit.TextIn(_right, "Belum ada pesan.", UiTheme.BodyFontSize,
                    UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
                return;
            }

            int index = Mathf.Clamp(_selectedMessage, 0, messages.Count - 1);
            Message chosen = messages[index];

            RectTransform content;
            UiKit.Scroll(_right, out content);
            UiKit.VerticalStack(content, 10f, new RectOffset(0, 0, 0, 0));

            UiKit.SectionTitle(content, SelaContent.SenderLabel(chosen.Sender), UiTheme.ArangHijau);
            UiKit.SectionTitle(content, chosen.Title, UiTheme.HijauMeja);

            TextMeshProUGUI body = UiKit.TextIn(content, chosen.Body, UiTheme.BodyFontSize, UiTheme.TextOnPaper,
                TextAlignmentOptions.TopLeft);
            UiKit.Height(body, 300f);
        }

        private void BuildNotes()
        {
            RectTransform content;
            UiKit.Scroll(_right, out content);
            UiKit.VerticalStack(content, 10f, new RectOffset(0, 0, 0, 0));

            SelaState state = SelaState.Current;

            UiKit.SectionTitle(content, SelaContent.Ui.NotesEvidence, UiTheme.ArangHijau);
            foreach (EvidenceId id in new[] { EvidenceId.E0, EvidenceId.E1, EvidenceId.E2 })
            {
                bool owned = state.HasEvidence(id);
                string text = owned ? SelaContent.Evidence.CardText(id) : SelaContent.Evidence.MissingText(id);
                TextMeshProUGUI body = UiKit.TextIn(content, SelaContent.Evidence.SlotLabel(id) + "\n" + text,
                    UiTheme.SmallFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
                UiKit.Height(body, 110f);
            }

            Button board = UiKit.Button(content, "Buka Meja Bukti", () => SelaBootstrap.Instance.OpenEvidenceBoard(),
                UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Height(board, 44f);

            UiKit.Divider(content, UiTheme.HijauMeja);
            UiKit.SectionTitle(content, SelaContent.Ui.NotesSchedule, UiTheme.ArangHijau);
            TextMeshProUGUI schedule = UiKit.TextIn(content,
                "Hari " + state.Day + " - " + SelaContent.SessionLabel(state.Session) + "\n" +
                "Bus berangkat pada pagi hari kedelapan.",
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            UiKit.Height(schedule, 70f);

            UiKit.Divider(content, UiTheme.HijauMeja);
            UiKit.SectionTitle(content, SelaContent.Ui.NotesRoomFacts, UiTheme.ArangHijau);
            TextMeshProUGUI facts = UiKit.TextIn(content,
                "Nomor kamar 07 terlihat pada pintu dan gantungan kunci.\n" +
                "Kartu proyek E0 sudah ada sejak awal.\n" +
                "Pesan Ibu tentang tiket masuk pada pagi hari keenam.",
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            UiKit.Height(facts, 90f);

            UiKit.Divider(content, UiTheme.HijauMeja);
            UiKit.SectionTitle(content, SelaContent.Ui.NotesHistory, UiTheme.ArangHijau);
            TextMeshProUGUI history = UiKit.TextIn(content,
                "Lamaran A: " + SelaContent.Offers.StatusLabel(state.StatusOf(OfferId.A), state.ApplicationOpened[0]) + "\n" +
                "Lamaran B: " + SelaContent.Offers.StatusLabel(state.StatusOf(OfferId.B), state.ApplicationOpened[1]) + "\n" +
                "Pilihan kontrak: " + (state.OfferChoice == OfferChoice.TidakAda
                    ? "Belum ada pilihan"
                    : SelaContent.OfferName((OfferId)((int)state.OfferChoice - 1))),
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            UiKit.Height(history, 90f);
        }

        private static List<Message> AllMessages()
        {
            List<Message> all = new List<Message>();
            SelaState state = SelaState.Current;
            for (int day = 1; day <= state.Day; day++)
            {
                all.AddRange(SelaContent.Messages.Daily(day, state.CurrentOfferContext()));
            }
            return all;
        }
    }
}
