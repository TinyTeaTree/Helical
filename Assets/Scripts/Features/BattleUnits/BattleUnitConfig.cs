using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class BattleUnitConfig
    {
        [SerializeField]
        private string _id;

        [SerializeField]
        private string _displayName;

        [SerializeField]
        private int _maxHealth;

        [SerializeField]
        private int _attackPower;

        [SerializeField]
        private int _defense;

        [SerializeField]
        private int _moveRange;

        public string Id => _id;
        public string DisplayName => _displayName;
        public int MaxHealth => _maxHealth;
        public int AttackPower => _attackPower;
        public int Defense => _defense;
        public int MoveRange => _moveRange;
    }
}

