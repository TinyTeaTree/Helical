using Agents;
using Core;
using Cysharp.Threading.Tasks;
using Services;

namespace Game
{
    public class BattleUnits : BaseVisualFeature<BattleUnitsVisual>, IBattleUnits, IAppLaunchAgent
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
            return _config?.GetBattleUnit(unitId);
        }
    }
}