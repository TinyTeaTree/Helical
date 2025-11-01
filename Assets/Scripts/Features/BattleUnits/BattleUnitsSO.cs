using Core;
using Services;
using UnityEngine;

namespace Game
{
    public class BattleUnitsSO : BaseConfigSO
    {
        [SerializeField] private BattleUnitsConfig _config;

        public override BaseConfig Config => _config;
    }
}