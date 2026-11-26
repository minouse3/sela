using System;
using SELA.Narrative;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Dialog konfirmasi tunggal. Semua peringatan memakai komponen yang sama supaya
    /// tidak ada window baru setiap kali dibutuhkan (DESIGN 9.14).
    /// </summary>
    public class ConfirmDialog : UiScreen
    {
        private UiSheet _sheet;
        private RectTransform _content;
        private RectTransform _footer;
        private string _bodyText = string.Empty;
        private string _confirmLabel = string.Empty;
        private Action _onConfirm;

        protected override string ScreenName
        {
            get { return "Confirm"; }
        }

        protected override void Build()
        {
            _sheet = new UiSheet(Root, string.Empty, new Vector2(560f, 360f), false);
            _content = UiKit.Rect(_sheet.Content, "Content");
            UiKit.VerticalStack(_content, 14f, new RectOffset(0, 0, 0, 0));
            _footer = _sheet.EnsureFooter(60f);
            UiKit.HorizontalStack(_footer, 12f, new RectOffset(0, 0, 0, 0));
        }

        /// <summary>Menampilkan dialog. Aksi hanya jalan bila tombol konfirmasi ditekan.</summary>
        public void Ask(string title, string body, string confirmLabel, Action onConfirm)
        {
            _bodyText = body ?? string.Empty;
            _confirmLabel = string.IsNullOrEmpty(confirmLabel) ? SelaContent.Ui.Confirm : confirmLabel;
            _onConfirm = onConfirm;
            _sheet.TitleText.text = title;
            Refresh();
        }

        public override void Refresh()
        {
            ClearChildren(_content);
            ClearChildren(_footer);

            TextMeshProUGUI body = UiKit.Paragraph(_content, _bodyText, UiTheme.BodyFontSize, UiTheme.TextOnPaper,
                TextAlignmentOptions.TopLeft);

            UiKit.Button(_footer, SelaContent.Ui.Cancel, Router.CloseTop, UiTheme.HijauMeja, UiTheme.TextOnPaper);
            UiKit.Button(_footer, _confirmLabel, Confirm, UiTheme.BiruTinta, UiTheme.TextOnInk);
        }

        private static void ClearChildren(RectTransform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);
            }
        }

        private void Confirm()
        {
            Action action = _onConfirm;
            _onConfirm = null;
            Router.Close(this);
            if (action != null)
            {
                action();
            }
        }
    }
}
