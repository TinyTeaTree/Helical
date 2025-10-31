using Agents;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class Lobby : BaseVisualFeature<LobbyVisual>, ILobby, IAppLaunchAgent
    {
        [Inject] public LobbyRecord Record { get; set; }
        [Inject] public IHud Hud { get; set; }
        
        public void Show()
        {
            _visual.Show();
            Hud.SetCanvas(_visual.GetComponent<Canvas>());
        }

        public void DisplayOptions()
        {
            _visual.DisplayOptions();
        }

        public void Hide()
        {
            _visual.Hide(immediate: false);
        }

        public async UniTask AppLaunch()
        {
            await CreateVisual();
            _visual.Hide(immediate: true);
        }
    }
}