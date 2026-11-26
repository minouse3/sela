using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Dasar tiap layar UI. Layar dibangun runtime oleh UiRouter, bukan dari prefab.
    /// </summary>
    public abstract class UiScreen
    {
        public RectTransform Root { get; private set; }
        public UiRouter Router { get; private set; }
        public bool IsOpen { get; private set; }

        /// <summary>Layar modal memblokir panel di belakangnya (DESIGN 9.4).</summary>
        public virtual bool IsModal
        {
            get { return true; }
        }

        protected abstract string ScreenName { get; }

        protected abstract void Build();

        public void Initialise(UiRouter router, RectTransform parent)
        {
            Router = router;
            Root = UiKit.Rect(parent, ScreenName);
            Root.gameObject.SetActive(false);
            Build();
        }

        public virtual void OnOpen()
        {
            IsOpen = true;
            Root.gameObject.SetActive(true);

            // Layar yang baru dibuka menggambar di atas layar yang sudah ada,
            // sehingga panel modal selalu menutupi HUD.
            Root.SetAsLastSibling();
            Refresh();
        }

        public virtual void OnClose()
        {
            IsOpen = false;
            Root.gameObject.SetActive(false);
        }

        /// <summary>Dipanggil setiap state berubah atau panel dibuka kembali.</summary>
        public virtual void Refresh()
        {
        }

        /// <summary>Memilih elemen fokus pertama saat panel dibuka.</summary>
        public virtual void FocusFirst()
        {
            Selectable first = Root.GetComponentInChildren<Selectable>();
            if (first != null)
            {
                EventSystem.current.SetSelectedGameObject(first.gameObject);
            }
        }

        /// <summary>Menutup panel teratas. Mengembalikan false bila panel ini tidak boleh ditutup dengan Esc.</summary>
        public virtual bool CloseFromEscape()
        {
            return true;
        }

        protected TextMeshProUGUI Title(string text, Color color)
        {
            TextMeshProUGUI tmp = UiKit.Text(Root, text, UiTheme.TitleFontSize, color, TextAlignmentOptions.Left, true);
            RectTransform rt = tmp.rectTransform;
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.offsetMin = new Vector2(0f, -60f);
            rt.offsetMax = new Vector2(0f, -14f);
            return tmp;
        }
    }
}
