using System.Collections.Generic;

namespace ChessRaid
{
    [System.Serializable]
    public class GridState
    {
        public List<ChampionState> Champions;
        public MobsState Mobs;
    }
}