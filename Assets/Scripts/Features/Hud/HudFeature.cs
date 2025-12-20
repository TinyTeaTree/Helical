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

        private readonly List<WidgetRegistration> _widgetRegistrations = new List<WidgetRegistration>();
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

            // Instantiate the widget using Summoner
            var widgetInstance = Summoner.CreateAsset(widgetPrefab, _visual.OnTopCanvas.transform);

            // Ensure the widget is anchored to bottom-left for proper screen space positioning
            var rectTransform = widgetInstance.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero; // bottom-left
            rectTransform.anchorMax = Vector2.zero; // bottom-left
            rectTransform.pivot = new Vector2(0.5f, 0.5f); // pivot at center

            // Set up the widget
            widgetInstance.TrackedTransform = trackedTransform;

            // Create registration and add it to the list
            var registration = new WidgetRegistration(widgetInstance, trackedTransform);
            _widgetRegistrations.Add(registration);

            // Position the widget immediately to avoid positioning issues on the first frame
            registration.UpdatePosition(_mainCamera, _visual.OnTopCanvas.transform as RectTransform);

            return widgetInstance;
        }

        /// <summary>
        /// Destroys the specified widget instance.
        /// </summary>
        /// <param name="widget">The widget instance to destroy</param>
        public void DestroyWidget(Widget widget)
        {
            var registration = _widgetRegistrations.Find(r => r.Widget == widget);
            if (registration != null)
            {
                _widgetRegistrations.Remove(registration);
                registration.Destroy();
            }
        }

        /// <summary>
        /// Gets the list of registered WidgetRegistrations and cleans up any destroyed widgets (used by HudVisual for position updates)
        /// </summary>
        internal List<WidgetRegistration> GetWidgetRegistrations()
        {
            // Clean up widgets that have been destroyed without proper cleanup
            var registrationsToRemove = new List<WidgetRegistration>();
            foreach (var registration in _widgetRegistrations)
            {
                if (registration.Widget == null || registration.Widget.Equals(null))
                {
                    registrationsToRemove.Add(registration);
                    registration.Destroy();
                    Notebook.NoteWarning("Widget was automatically cleaned up because it was destroyed without calling DestroyWidget().");
                }
            }

            foreach (var registration in registrationsToRemove)
            {
                _widgetRegistrations.Remove(registration);
            }

            return new List<WidgetRegistration>(_widgetRegistrations);
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