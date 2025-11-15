using UnityEngine;

namespace Game
{
    [System.Serializable]
    public struct CastleData
    {
        public Vector2Int Coordinate;
        public HexDirection Direction;
        public string CastleType;
    }

    [System.Serializable]
    public struct PredeterminedUnitData
    {
        public string UnitId;
        public Vector2Int Coordinate;
        public int Level;
        public string PlayerId; // "Bot" for bot units, player ID for player units
    }

    [System.Serializable]
    public class GridData
    {
        public int Width;
        public int Height;
        public HexData[] Cells;
        public CastleData[] Castles;
        public PredeterminedUnitData[] PredeterminedUnits;

        public void SetCell(HexData hex)
        {
            Cells[GetIndex(hex.Coordinate.x, hex.Coordinate.y)] = hex;
        }

        public HexData GetCell(Vector2Int coordinate)
        {
            return Cells[GetIndex(coordinate.x, coordinate.y)];
        }
        
        public HexData GetCell(int x, int y)
        {
            return Cells[GetIndex(x, y)];
        }

        private int GetIndex(int x, int y)
        {
            return y * Width + x;
        }
    }
}

