using UnityEngine;

namespace Game
{
    public class BattleUnitData
    {
        public string PlayerId { get; set; }
        public string BattleUnitId { get; set; }
        
        public Vector2Int Coordinate { get; set; }
        public HexDirection Direction { get; set; }
        
        public int Health { get; set; }
        public int Level { get; set; }
        public bool IsDead { get; set; }
    }
}