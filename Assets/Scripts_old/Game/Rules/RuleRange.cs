using System.Collections.Generic;

namespace ChessRaid
{
    [System.Serializable]
    public class RuleRange
    {
        public List<RuleRangeEntry> Group;
    }

    [System.Serializable]
    public class RuleRangeEntry
    {
        public List<Direction> Path;
    }
}