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
    /// Gets the adjacent coordinate in the specified direction from the given coordinate.
    /// </summary>
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
}
