using Core;
using Services;
using UnityEngine;

namespace Game
{
    public class CastleConfigSO : BaseConfigSO
    {
        [SerializeField] private CastleConfig _config;

        public override BaseConfig Config => _config;
    }
}
