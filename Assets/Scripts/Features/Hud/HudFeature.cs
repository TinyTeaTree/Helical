using System.Collections.Generic;
using System.Threading.Tasks;
using Agents;
using Core;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;

namespace Game
{
    public class HudFeature : BaseVisualFeature<HudVisual>, IHud, IAppLaunchAgent
    {
        [Inject] public ISummoningService Summoner { get; set; }

        private readonly Dictionary<Transform, WidgetRegistration> _widgetRegistrations = new Dictionary<Transform, WidgetRegistration>();
        private Camera _mainCamera;

        public bool IsReady { get; private set; }
        public Camera HudCamera => _visual?.HudCamera;
        public Transform HudRoot => _visual?.HudRoot;

        public void SetCanvas(Canvas visualCanvas)
        {
            if (!IsReady)
            {
                Notebook.NoteError("Can't call Hud while its not ready");
                return;
            }

            if (visualCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                visualCanvas.worldCamera = HudCamera;
                visualCanvas.planeDistance = 1f;
            }

            visualCanvas.transform.SetParent(HudRoot);
        }

        /// <summary>
        /// Creates and registers a new Widget from a prefab and tracked transform.
        /// </summary>
        /// <param name="widgetPrefab">The Widget prefab to instantiate</param>
        /// <param name="trackedTransform">The world transform to track</param>
        /// <returns>The created widget instance</returns>
        public Widget CreateWidget(Widget widgetPrefab, Transform trackedTransform)
        {
            if (!IsReady)
            {
                Notebook.NoteError("Can't create Widget while Hud is not ready");
                return null;
            }

            if (_widgetRegistrations.ContainsKey(trackedTransform))
            {
                Notebook.NoteError($"Widget already exists for tracked transform: {trackedTransform.name}");
                return null;
            }

            // Instantiate the widget using Summoner
            var widgetInstance = Summoner.CreateAsset(widgetPrefab, _visual.OnTopCanvas.transform);

            // Ensure the widget is anchored to bottom-left for proper screen space positioning
            var rectTransform = widgetInstance.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero; // bottom-left
            rectTransform.anchorMax = Vector2.zero; // bottom-left
            rectTransform.pivot = new Vector2(0.5f, 0.5f); // pivot at center

            // Set up the widget
            widgetInstance.TrackedTransform = trackedTransform;

            // Create registration and store it
            var registration = new WidgetRegistration(widgetInstance, trackedTransform);
            _widgetRegistrations[trackedTransform] = registration;

            return widgetInstance;
        }

        /// <summary>
        /// Destroys the widget associated with the specified tracked transform.
        /// </summary>
        /// <param name="trackedTransform">The transform that was being tracked by the widget</param>
        public void DestroyWidget(Transform trackedTransform)
        {
            if (_widgetRegistrations.TryGetValue(trackedTransform, out var registration))
            {
                _widgetRegistrations.Remove(trackedTransform);
                registration.Destroy();
            }
        }

        /// <summary>
        /// Gets the list of registered WidgetRegistrations and cleans up any with null transforms (used by HudVisual for position updates)
        /// </summary>
        internal List<WidgetRegistration> GetWidgetRegistrations()
        {
            // Clean up widgets with null transforms (indicates the tracked object was destroyed without proper cleanup)
            var transformsToRemove = new List<Transform>();
            foreach (var kvp in _widgetRegistrations)
            {
                if (kvp.Key == null)
                {
                    transformsToRemove.Add(kvp.Key);
                    kvp.Value.Destroy();
                    Notebook.NoteWarning("Widget was automatically cleaned up because its tracked transform became null. Consider calling DestroyWidget() when destroying tracked objects.");
                }
            }

            foreach (var transform in transformsToRemove)
            {
                _widgetRegistrations.Remove(transform);
            }

            return new List<WidgetRegistration>(_widgetRegistrations.Values);
        }

        public async UniTask AppLaunch()
        {
            await SetupVisual();
        }

        public async UniTask SetupVisual()
        {
            await CreateVisual();
            _visual.HudCamera = Camera.main;
            _mainCamera = Camera.main;
            IsReady = true;
        }
    }
}