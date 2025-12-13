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
        public class AbilityCooldownData
        {
            public AbilityMode Ability;
            public int UsedThisTurn;
            public int TurnsToCooldown;
        }

        [System.Serializable]
        public class Turn
        {
            public List<Action> Actions = new List<Action>();
        }
        
        public string PlayerId { get; set; }
        public string BattleUnitId { get; set; }
        public string InstanceGuid { get; set; } = System.Guid.NewGuid().ToString();

        public Vector2Int Coordinate { get; set; }
        public HexDirection Direction { get; set; }
        
        public Turn TurnOrder { get; set; } = new Turn();

        public List<AbilityCooldownData> AbilityCooldowns { get; set; } = new List<AbilityCooldownData>();

        public int Health { get; set; }
        public int Level { get; set; }
        public bool IsDead { get; set; }
        public bool DebugAllowTurnOrdering { get; set; }
    }
}