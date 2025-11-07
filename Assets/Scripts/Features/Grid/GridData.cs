using UnityEngine;

namespace Game
{
    public class GridData
    {
        public string Id;
        public readonly int Width;
        public readonly int Height;
        private readonly HexData[,] Cells;

        public GridData(int width, int height, string id)
        {
            Width = width;
            Height = height;
            Cells = new HexData[width, height];
            Id = id;
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

