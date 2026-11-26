using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Mengatur scrollbar milik SELA: thumb hanya muncul kalau isi area baca lebih
    /// tinggi dari viewport, panjangnya mengikuti perbandingan isi, dan bisa
    /// digeser dengan mouse. ScrollRect bawaan selalu menyiapkan ruang scrollbar,
    /// jadi komponen ini yang menggambar batangnya sendiri.
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollBarDriver : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        public RectTransform Track;
        public RectTransform Thumb;

        private ScrollRect _scroll;
        private Image _thumbImage;
        private Color _visible = UiTheme.HijauMeja;
        private bool _lastNeeded = true;
        private float _dragOriginY;
        private float _dragOriginPosition;

        private void Awake()
        {
            _scroll = GetComponent<ScrollRect>();
        }

        private void LateUpdate()
        {
            if (_thumbImage == null)
            {
                if (Thumb == null)
                {
                    return;
                }

                _thumbImage = Thumb.GetComponent<Image>();
                if (_thumbImage == null)
                {
                    return;
                }
            }

            float contentHeight = ContentHeight();
            float viewHeight = ViewHeight();
            bool needed = contentHeight > viewHeight + 6f;

            if (needed != _lastNeeded)
            {
                _lastNeeded = needed;
                _thumbImage.color = needed ? _visible : new Color(0f, 0f, 0f, 0f);
            }

            if (!needed)
            {
                return;
            }

            LayoutThumb(contentHeight, viewHeight);
        }

        private void LayoutThumb(float contentHeight, float viewHeight)
        {
            float trackHeight = Track.rect.height;
            float ratio = Mathf.Clamp01(viewHeight / Mathf.Max(contentHeight, 1f));
            float thumbHeight = Mathf.Max(24f, trackHeight * ratio);
            float travel = Mathf.Max(0f, trackHeight - thumbHeight);

            RectTransform rect = Thumb;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, thumbHeight);
            rect.anchoredPosition = new Vector2(0f, -(1f - _scroll.verticalNormalizedPosition) * travel);
        }

        private float ContentHeight()
        {
            RectTransform content = _scroll.content;
            return content != null ? content.rect.height : 0f;
        }

        private float ViewHeight()
        {
            RectTransform viewport = _scroll.viewport;
            return viewport != null ? viewport.rect.height : 0f;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragOriginY = eventData.position.y;
            _dragOriginPosition = _scroll.verticalNormalizedPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            float trackHeight = Track.rect.height;
            float travel = trackHeight - Thumb.rect.height;
            if (travel <= 0.5f)
            {
                return;
            }

            float delta = (eventData.position.y - _dragOriginY) / travel;
            _scroll.verticalNormalizedPosition = Mathf.Clamp01(_dragOriginPosition + delta);
        }
    }
}
