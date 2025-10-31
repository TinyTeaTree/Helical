using Core;
using Services;
using UnityEngine;

namespace Game
{
    public class LoadingScreenSO : BaseConfigSO
    {
        [SerializeField] private LoadingScreenConfig _config;

        public override BaseConfig Config => _config;
    }
}