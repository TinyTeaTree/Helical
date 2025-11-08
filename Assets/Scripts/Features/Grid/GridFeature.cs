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

        public UniTask LoadGrid(string gridId)
        {
            var gridSO = UnityEngine.Object.Instantiate(_gridResourcePack.GetGrid(gridId)); //Duplicate SO to not modify Resources

            var gridData = gridSO.GetData();

            Record.GridData = gridData;
            Record.GridId = gridId;

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
            var gridSO = _gridResourcePack.GetGrid(Record.GridId);
            position = gridSO.CameraAnchorPrefab.transform.position;
            rotation = gridSO.CameraAnchorPrefab.transform.rotation;
        }
    }
}