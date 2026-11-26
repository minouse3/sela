using SELA.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Pengenalan pertama. Enam langkah berurutan sesuai DESIGN 9.15. Tiap langkah memakai
    /// komponen yang sedang dipakai, jadi pemain mengenali bentuknya sebelum memakainya.
    /// </summary>
    public class TutorialScreen : UiScreen
    {
        private int _step;

        private UiSheet _sheet;
        private RectTransform _content;
        private RectTransform _footer;

        protected override string ScreenName
        {
            get { return "Tutorial"; }
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, SelaContent.Tutorial.Title(), new Vector2(760f, 600f), true);

            RectTransform scrollContent;
            UiKit.Scroll(_sheet.Content, out scrollContent);
            _content = scrollContent;

            _footer = _sheet.EnsureFooter(60f);
            UiKit.HorizontalStack(_footer, 12f, new RectOffset(0, 0, 0, 0));
        }

        public override void Refresh()
        {
            ClearChildren(_content);
            ClearChildren(_footer);

            TextMeshProUGUI label = UiKit.Text(_content, SelaContent.Tutorial.StepLabel(_step),
                UiTheme.SmallFontSize, UiTheme.HijauMeja, TextAlignmentOptions.TopLeft);
            UiKit.Height(label, 26f);

            UiKit.SectionTitle(_content, SelaContent.Tutorial.StepTitle(_step), UiTheme.ArangHijau);

            TextMeshProUGUI body = UiKit.Text(_content, SelaContent.Tutorial.StepBody(_step),
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            UiKit.Height(body, 190f);

            BuildDemo();

            UiKit.Button(_footer, SelaContent.Tutorial.Skip(), Skip, UiTheme.HijauMeja, UiTheme.TextOnPaper);

            bool last = _step >= SelaContent.Tutorial.StepCount - 1;
            UiKit.Button(_footer, last ? SelaContent.Tutorial.Start() : SelaContent.Tutorial.Next(),
                last ? Finish : Next, UiTheme.BiruTinta, UiTheme.TextOnInk);
        }

        /// <summary>Pratinjau komponen yang dipakai pada langkah ini.</summary>
        private void BuildDemo()
        {
            Image frame = UiKit.PaperPanel(_content, "DemoFrame", UiTheme.LatarSlot);
            RectTransform frameRect = frame.rectTransform;
            frameRect.sizeDelta = new Vector2(0f, 190f);
            LayoutElement element = frameRect.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 190f;
            element.preferredHeight = 190f;

            RectTransform inner = UiKit.Rect(frameRect, "Inner");
            inner.offsetMin = new Vector2(12f, 12f);
            inner.offsetMax = new Vector2(-12f, -12f);
            UiKit.VerticalStack(inner, 6f, new RectOffset(0, 0, 0, 0));

            switch (_step)
            {
                case 0: BuildReadDemo(inner); break;
                case 1: BuildActivityDemo(inner); break;
                case 2: BuildChoiceDemo(inner); break;
                case 3: BuildSessionDemo(inner); break;
                case 4: BuildCvDemo(inner); break;
                default: BuildSendDemo(inner); break;
            }
        }

        private void BuildReadDemo(RectTransform parent)
        {
            MockRow(parent, "Ibu", "Tiket bus berangkat pagi hari kedelapan.", UiTheme.LatarKartu);
            MockRow(parent, "Studio Lajur", "Lowongan untuk wisuda baru.", UiTheme.LatarKartu);
            MockNote(parent, "Membaca tidak memakai sesi.");
        }

        private void BuildActivityDemo(RectTransform parent)
        {
            MockTitle(parent, "Aktivitas");
            MockText(parent, "Lokasi: Meja - Durasi: 1 sesi");
            MockText(parent, "Tenaga: 3 → 2\nTekanan: 1 → 2");
            MockButton(parent, "Gunakan satu sesi", UiTheme.BiruTinta, UiTheme.TextOnInk);
            MockButton(parent, "Batal", UiTheme.HijauMeja, UiTheme.TextOnPaper);
        }

        private void BuildChoiceDemo(RectTransform parent)
        {
            MockTitle(parent, "Pesan mana yang lebih membantu pengguna?");
            MockCard(parent, "Pilihan 1", "Alamat email dan Isi alamat email", true);
            MockCard(parent, "Pilihan 2", "Input dan Error", false);
        }

        private void BuildSessionDemo(RectTransform parent)
        {
            MockTitle(parent, "Hari 1");
            MockRow(parent, "Pagi", "Sudah lewat", UiTheme.LatarSlot);
            MockRow(parent, "Siang", "Sekarang", UiTheme.KuningKusam);
            MockRow(parent, "Malam", "Belum mulai", UiTheme.LatarSlot);
        }

        private void BuildCvDemo(RectTransform parent)
        {
            MockTitle(parent, "Bagian yang dikerjakan");
            MockRow(parent, "Membantu pengembangan antarmuka proyek kuliah", "Dipilih", UiTheme.KuningKusam);
            MockNote(parent, SelaContent.Cv.UnsupportedNote());
        }

        private void BuildSendDemo(RectTransform parent)
        {
            MockTitle(parent, "Kirim lamaran");
            MockText(parent, "Bagian yang dikerjakan: Membantu pengembangan antarmuka proyek kuliah");
            MockText(parent, "Bukti yang ikut: Proyek kuliah");
            MockButton(parent, "Kirim lamaran", UiTheme.BiruTinta, UiTheme.TextOnInk);
        }

        // -- Komponen pratinjau ---------------------------------------------------------

        private void MockTitle(RectTransform parent, string text)
        {
            TextMeshProUGUI title = UiKit.TextIn(parent, text, UiTheme.SectionFontSize, UiTheme.ArangHijau,
                TextAlignmentOptions.TopLeft, true);
            UiKit.Height(title, 30f);
        }

        private void MockText(RectTransform parent, string text)
        {
            TextMeshProUGUI body = UiKit.TextIn(parent, text, UiTheme.SmallFontSize, UiTheme.TextOnPaper,
                TextAlignmentOptions.TopLeft);
            UiKit.Height(body, 44f);
        }

        private void MockNote(RectTransform parent, string text)
        {
            TextMeshProUGUI note = UiKit.TextIn(parent, text, UiTheme.SmallFontSize, UiTheme.HijauMeja,
                TextAlignmentOptions.TopLeft);
            UiKit.Height(note, 26f);
        }

        private void MockRow(RectTransform parent, string title, string state, Color color)
        {
            Image row = UiKit.PaperPanel(parent, "Row", color);
            RectTransform rt = row.rectTransform;
            rt.sizeDelta = new Vector2(0f, 34f);
            LayoutElement element = rt.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 34f;
            element.preferredHeight = 34f;

            TextMeshProUGUI text = UiKit.TextIn(rt, title + " - " + state, UiTheme.SmallFontSize,
                UiTheme.TextOnPaper, TextAlignmentOptions.Left);
            text.rectTransform.offsetMin = new Vector2(12f, 2f);
            text.rectTransform.offsetMax = new Vector2(-12f, -2f);
        }

        private void MockCard(RectTransform parent, string title, string body, bool selected)
        {
            Image frame = UiKit.PaperPanel(parent, "Card", selected ? UiTheme.ArangHijau : UiTheme.HijauMeja);
            RectTransform rt = frame.rectTransform;
            rt.sizeDelta = new Vector2(0f, 58f);
            LayoutElement element = rt.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 58f;
            element.preferredHeight = 58f;

            Image inner = UiKit.PaperPanel(rt, "Inner", UiTheme.LatarKartu);
            RectTransform innerRect = inner.rectTransform;
            innerRect.anchorMin = Vector2.zero;
            innerRect.anchorMax = Vector2.one;
            innerRect.offsetMin = new Vector2(3f, 3f);
            innerRect.offsetMax = new Vector2(-3f, -3f);
            inner.raycastTarget = false;

            TextMeshProUGUI text = UiKit.TextIn(innerRect, title + ": " + body, UiTheme.SmallFontSize,
                UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);
            text.rectTransform.offsetMin = new Vector2(12f, 8f);
            text.rectTransform.offsetMax = new Vector2(-12f, -8f);
        }

        private void MockButton(RectTransform parent, string label, Color background, Color textColor)
        {
            Image bg = UiKit.PaperPanel(parent, "MockButton", background);
            RectTransform rt = bg.rectTransform;
            rt.sizeDelta = new Vector2(0f, 40f);
            LayoutElement element = rt.gameObject.AddComponent<LayoutElement>();
            element.minHeight = 40f;
            element.preferredHeight = 40f;

            TextMeshProUGUI text = UiKit.TextIn(rt, label, UiTheme.SmallFontSize, textColor,
                TextAlignmentOptions.Center, true);
            text.rectTransform.offsetMin = new Vector2(12f, 4f);
            text.rectTransform.offsetMax = new Vector2(-12f, -4f);
        }

        private static void ClearChildren(RectTransform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(parent.GetChild(i).gameObject);
            }
        }

        private void Next()
        {
            _step = Mathf.Min(_step + 1, SelaContent.Tutorial.StepCount - 1);
            Refresh();
        }

        private void Skip()
        {
            SelaState state = SelaState.Current;
            state.TutorialSeen = true;
            state.NotifyChanged();
            SelaSaveStore.Write(state);
            Router.Close(this);
        }

        private void Finish()
        {
            SelaState state = SelaState.Current;
            state.TutorialSeen = true;
            state.NotifyChanged();
            SelaSaveStore.Write(state);
            Router.Close(this);
        }
    }
}
