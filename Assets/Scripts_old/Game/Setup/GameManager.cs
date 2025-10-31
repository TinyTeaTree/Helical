using Newtonsoft.Json;

namespace ChessRaid
{
    public class GameManager : BaseContext
    {
        protected override void CreateControllers()
        {
            var dataWarehouse = DataWarehouse.Single;
            dataWarehouse.Init();
            _controllerGroup.Add(dataWarehouse);

            var playerState = new PlayerState()
            {
                CurrentLevel = "Demo",
            };

            var levelManager = LevelManager.Single;
            levelManager.Init();
            var levelDef = levelManager.GetLevel(playerState.CurrentLevel);
            _controllerGroup.Add(levelManager);

            playerState.Grid = JsonConvert.DeserializeObject<GridState>(JsonConvert.SerializeObject(levelDef.StartingState));
            var playerManager = PlayerManager.Single;
            playerManager.Init(playerState);
            _controllerGroup.Add(playerManager);


            _controllerGroup.Add(TurnModel.Single);
            _controllerGroup.Add(RulesManager.Single);
            _controllerGroup.Add(TurnManager.Single);
        }

        protected override void PostStart()
        {
            GridManager.Single.SetUp();
            Squad.Single.SetUp();
        }
    }
}