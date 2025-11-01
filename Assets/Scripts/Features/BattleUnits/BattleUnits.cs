using Agents;
using Core;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;

namespace Game
{
    public class BattleUnits : BaseVisualFeature<BattleUnitsVisual>, IBattleUnits, IAppLaunchAgent, IBattleLaunchAgent
    {
        [Inject] public BattleUnitsRecord Record { get; set; }
        [Inject] public ILocalConfigService ConfigService { get; set; }

        private BattleUnitsConfig _config;
        private BattleUnitsAssetPack _assetPack;

        public BattleUnitsConfig Config => _config;
        public BattleUnitsAssetPack AssetPack => _assetPack;

        public async UniTask AppLaunch()
        {
            await CreateVisual();

            _config = ConfigService.GetConfig<BattleUnitsConfig>();
            _assetPack = await Summoner.SummoningService.LoadAssetPack<BattleUnitsAssetPack>();
        }

        public BattleUnitConfig GetUnitConfig(string unitId)
        {
            return _config.GetBattleUnit(unitId);
        }

        public UniTask BattleLaunch()
        {
            Record.BattleUnits.Clear();
            
            // Temporary until we have a spawn system
            Record.BattleUnits.Add(new BattleUnitData()
            {
                BattleUnitId = "Skeleton",
                Coordinate = new Vector2Int(15, 15),
                Health = 40,
                IsDead = false,
                PlayerId = _bootstrap.Features.Get<IPlayerAccount>().PlayerId
            });
            
            return UniTask.CompletedTask;
        }
    }
}