using SELA.Gameplay;
using SELA.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Panel konfirmasi aktivitas satu sesi: biaya, kondisi sebelum dan sesudah, hasil khusus,
    /// dan tombol konfirmasi. Semua sembilan aktivitas memakai panel ini (DESIGN 9.5).
    /// </summary>
    public class ActivityPanel : UiScreen
    {
        private SelaActivity _activity = SelaActivity.Beristirahat;
        private int _stage;
        private int _choiceIndex = -1;

        private UiSheet _sheet;
        private RectTransform _content;
        private RectTransform _footer;

        protected override string ScreenName
        {
            get { return "Activity"; }
        }

        public void SetActivity(SelaActivity activity)
        {
            _activity = activity;
            _stage = 0;
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, "Aktivitas", new Vector2(760f, 580f), false);
            RectTransform scrollContent;
            UiKit.Scroll(_sheet.Content, out scrollContent);
            _content = scrollContent;

            // Tombol aksi hidup di footer tetap supaya tidak ikut tergulir dan terpotong (DESIGN 9.5).
            _footer = _sheet.EnsureFooter(58f);
            UiKit.HorizontalStack(_footer, 12f, new RectOffset(0, 0, 0, 0));
        }

        public override void Refresh()
        {
            Clear(_content);
            Clear(_footer);

            if (_stage == 1)
            {
                BuildChoice();
                return;
            }

            if (_stage == 2)
            {
                BuildWrongAnswer();
                return;
            }

            if (_stage == 3)
            {
                BuildResult();
                return;
            }

            SelaState state = SelaState.Current;

            UiKit.SectionTitle(_content, SelaContent.Ui.ActivityTitle(_activity), UiTheme.ArangHijau);

            TextMeshProUGUI location = UiKit.Paragraph(_content,
                "Lokasi: " + SelaContent.Ui.ActivityLocation(_activity) + "\nDurasi: " + SelaContent.Ui.OneSession,
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            string cost = SelaContent.Ui.ActivityCost(_activity);
            TextMeshProUGUI costText = UiKit.Paragraph(_content,
                "Tenaga: " + state.Energy + " -> " + SelaSessionFlow.PreviewEnergy(state, _activity) + "\n" +
                "Tekanan: " + state.Pressure + " -> " + SelaSessionFlow.PreviewPressure(state, _activity) + "\n" +
                "Sesi berikutnya: " + SelaContent.SessionLabel(NextSession(state)),
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            TextMeshProUGUI note = UiKit.Paragraph(_content, cost + "\n" + SelaContent.Recovery.RecoveryNote(_activity),
                UiTheme.SmallFontSize, UiTheme.HijauMeja, TextAlignmentOptions.TopLeft);

            string blocked = SelaSessionFlow.BlockedReason(state, _activity);
            if (blocked != null)
            {
                TextMeshProUGUI warning = UiKit.Paragraph(_content, blocked, UiTheme.BodyFontSize,
                    UiTheme.MerahPeringatan, TextAlignmentOptions.TopLeft);
            }
            else if (SelaSessionFlow.ShouldTriggerBreak(state))
            {
                TextMeshProUGUI breakNote = UiKit.Paragraph(_content,
                    "Tekanan sudah 6. Aktivitas ini akan memicu Jeda setelah selesai.",
                    UiTheme.SmallFontSize, UiTheme.TanahTirai, TextAlignmentOptions.TopLeft);
            }

            Button use = UiKit.Button(_footer, SelaContent.Ui.UseSession, UseSession,
                UiTheme.BiruTinta, UiTheme.TextOnInk);
            use.interactable = blocked == null;

            Button cancel = UiKit.Button(_footer, SelaContent.Ui.Cancel, Router.CloseTop,
                UiTheme.HijauMeja, UiTheme.TextOnPaper);
        }

        private static void Clear(RectTransform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(parent.GetChild(i).gameObject);
            }
        }

        private static SelaSession NextSession(SelaState state)
        {
            switch (state.Session)
            {
                case SelaSession.Pagi: return SelaSession.Siang;
                case SelaSession.Siang: return SelaSession.Malam;
                default: return SelaSession.Malam;
            }
        }

        private void UseSession()
        {
            // Undangan interview pada sesi siang terlewat kalau aktivitas lain yang dipakai.
            OfferId? invitation = SelaState.Current.InterviewOfferToday();
            if (invitation.HasValue && SelaState.Current.Session == SelaSession.Siang &&
                _activity != SelaActivity.IkutiInterview)
            {
                SelaBootstrap.Instance.AskConfirm(
                    "Undangan interview hari ini",
                    "Interview dengan " + SelaContent.OfferName(invitation.Value) +
                    " hari ini siang akan terlewat. Tekanan naik 1 di samping akibat aktivitas ini. Tetap lakukan aktivitas?",
                    ConfirmUseSession);
                return;
            }

            ConfirmUseSession();
        }

        private void ConfirmUseSession()
        {
            if (_activity == SelaActivity.RapikanContohAntarmuka ||
                _activity == SelaActivity.LengkapiCatatanPengujian)
            {
                _stage = 1;
                _choiceIndex = -1;
                Refresh();
                return;
            }

            // Tiga aktivitas ini dijalankan dari laptop, jadi panel ini hanya mengarahkan ke sana.
            if (_activity == SelaActivity.KirimLamaran ||
                _activity == SelaActivity.PeriksaPerusahaan ||
                _activity == SelaActivity.IkutiInterview)
            {
                Router.Close(this);
                SelaBootstrap.Instance.OpenLaptop();
                return;
            }

            ApplyCost();
            Router.Close(this);
        }

        /// <summary>
        /// Biaya dan hasil berlaku satu kali. Setelah itu dicek Jeda, lalu Retakan terjadwal,
        /// baru sesi berpindah dan checkpoint disimpan (GDD 3.2).
        /// </summary>
        private void ApplyCost()
        {
            SelaState state = SelaState.Current;
            SelaSessionFlow.Resolve(state, _activity);
            state.NotifyChanged();
            SelaSaveStore.Write(state);

            SelaBootstrap.Instance.FinishActivityEvents();
        }

        private void BuildChoice()
        {
            bool isE1 = _activity == SelaActivity.RapikanContohAntarmuka;

            UiKit.SectionTitle(_content, isE1 ? SelaContent.Evidence.E1Question() : SelaContent.Evidence.E2Question(),
                UiTheme.ArangHijau);

            string correct = isE1 ? SelaContent.Evidence.E1ChoiceCorrect() : SelaContent.Evidence.E2ChoiceCorrect();
            string wrong = isE1 ? SelaContent.Evidence.E1ChoiceWrong() : SelaContent.Evidence.E2ChoiceWrong();

            if (isE1)
            {
                UiKit.SectionTitle(_content, "Pasangan A: " + SelaContent.Evidence.E1PairBefore[0] + " / " +
                                            SelaContent.Evidence.E1PairBefore[1], UiTheme.HijauMeja);
                UiKit.SectionTitle(_content, "Pasangan B: " + SelaContent.Evidence.E1PairAfter[0] + " / " +
                                            SelaContent.Evidence.E1PairAfter[1], UiTheme.HijauMeja);
            }
            else
            {
                UiKit.SectionTitle(_content, "Tiga kasus uji: kolom kosong, format salah, dan data valid. " +
                                            "Hasil dan batas pengujian ikut tercatat.", UiTheme.HijauMeja);
            }

            // Dua kartu besar dengan komponen Meja Bukti (DESIGN 9.5).
            UiKit.SelectableCard(_content, "Pilihan 1", correct, true, _choiceIndex == 0,
                () => PickChoice(0), 92f);
            UiKit.SelectableCard(_content, "Pilihan 2", wrong, true, _choiceIndex == 1,
                () => PickChoice(1), 92f);

            Button confirm = UiKit.Button(_footer, "Pakai pilihan ini", ConfirmChoice,
                UiTheme.BiruTinta, UiTheme.TextOnInk);
            confirm.interactable = _choiceIndex >= 0;

            Button back = UiKit.Button(_footer, SelaContent.Ui.Cancel, () =>
            {
                _stage = 0;
                _choiceIndex = -1;
                Refresh();
            }, UiTheme.HijauMeja, UiTheme.TextOnPaper);
        }

        private void PickChoice(int index)
        {
            _choiceIndex = index;
            Refresh();
        }

        private void ConfirmChoice()
        {
            if (_choiceIndex < 0)
            {
                return;
            }
            FinishChoice(_choiceIndex == 0);
        }

        /// <summary>
        /// Pilihan kurang tepat menampilkan alasan lalu dapat diulang tanpa biaya atau perubahan stat
        /// (DESIGN 9.5).
        /// </summary>
        private void FinishChoice(bool correct)
        {
            bool isE1 = _activity == SelaActivity.RapikanContohAntarmuka;

            if (!correct)
            {
                _stage = 2;
                Refresh();
                return;
            }

            EvidenceId id = isE1 ? EvidenceId.E1 : EvidenceId.E2;
            SelaState state = SelaState.Current;
            if (!state.HasEvidence(id))
            {
                state.Evidence.Add(id);
            }

            ApplyCost();
            _stage = 3;
            Refresh();
        }

        private void BuildWrongAnswer()
        {
            bool isE1 = _activity == SelaActivity.RapikanContohAntarmuka;

            UiKit.SectionTitle(_content, "Belum tepat", UiTheme.ArangHijau);

            TextMeshProUGUI reason = UiKit.Paragraph(_content,
                isE1 ? SelaContent.Evidence.E1Feedback(false) : SelaContent.Evidence.E2Feedback(false),
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            Button retry = UiKit.Button(_footer, "Coba lagi", () =>
            {
                _stage = 1;
                _choiceIndex = -1;
                Refresh();
            }, UiTheme.BiruTinta, UiTheme.TextOnInk);

            Button back = UiKit.Button(_footer, SelaContent.Ui.Cancel, () =>
            {
                _stage = 0;
                Refresh();
            }, UiTheme.HijauMeja, UiTheme.TextOnPaper);
        }

        private void BuildResult()
        {
            bool isE1 = _activity == SelaActivity.RapikanContohAntarmuka;
            SelaState state = SelaState.Current;

            UiKit.SectionTitle(_content, "Hasil", UiTheme.ArangHijau);
            TextMeshProUGUI result = UiKit.Paragraph(_content,
                (isE1 ? SelaContent.Evidence.E1Feedback(true) : SelaContent.Evidence.E2Feedback(true)) +
                "\n\nTenaga " + state.Energy + "/" + SelaState.MaxEnergy +
                ", Tekanan " + state.Pressure + "/" + SelaState.MaxPressure +
                ". Sesi sekarang: " + SelaContent.SessionLabel(state.Session) + ".",
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            Button close = UiKit.Button(_footer, SelaContent.Ui.Close, Router.CloseTop,
                UiTheme.HijauMeja, UiTheme.TextOnPaper);
        }
    }

    /// <summary>
    /// Jeda otomatis saat Tekanan mencapai 6. Dua pilihan setara, lalu kalimat singkat yang
    /// boleh dilewati (DESIGN 9.7).
    /// </summary>
    public class BreakScreen : UiScreen
    {
        private UiSheet _sheet;
        private RectTransform _content;
        private string _message;

        protected override string ScreenName
        {
            get { return "Break"; }
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, SelaContent.Ui.BreakTitle, new Vector2(620f, 420f), false);
            _content = UiKit.Rect(_sheet.Content, "Content");
            UiKit.VerticalStack(_content, 14f, new RectOffset(0, 0, 0, 0));
        }

        public override void Refresh()
        {
            for (int i = _content.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(_content.GetChild(i).gameObject);
            }

            if (_message != null)
            {
                BuildMessage();
                return;
            }

            TextMeshProUGUI text = UiKit.Paragraph(_content,
                "Tekanan sudah penuh. Kamu boleh berhenti sebentar. Tidak ada pilihan yang terkunci, dan aktivitas pemulihan tetap tersedia.",
                UiTheme.BodyFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            UiKit.Button(_content, SelaContent.Ui.BreakSit,
                () => Finish("Duduk sebentar. Kamar diam sebentar, lalu suara lorong kembali."),
                UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Button(_content, SelaContent.Ui.BreakMessageDita,
                () => Finish("Dita menjawab singkat. Tidak ada yang perlu dijelaskan lebih jauh."),
                UiTheme.HijauMeja, UiTheme.TextOnPaper);

            TextMeshProUGUI summary = UiKit.Paragraph(_content, SelaContent.Ui.BreakSummary, UiTheme.SmallFontSize,
                UiTheme.HijauMeja, TextAlignmentOptions.TopLeft);
        }

        private void BuildMessage()
        {
            TextMeshProUGUI text = UiKit.Paragraph(_content, _message, UiTheme.BodyFontSize,
                UiTheme.TextOnPaper, TextAlignmentOptions.TopLeft);

            TextMeshProUGUI summary = UiKit.Paragraph(_content, SelaContent.Ui.BreakSummary, UiTheme.SmallFontSize,
                UiTheme.HijauMeja, TextAlignmentOptions.TopLeft);

            Button next = UiKit.Button(_content, "Lanjut", Finish, UiTheme.BiruTinta, UiTheme.TextOnInk);
            UiKit.Height(next, 48f);
        }

        private void Finish(string message)
        {
            _message = message;
            Refresh();
        }

        private void Finish()
        {
            SelaState state = SelaState.Current;
            state.SetPressure(4);
            state.SetEnergy(Mathf.Max(state.Energy, 1));
            state.BreakUsedToday = true;
            state.NotifyChanged();
            SelaSaveStore.Write(state);
            _message = null;
            Router.Close(this);
            SelaBootstrap.Instance.FinishActivityEvents();
        }
    }
}
