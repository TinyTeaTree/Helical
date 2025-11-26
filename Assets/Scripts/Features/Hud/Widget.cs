using UnityEngine;

namespace Game
{
    /// <summary>
    /// A MonoBehaviour widget that exists on the OnTopCanvas and can track a world transform.
    /// Attach this to prefabs that should be positioned on the HUD canvas based on world positions.
    /// </summary>
    public class Widget : MonoBehaviour
    {
        [SerializeField] private Vector3 _worldOffset;

        /// <summary>
        /// The world transform to track for positioning. Set this to make the widget follow a world object.
        /// </summary>
        public Transform TrackedTransform { get; set; }

        /// <summary>
        /// Optional offset to apply to the world position before converting to screen space
        /// </summary>
        public Vector3 WorldOffset
        {
            get => _worldOffset;
            set => _worldOffset = value;
        }

        /// <summary>
        /// Whether this widget is currently active and should be updated
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Updates the widget's position on the canvas based on the tracked transform.
        /// This should be called by the HudFeature each frame.
        /// </summary>
        /// <param name="camera">The camera to use for world-to-screen conversion</param>
        /// <param name="canvasRect">The canvas rect transform</param>
        public void UpdatePosition(Camera camera, RectTransform canvasRect)
        {
            if (!IsActive || TrackedTransform == null || camera == null)
            {
                gameObject.SetActive(false);
                return;
            }

            // Convert world position to screen position
            Vector3 worldPosition = TrackedTransform.position + _worldOffset;
            Vector3 screenPosition = camera.WorldToScreenPoint(worldPosition);

            // Check if the position is in front of the camera
            if (screenPosition.z > 0)
            {
                // Convert screen position to canvas position
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    screenPosition,
                    camera,
                    out Vector2 canvasPosition);

                // Update the widget's position
                var rectTransform = transform as RectTransform;
                rectTransform.anchoredPosition = canvasPosition;

                // Ensure the widget is visible
                gameObject.SetActive(true);
            }
            else
            {
                // Hide widgets behind the camera
                gameObject.SetActive(false);
            }
        }
    }
}
