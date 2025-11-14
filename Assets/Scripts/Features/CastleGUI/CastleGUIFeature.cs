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
            // Get castle type from grid data
            string castleType = GetCastleTypeAtCoordinate(coordinate);

            // Get purchasable units for this castle type
            List<PurchasableUnitData> purchasableUnits = GetPurchasableUnits(castleType);

            // Get castle display name
            string castleName = GetCastleDisplayName(castleType);

            // Show the castle GUI with unit data
            _visual.ShowCastleSelection(coordinate, castleName, purchasableUnits);
        }

        private string GetCastleTypeAtCoordinate(Vector2Int coordinate)
        {
            var gridData = Grid.GetGridData();
            if (gridData.Castles == null)
            {
                return null;
            }

            foreach (var castleData in gridData.Castles)
            {
                if (castleData.Coordinate == coordinate)
                {
                    return castleData.CastleType;
                }
            }

            return null;
        }

        private string GetCastleDisplayName(string castleType)
        {
            var castleConfig = Castle.Config.GetCastleConfig(castleType);
            return castleConfig != null ? castleConfig.DisplayName : castleType;
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

        public void HideCastleSelection()
        {
            _visual.HideCastleSelection();
        }
    }
}