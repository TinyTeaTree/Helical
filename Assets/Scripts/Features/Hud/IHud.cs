using Core;
using UnityEngine;

namespace Game
{
    public interface IHud : IFeature
    {
        bool IsReady { get; }
        Camera HudCamera { get; }
        Transform HudRoot { get; }
        void SetCanvas(Canvas visualCanvas);

        /// <summary>
        /// Creates and registers a new Widget from a prefab and tracked transform.
        /// </summary>
        /// <param name="widgetPrefab">The Widget prefab to instantiate</param>
        /// <param name="trackedTransform">The world transform to track</param>
        /// <returns>The created widget instance</returns>
        Widget CreateWidget(Widget widgetPrefab, Transform trackedTransform);

        /// <summary>
        /// Destroys the widget associated with the specified tracked transform.
        /// </summary>
        /// <param name="trackedTransform">The transform that was being tracked by the widget</param>
        void DestroyWidget(Transform trackedTransform);
    }
}