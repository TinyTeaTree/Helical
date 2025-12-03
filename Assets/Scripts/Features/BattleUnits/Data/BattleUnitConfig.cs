using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class BattleUnitConfig
    {
        [System.Serializable]
        public class Action
        {
            public AbilityMode Ability;
            public int ActionPointsRequired = 20;
            /// <summary>
            /// When in the duration of the ActionPoints is the Ability actually triggered. 
            /// </summary>
            public int ActionInterception = 10;
            /// <summary>
            /// How many times per turn can this action be performed
            /// </summary>
            public int MaxPerTurn = 1;
            

            /// <summary>
            /// How many turns to wait for this Action to be reusable.
            /// </summary>
            public int TurnCooldown = 1;
        }
        
        [SerializeField] private string _id;

        [SerializeField] private string _displayName;

        //The amount of Actions Points that can be used per turn, each action point is a Tick in the Turn Meter
        [SerializeField] private int _actionPoints;

        [SerializeField] private List<Action> _actions;

        [SerializeField] private int _maxHealth;

        [SerializeField] private int _attackPower;

        [SerializeField] private int _defense;

        [SerializeField] private int _moveRange;
        
        

        public string Id => _id;
        public string DisplayName => _displayName;
        public int MaxHealth => _maxHealth;
        public int AttackPower => _attackPower;
        public int Defense => _defense;
        public int MoveRange => _moveRange;

        public int ActionPoints => _actionPoints;
        public List<Action> Actions => _actions;
    }
}