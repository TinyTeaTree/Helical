using System;
using System.Threading.Tasks;
using Agents;
using Core;
using Cysharp.Threading.Tasks;

namespace Game
{
    public class GameLaunchFlow : SequenceFlow
    {
        public GameLaunchFlow(IBootstrap bootstrap)
        {
            var loginFlow = Create()
                .AddNext(asyncMethod: bootstrap.Features.Get<IPlayerAccount>().Login);
            
            this.AddNext(action: () => bootstrap.Agents.Get<IAppLaunchAgent>().AppLaunch())
                .AddNext(action: () => bootstrap.Features.Get<ILoadingScreen>().Show(LoadingScreenType.Start))
                .AddNext(asyncMethod: () => UniTask.Delay(TimeSpan.FromSeconds(2f))) // Pretend to load something
                .AddParallel(asyncMethod: loginFlow.ExecuteAsync)
                .AddNext(action: () => bootstrap.Features.Get<ILoadingScreen>().Hide())
                .AddNext(action: () => bootstrap.Features.Get<ILobby>().Show())
                .AddNext(asyncMethod: () => UniTask.Delay(TimeSpan.FromSeconds(0.5f)))
                .AddNext(action: () => bootstrap.Features.Get<ILobby>().DisplayOptions())
                ;
        }
    }
}