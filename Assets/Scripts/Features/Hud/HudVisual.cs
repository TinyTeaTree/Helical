using Core;
using UnityEngine;

namespace Game
{
    public class HudVisual : BaseVisual<Hud>
    {
        public Camera HudCamera { get; set; }
    
        public Transform HudRoot => transform;
    }
}