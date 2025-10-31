using System.Collections.Generic;
using UnityEngine;

namespace ChessRaid
{

    [CreateAssetMenu(fileName = "Champion SO", menuName = "Chess Raid/Definitions/Champion SO")]
    public class ChampionsSO : ScriptableObject
    {
        public List<ChampionDef> ChampionDefinitions;

    }
}