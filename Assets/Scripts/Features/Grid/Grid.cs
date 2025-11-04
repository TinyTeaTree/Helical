using Agents;
using Core;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;

namespace Game
{
    public class Grid : BaseVisualFeature<GridVisual>, IGrid, IAppLaunchAgent
    {
        [Inject] public GridRecord Record { get; set; }

        private GridResourcePack _gridResourcePack;
        
        public GridVisual Visual => _visual;
        
        public async UniTask AppLaunch()
        {
            await CreateVisual();

            _gridResourcePack = await Summoner.SummoningService.LoadAssetPack<GridResourcePack>();
        }

        public GridData ToGridData(GridSO gridSO)
        {
            if (gridSO == null)
            {
                return null;
            }

            var gridData = new GridData(gridSO.Width, gridSO.Height);
            
            foreach (var hexData in gridSO.Cells)
            {
                gridData.SetCell(hexData);
            }

            return gridData;
        }

        public GridSO FromGridData(GridData gridData, string id)
        {
            if (gridData == null)
            {
                return null;
            }

            var gridSO = ScriptableObject.CreateInstance<GridSO>();
            gridSO.Id = id;
            gridSO.Width = gridData.Width;
            gridSO.Height = gridData.Height;
            
            var cells = new System.Collections.Generic.List<HexData>();
            
            for (int x = 0; x < gridData.Width; x++)
            {
                for (int y = 0; y < gridData.Height; y++)
                {
                    var cell = gridData.GetCell(x, y);
                    if (cell.Type != HexType.None)
                    {
                        // Ensure coordinate matches grid position
                        var hexData = new HexData
                        {
                            Coordinate = new Vector2Int(x, y),
                            Type = cell.Type
                        };
                        cells.Add(hexData);
                    }
                }
            }

            gridSO.Cells = cells;
            
            return gridSO;
        }

        public UniTask LoadGrid(string gridId)
        {
            var gridSO = _gridResourcePack.GetGrid(gridId);
            var gridData = ToGridData(gridSO);

            // Store grid data in record for later access
            Record.GridData = gridData;

            _visual.Build(gridData, _gridResourcePack);
            return UniTask.CompletedTask;
        }

        public Vector3 GetWorldPosition(Vector2Int coordinate)
        {
            var worldXZ = coordinate.ToWorldXZ();
            return new Vector3(worldXZ.x, 0f, worldXZ.y);
        }

        public bool IsValidHex(Vector2Int coordinate)
        {
            if (Record.GridData == null)
            {
                return false;
            }

            // Check if coordinate is within grid bounds
            if (coordinate.x < 0 || coordinate.x >= Record.GridData.Width ||
                coordinate.y < 0 || coordinate.y >= Record.GridData.Height)
            {
                return false;
            }

            // Check if hex type is not None (empty)
            var hexData = Record.GridData.GetCell(coordinate);
            return hexData.Type != HexType.None;
        }

        public HexData GetHexData(Vector2Int coordinate)
        {
            if (Record.GridData == null)
            {
                return new HexData { Type = HexType.None };
            }

            // Check bounds
            if (coordinate.x < 0 || coordinate.x >= Record.GridData.Width ||
                coordinate.y < 0 || coordinate.y >= Record.GridData.Height)
            {
                return new HexData { Type = HexType.None };
            }

            return Record.GridData.GetCell(coordinate);
        }
    }
}