using Core;
using Services;
using UnityEngine;

namespace Game
{
    public class CursorSO : BaseConfigSO
    {
        [SerializeField] private CursorConfig _config;

        public override BaseConfig Config => _config;
    }
}