using Agents;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class Lobby : BaseVisualFeature<LobbyVisual>, ILobby, IAppLaunchAgent, ILoggedInAgent
    {
        [Inject] public LobbyRecord Record { get; set; }
        [Inject] public IHud Hud { get; set; }
        [Inject] public PlayerSettingsRecord SettingsRecord { get; set; }
        [Inject] public IPlayerSettings PlayerSettings { get; set; }

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

        public void SyncSettings()
        {
            SettingsRecord.Effects = _visual.EffectOn;
            SettingsRecord.Music = _visual.MusicOn;
            
            PlayerSettings.SaveSettings();
        }

        public void LoggedIn()
        {
            _visual.SetupSettings(SettingsRecord);
        }

        public void OnExitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif        
        }

        public void OnNewGameClicked()
        {
            new BattleLaunchFlow(_bootstrap).Execute();
        }
    }
}