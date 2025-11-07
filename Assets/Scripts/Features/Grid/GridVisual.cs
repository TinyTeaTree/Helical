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
        private GameObject _glueInstance;

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

                    // Initialize HexOperator with coordinate
                    var hexOperator = instance.GetComponent<HexOperator>();
                    if (hexOperator != null)
                    {
                        hexOperator.Initialize(coordinate);
                    }

                    // Cache the hex data with its instance
                    _hexCache[coordinate] = (cell, instance);
                }
            }
        }

        public void BuildGlue(GameObject glue)
        {
            _glueInstance = Summoner.CreateAsset(glue, _gridTransform);
            _glueInstance.transform.localPosition = Vector3.zero;
        }

        public Dictionary<Vector2Int, (HexData hexData, HexOperator instance)> GetHexCache()
        {
            return _hexCache;
        }
        
        /// <summary>
        /// Gets the HexOperator at a specific coordinate
        /// </summary>
        public HexOperator GetHexOperatorAtCoordinate(Vector2Int coordinate)
        {
            if (_hexCache.TryGetValue(coordinate, out var cached))
            {
                return cached.instance.GetComponent<HexOperator>();
            }
            return null;
        }
    }
}