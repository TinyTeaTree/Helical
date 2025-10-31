using System.Collections.Generic;
using ShessRaid;
using UnityEngine;

namespace ChessRaid
{
    [CreateAssetMenu(fileName = "Mob Collection", menuName = "Chess Raid/Definitions/Mobs")]
    public class MobsSO : ScriptableObject
    {
        public List<MobDef> Mobs;
    }
}