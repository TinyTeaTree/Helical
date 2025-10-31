using UnityEngine;

public class WagUIFitter : MonoBehaviour
{
    [SerializeField] RectTransform _toFit;
    [SerializeField] Rect _padding;
    [SerializeField] bool _horizontal;
    [SerializeField] bool _vertical;

    RectTransform rectTransform => transform as RectTransform;

    [ContextMenu("Fit")]
    public void Fit()
    {
        if (_horizontal)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _toFit.rect.width + _padding.width);
        }
        if (_vertical)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _toFit.rect.height + _padding.height);
        }
    }
}
