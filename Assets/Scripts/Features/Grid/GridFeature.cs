using Agents;
using Core;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;

namespace Game
{
    public class GridFeature : BaseVisualFeature<GridVisual>, IGrid, IAppLaunchAgent
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

            var gridData = new GridData(gridSO.Width, gridSO.Height, gridSO.Id);
            
            foreach (var hexData in gridSO.Cells)
            {
                gridData.SetCell(hexData);
            }

            return gridData;
        }

        public UniTask LoadGrid(string gridId)
        {
            var gridSO = _gridResourcePack.GetGrid(gridId);
            if (gridSO == null)
            {
                Notebook.NoteError($"GridSO with id {gridId} not found.");
                Record.GridData = null;
                return UniTask.CompletedTask;
            }

            var gridData = ToGridData(gridSO);

            // Store grid data in record for later access
            Record.GridData = gridData;

            _visual.Build(gridData, _gridResourcePack);
            _visual.BuildGlue(gridSO.GluePrefab);
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
        
        public HexOperator GetHexOperatorAtCoordinate(Vector2Int coordinate)
        {
            return _visual.GetHexOperatorAtCoordinate(coordinate);
        }

        public void GetCameraAnchor(out Vector3 position, out Quaternion rotation)
        {
            var gridSO = _gridResourcePack.GetGrid(Record.GridData.Id);
            position = gridSO.CameraAnchorPrefab.transform.position;
            rotation = gridSO.CameraAnchorPrefab.transform.rotation;
        }
    }
}