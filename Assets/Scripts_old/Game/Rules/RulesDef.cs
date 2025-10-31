using UnityEditor;
using UnityEngine;

namespace ChessRaid
{
    [CreateAssetMenu(fileName = "Rules", menuName = "Chess Raid/Rules", order = 0)]
    public class RulesDef : ScriptableObject
    {
        public ChampionRuleSet RuleSet;
    }
}