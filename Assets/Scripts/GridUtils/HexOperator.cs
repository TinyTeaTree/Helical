using UnityEngine;

namespace Game
{
    /// <summary>
    /// Controls the visual state of a hex tile.
    /// Manages outline glow and mesh elevation between normal and highlighted states.
    /// </summary>
    public class HexOperator : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private MeshRenderer _outlineMeshRenderer;

        [SerializeField] private GameObject _playerOutline;
        [SerializeField] private GameObject _botOutline;
        
        private Vector2Int _coordinate;

        //private const float ElevationHeight = 0.1f;

        private void Awake()
        {
            // Start in normal state
            UpdateSelectedMaterial(false);
            UpdatePlayerUnit(false, false);
        }

        /// <summary>
        /// Sets the hex selection state.
        /// </summary>
        public void SetSelected(bool selected)
        {
            UpdateSelectedMaterial(selected);
        }

        /// <summary>
        /// Sets whether this hex has a player unit.
        /// </summary>
        public void SetHasPlayerUnit(bool hasUnit)
        {
            UpdatePlayerUnit(hasUnit, false);
        }

        /// <summary>
        /// Sets whether this hex has a bot unit.
        /// </summary>
        public void SetHasBotUnit(bool hasUnit)
        {
            UpdatePlayerUnit(false, hasUnit);
        }

        /// <summary>
        /// Updates the material based on current state.
        /// Priority: Selection > Bot Outline > Player Outline > Normal
        /// </summary>
        private void UpdateSelectedMaterial(bool isSelected)
        {
            _meshRenderer.material.SetFloat("_GlowIntensity", isSelected ? 0.5f : 0f);
        }
        
        private void UpdatePlayerUnit(bool isPlayer, bool isBot)
        {
            _playerOutline.gameObject.SetActive(isPlayer);
            _botOutline.gameObject.SetActive(isBot);
        }

        /// <summary>
        /// Legacy method for compatibility - sets selected state.
        /// </summary>
        public void SetNormalState()
        {
            SetSelected(false);
        }

        /// <summary>
        /// Legacy method for compatibility - sets selected state.
        /// </summary>
        public void SetGlowingState()
        {
            SetSelected(true);
        }

        /// <summary>
        /// Initializes the hex with its grid coordinate.
        /// </summary>
        public void Initialize(Vector2Int coordinate)
        {
            _coordinate = coordinate;
        }

        /// <summary>
        /// Gets the coordinate of this hex in the grid.
        /// </summary>
        public Vector2Int Coordinate => _coordinate;
    }
}

