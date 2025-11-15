using Agents;
using Core;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Services;
using UnityEngine;

namespace Game
{
    public class PurchasableUnitData
    {
        public string UnitId { get; set; }
        public string UnitName { get; set; }
        public Sprite UnitIcon { get; set; }
        public int GoldCost { get; set; }
    }

    public class CastleGUIFeature : BaseVisualFeature<CastleGUIVisual>, ICastleGUI, IBattleLaunchAgent
    {
        [Inject] public ICastle Castle { get; set; }
        [Inject] public ILocalConfigService ConfigService { get; set; }
        [Inject] public IGrid Grid { get; set; }
        [Inject] public IBattleUnits BattleUnits { get; set; }
        [Inject] public IGridSelection GridSelection { get; set; }
        [Inject] public IBattleAssets BattleAssets { get; set; }

        private BattleUnitsConfig _battleUnitsConfig;
        private BattleUnitsAssetPack _battleUnitsAssetPack;

        public async UniTask BattleLaunch()
        {
            await SetupVisual();
        }

        private async UniTask SetupVisual()
        {
            if (_visual != null)
            {
                Notebook.NoteError($"Visual already exists for {typeof(CastleGUIVisual)}");
                return;
            }

            // Load battle units config
            _battleUnitsConfig = ConfigService.GetConfig<BattleUnitsConfig>();
            _battleUnitsAssetPack = await Summoner.SummoningService.LoadAssetPack<BattleUnitsAssetPack>();


            await CreateVisual();
            _visual.gameObject.SetActive(true);
            HideCastleSelection();
        }

        public void ShowCastleSelection(Vector2Int coordinate)
        {
            string castleType = GetCastleTypeAtCoordinate(coordinate);
            List<PurchasableUnitData> purchasableUnits = GetPurchasableUnits(castleType);
            string castleName = GetCastleDisplayName(castleType);
            _visual.ShowCastleSelection(coordinate, castleName, purchasableUnits);
        }

        private string GetCastleTypeAtCoordinate(Vector2Int coordinate)
        {
            var gridData = Grid.GetGridData();
            foreach (var castleData in gridData.Castles)
            {
                if (castleData.Coordinate == coordinate)
                {
                    return castleData.CastleType;
                }
            }

            throw new System.InvalidOperationException($"No castle found at coordinate {coordinate}");
        }

        private string GetCastleDisplayName(string castleType)
        {
            var castleConfig = Castle.Config.GetCastleConfig(castleType);
            return castleConfig.DisplayName;
        }

        private List<PurchasableUnitData> GetPurchasableUnits(string castleType)
        {
            var purchasableUnits = new List<PurchasableUnitData>();
            var availableUnits = Castle.GetAvailableUnits(castleType);

            foreach (var unitEntry in availableUnits)
            {
                var battleUnitConfig = _battleUnitsConfig.GetBattleUnit(unitEntry.UnitId);

                var unitIcon = _battleUnitsAssetPack.GetUnitPhoto(unitEntry.UnitId);

                purchasableUnits.Add(new PurchasableUnitData
                {
                    UnitId = unitEntry.UnitId,
                    UnitName = battleUnitConfig.DisplayName,
                    UnitIcon = unitIcon,
                    GoldCost = unitEntry.GoldCost
                });
            }

            return purchasableUnits;
        }

        public void PurchaseUnit(string unitId)
        {
            Vector2Int castleCoordinate = GridSelection.GetSelectedCoordinate();
            string castleType = GetCastleTypeAtCoordinate(castleCoordinate);
            HexDirection castleDirection = GetCastleDirectionAtCoordinate(castleCoordinate);
            Vector2Int spawnCoordinate = GridUtils.NextHex(castleCoordinate, castleDirection);
            int unitCost = Castle.GetUnitCost(castleType, unitId);

            // Check if player can afford the unit
            if (!BattleAssets.CanAfford(unitCost))
            {
                Notebook.NoteWarning($"Cannot afford {unitId} - costs {unitCost} gold, have {BattleAssets.GoldAmount}");
                return;
            }

            // Check if the spawn location is valid
            if (!Grid.IsValidForAbility(AbilityMode.Spawn, spawnCoordinate))
            {
                Notebook.NoteWarning($"Cannot spawn {unitId} at {spawnCoordinate} - invalid location");
                return;
            }

            // Check if a unit already exists at the spawn location
            if (BattleUnits.GetUnitData(spawnCoordinate) != null)
            {
                Notebook.NoteWarning($"Cannot spawn {unitId} at {spawnCoordinate} - location occupied");
                return;
            }

            // Now deduct the gold
            BattleAssets.TrySpendGold(unitCost);

            Notebook.NoteData($"Purchasing {unitId} for {unitCost} gold at {castleType}");
            BattleUnits.SpawnUnitAtCoordinate(unitId, spawnCoordinate);
        }

        private HexDirection GetCastleDirectionAtCoordinate(Vector2Int coordinate)
        {
            var gridData = Grid.GetGridData();

            foreach (var castleData in gridData.Castles)
            {
                if (castleData.Coordinate == coordinate)
                {
                    return castleData.Direction;
                }
            }

            throw new System.InvalidOperationException($"No castle found at coordinate {coordinate}");
        }

        public void HideCastleSelection()
        {
            _visual.HideCastleSelection();
        }
    }
}