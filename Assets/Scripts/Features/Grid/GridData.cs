using UnityEngine;

namespace Game
{
    public class GridData
    {
        public readonly int Width;
        public readonly int Height;
        private readonly HexData[,] Cells;

        public GridData(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new HexData[width, height];
        }

        public void SetCell(HexData hex)
        {
            Cells[hex.Coordinate.x, hex.Coordinate.y] = hex;
        }

        public HexData GetCell(Vector2Int coordinate)
        {
            return Cells[coordinate.x, coordinate.y];
        }
        
        public HexData GetCell(int x, int y)
        {
            return Cells[x, y];
        }
    }
}

