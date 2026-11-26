using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Panel kertas di atas latar gelap, dengan judul dan area isi.
    /// Semua layar panel memakai komponen ini agar posisinya konsisten (DESIGN 9.4).
    /// </summary>
    public class UiSheet
    {
        public RectTransform Root;
        public RectTransform Sheet;
        public RectTransform Content;
        public RectTransform Footer;
        public TextMeshProUGUI TitleText;

        public UiSheet(RectTransform parent, string title, Vector2 size, bool showCloseButton)
        {
            Root = UiKit.ModalLayer(parent, "Layer");
            Root.SetAsLastSibling();

            Image paper = UiKit.PaperPanel(Root, "Sheet", UiTheme.LatarPanel);
            Sheet = paper.rectTransform;
            Sheet.anchorMin = new Vector2(0.5f, 0.5f);
            Sheet.anchorMax = new Vector2(0.5f, 0.5f);
            Sheet.pivot = new Vector2(0.5f, 0.5f);
            Sheet.sizeDelta = size;

            RectTransform header = UiKit.Box(Sheet, "Header", new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(28f, -78f), new Vector2(-28f, -20f));

            TitleText = UiKit.TextIn(header, title, UiTheme.TitleFontSize, UiTheme.TextOnPaper, TextAlignmentOptions.Left, true);

            UiKit.Divider(Sheet, UiTheme.HijauMeja);

            Content = UiKit.Box(Sheet, "Content", Vector2.zero, Vector2.one,
                new Vector2(28f, 78f), new Vector2(-28f, -92f));

            if (showCloseButton)
            {
                Button close = UiKit.Button(Sheet, "x", null, new Color(0f, 0f, 0f, 0f), UiTheme.TextOnPaper);
                RectTransform rt = close.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(1f, 1f);
                rt.anchorMax = new Vector2(1f, 1f);
                rt.pivot = new Vector2(1f, 1f);
                rt.anchoredPosition = new Vector2(-20f, -16f);
                rt.sizeDelta = new Vector2(44f, 40f);
                close.GetComponent<Image>().raycastTarget = true;

                if (UiRouter.Instance != null)
                {
                    close.onClick.AddListener(UiRouter.Instance.CloseTop);
                }
            }
        }

        /// <summary>
        /// Area tombol tetap terlihat di bawah area scroll. Halaman baca memakai ini supaya
        /// tombol Kembali tidak ikut tergulir (DESIGN 9.15).
        /// </summary>
        public RectTransform EnsureFooter(float height = 68f)
        {
            if (Footer != null)
            {
                return Footer;
            }

            Footer = UiKit.Box(Sheet, "Footer", new Vector2(0f, 0f), new Vector2(1f, 0f),
                new Vector2(28f, 18f), new Vector2(-28f, 18f + height));

            // Area isi dikecilkan supaya tidak tertutup footer.
            Content.offsetMin = new Vector2(28f, 18f + height + 10f);
            return Footer;
        }

        /// <summary>Menambahkan baris tombol di bagian bawah panel.</summary>
        public RectTransform FooterButton(Transform parent, string label, System.Action onClick, bool primary)
        {
            Color background = primary ? UiTheme.BiruTinta : UiTheme.HijauMeja;
            Color text = primary ? UiTheme.TextOnInk : UiTheme.TextOnPaper;
            Button button = UiKit.Button(parent, label, onClick, background, text);
            return button.GetComponent<RectTransform>();
        }
    }
}
