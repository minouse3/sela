using SELA.Gameplay;
using SELA.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Sesi interview: potret, dua pertanyaan, tiga respons, dan koreksi klaim (DESIGN 9.10).
    /// </summary>
    public class InterviewScreen : UiScreen
    {
        private OfferId _offer = OfferId.A;
        private int _questionIndex;
        private int _answerIndex = -1;
        private bool _offeredCorrection;
        private bool _finished;
        private int _totalValue;
        private bool _everyAnswerAccepted = true;
        private bool _costApplied;

        private UiSheet _sheet;
        private RectTransform _portraitBox;
        private RectTransform _questionArea;
        private RectTransform _responseArea;
        private TextMeshProUGUI _portraitName;
        private TextMeshProUGUI _portraitRole;
        private TextMeshProUGUI _questionTitle;
        private TextMeshProUGUI _questionText;
        private TextMeshProUGUI _answerText;

        protected override string ScreenName
        {
            get { return "Interview"; }
        }

        public void SetOffer(OfferId offer)
        {
            _offer = offer;
            _questionIndex = 0;
            _answerIndex = -1;
            _offeredCorrection = false;
            _finished = false;
            _totalValue = 0;
            _everyAnswerAccepted = true;
            _costApplied = false;
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, "Interview", new Vector2(1080f, 600f), false);

            _portraitBox = UiKit.Box(_sheet.Content, "Portrait", new Vector2(0f, 0f), new Vector2(0f, 1f),
                new Vector2(0f, 0f), new Vector2(300f, 0f));

            Image portraitBg = UiKit.PaperPanel(_portraitBox, "PortraitCard", UiTheme.LatarSlot);
            RectTransform portraitRect = portraitBg.rectTransform;
            portraitRect.anchorMin = new Vector2(0f, 1f);
            portraitRect.anchorMax = new Vector2(1f, 1f);
            portraitRect.pivot = new Vector2(0.5f, 1f);
            portraitRect.offsetMin = new Vector2(0f, -320f);
            portraitRect.offsetMax = new Vector2(0f, 0f);

            _portraitName = UiKit.TextIn(portraitBg.transform, string.Empty, UiTheme.SectionFontSize,
                UiTheme.TextOnPaper, TextAlignmentOptions.Center, true);
            _portraitName.rectTransform.anchorMin = new Vector2(0f, 0f);
            _portraitName.rectTransform.anchorMax = new Vector2(1f, 1f);
            _portraitName.rectTransform.offsetMin = new Vector2(10f, 10f);
            _portraitName.rectTransform.offsetMax = new Vector2(-10f, -10f);

            _portraitRole = UiKit.TextIn(_portraitBox, string.Empty, UiTheme.SmallFontSize, UiTheme.TextOnPaper,
                TextAlignmentOptions.TopLeft);
            _portraitRole.rectTransform.anchorMin = new Vector2(0f, 1f);
            _portraitRole.rectTransform.anchorMax = new Vector2(1f, 1f);
            _portraitRole.rectTransform.pivot = new Vector2(0.5f, 1f);
            _portraitRole.rectTransform.offsetMin = new Vector2(0f, -356f);
            _portraitRole.rectTransform.offsetMax = new Vector2(0f, -320f);

            _questionArea = UiKit.Box(_sheet.Content, "Question", Vector2.zero, Vector2.one,
                new Vector2(320f, 0f), new Vector2(0f, 0f));

            _questionTitle = UiKit.Text(_questionArea, string.Empty, UiTheme.SmallFontSize, UiTheme.HijauMeja,
                TextAlignmentOptions.TopLeft, true);
            RectTransform qTitleRect = _questionTitle.rectTransform;
            qTitleRect.anchorMin = new Vector2(0f, 1f);
            qTitleRect.anchorMax = new Vector2(1f, 1f);
            qTitleRect.pivot = new Vector2(0.5f, 1f);
            qTitleRect.offsetMin = new Vector2(0f, -30f);
            qTitleRect.offsetMax = new Vector2(0f, 0f);

            _questionText = UiKit.Text(_questionArea, string.Empty, UiTheme.SectionFontSize, UiTheme.TextOnPaper,
                TextAlignmentOptions.TopLeft, true);
            RectTransform qRect = _questionText.rectTransform;
            qRect.anchorMin = new Vector2(0f, 1f);
            qRect.anchorMax = new Vector2(1f, 1f);
            qRect.pivot = new Vector2(0.5f, 1f);
            qRect.offsetMin = new Vector2(0f, -84f);
            qRect.offsetMax = new Vector2(0f, -34f);

            _responseArea = UiKit.Box(_questionArea, "Responses", Vector2.zero, Vector2.one,
                new Vector2(0f, 0f), new Vector2(0f, -90f));
            UiKit.VerticalStack(_responseArea, 10f, new RectOffset(0, 0, 0, 0));

            _answerText = UiKit.Text(_questionArea, string.Empty, UiTheme.BodyFontSize, UiTheme.ArangHijau,
                TextAlignmentOptions.TopLeft);
            RectTransform aRect = _answerText.rectTransform;
            aRect.anchorMin = new Vector2(0f, 0f);
            aRect.anchorMax = new Vector2(1f, 0f);
            aRect.pivot = new Vector2(0.5f, 0f);
            aRect.offsetMin = new Vector2(0f, 0f);
            aRect.offsetMax = new Vector2(0f, 90f);
        }

        public override void Refresh()
        {
            _portraitName.text = SelaContent.Interviews.RecruiterName(_offer);
            _portraitRole.text = SelaContent.Interviews.RecruiterRole(_offer) + "\n" +
                                 SelaContent.OfferTitle(_offer);

            Clear(_responseArea);
            _answerText.text = string.Empty;

            if (_finished)
            {
                BuildResult();
                return;
            }

            InterviewQuestion[] questions = SelaContent.Interviews.Questions(_offer);
            InterviewQuestion question = questions[Mathf.Clamp(_questionIndex, 0, questions.Length - 1)];

            _questionTitle.text = SelaContent.Ui.QuestionIndex + " " + (_questionIndex + 1) + "/" + questions.Length;
            _questionText.text = question.Question;

            if (!_offeredCorrection && _questionIndex == 0 && SelaState.Current.ApplicationCorrected[(int)_offer] == false)
            {
                Button correct = UiKit.Button(_responseArea, SelaContent.Ui.InterviewCorrection, () =>
                {
                    SelaState.Current.ApplicationCorrected[(int)_offer] = true;
                    _offeredCorrection = true;
                    Refresh();
                }, UiTheme.TanahTirai, UiTheme.TextOnPaper);
                UiKit.Height(correct, 44f);

                Button continueUncorrected = UiKit.Button(_responseArea, SelaContent.Ui.InterviewContinue, () =>
                {
                    _offeredCorrection = true;
                    Refresh();
                }, UiTheme.LatarSlot, UiTheme.TextOnPaper);
                UiKit.Height(continueUncorrected, 44f);
                return;
            }

            for (int i = 0; i < question.Responses.Length; i++)
            {
                int captured = i;
                Button button = UiKit.Button(_responseArea, question.Responses[i], () =>
                {
                    _answerIndex = captured;
                    ApplySessionCost();
                    Refresh();
                }, UiTheme.LatarSlot, UiTheme.TextOnPaper);
                UiKit.Height(button, 62f);
            }

            if (_answerIndex >= 0)
            {
                _answerText.text = "\"" + question.Responses[_answerIndex] + "\"";
                Button next = UiKit.Button(_responseArea, SelaContent.Ui.InterviewNext, Next,
                    UiTheme.BiruTinta, UiTheme.TextOnInk);
                UiKit.Height(next, 44f);
            }
        }

        private void Next()
        {
            InterviewQuestion[] questions = SelaContent.Interviews.Questions(_offer);
            InterviewQuestion question = questions[Mathf.Clamp(_questionIndex, 0, questions.Length - 1)];

            int value = AnswerValue(question, _answerIndex);
            _totalValue += value;
            if (value < 1)
            {
                _everyAnswerAccepted = false;
            }

            if (_questionIndex + 1 < questions.Length)
            {
                _questionIndex++;
                _answerIndex = -1;
                Refresh();
                return;
            }
            _finished = true;
            Refresh();
        }

        /// <summary>
        /// Nilai internal jawaban. Pertanyaan pertama bernilai 2 hanya bila bukti yang relevan
        /// ikut dalam snapshot lamaran, kalau tidak nilainya 0 (GDD 3.7).
        /// </summary>
        private int AnswerValue(InterviewQuestion question, int answerIndex)
        {
            int value = question.Values[Mathf.Clamp(answerIndex, 0, question.Values.Length - 1)];
            if (_questionIndex == 0 && value == 2)
            {
                EvidenceId needed = _offer == OfferId.A ? EvidenceId.E1 : EvidenceId.E2;
                if (!SelaState.Current.SentHasEvidence(_offer, needed))
                {
                    return 0;
                }
            }
            return value;
        }

        private bool RequiredEvidenceSent()
        {
            SelaState state = SelaState.Current;
            if (!state.SentHasEvidence(_offer, EvidenceId.E1))
            {
                return false;
            }
            return _offer != OfferId.B || state.SentHasEvidence(_offer, EvidenceId.E2);
        }

        /// <summary>
        /// Hasil interview. Tawaran diberikan bila bukti wajib ada pada snapshot, tidak ada klaim
        /// berlebihan yang belum diluruskan, setiap jawaban bernilai minimal 1, dan total
        /// minimal 3 dari 4 (GDD 3.7).
        /// </summary>
        private void BuildResult()
        {
            SelaState state = SelaState.Current;

            bool evidenceOk = RequiredEvidenceSent();
            bool claimsOk = !state.SentHasExcessiveClaim(_offer) || state.ApplicationCorrected[(int)_offer];
            bool answersOk = _everyAnswerAccepted && _totalValue >= 3;
            bool granted = evidenceOk && claimsOk && answersOk;

            _questionTitle.text = granted ? "Tawaran" : "Belum lolos";

            TextMeshProUGUI reasons = UiKit.Text(_responseArea, string.Empty, UiTheme.SmallFontSize,
                UiTheme.ArangHijau, TextAlignmentOptions.TopLeft);
            RectTransform rRect = reasons.rectTransform;
            rRect.anchorMin = new Vector2(0f, 0f);
            rRect.anchorMax = new Vector2(1f, 1f);
            rRect.offsetMin = Vector2.zero;
            rRect.offsetMax = Vector2.zero;

            if (granted)
            {
                _questionText.text = SelaContent.Interviews.OfferLetter(_offer);
                reasons.text = "Tawaran ini berlaku sampai malam hari ketujuh. Pilihannya bisa diganti " +
                               "atau ditarik sebelum keputusan difinalisasi.";

                state.SetStatus(_offer, ApplicationStatus.Ditawari);
                state.OfferChoice = (OfferChoice)((int)_offer + 1);
                state.ApplicationInterviewScore[(int)_offer] = _totalValue;
                state.ApplicationOutcome[(int)_offer] = "Tawaran diterima: " + SelaContent.OfferName(_offer) + ".";
            }
            else
            {
                string[] reasons3 = SelaContent.Interviews.RejectionReasons();
                string main = !evidenceOk ? reasons3[0] : !claimsOk ? reasons3[1] : reasons3[2];

                _questionText.text = "Belum ada tawaran dari " + SelaContent.OfferName(_offer) + ".";
                reasons.text = main + "\n\nAlasan lain yang juga berlaku tersimpan di Catatan.";

                state.SetStatus(_offer, ApplicationStatus.Ditolak);
                state.ApplicationInterviewScore[(int)_offer] = -2;
                state.ApplicationOutcome[(int)_offer] = main;
            }

            state.NotifyChanged();
            SelaSaveStore.Write(state);

            Button close = UiKit.Button(_responseArea, SelaContent.Ui.Close, CloseResult,
                UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Height(close, 44f);

            if (granted)
            {
                Button offer = UiKit.Button(_responseArea, "Baca tawaran", () =>
                {
                    CloseResult();
                    SelaBootstrap.Instance.OpenDecision();
                }, UiTheme.BiruTinta, UiTheme.TextOnInk);
                UiKit.Height(offer, 44f);
            }
        }

        private void CloseResult()
        {
            _finished = false;
            _questionIndex = 0;
            _answerIndex = -1;
            Router.Close(this);
            SelaBootstrap.Instance.AdvanceSessionAfterActivity();
        }

        /// <summary>Biaya interview satu sesi berlaku sekali, saat jawaban pertama dipilih (GDD 3.3).</summary>
        private void ApplySessionCost()
        {
            if (_costApplied)
            {
                return;
            }
            _costApplied = true;

            SelaState state = SelaState.Current;
            SelaSessionFlow.Resolve(state, SelaActivity.IkutiInterview);
            state.NotifyChanged();
            SelaSaveStore.Write(state);
        }

        private static void Clear(RectTransform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(parent.GetChild(i).gameObject);
            }
        }
    }
}
