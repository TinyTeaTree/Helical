using Core;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;

namespace Game
{
    public class CursorFeature : BaseFeature, ICursor
    {
        [Inject] public ILocalConfigService ConfigService { get; set; }

        private CursorConfig _config;

        public override void Bootstrap(IBootstrap bootstrap)
        {
            base.Bootstrap(bootstrap);

            _config = ConfigService.GetConfig<CursorConfig>();
        }

        public void SetCursorMode(AbilityMode mode)
        {
            var texture = _config.GetCursorTexture(mode);
            if (texture != null)
            {
                Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
            }
            else
            {
                // Reset to default cursor if no texture found
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }
    }
}