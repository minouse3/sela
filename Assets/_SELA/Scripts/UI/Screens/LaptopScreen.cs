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
        private RectTransform _detail;
        private RectTransform _detailHeader;
        private RectTransform _actions;
        private RectTransform _actionRule;
        private RectTransform _divider;
        private readonly List<Button> _tabButtons = new List<Button>();
        private int _tab = TabApplications;
        private int _selectedCompany;
        private int _actionCount;
        private int _selectedMessage;

        protected override string ScreenName
        {
            get { return "Laptop"; }
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, "Laptop", new Vector2(1080f, 620f), true);

            // Baris tutup di bawah panel, supaya bagian bawah tidak terlihat kosong (DESIGN 9.15).
            RectTransform footer = _sheet.EnsureFooter(52f);
            UiKit.HorizontalStack(footer, 10f, new RectOffset(0, 0, 0, 0));
            UiKit.Button(footer, SelaContent.Ui.Close, Router.CloseTop, UiTheme.LatarSlot, UiTheme.TextOnPaper);

            _tabRow = UiKit.Box(_sheet.Content, "Tabs", new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0f, -46f), new Vector2(0f, 0f));

            string[] labels = { SelaContent.Ui.TabApplications, SelaContent.Ui.TabMessages, SelaContent.Ui.TabNotes };
            float tabWidth = 1f / labels.Length;
            for (int i = 0; i < labels.Length; i++)
            {
                int captured = i;
                Button button = UiKit.Button(_tabRow, labels[i], () => SelectTab(captured), UiTheme.LatarSlot, UiTheme.TextOnPaper);
                RectTransform rect = button.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(i * tabWidth, 0f);
                rect.anchorMax = new Vector2((i + 1) * tabWidth, 1f);
                rect.offsetMin = new Vector2(4f, 0f);
                rect.offsetMax = new Vector2(-4f, 0f);
                _tabButtons.Add(button);
            }

            RectTransform body = UiKit.Box(_sheet.Content, "Body", Vector2.zero, Vector2.one,
                new Vector2(0f, 0f), new Vector2(0f, -54f));

            // Kolom kiri 32 persen, kolom kanan sisa. Dipakai anchor supaya batasnya tidak bergeser.
            _leftList = UiKit.Column(body, "List", 0f, 0.32f);
            UiKit.VerticalStack(_leftList, 8f, new RectOffset(0, 0, 0, 0));

            _right = UiKit.Column(body, "Detail", 0.32f, 1f);

            // Header tetap di atas: nama perusahaan dan statusnya selalu terlihat,
            // walau dokumennya sedang digulir.
            _detailHeader = UiKit.Box(_right, "DetailHeader", new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0f, -42f), new Vector2(0f, 0f));

            // Baris aksi dikunci di bawah kolom kanan, jadi tombol penting tidak pernah
            // ikut tergulir atau terpotong (DESIGN 9.15).
            _actions = UiKit.Box(_right, "Actions", new Vector2(0f, 0f), new Vector2(1f, 0f),
                new Vector2(0f, 0f), new Vector2(0f, 62f));
            UiKit.ActionRow(_actions, 10f, new RectOffset(0, 0, 0, 0));

            // Garis batas memisahkan area isi dari baris tombol, supaya teks yang terpotong
            // berhenti pada garis yang disengaja.
            Image rule = UiKit.PaperPanel(_right, "ActionRule", UiTheme.HijauMeja);
            _actionRule = rule.rectTransform;
            RectTransform ruleRect = rule.rectTransform;
            ruleRect.anchorMin = new Vector2(0f, 0f);
            ruleRect.anchorMax = new Vector2(1f, 0f);
            ruleRect.pivot = new Vector2(0.5f, 0f);
            ruleRect.sizeDelta = new Vector2(0f, 2f);
            ruleRect.anchoredPosition = new Vector2(0f, 64f);
            rule.raycastTarget = false;

            // Area isi kolom kanan dikecilkan supaya tidak tertutup baris aksi.
            _detail = UiKit.Box(_right, "Scroll", Vector2.zero, Vector2.one,
                new Vector2(0f, 70f), new Vector2(0f, -44f));

            // Garis pembatas dibuat paling akhir supaya tidak tertutup kartu di kolom kiri.
            _divider = UiKit.VerticalDivider(body, 0.32f).rectTransform;
        }

        /// <summary>
        /// Memilih tata letak kolom. Tab Lamaran dan Pesan memakai daftar di kiri;
        /// tab Catatan memakai satu kolom di tengah supaya baris teksnya tidak terlalu panjang.
        /// </summary>
        private void ApplyLayout(bool split)
        {
            _leftList.gameObject.SetActive(split);
            _divider.gameObject.SetActive(split);

            _right.anchorMin = new Vector2(split ? 0.32f : 0.16f, 0f);
            _right.anchorMax = new Vector2(split ? 1f : 0.84f, 1f);
            _right.offsetMin = Vector2.zero;
            _right.offsetMax = Vector2.zero;
        }

        /// <summary>Baris aksi disembunyikan kalau tab ini tidak punya tombol aksi.</summary>
        private void ApplyActionBar()
        {
            bool hasActions = _actionCount > 0;
            _actions.gameObject.SetActive(hasActions);
            _actionRule.gameObject.SetActive(hasActions);
            _detail.offsetMin = new Vector2(0f, hasActions ? 70f : 10f);
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
            Clear(_detail);
            Clear(_detailHeader);
            Clear(_actions);
            _actionCount = 0;

            switch (_tab)
            {
                case TabApplications: ApplyLayout(true); BuildApplications(); break;
                case TabMessages: ApplyLayout(true); BuildMessages(); break;
                default: ApplyLayout(false); BuildNotes(); break;
            }

            ApplyActionBar();
        }

        /// <summary>Menambahkan tombol ke baris aksi tetap di bawah kolom kanan.</summary>
        private Button AddAction(string label, System.Action onClick, Color background, Color textColor)
        {
            _actionCount++;
            return AddAction( label, onClick, background, textColor);
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
            SelaState state = SelaState.Current;
            for (int i = 0; i < offers.Length; i++)
            {
                int captured = i;
                OfferId offer = offers[i];
                string statusText = SelaContent.Offers.StatusLabel(state.StatusOf(offer), offer, state.Day);
                UiKit.ListButton(_leftList, SelaContent.OfferName(offer), statusText, _selectedCompany == i,
                    () => { _selectedCompany = captured; Refresh(); });
            }

            OfferId selected = offers[Mathf.Clamp(_selectedCompany, 0, 2)];
            bool published = state.Day >= SelaContent.Offers.OpeningDay(selected);
            bool verified = state.VerificationDone[(int)selected];
            bool isOfficial = selected == OfferId.A || selected == OfferId.B;

            BuildDetailHeader(SelaContent.OfferName(selected) + " - " + SelaContent.OfferTitle(selected),
                SelaContent.Offers.StatusLabel(state.StatusOf(selected), selected, state.Day));

            RectTransform content;
            UiKit.Scroll(_detail, out content);
            UiKit.VerticalStack(content, 10f, new RectOffset(0, 0, 0, 0));

            if (!published)
            {
                // Iklannya belum terbit: beri tanggalnya, jangan hanya kunci (DESIGN 9.9).
                TextMeshProUGUI waiting = UiKit.Paragraph(content,
                    SelaContent.Offers.NotPublishedReason(selected),
                    UiTheme.DocumentFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
                UiKit.Spacer(content, 10f);
                return;
            }

            TextMeshProUGUI document = UiKit.Paragraph(content, SelaContent.Offers.Document(selected),
                UiTheme.DocumentFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            // Semua tombol aksi diletakkan di baris tetap, bukan di dalam area scroll.
            if (isOfficial)
            {
                AddAction( SelaContent.Ui.OpenCv, () => SelaBootstrap.Instance.OpenCv(),
                    UiTheme.HijauMeja, UiTheme.TextOnPaper);
            }

            if (verified)
            {
                AddAction( SelaContent.Ui.ReadVerification,
                    () => SelaBootstrap.Instance.OpenVerification(selected),
                    UiTheme.HijauMeja, UiTheme.TextOnPaper);
            }
            else
            {
                AddAction( SelaContent.Ui.VerifyCompany,
                    () => SelaBootstrap.Instance.OpenVerification(selected),
                    UiTheme.BiruTinta, UiTheme.TextOnInk);
            }

            if (isOfficial)
            {
                string blocked = SelaContent.Offers.SendBlockedReason(state.StatusOf(selected), selected,
                    state.Day, false);
                Button send = AddAction( SelaContent.Ui.SendApplication,
                    () => SelaBootstrap.Instance.OpenApplicationSnapshot(selected),
                    UiTheme.BiruTinta, UiTheme.TextOnInk);

                if (blocked != null)
                {
                    TextMeshProUGUI reason = UiKit.Paragraph(content, blocked, UiTheme.SmallFontSize,
                        UiTheme.MerahPeringatan, TextAlignmentOptions.TopLeft);
                    send.interactable = false;
                }
            }

            // Tombol interview hanya ada pada sesi siang hari undangan (DESIGN 9.11).
            OfferId? invitation = state.InterviewOfferToday();
            if (invitation.HasValue && invitation.Value == selected && state.Session == SelaSession.Siang)
            {
                AddAction( SelaContent.Ui.FollowInterview,
                    () => SelaBootstrap.Instance.OpenInterview(selected),
                    UiTheme.BiruTinta, UiTheme.TextOnInk);

                TextMeshProUGUI note = UiKit.Paragraph(content,
                    "Undangan hari ini siang. Interview perlu Tenaga 1, dan memakai satu sesi.",
                    UiTheme.SmallFontSize, UiTheme.HijauMeja, TextAlignmentOptions.TopLeft);
            }

            if (!string.IsNullOrEmpty(state.ApplicationOutcome[(int)selected]))
            {
                TextMeshProUGUI outcome = UiKit.Paragraph(content, state.ApplicationOutcome[(int)selected],
                    UiTheme.SmallFontSize, UiTheme.ArangHijau, TextAlignmentOptions.TopLeft);
            }

            // Jarak supaya isi terakhir tidak menempel ke baris tombol.
            UiKit.Spacer(content, 10f);
        }

        /// <summary>Header tetap kolom kanan: judul di kiri, lencana status di kanan.</summary>
        private void BuildDetailHeader(string title, string badge)
        {
            TextMeshProUGUI head = UiKit.TextIn(_detailHeader, title, UiTheme.SmallFontSize + 5f,
                UiTheme.ArangHijau, TextAlignmentOptions.MidlineLeft, true);
            head.rectTransform.offsetMin = new Vector2(14f, 0f);
            head.rectTransform.offsetMax = new Vector2(-288f, 0f);
            head.textWrappingMode = TextWrappingModes.NoWrap;
            head.enableAutoSizing = true;
            head.fontSizeMin = 14f;
            head.fontSizeMax = UiTheme.Scaled(UiTheme.SmallFontSize + 5f);

            TextMeshProUGUI tag = UiKit.TextIn(_detailHeader, badge, UiTheme.SmallFontSize, UiTheme.HijauMeja,
                TextAlignmentOptions.MidlineRight);
            RectTransform tagRect = tag.rectTransform;
            tagRect.anchorMin = new Vector2(1f, 0f);
            tagRect.anchorMax = new Vector2(1f, 1f);
            tagRect.pivot = new Vector2(1f, 0.5f);
            tagRect.sizeDelta = new Vector2(260f, 0f);
            tagRect.anchoredPosition = new Vector2(-14f, 0f);
            tag.textWrappingMode = TextWrappingModes.NoWrap;
            tag.enableAutoSizing = true;
            tag.fontSizeMin = 12f;
            tag.fontSizeMax = UiTheme.Scaled(UiTheme.SmallFontSize);
        }

        private void BuildMessages()
        {
            List<Message> messages = AllMessages();
            for (int i = 0; i < messages.Count; i++)
            {
                int captured = i;
                Message message = messages[i];
                bool unread = !SelaState.Current.ReadMessages.Contains(i);
                string title = (unread ? "Baru - " : string.Empty) + SelaContent.SenderLabel(message.Sender);
                UiKit.ListButton(_leftList, title, message.Title, _selectedMessage == i,
                    () => { _selectedMessage = captured; Refresh(); });
            }

            if (messages.Count == 0)
            {
                BuildDetailHeader("Pesan", "Belum ada pesan");
                TextMeshProUGUI empty = UiKit.Paragraph(_detail, "Belum ada pesan masuk untuk Raka.",
                    UiTheme.DocumentFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
                return;
            }

            int index = Mathf.Clamp(_selectedMessage, 0, messages.Count - 1);
            Message chosen = messages[index];
            bool belumDibaca = !SelaState.Current.ReadMessages.Contains(index);

            BuildDetailHeader(SelaContent.SenderLabel(chosen.Sender), belumDibaca ? "Belum dibaca" : "Sudah dibaca");

            RectTransform content;
            UiKit.Scroll(_detail, out content);
            UiKit.VerticalStack(content, 10f, new RectOffset(0, 0, 0, 0));

            UiKit.SectionTitle(content, chosen.Title, UiTheme.HijauMeja);

            TextMeshProUGUI body = UiKit.Paragraph(content, chosen.Body, UiTheme.DocumentFontSize,
                UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
        }

        private void BuildNotes()
        {
            SelaState state = SelaState.Current;
            BuildDetailHeader("Catatan Raka", "Hari " + state.Day + " - " + SelaContent.SessionLabel(state.Session));

            RectTransform content;
            UiKit.Scroll(_detail, out content);
            UiKit.VerticalStack(content, 10f, new RectOffset(0, 0, 0, 0));

            UiKit.SectionTitle(content, SelaContent.Ui.NotesEvidence, UiTheme.ArangHijau);
            foreach (EvidenceId id in new[] { EvidenceId.E0, EvidenceId.E1, EvidenceId.E2 })
            {
                bool owned = state.HasEvidence(id);
                string text = owned ? SelaContent.Evidence.CardText(id) : SelaContent.Evidence.MissingText(id);
                TextMeshProUGUI body = UiKit.Paragraph(content, SelaContent.Evidence.SlotLabel(id) + "\n" + text,
                    UiTheme.SmallFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            }

            AddAction( "Buka Meja Bukti", () => SelaBootstrap.Instance.OpenEvidenceBoard(),
                UiTheme.HijauMeja, UiTheme.TextOnPaper);

            UiKit.Divider(content, UiTheme.HijauMeja);
            UiKit.SectionTitle(content, SelaContent.Ui.NotesSchedule, UiTheme.ArangHijau);
            TextMeshProUGUI schedule = UiKit.Paragraph(content,
                "Hari " + state.Day + " - " + SelaContent.SessionLabel(state.Session) + "\n" +
                "Bus berangkat pada pagi hari kedelapan.",
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            UiKit.Divider(content, UiTheme.HijauMeja);
            UiKit.SectionTitle(content, SelaContent.Ui.NotesRoomFacts, UiTheme.ArangHijau);
            TextMeshProUGUI facts = UiKit.Paragraph(content,
                "Nomor kamar 07 terlihat pada pintu dan gantungan kunci.\n" +
                "Kartu proyek E0 sudah ada sejak awal.\n" +
                "Pesan Ibu tentang tiket masuk pada pagi hari keenam.",
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            UiKit.Divider(content, UiTheme.HijauMeja);
            UiKit.SectionTitle(content, SelaContent.Ui.NotesHistory, UiTheme.ArangHijau);
            TextMeshProUGUI history = UiKit.Paragraph(content,
                "Lamaran A: " + SelaContent.Offers.StatusLabel(state.StatusOf(OfferId.A), OfferId.A, state.Day) + "\n" +
                "Lamaran B: " + SelaContent.Offers.StatusLabel(state.StatusOf(OfferId.B), OfferId.B, state.Day) + "\n" +
                "Pilihan kontrak: " + (state.OfferChoice == OfferChoice.TidakAda
                    ? "Belum ada pilihan"
                    : SelaContent.OfferName((OfferId)((int)state.OfferChoice - 1))),
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
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
