using UnityEngine;

public enum HexDirection
{
    North = 0,
    NorthEast = 60,
    SouthEast = 120,
    South = 180,
    SouthWest = 240,
    NorthWest = 300
}

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

    public static Quaternion ToRotation(this HexDirection direction)
    {
        return Quaternion.Euler(0f, (float)direction, 0f);
    }

    public static float ToAngle(this HexDirection direction)
    {
        return (float)direction;
    }
}
