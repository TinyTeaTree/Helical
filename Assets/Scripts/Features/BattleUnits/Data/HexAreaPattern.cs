using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Represents a pattern of hexes relative to a unit's position and orientation.
    /// Uses Vector2Int coordinates (x,y) just like the game grid system.
    /// Used for defining ability target areas (attack ranges, movement patterns, etc.).
    /// </summary>
    [Serializable]
    public class HexAreaPattern
    {
        [Tooltip("Size of the pattern grid (odd numbers work best for centering)")]
        [SerializeField] private int _gridSize = 5;

        [Tooltip("Pattern data stored as 1D array. True = hex is included in pattern")]
        [SerializeField] private List<bool> _patternData = new List<bool>();

        /// <summary>
        /// Gets the size of the hex grid pattern
        /// </summary>
        public int GridSize => _gridSize;

        /// <summary>
        /// Gets the pattern data as a list of booleans
        /// </summary>
        public List<bool> PatternData => _patternData;

        /// <summary>
        /// Initializes the pattern with the specified grid size
        /// </summary>
        public void Initialize(int gridSize)
        {
            _gridSize = gridSize;
            int totalCells = gridSize * gridSize;
            _patternData = new List<bool>(new bool[totalCells]);
        }

        /// <summary>
        /// Sets whether a specific grid position is enabled
        /// </summary>
        /// <param name="x">X coordinate in the grid (increases right/east)</param>
        /// <param name="y">Y coordinate in the grid (increases up/north)</param>
        /// <param name="enabled">Whether this hex is part of the pattern</param>
        public void SetHex(int x, int y, bool enabled)
        {
            if (x < 0 || x >= _gridSize || y < 0 || y >= _gridSize)
                return;

            int index = y * _gridSize + x;
            if (index >= _patternData.Count)
                _patternData.AddRange(new bool[index - _patternData.Count + 1]);

            _patternData[index] = enabled;
        }

        /// <summary>
        /// Gets whether a specific grid position is enabled
        /// </summary>
        /// <param name="x">X coordinate in the grid</param>
        /// <param name="y">Y coordinate in the grid</param>
        /// <returns>True if the hex is part of the pattern</returns>
        public bool GetHex(int x, int y)
        {
            if (x < 0 || x >= _gridSize || y < 0 || y >= _gridSize)
                return false;

            int index = y * _gridSize + x;
            return index < _patternData.Count && _patternData[index];
        }

        /// <summary>
        /// Gets all enabled hex coordinates relative to the center
        /// </summary>
        /// <param name="orientation">Unit's facing direction</param>
        /// <returns>List of world coordinates relative to unit position</returns>
        public List<Vector2Int> GetEnabledHexes(HexDirection orientation)
        {
            var result = new List<Vector2Int>();
            int center = _gridSize / 2;

            for (int y = 0; y < _gridSize; y++)
            {
                for (int x = 0; x < _gridSize; x++)
                {
                    if (GetHex(x, y))
                    {
                        // Convert grid coordinates to relative hex coordinates
                        int relativeX = x - center;
                        int relativeY = y - center;

                        // Apply orientation transformation
                        var relativeCoord = ApplyOrientation(new Vector2Int(relativeX, relativeY), orientation);
                        result.Add(relativeCoord);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Applies orientation transformation to relative coordinates
        /// </summary>
        private Vector2Int ApplyOrientation(Vector2Int relativeCoord, HexDirection direction)
        {
            // Each hex direction represents a 60-degree rotation
            int rotations = (int)direction / 60;
            for (int i = 0; i < rotations; i++)
            {
                // Rotate 60 degrees clockwise in hex space
                // For hex grids, rotation follows specific patterns
                var rotated = RotateHexCoordinate(relativeCoord);
                relativeCoord = rotated;
            }
            return relativeCoord;
        }

        /// <summary>
        /// Rotates a hex coordinate 60 degrees clockwise
        /// </summary>
        private Vector2Int RotateHexCoordinate(Vector2Int coord)
        {
            // Convert to axial coordinates, rotate, convert back
            var axial = OffsetToAxial(coord);
            var rotated = new Vector2Int(-axial.y, axial.x + axial.y);
            return AxialToOffset(rotated);
        }

        /// <summary>
        /// Converts offset coordinates to axial coordinates
        /// </summary>
        private Vector2Int OffsetToAxial(Vector2Int offset)
        {
            int x = offset.x;
            int y = offset.y - (offset.x - (offset.x & 1)) / 2;
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// Converts axial coordinates to offset coordinates
        /// </summary>
        private Vector2Int AxialToOffset(Vector2Int axial)
        {
            int x = axial.x;
            int y = axial.y + (axial.x - (axial.x & 1)) / 2;
            return new Vector2Int(x, y);
        }
    }
}
