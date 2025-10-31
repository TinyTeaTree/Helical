using System.Collections.Generic;
using UnityEngine;

namespace ChessRaid
{
    [CreateAssetMenu(fileName = "Grid Level", menuName = "Chess Raid/Level/Grid")]
    public class GridLevelSO : ScriptableObject
    {
        public string Id;
        public List<HexOrientation> HexMap;
        public Vector3 LevelPlacement;
        public GridState StartingState;
        public LevelView LevelView;
    }
}