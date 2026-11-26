using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SELA.UI
{
    /// <summary>
    /// Membuat tinggi elemen teks mengikuti isi teksnya, jadi paragraf panjang tidak pernah
    /// menimpa isi yang ada di bawahnya. TextMeshPro tidak mengimplementasikan ILayoutElement,
    /// jadi komponen ini menjembataninya: layout group menanyakan tinggi yang diukur TMP.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TmpAutoHeight : MonoBehaviour, ILayoutElement
    {
        /// <summary>Tinggi minimum, dipakai kalau isinya cuma satu-dua baris.</summary>
        public float MinHeight = 0f;

        /// <summary>Tambahan ruang di bawah teks, supaya baris terakhir tidak menempel.</summary>
        public float BottomPadding = 0f;

        public float preferredHeight
        {
            get { return Mathf.Max(MinHeight, Measured()); }
        }

        public float minHeight
        {
            get { return MinHeight; }
        }

        public float preferredWidth
        {
            get { return 0f; }
        }

        public float minWidth
        {
            get { return 0f; }
        }

        public float flexibleHeight
        {
            get { return 0f; }
        }

        public float flexibleWidth
        {
            get { return 0f; }
        }

        public int layoutPriority
        {
            get { return 1; }
        }

        public void CalculateLayoutInputHorizontal()
        {
        }

        public void CalculateLayoutInputVertical()
        {
        }

        private float Measured()
        {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            if (text == null)
            {
                return 0f;
            }

            RectTransform rect = (RectTransform)transform;
            float width = rect.rect.width;
            if (width <= 1f)
            {
                return 0f;
            }

            if (text.textWrappingMode == TextWrappingModes.NoWrap)
            {
                return text.preferredHeight + BottomPadding;
            }

            return text.GetPreferredValues(text.text, width, 0f).y + BottomPadding;
        }
    }
}
