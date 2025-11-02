using UnityEngine;

namespace Game
{
    /// <summary>
    /// Detection script for hex tiles. Should be attached to hex prefabs with a collider.
    /// Stores the hex coordinate and provides access to the HexOperator.
    /// </summary>
    [RequireComponent(typeof(HexOperator))]
    public class HexDetection : MonoBehaviour
    {
        [SerializeField] private HexOperator hexOperator;
        
        private Vector2Int _coordinate;
        private bool _isInitialized;

        /// <summary>
        /// Initializes the hex detection with its grid coordinate.
        /// This should be called when the hex is spawned.
        /// </summary>
        public void Initialize(Vector2Int coordinate)
        {
            _coordinate = coordinate;
            _isInitialized = true;
        }

        /// <summary>
        /// Gets the coordinate of this hex in the grid.
        /// </summary>
        public Vector2Int Coordinate => _coordinate;

        /// <summary>
        /// Gets the HexOperator component for controlling visual states.
        /// </summary>
        public HexOperator Operator => hexOperator;

        /// <summary>
        /// Returns whether this hex detection has been initialized with coordinates.
        /// </summary>
        public bool IsInitialized => _isInitialized;
    }
}

