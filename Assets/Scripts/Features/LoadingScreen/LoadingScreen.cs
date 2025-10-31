using System.Linq;
using System.Threading.Tasks;
using Agents;
using Core;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;

namespace Game
{
    public class LoadingScreen : BaseVisualFeature<LoadingScreenVisual>, ILoadingScreen, IAppLaunchAgent
    {
        [Inject] public LoadingScreenRecord Record { get; set; }
        [Inject] public ISummoningService Summoner { get; set; }

        public bool IsShowing { get; private set; }

        private LoadingScreenConfig Config { get; set; }

        public async UniTask AppLaunch()
        {
            Config = _bootstrap.Services.Get<ILocalConfigService>().GetConfig<LoadingScreenConfig>();
            await CreateVisual();
            var hud = _bootstrap.Features.Get<IHud>();
            await TaskUtils.WaitUntil(() => hud.IsReady);
            hud.SetCanvas(_visual.Canvas);
        }

        public void Show(LoadingScreenType type)
        {
            if (IsShowing)
            {
                Notebook.NoteWarning("Loading screen is already showing");
                return;
            }

            Record.Progress = 0f;

            var assetPath = Config.Datas.First(d => d.Type == type).AssetPath;

            var page = Summoner.LoadResource<LoadingScreenPage>(assetPath);

            _visual.ShowPage(page);

            IsShowing = true;
        }

        public void SetProgress(float normalizedProgress)
        {
            if (!IsShowing)
            {
                Notebook.NoteWarning("Loading Screen is not showing");
                return;
            }

            Record.Progress = normalizedProgress;
            _visual.UpdateProgress();
        }

        public void Hide()
        {
            if (!IsShowing)
            {
                Notebook.NoteWarning("Loading Screen is not showing");
                return;
            }

            _visual.Close();
            IsShowing = false;
        }
    }
}