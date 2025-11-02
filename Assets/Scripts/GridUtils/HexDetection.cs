using UnityEngine;

namespace Game
{
    /// <summary>
    /// Detection script for hex tiles. Should be attached to hex prefabs with a collider.
    /// Provides access to the HexOperator.
    /// </summary>
    public class HexDetection : MonoBehaviour
    {
        [SerializeField] private HexOperator hexOperator;

        /// <summary>
        /// Gets the coordinate of this hex from the HexOperator.
        /// </summary>
        public Vector2Int Coordinate => hexOperator.Coordinate;

        /// <summary>
        /// Gets the HexOperator component for controlling visual states.
        /// </summary>
        public HexOperator Operator => hexOperator;
    }
}

