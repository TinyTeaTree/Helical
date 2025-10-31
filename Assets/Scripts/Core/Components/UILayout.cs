using UnityEngine;

namespace Core
{
    [RequireComponent(typeof(RectTransform))]
    public class UILayout : MonoBehaviour
    {
        [SerializeField] private RectTransform.Axis _axis;
        [SerializeField] private float _padding;
        [SerializeField] private float _leftMargin;
        [SerializeField] private float _rightMargin;
        
        private RectTransform RTransform => transform as RectTransform;

        [ContextMenu("Layout")]
        public void Layout()
        {
            float totalWidth = 0;
            for (int i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i);
                var childRect = (child as RectTransform).rect;
                var childWidth = childRect.width;

                totalWidth += childWidth;
            }

            totalWidth += (transform.childCount - 1) * _padding;
            totalWidth += _leftMargin + _rightMargin;
            
            RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);

            float position = -totalWidth / 2f + _leftMargin;
            for (int i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i) as RectTransform;
                var childRect = child.rect;
                var childWidth = childRect.width;

                child.anchoredPosition = new Vector2(position + childWidth / 2, 0);

                position += childWidth;
                position += _padding;
            }
        }
    }
}