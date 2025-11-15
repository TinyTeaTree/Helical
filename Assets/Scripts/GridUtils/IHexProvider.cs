using UnityEngine;

namespace Game
{
    /// <summary>
    /// Interface for providing hex grid data to pathfinding algorithms.
    /// </summary>
    public interface IHexProvider
    {
        /// <summary>
        /// Gets the grid data containing hex information.
        /// </summary>
        GridData GetGridData();

        /// <summary>
        /// Checks if a coordinate is valid for movement (within bounds and traversable).
        /// </summary>
        bool IsValidForMovement(Vector2Int coordinate);

        /// <summary>
        /// Checks if a coordinate is occupied by a unit (blocking movement).
        /// </summary>
        bool IsOccupied(Vector2Int coordinate);
    }
}
