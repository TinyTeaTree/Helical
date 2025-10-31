namespace ChessRaid
{
    public class PlayerManager : WagSingleton<PlayerManager>
    {
        private PlayerState _playerState;
        private LevelManager _levelManager;
        private RulesManager _rules;

        public PlayerState State => _playerState;

        public override void Awake(ContextGroup<IController> group)
        {
            _levelManager = group.Get<LevelManager>();
            _rules = group.Get<RulesManager>();
        }

        public void Init(PlayerState state)
        {
            _playerState = state;
        }

        public override void Start()
        {
            foreach(var champ in _playerState.Grid.Champions)
            {
                var def = _rules.GetChampionDef(champ.ChampionId);
                champ.Health = def.Stats.Health;
                champ.ActionPoints = def.Stats.Speed * 10;
            }
        }
    }
}