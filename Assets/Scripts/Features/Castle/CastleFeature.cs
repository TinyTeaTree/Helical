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

        private CastleAssetPack _castleAssetPack;

        public CastleAssetPack CastleAssetPack => _castleAssetPack;

        public async UniTask AppLaunch()
        {
            await CreateVisual();
            _castleAssetPack = await Summoner.SummoningService.LoadAssetPack<CastleAssetPack>();
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