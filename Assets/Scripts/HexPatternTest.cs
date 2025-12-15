using UnityEngine;

namespace Game
{
    /// <summary>
    /// Test component to demonstrate the HexAreaPattern custom inspector.
    /// Attach this to any GameObject to see the hex pattern editor in action.
    /// Grid is positioned below the field, North (increasing y) goes upward.
    /// </summary>
    public class HexPatternTest : MonoBehaviour
    {
        [Tooltip("Test hex area pattern - yellow center = unit position, North goes up")]
        public HexAreaPattern testPattern;
    }
}
