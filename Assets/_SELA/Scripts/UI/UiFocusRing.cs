using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Menyalakan garis fokus saat elemen mendapat fokus keyboard (DESIGN 9.1).
    /// </summary>
    public class UiFocusRing : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        private Outline _outline;
        private Color _color;

        public void Setup(Color color)
        {
            _color = color;
            _outline = gameObject.GetComponent<Outline>();
            if (_outline == null)
            {
                _outline = gameObject.AddComponent<Outline>();
            }
            _outline.effectColor = color;
            _outline.effectDistance = new Vector2(UiTheme.FocusOutlineWidth, -UiTheme.FocusOutlineWidth);
            _outline.useGraphicAlpha = true;
            _outline.enabled = false;
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (_outline != null)
            {
                _outline.effectColor = _color;
                _outline.enabled = true;
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (_outline != null)
            {
                _outline.enabled = false;
            }
        }
    }
}
