using UnityEngine;

namespace Game
{
    [System.Serializable]
    public struct HexData
    {
        public Vector2Int Coordinate;
        public HexType Type;
    }
}