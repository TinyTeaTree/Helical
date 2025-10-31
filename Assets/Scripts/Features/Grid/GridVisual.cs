using System.Collections.Generic;
using Core;
using Services;
using UnityEngine;

namespace Game
{
    public class GridVisual : BaseVisual<Grid>
    {
        [SerializeField] private Transform _gridTransform;
        private Dictionary<Vector2Int, (HexData hexData, GameObject instance)> _hexCache = new Dictionary<Vector2Int, (HexData, GameObject)>();

        public void Build(GridData gridData, GridResourcePack resourcePack)
        {
            if (gridData == null || resourcePack == null)
            {
                return;
            }

            // Create Row transforms and instantiate hexes
            for (int x = 0; x < gridData.Width; x++)
            {
                // Create Row parent for this row
                var rowObject = new GameObject($"Row_{x}");
                rowObject.transform.SetParent(_gridTransform);
                rowObject.transform.localPosition = Vector3.zero;

                for (int y = 0; y < gridData.Height; y++)
                {
                    var cell = gridData.GetCell(x, y);
                    var coordinate = new Vector2Int(x, y);

                    var prefab = resourcePack.GetHex(cell.Type);
                    if (prefab == null)
                    {
                        continue;
                    }

                    var instance = Summoner.CreateAsset(prefab, rowObject.transform);
                    instance.name = $"Hex_({x},{y})";
                    
                    var worldXZ = cell.Coordinate.ToWorldXZ();
                    instance.transform.localPosition = new Vector3(worldXZ.x, 0f, worldXZ.y);

                    // Cache the hex data with its instance
                    _hexCache[coordinate] = (cell, instance);
                }
            }
        }

        public Dictionary<Vector2Int, (HexData hexData, GameObject instance)> GetHexCache()
        {
            return _hexCache;
        }
    }
}