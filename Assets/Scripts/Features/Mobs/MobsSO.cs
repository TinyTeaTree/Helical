using Core;
using Services;
using UnityEngine;

namespace Game
{
    public class MobsSO : BaseConfigSO
    {
        [SerializeField] private MobsConfig _config;

        public override BaseConfig Config => _config;
    }
}