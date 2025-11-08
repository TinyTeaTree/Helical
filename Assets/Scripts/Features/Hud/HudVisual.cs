using Core;
using UnityEngine;

namespace Game
{
    public class HudVisual : BaseVisual<HudFeature>
    {
        public Camera HudCamera { get; set; }
    
        public Transform HudRoot => transform;
    }
}