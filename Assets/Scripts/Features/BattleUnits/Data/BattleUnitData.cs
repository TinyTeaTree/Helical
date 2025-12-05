using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class BattleUnitData
    {
        [System.Serializable]
        public class Action
        {
            public AbilityMode Ability;
            public Vector2Int Target;
            public int ActionPoints;
            public int Interception;

            public int ActionPointStart; //TODO; seed this, this stars at 0
        }

        [System.Serializable]
        public class Turn
        {
            public List<Action> Actions = new List<Action>();
        }
        
        public string PlayerId { get; set; }
        public string BattleUnitId { get; set; }
        
        public Vector2Int Coordinate { get; set; }
        public HexDirection Direction { get; set; }
        
        public Turn TurnOrder { get; set; } = new Turn();
        
        public int Health { get; set; }
        public int Level { get; set; }
        public bool IsDead { get; set; }
        public bool DebugAllowTurnOrdering { get; set; }
    }
}