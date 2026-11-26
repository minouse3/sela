using System.Text;
using UnityEngine;
using UnityEngine.UI;
using SELA.UI;

public static class ScrollProbe
{
    public static string Run()
    {
        StringBuilder sb = new StringBuilder();
        Transform ui = GameObject.Find("SELA UI").transform;
        ScrollRect[] all = ui.GetComponentsInChildren<ScrollRect>(true);
        foreach (ScrollRect s in all)
        {
            if (!s.gameObject.activeInHierarchy) { continue; }
            RectTransform c = s.content;
            RectTransform v = s.viewport;
            sb.Append("rectH=").Append(s.GetComponent<RectTransform>().rect.height.ToString("0"));
            sb.Append(" viewH=").Append(v != null ? v.rect.height.ToString("0") : "null");
            sb.Append(" contentH=").Append(c != null ? c.rect.height.ToString("0") : "null");
            ScrollBarDriver d = s.GetComponent<ScrollBarDriver>();
            if (d != null)
            {
                sb.Append(" trackH=").Append(d.Track.rect.height.ToString("0"));
                sb.Append(" thumbH=").Append(d.Thumb.rect.height.ToString("0"));
                sb.Append(" thumbY=").Append(d.Thumb.anchoredPosition.y.ToString("0"));
                sb.Append(" norm=").Append(s.verticalNormalizedPosition.ToString("0.00"));
            }
            sb.Append(" | ");
        }
        return sb.ToString();
    }
}
