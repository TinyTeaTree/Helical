using System.Collections.Generic;
using Agents;
using Core;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;

namespace Game
{
    public class CastleFeature : BaseVisualFeature<CastleVisual>, ICastle, IAppLaunchAgent, IBattleLaunchAgent
    {
        [Inject] public CastleRecord Record { get; set; }
        [Inject] public IGrid Grid { get; set; }
        [Inject] public ILocalConfigService ConfigService { get; set; }

        private CastleAssetPack _castleAssetPack;
        private CastleConfig _config;

        public CastleAssetPack CastleAssetPack => _castleAssetPack;
        public CastleConfig Config => _config;

        public async UniTask AppLaunch()
        {
            await CreateVisual();
            _castleAssetPack = await Summoner.SummoningService.LoadAssetPack<CastleAssetPack>();
            _config = ConfigService.GetConfig<CastleConfig>();
        }

        public List<CastleUnitPurchaseEntry> GetAvailableUnits(string castleType)
        {
            return _config.GetAvailableUnits(castleType);
        }

        public int GetUnitCost(string castleType, string unitId)
        {
            return _config.GetUnitCost(castleType, unitId);
        }

        public UniTask BattleLaunch()
        {
            var gridData = Grid.GetGridData();
            if (gridData.Castles == null)
            {
                return UniTask.CompletedTask;
            }

            foreach (var castleData in gridData.Castles)
            {
                _visual.SpawnCastle(castleData);
            }

            return UniTask.CompletedTask;
        }
    }
}