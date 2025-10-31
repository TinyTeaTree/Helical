using Core;
using Services;
using UnityEngine;

namespace Game
{
    public class PlayerAccountSO : BaseConfigSO
    {
        [SerializeField] private PlayerAccountConfig _config;

        public override BaseConfig Config => _config;
    }
}