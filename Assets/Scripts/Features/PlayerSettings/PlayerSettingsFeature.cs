using Core;

namespace Game
{
    public class PlayerSettingsFeature : BaseFeature, IPlayerSettings
    {
        [Inject] public PlayerSettingsRecord Record { get; set; }
        [Inject] public IPlayerSaveService PlayerSaveService { get; set; }
        
        public void SaveSettings()
        {
            PlayerSaveService.SyncPlayerData();
        }
    }
}