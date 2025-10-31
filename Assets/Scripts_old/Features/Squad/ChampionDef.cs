using UnityEngine;

namespace ChessRaid
{
    [CreateAssetMenu(fileName = "Champion Id", menuName = "Chess Raid/Definitions/Champion")]
    public class ChampionDef : ScriptableObject
    {
        public string Id;
        public string Name;

        public Champion Prefab;
        public RulesDef Rules;
        public ChampionStats Stats;

        public Sprite ProfilePicture;
    }
}