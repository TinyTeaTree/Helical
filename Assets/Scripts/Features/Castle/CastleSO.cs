using Core;
using Services;

namespace Game
{
    public class CastleSO : BaseConfigSO
    {
        public CastleConfig _config;

        public override BaseConfig Config => _config;
    }
}
