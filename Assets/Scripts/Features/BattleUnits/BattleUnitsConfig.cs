using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class BattleUnitsConfig : BaseConfig
    {
        [SerializeField]
        private List<BattleUnitConfig> _battleUnits = new List<BattleUnitConfig>();

        public List<BattleUnitConfig> BattleUnits => _battleUnits;

        public BattleUnitConfig GetBattleUnit(string id)
        {
            return _battleUnits.Find(unit => unit.Id == id);
        }
    }
}