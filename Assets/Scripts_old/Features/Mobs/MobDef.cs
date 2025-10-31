using ChessRaid;
using UnityEngine;

namespace ShessRaid
{
    [CreateAssetMenu(fileName = "Mob Definition", menuName = "Chess Raid/Definitions/Mob")]
    public class MobDef : ScriptableObject
    {
        public Mob Prefab;
    }
}