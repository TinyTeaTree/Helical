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

public static class HexDirectionExtensions
{
    public static Quaternion ToRotation(this HexDirection direction)
    {
        return Quaternion.Euler(0f, (float)direction, 0f);
    }

    public static float ToAngle(this HexDirection direction)
    {
        return (float)direction;
    }
    
    /// <summary>
    /// Converts HexDirection to a normalized direction vector in world space (XZ plane)
    /// </summary>
    public static Vector3 ToVector3(this HexDirection direction)
    {
        float angleRad = direction.ToAngle() * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angleRad), 0f, Mathf.Cos(angleRad));
    }
    
    /// <summary>
    /// Calculates the nearest HexDirection from sourceCoordinate to targetCoordinate using dot product
    /// </summary>
    public static HexDirection GetNearestDirection(Vector2Int sourceCoordinate, Vector2Int targetCoordinate)
    {
        // Calculate world positions
        Vector2 sourceWorld = sourceCoordinate.ToWorldXZ();
        Vector2 targetWorld = targetCoordinate.ToWorldXZ();
        
        // Calculate direction vector (in XZ plane)
        Vector3 directionVector = new Vector3(
            targetWorld.x - sourceWorld.x,
            0f,
            targetWorld.y - sourceWorld.y
        ).normalized;
        
        // Test all 6 hex directions and find the one with highest dot product
        HexDirection bestDirection = HexDirection.North;
        float bestDotProduct = -2f; // Start with impossible value
        
        HexDirection[] allDirections = new HexDirection[]
        {
            HexDirection.North,
            HexDirection.NorthEast,
            HexDirection.SouthEast,
            HexDirection.South,
            HexDirection.SouthWest,
            HexDirection.NorthWest
        };
        
        foreach (var hexDir in allDirections)
        {
            Vector3 hexDirVector = hexDir.ToVector3();
            float dotProduct = Vector3.Dot(directionVector, hexDirVector);
            
            if (dotProduct > bestDotProduct)
            {
                bestDotProduct = dotProduct;
                bestDirection = hexDir;
            }
        }
        
        return bestDirection;
    }
    
    /// <summary>
    /// Calculates the rotation direction from current to target HexDirection.
    /// Returns +1 for clockwise rotation, -1 for counter-clockwise rotation, 0 if already facing target.
    /// </summary>
    public static int GetRotationDirection(this HexDirection current, HexDirection target)
    {
        if (current == target)
        {
            return 0;
        }
        
        float currentAngle = current.ToAngle();
        float targetAngle = target.ToAngle();
        
        // Calculate the difference
        float diff = targetAngle - currentAngle;
        
        // Normalize to [-180, 180] range
        while (diff > 180f)
            diff -= 360f;
        while (diff < -180f)
            diff += 360f;
        
        // Positive difference means clockwise, negative means counter-clockwise
        return diff > 0 ? 1 : -1;
    }
}

