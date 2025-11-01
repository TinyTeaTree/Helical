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

            _visual.Build(gridData, _gridResourcePack);
            return UniTask.CompletedTask;
        }
    }
}