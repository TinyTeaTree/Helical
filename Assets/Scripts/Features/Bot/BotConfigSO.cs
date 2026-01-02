using System.Collections.Generic;
using Core;
using Services;
using UnityEngine;

namespace Game
{
    public class BotConfigSO : BaseConfigSO
    {
        [SerializeField] private BotConfig _config;

        public override BaseConfig Config => _config;
    }
}
