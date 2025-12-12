using System.Collections.Generic;
using UnityEngine;

public static class GridUtils
{
    public const float NextGridStep = 1f;
    public const float HalfGridStep = 0.5f;
    public static readonly float AdjacentGridStep = 1 / Mathf.Sqrt(3f);
    public static readonly float HexMinRadius = AdjacentGridStep * 0.5f;
    public static readonly float HexMaxRadius = 1 / Mathf.Sqrt(9.6f);
    public static readonly float HexUpStep = AdjacentGridStep * 0.5f;
    public static readonly float HexLevelStep = HexMinRadius * 2;
    public static readonly float HexScaleModifier = HexMaxRadius * 2f;
    

    /// <summary>
    /// Converts hex grid coordinates to 2D mathematical coordinates for calculations.
    /// Returns (x, y) representing position on a 2D plane accounting for hex grid staggering.
    /// Used for UI calculations, direction math, and grid-based algorithms.
    /// </summary>
    /// <param name="coord">The hex grid coordinate to convert</param>
    /// <returns>Vector2 representing 2D mathematical position</returns>
    public static Vector2 ToWorldXZ(this Vector2Int coord)
    {
        Vector2 pos = new Vector2
        {
            x = coord.x * HalfGridStep,
            y = coord.y * HexLevelStep
        };

        pos.y += (coord.x % 2) * HexUpStep;

        return pos;
    }
    
    /// <summary>
    /// Converts hex grid coordinates to 3D Unity world coordinates.
    /// Returns (x, 0, z) representing actual world position for Unity transforms.
    /// Used for positioning game objects, projectiles, and 3D world calculations.
    /// </summary>
    /// <param name="coord">The hex grid coordinate to convert</param>
    /// <returns>Vector3 representing 3D world position with Y=0</returns>
    public static Vector3 ToWorldX0Z(this Vector2Int coord)
    {
        Vector3 pos = new Vector3
        {
            x = coord.x * HalfGridStep,
            y = 0,
            z = coord.y * HexLevelStep
        };

        pos.z += (coord.x % 2) * HexUpStep;

        return pos;
    }

    /// <summary>
    /// Gets the adjacent coordinate in the specified direction from the given coordinate.
    /// Accounts for hex grid staggering (even/odd rows have different neighbor patterns).
    /// </summary>
    /// <param name="fromCoord">The starting coordinate</param>
    /// <param name="direction">The hex direction to move</param>
    /// <returns>The coordinate one step away in the specified direction</returns>
    public static Vector2Int NextHex(Vector2Int fromCoord, HexDirection direction)
    {
        bool isEvenRow = fromCoord.x % 2 == 0;

        return direction switch
        {
            HexDirection.North => new Vector2Int(fromCoord.x, fromCoord.y + 1),
            HexDirection.South => new Vector2Int(fromCoord.x, fromCoord.y - 1),
            HexDirection.NorthEast => isEvenRow
                ? new Vector2Int(fromCoord.x + 1, fromCoord.y)
                : new Vector2Int(fromCoord.x + 1, fromCoord.y + 1),
            HexDirection.SouthEast => isEvenRow
                ? new Vector2Int(fromCoord.x + 1, fromCoord.y - 1)
                : new Vector2Int(fromCoord.x + 1, fromCoord.y),
            HexDirection.NorthWest => isEvenRow
                ? new Vector2Int(fromCoord.x - 1, fromCoord.y)
                : new Vector2Int(fromCoord.x - 1, fromCoord.y + 1),
            HexDirection.SouthWest => isEvenRow
                ? new Vector2Int(fromCoord.x - 1, fromCoord.y - 1)
                : new Vector2Int(fromCoord.x - 1, fromCoord.y),
            _ => fromCoord
        };
    }

    /// <summary>
    /// Calculates the hex distance between two coordinates using cube coordinates.
    /// Returns the number of hex steps needed to travel from one hex to another.
    /// This is the minimum number of hex moves required to go from point A to point B.
    /// </summary>
    /// <param name="a">First hex coordinate</param>
    /// <param name="b">Second hex coordinate</param>
    /// <returns>Integer distance in hex steps</returns>
    public static int HexDistance(Vector2Int a, Vector2Int b)
    {
        // Convert to cube coordinates for distance calculation
        var cubeA = AxialToCube(a);
        var cubeB = AxialToCube(b);

        // Calculate distance using cube coordinates
        return Mathf.Max(
            Mathf.Abs(cubeA.x - cubeB.x),
            Mathf.Abs(cubeA.y - cubeB.y),
            Mathf.Abs(cubeA.z - cubeB.z)
        );
    }

    /// <summary>
    /// Converts axial coordinates (x, y) to cube coordinates (x, y, z)
    /// </summary>
    private static Vector3Int AxialToCube(Vector2Int axial)
    {
        int x = axial.x;
        int z = axial.y - (axial.x - (axial.x & 1)) / 2; // Adjust for even/odd rows
        int y = -x - z;
        return new Vector3Int(x, y, z);
    }

    /// <summary>
    /// Gets all coordinates within a given hex distance from a center coordinate.
    /// Returns all hex coordinates that are within 'range' steps of the center coordinate.
    /// Includes the center coordinate itself when range >= 0.
    /// </summary>
    /// <param name="center">The center coordinate to search from</param>
    /// <param name="range">Maximum hex distance to include (0 = only center, 1 = adjacent, etc.)</param>
    /// <returns>List of all coordinates within the specified range</returns>
    public static List<Vector2Int> GetCoordinatesInRange(Vector2Int center, int range)
    {
        var result = new List<Vector2Int>();

        for (int dx = -range; dx <= range; dx++)
        {
            int minDy = Mathf.Max(-range, -dx - range);
            int maxDy = Mathf.Min(range, -dx + range);

            for (int dy = minDy; dy <= maxDy; dy++)
            {
                var coord = new Vector2Int(center.x + dx, center.y + dy);
                if (HexDistance(center, coord) <= range)
                {
                    result.Add(coord);
                }
            }
        }

        return result;
    }
}
