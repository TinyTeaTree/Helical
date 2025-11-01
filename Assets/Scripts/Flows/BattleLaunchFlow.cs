using System;
using Agents;
using Core;
using Cysharp.Threading.Tasks;

namespace Game
{
    public class BattleLaunchFlow : SequenceFlow
    {
        public BattleLaunchFlow(IBootstrap bootstrap)
        {
            this.AddNext(action: () => bootstrap.Features.Get<ILobby>().Hide())
                .AddNext(action: () => bootstrap.Features.Get<ILoadingScreen>().Show(LoadingScreenType.Battle))
                .AddNext(asyncMethod: () => UniTask.Delay(TimeSpan.FromSeconds(2f)))
                .AddNext(asyncMethod: () => bootstrap.Features.Get<IGrid>().LoadGrid("Basic"))
                .AddNext(asyncMethod: () => bootstrap.Features.Get<ICameraMove>().SetupVisual())
                .AddNext(action: () => bootstrap.Features.Get<ICameraMove>().InitializeBounds())
                .AddNext(action: () => bootstrap.Features.Get<ICameraMove>().Start())
                .AddNext(action: () => bootstrap.Features.Get<ILoadingScreen>().Hide())
                ;
        }
    }
}

