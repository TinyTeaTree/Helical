using UnityEngine;

namespace Game
{
    /// <summary>
    /// Represents a registration of a Widget instance with the HudFeature.
    /// Contains the instantiated Widget MonoBehaviour and tracking information.
    /// </summary>
    public class WidgetRegistration
    {
        /// <summary>
        /// The instantiated Widget MonoBehaviour
        /// </summary>
        public Widget Widget { get; private set; }

        /// <summary>
        /// The world transform being tracked by this widget
        /// </summary>
        public Transform TrackedTransform { get; private set; }

        public WidgetRegistration(Widget widget, Transform trackedTransform)
        {
            Widget = widget;
            TrackedTransform = trackedTransform;
        }

        /// <summary>
        /// Updates the widget's position. Called by HudFeature each frame.
        /// </summary>
        public void UpdatePosition(Camera camera, RectTransform canvasRect)
        {
            if (Widget != null)
            {
                Widget.UpdatePosition(camera, canvasRect);
            }
        }

        /// <summary>
        /// Destroys the widget instance
        /// </summary>
        public void Destroy()
        {
            if (Widget != null)
            {
                Object.Destroy(Widget.gameObject);
                Widget = null;
            }
        }
    }
}
