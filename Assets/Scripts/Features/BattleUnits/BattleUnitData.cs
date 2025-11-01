using UnityEngine;

namespace Game
{
    public class BattleUnitData
    {
        public string PlayerId { get; set; }
        public string BattleUnitId { get; set; }
        
        public Vector2Int Coordinate { get; set; }
        
        public int Health { get; set; }
        public bool IsDead { get; set; }
    }
}