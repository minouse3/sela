using System.Collections.Generic;
using SELA.Gameplay;
using SELA.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Ponsel di meja. Membaca pesan tidak memakai sesi, menghubungi Dita memakai satu sesi
    /// dan memberi pemulihan (DESIGN 9.11).
    /// </summary>
    public class PhoneScreen : UiScreen
    {
        private UiSheet _sheet;
        private RectTransform _content;

        protected override string ScreenName
        {
            get { return "Ponsel"; }
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, "Ponsel", new Vector2(560f, 460f), false);
            _content = UiKit.Rect(_sheet.Content, "Content");
            UiKit.VerticalStack(_content, 14f, new RectOffset(0, 0, 0, 0));
        }

        public override void Refresh()
        {
            for (int i = _content.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(_content.GetChild(i).gameObject);
            }

            SelaState state = SelaState.Current;
            int unread = UnreadCount(state);

            UiKit.SectionTitle(_content, "Pesan masuk", UiTheme.ArangHijau);

            TextMeshProUGUI summary = UiKit.Paragraph(_content,
                unread > 0
                    ? "Ada " + unread + " pesan baru. Membaca pesan tidak memakai sesi."
                    : "Semua pesan sudah dibaca.",
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            Button read = UiKit.Button(_content, "Baca pesan - gratis sesi", () =>
            {
                Router.Close(this);
                SelaBootstrap.Instance.OpenLaptopMessages();
            }, UiTheme.BiruTinta, UiTheme.TextOnInk);
            UiKit.Height(read, 48f);

            Button call = UiKit.Button(_content, "Hubungi Dita - 1 sesi (Tenaga +2, Tekanan -2)", () =>
            {
                Router.Close(this);
                SelaBootstrap.Instance.OpenActivity(SelaActivity.HubungiDita);
            }, UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Height(call, 48f);

            TextMeshProUGUI note = UiKit.Paragraph(_content,
                "Percakapan pertama tiap hari berbeda. Mengulang conversation tidak memberi pemulihan baru.",
                UiTheme.SmallFontSize, UiTheme.HijauMeja, TextAlignmentOptions.TopLeft);

            Button close = UiKit.Button(_content, SelaContent.Ui.Close, Router.CloseTop,
                UiTheme.LatarSlot, UiTheme.TextOnPaper);
            UiKit.Height(close, 44f);
        }

        private static int UnreadCount(SelaState state)
        {
            int count = 0;
            int index = 0;
            for (int day = 1; day <= state.Day; day++)
            {
                List<Message> daily = SelaContent.Messages.Daily(day, state.CurrentOfferContext());
                for (int i = 0; i < daily.Count; i++, index++)
                {
                    if (!state.ReadMessages.Contains(index))
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }

    /// <summary>
    /// Kalender tujuh hari di dinding. Kotak hari aktif memakai bingkai tebal dan label Hari ini,
    /// di bawahnya urutan Pagi, Siang, Malam (DESIGN 9.6).
    /// </summary>
    public class CalendarScreen : UiScreen
    {
        private UiSheet _sheet;
        private RectTransform _content;

        protected override string ScreenName
        {
            get { return "Kalender"; }
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, "Kalender", new Vector2(760f, 520f), false);
            _content = UiKit.Rect(_sheet.Content, "Content");
            UiKit.VerticalStack(_content, 14f, new RectOffset(0, 0, 0, 0));
        }

        public override void Refresh()
        {
            for (int i = _content.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(_content.GetChild(i).gameObject);
            }

            SelaState state = SelaState.Current;

            UiKit.SectionTitle(_content, "Tujuh hari di kos", UiTheme.ArangHijau);

            RectTransform row = UiKit.Rect(_content, "Days");
            UiKit.HorizontalStack(row, 8f, new RectOffset(0, 0, 0, 0));
            for (int day = 1; day <= 7; day++)
            {
                bool active = day == state.Day;
                bool passed = day < state.Day;
                Button box = UiKit.Button(row, day.ToString(), null,
                    active ? UiTheme.KuningKusam : passed ? UiTheme.LatarSlot : UiTheme.LatarKartu,
                    UiTheme.TextOnPaper);
                box.interactable = false;

                // Tombol ini hanya info, jadi warna kotak tetap apa adanya meski tidak bisa diklik.
                ColorBlock colors = box.colors;
                colors.disabledColor = Color.white;
                box.colors = colors;

                LayoutElement element = box.GetComponent<LayoutElement>();
                element.minHeight = 56f;
                element.preferredHeight = 56f;
                element.flexibleWidth = 1f;
            }

            TextMeshProUGUI legend = UiKit.Paragraph(_content, DayLegend(state), UiTheme.SmallFontSize,
                UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            UiKit.Divider(_content, UiTheme.HijauMeja);

            UiKit.SectionTitle(_content, "Urutan hari ini", UiTheme.ArangHijau);
            string sessions = string.Empty;
            for (int i = 0; i < 3; i++)
            {
                SelaSession session = (SelaSession)i;
                sessions += (i > 0 ? "  -  " : "") + SelaContent.SessionLabel(session) +
                            (session == state.Session ? "  (sekarang)" : string.Empty);
            }
            TextMeshProUGUI sessionText = UiKit.Paragraph(_content, sessions, UiTheme.BodyFontSize,
                UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            UiKit.SectionTitle(_content, "Jadwal", UiTheme.ArangHijau);
            TextMeshProUGUI schedule = UiKit.Paragraph(_content, ScheduleText(state), UiTheme.BodyFontSize,
                UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            TextMeshProUGUI hint = UiKit.Paragraph(_content,
                "Bus berangkat pada pagi hari kedelapan. Tidak ada kotak hari kedelapan yang dapat dimainkan.",
                UiTheme.SmallFontSize, UiTheme.HijauMeja, TextAlignmentOptions.TopLeft);

            Button close = UiKit.Button(_content, SelaContent.Ui.Close, Router.CloseTop,
                UiTheme.LatarSlot, UiTheme.TextOnPaper);
            UiKit.Height(close, 44f);
        }

        /// <summary>Keterangan kotak kalender: hari ini dan hari yang sudah lewat.</summary>
        private static string DayLegend(SelaState state)
        {
            List<int> passed = new List<int>();
            for (int day = 1; day < state.Day; day++)
            {
                passed.Add(day);
            }

            string done = passed.Count == 0
                ? "Belum ada hari yang terlewat."
                : "Sudah lewat: " + string.Join(", ", passed.ConvertAll(d => "Hari " + d).ToArray()) + ".";
            return "Kotak kuning menandai hari ini: Hari " + state.Day + ". " + done;
        }

        private static string ScheduleText(SelaState state)
        {
            string text = "Hari " + state.Day + " - " + SelaContent.SessionLabel(state.Session);

            OfferId? invitation = state.InterviewOfferToday();
            if (invitation.HasValue)
            {
                text += "\nUndangan interview hari ini siang: " + SelaContent.OfferName(invitation.Value) +
                        ". Interview perlu Tenaga 1.";
            }
            else
            {
                text += "\nTidak ada undangan interview hari ini.";
            }

            for (int i = 0; i < 2; i++)
            {
                OfferId offer = (OfferId)i;
                if (state.ApplicationSentDay[i] > 0)
                {
                    text += "\n" + SelaContent.OfferName(offer) + ": lamaran dikirim pada hari " +
                            state.ApplicationSentDay[i] + ".";
                }
            }

            return text;
        }
    }

    /// <summary>
    /// Kasur. Sebelum malam selesai kasur hanya menawarkan istirahat. Sesudahnya tombol
    /// Tutup hari muncul, dan pada malam ketujuh kasur membuka keputusan final (DESIGN 9.6).
    /// </summary>
    public class BedScreen : UiScreen
    {
        private UiSheet _sheet;
        private RectTransform _content;
        private bool _showMorning;

        protected override string ScreenName
        {
            get { return "Kasur"; }
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, "Kasur", new Vector2(600f, 480f), false);
            _content = UiKit.Rect(_sheet.Content, "Content");
            UiKit.VerticalStack(_content, 14f, new RectOffset(0, 0, 0, 0));
        }

        public override void Refresh()
        {
            for (int i = _content.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(_content.GetChild(i).gameObject);
            }

            SelaState state = SelaState.Current;

            if (_showMorning)
            {
                BuildMorning(state);
                return;
            }

            UiKit.SectionTitle(_content, state.Day >= 7 ? "Tinjau keputusan terakhir" : "Kasur", UiTheme.ArangHijau);

            TextMeshProUGUI note = UiKit.Paragraph(_content,
                state.Day >= 7
                    ? "Besok pagi bus berangkat. Malam terakhir di kamar 07, dan keputusan belum ditutup."
                    : "Beristirahat memakai satu sesi. Tidur menutup hari dan memulai hari baru.",
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            Button rest = UiKit.Button(_content, "Beristirahat - 1 sesi (Tenaga +3, Tekanan -2)", () =>
            {
                Router.Close(this);
                SelaBootstrap.Instance.OpenActivity(SelaActivity.Beristirahat);
            }, UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Height(rest, 48f);

            if (state.Day >= 7)
            {
                Button decide = UiKit.Button(_content, "Buka keputusan akhir", () =>
                {
                    Router.Close(this);
                    SelaBootstrap.Instance.OpenDecision();
                }, UiTheme.BiruTinta, UiTheme.TextOnInk);
                UiKit.Height(decide, 48f);
            }
            else if (state.Session == SelaSession.Malam && state.SessionResolved)
            {
                Button sleep = UiKit.Button(_content, "Tutup hari (Tenaga +2, Tekanan -1)", ConfirmSleep,
                    UiTheme.BiruTinta, UiTheme.TextOnInk);
                UiKit.Height(sleep, 48f);
            }
            else
            {
                TextMeshProUGUI reason = UiKit.Paragraph(_content,
                    "Tombol Tutup hari belum tersedia. Selesaikan sesi malam dulu.",
                    UiTheme.BodyFontSize, UiTheme.MerahPeringatan, TextAlignmentOptions.TopLeft);
            }

            Button close = UiKit.Button(_content, SelaContent.Ui.Close, Router.CloseTop,
                UiTheme.LatarSlot, UiTheme.TextOnPaper);
            UiKit.Height(close, 44f);
        }

        private void ConfirmSleep()
        {
            SelaBootstrap.Instance.AskConfirm("Tutup hari",
                "Hari ditutup. Tenaga +2, Tekanan -1, lalu hari baru dimulai pada sesi Pagi.",
                Sleep);
        }

        private void Sleep()
        {
            SelaState state = SelaState.Current;
            SelaSessionFlow.Sleep(state);
            state.NotifyChanged();
            SelaSaveStore.Write(state);
            _showMorning = true;
            Refresh();
        }

        private void BuildMorning(SelaState state)
        {
            UiKit.SectionTitle(_content, "Hari " + state.Day + " - Pagi", UiTheme.ArangHijau);

            TextMeshProUGUI summary = UiKit.Paragraph(_content,
                "Tenaga " + state.Energy + "/" + SelaState.MaxEnergy +
                ", Tekanan " + state.Pressure + "/" + SelaState.MaxPressure + ".\n" +
                "Sesi Pagi dibuka. Tekanan kembali ke 4 dan tidak memakai sesi.",
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            OfferId? invitation = state.InterviewOfferToday();
            if (invitation.HasValue)
            {
                TextMeshProUGUI invite = UiKit.Paragraph(_content,
                    "Interview siang ini memerlukan Tenaga 1. Tenaga saat ini " + state.Energy + ".",
                    UiTheme.BodyFontSize, UiTheme.TanahTirai, TextAlignmentOptions.TopLeft);
            }

            Button close = UiKit.Button(_content, "Lanjut", () =>
            {
                _showMorning = false;
                Router.Close(this);
            }, UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Height(close, 44f);
        }
    }

    /// <summary>
    /// Menu ruang: daftar pintu masuk ke semua panel di kamar. Sementara ini dipakai sebagai
    /// jalan cepat saat trigger benda 3D belum disambungkan.
    /// </summary>
    public class RoomMenuScreen : UiScreen
    {
        private UiSheet _sheet;
        private RectTransform _content;

        protected override string ScreenName
        {
            get { return "MenuRuang"; }
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, "Isi kamar", new Vector2(680f, 560f), false);
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

            UiKit.SectionTitle(_content, "Benda", UiTheme.ArangHijau);
            AddButton("Laptop - lamaran, pesan, catatan", () => SelaBootstrap.Instance.OpenLaptop());
            AddButton("Ponsel - pesan dan Dita", () => SelaBootstrap.Instance.OpenPhone());
            AddButton("Kalender tujuh hari", () => SelaBootstrap.Instance.OpenCalendar());
            AddButton("Kasur - istirahat dan tutup hari", () => SelaBootstrap.Instance.OpenBed());
            AddButton("Catatan harian", () => SelaBootstrap.Instance.OpenNotes());
            AddButton("Meja bukti - kartu dan penjepit", () => SelaBootstrap.Instance.OpenEvidenceBoard());

            UiKit.Divider(_content, UiTheme.HijauMeja);
            UiKit.SectionTitle(_content, "Aktivitas", UiTheme.ArangHijau);

            SelaState state = SelaState.Current;
            for (int i = 0; i < 9; i++)
            {
                SelaActivity activity = (SelaActivity)i;
                string blocked = SelaSessionFlow.BlockedReason(state, activity);
                string label = SelaContent.Ui.ActivityTitle(activity) + " - " + SelaContent.Ui.ActivityCost(activity);
                Button button = AddButton(label, () => SelaBootstrap.Instance.OpenActivity(activity));
                if (blocked != null)
                {
                    button.interactable = false;
                }
            }

            UiKit.Divider(_content, UiTheme.HijauMeja);
            UiKit.SectionTitle(_content, "Kemajuan cerita", UiTheme.ArangHijau);
            AddButton("Keputusan akhir", () => SelaBootstrap.Instance.OpenDecision());
            AddButton("Penutupan cerita", () => SelaBootstrap.Instance.OpenEpilog());
        }

        private Button AddButton(string label, System.Action onClick)
        {
            // Menu ruang ditutup dulu, jadi panel yang dibuka tampil sendirian dan tidak bertumpuk.
            Button button = UiKit.Button(_content, label, () =>
            {
                Router.Close(this);
                onClick();
            }, UiTheme.LatarSlot, UiTheme.TextOnPaper);
            UiKit.Height(button, 44f);
            return button;
        }
    }
}
