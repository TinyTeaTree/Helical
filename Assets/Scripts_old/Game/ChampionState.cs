namespace ChessRaid
{
    [System.Serializable]
    public class ChampionState
    {
        public string ChampionId;
        public Direction Direction;
        public Coord Location;
        public Team Team;

        public int Health;
        public int ActionPoints;
        public bool ActionsBlocked;
    }
}