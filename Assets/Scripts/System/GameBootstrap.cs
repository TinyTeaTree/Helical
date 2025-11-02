using Agents;
using Core;
using Factories;
using Services;

namespace Game
{
    public class GameBootstrap : GameInfra
    {
        protected override void AddServices()
        {
            var notebook = new NotebookService();
            Notebook.NotebookService = notebook;

            _services.Add<INotebookService>(notebook);
            _services.Add<ISummoningService>(new SummoningService());
            _services.Add<ILocalConfigService>(new LocalConfigService());
            _services.Add<ISoundPlayerService>(new SoundPlayerService());
            _services.Add<IPlayerSaveService>(new PlayerSaveService());
            _services.Add<IRecordService>(new RecordService());
            //<New Service>
        }

        protected override void AddFeatures()
        {
            _features.Add<ILoadingScreen>(new LoadingScreen());
            _features.Add<IHud>(new Hud());
            _features.Add<ILobby>(new Lobby());
            _features.Add<IArena>(new Arena());
            _features.Add<IGrid>(new Grid());
            _features.Add<IMobs>(new Mobs());
            _features.Add<IPlayerAccount>(new PlayerAccount());
            _features.Add<IPlayerSettings>(new PlayerSettings());
            _features.Add<ICameraMove>(new CameraMove());
            _features.Add<IBattleUnits>(new BattleUnits());
            _features.Add<IGridSelection>(new GridSelection());
            //<New Feature>
        }

        protected override void AddFactories()
        {
            _factories.Add(typeof(HudVisual), new ResourceFactory(Addresses.HudVisual));
            _factories.Add(typeof(LoadingScreenVisual), new ResourceFactory(Addresses.LoadingScreenHud));
            _factories.Add(typeof(LobbyVisual), new ResourceFactory(Addresses.LobbyVisual));
            _factories.Add(typeof(GridVisual), new ResourceFactory(Addresses.GridVisual));
            _factories.Add(typeof(BattleUnitsVisual), new GenerateVisualFactory());
            _factories.Add(typeof(GridSelectionVisual), new GenerateVisualFactory());
        }

        protected override void AddAgents()
        {
            _agents.Add<IAppLaunchAgent>(new AppLaunchAgent());
            _agents.Add<IAppExitAgent>(new AppExitAgent());
            _agents.Add<ILogoutAgent>(new LogoutAgent());
            _agents.Add<ILoggedInAgent>(new LoggedInAgent());
            _agents.Add<IBattleLaunchAgent>(new BattleLaunchAgent());
            //<New Agent>
        }

        protected override void AddRecords()
        {
            _records.Add(typeof(LobbyRecord), new LobbyRecord());
            _records.Add(typeof(GridRecord), new GridRecord());
            _records.Add(typeof(MobsRecord), new MobsRecord());
            _records.Add(typeof(LoadingScreenRecord), new LoadingScreenRecord());
            _records.Add(typeof(PlayerAccountRecord), new PlayerAccountRecord());
            _records.Add(typeof(PlayerSettingsRecord), new PlayerSettingsRecord());
            _records.Add(typeof(BattleUnitsRecord), new BattleUnitsRecord());
            _records.Add(typeof(GridSelectionRecord), new GridSelectionRecord());
            //<New Record>
        }

        protected override void BootstrapCustoms()
        {
            BootstrapRecordService();
            BootstrapSoundPlayerService();
            BootstrapSummoningService();
            BootstrapLocalConfigurationService();
			BootstrapGenericSounds();
            BootstrapSavedRecords();

            AppExitAgent.SelfRegister(_agents.Get<IAppExitAgent>());
        }

        private void BootstrapRecordService()
        {
            var recordService = _services.Get<IRecordService>();
            recordService.SetUp(_records.Values);
        }

        private void BootstrapSoundPlayerService()
        {
            var soundPlayer = _services.Get<ISoundPlayerService>();
            DJ.SoundPlayer = soundPlayer;
        }

        private void BootstrapLocalConfigurationService()
        {
            var localConfigService = _services.Get<ILocalConfigService>();
            var localConfigSO = Services.Get<ISummoningService>().LoadResource<LocalConfigCollectionSO>(Addresses.LocalConfigs);
            localConfigService.SetConfigSO(localConfigSO);
        }
        
        //If you want your Record to be saved, add your Record here
        private void BootstrapSavedRecords()
        {
            var saveService = _services.Get<IPlayerSaveService>();
            
            saveService.AddSaveRecord(_records[typeof(PlayerAccountRecord)]);
            saveService.AddSaveRecord(_records[typeof(PlayerSettingsRecord)]);
        }  

		private void BootstrapGenericSounds()
		{
			var collection = Services.Get<ISummoningService>().LoadResource<GenericSoundCollectionSO>(Addresses.GenericSoundCollection);
			DJ.GenericCollection = collection;
		}

        private void BootstrapSummoningService()
        {
            var summoner = _services.Get<ISummoningService>();
            Summoner.SummoningService = summoner;

            //Add Provider for each Asset Pack type
            summoner.SetProvider(typeof(GridResourcePack), new ResourceAssetPackProvider(Addresses.GridResourcePack));
            summoner.SetProvider(typeof(BattleUnitsAssetPack), new ResourceAssetPackProvider(Addresses.BattleUnitsAssetPack));
        }

        protected override void StartGame()
        {
            new GameLaunchFlow(this).Execute();
        }
    }
}
