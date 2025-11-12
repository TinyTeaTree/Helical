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
        [Inject] public IBattleUnits BattleUnits { get; set; }

        private GridResourcePack _gridResourcePack;
        
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

        public GridData GetGridData()
        {
            return Record.GridData;
        }

        public GridVisual GetGridVisual()
        {
            return _visual;
        }

        public bool IsValidForAbility(AbilityMode ability, Vector2Int coordinate)
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

            var hexData = Record.GridData.GetCell(coordinate);

            // Check if hex type is not None (empty)
            if (hexData.Type == HexType.None)
            {
                return false;
            }

            // Check ability-specific rules
            switch (ability)
            {
                case AbilityMode.Move:
                case AbilityMode.Spawn:
                    // Cannot move or spawn on water
                    if (hexData.Type == HexType.Water)
                    {
                        return false;
                    }
                    // Cannot move to hexes that already have units
                    if (BattleUnits.GetUnitData(coordinate) != null)
                    {
                        return false;
                    }
                    break;

                case AbilityMode.Attack:
                    // For attack, we might want different rules later
                    // For now, allow attacking any valid hex
                    break;

                case AbilityMode.Rotate:
                    // For rotate, we might want different rules later
                    // For now, allow rotating on any valid hex
                    break;

                default:
                    // For other abilities, just check basic validity
                    break;
            }

            return true;
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