using UnityEngine;

namespace Game
{
    /// <summary>
    /// Controls the visual state of a hex tile.
    /// Manages outline glow and mesh elevation between normal and highlighted states.
    /// </summary>
    public class HexOperator : MonoBehaviour
    {
        [SerializeField] private GameObject outline;
        [SerializeField] private Transform mesh;

        private Vector3 _normalPosition;
        private Vector3 _elevatedPosition;
        private bool _isGlowing;

        private const float ElevationHeight = 0.1f;

        private void Awake()
        {
            _normalPosition = mesh.localPosition;
            _elevatedPosition = _normalPosition + new Vector3(0, ElevationHeight, 0);
            
            // Start in normal state
            SetNormalState();
        }

        /// <summary>
        /// Sets the hex to normal state: outline hidden, mesh at origin.
        /// </summary>
        public void SetNormalState()
        {
            _isGlowing = false;
            outline.SetActive(false);
            mesh.localPosition = _normalPosition;
        }

        /// <summary>
        /// Sets the hex to glowing state: outline visible, mesh elevated.
        /// </summary>
        public void SetGlowingState()
        {
            _isGlowing = true;
            outline.SetActive(true);
            mesh.localPosition = _elevatedPosition;
        }

        /// <summary>
        /// Toggles between normal and glowing states.
        /// </summary>
        public void ToggleState()
        {
            if (_isGlowing)
            {
                SetNormalState();
            }
            else
            {
                SetGlowingState();
            }
        }

        /// <summary>
        /// Gets whether the hex is currently in glowing state.
        /// </summary>
        public bool IsGlowing => _isGlowing;
    }
}

