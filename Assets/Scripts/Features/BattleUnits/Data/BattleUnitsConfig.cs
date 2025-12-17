using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class BattleUnitsConfig : BaseConfig
    {
        // Individual SOs for each battle unit (organized structure)
        [SerializeField]
        private List<BattleUnitConfigSO> _battleUnitConfigs = new List<BattleUnitConfigSO>();

        public List<BattleUnitConfigSO> BattleUnitConfigs => _battleUnitConfigs;

        public BattleUnitConfig GetBattleUnit(string id)
        {
            var configSO = _battleUnitConfigs.Find(so => so.Config.Id == id);
            return configSO?.Config;
        }
    }
}