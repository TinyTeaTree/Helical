using System.Collections.Generic;
using Core;
using Services;
using UnityEngine;

namespace Game
{
    public class GridVisual : BaseVisual<Grid>
    {
        [SerializeField] private Transform _gridTransform;
        private Dictionary<Vector2Int, (HexData hexData, HexOperator instance)> _hexCache = new();

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
                    if(cell.Type == HexType.None)
                        continue;
                    
                    var coordinate = new Vector2Int(x, y);

                    var prefab = resourcePack.GetHex(cell.Type);
                    var instance = Summoner.CreateAsset(prefab, rowObject.transform);
                    instance.name = $"Hex_({x},{y})";
                    
                    var worldXZ = cell.Coordinate.ToWorldXZ();
                    instance.transform.localPosition = new Vector3(worldXZ.x, 0f, worldXZ.y);
                    instance.transform.localScale = Vector3.one * GridUtils.HexScaleModifier;

                    // Initialize HexDetection if present
                    var hexDetection = instance.GetComponent<HexDetection>();
                    if (hexDetection != null)
                    {
                        hexDetection.Initialize(coordinate);
                    }

                    // Cache the hex data with its instance
                    _hexCache[coordinate] = (cell, instance);
                }
            }
        }

        public Dictionary<Vector2Int, (HexData hexData, HexOperator instance)> GetHexCache()
        {
            return _hexCache;
        }
    }
}