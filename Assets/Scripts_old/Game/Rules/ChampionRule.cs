namespace ChessRaid
{
    /// <summary>
    /// This describes a single Champion Rule
    /// </summary>
    [System.Serializable]
    public class ChampionRule
    {
        public string RuleName;
        public ActionType Action;
        public RuleRange Range;

        public int ActionPoints;
        public bool SupportMultiple;
        public bool BlockPostActions;
    }
}