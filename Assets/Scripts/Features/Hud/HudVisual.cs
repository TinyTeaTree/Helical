using Core;
using UnityEngine;

namespace Game
{
    public class HudVisual : BaseVisual<HudFeature>
    {

        [SerializeField] private Canvas _onTopCanvas;

        public Camera HudCamera { get; set; }

        public Transform HudRoot => transform;
        public Canvas OnTopCanvas => _onTopCanvas;

        private void Update()
        {
            UpdateOnTopWidgets();
        }

        private void UpdateOnTopWidgets()
        {
            if (Feature == null || HudCamera == null)
            {
                return;
            }

            // Access the widget registrations from the feature and update their positions
            foreach (var registration in Feature.GetWidgetRegistrations())
            {
                registration.UpdatePosition(HudCamera, OnTopCanvas.transform as RectTransform);
            }
        }
    }
}