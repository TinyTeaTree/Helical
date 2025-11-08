using System.Threading.Tasks;
using Agents;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class HudFeature : BaseVisualFeature<HudVisual>, IHud, IAppLaunchAgent
    {
        public bool IsReady { get; private set; }
        public Camera HudCamera => _visual?.HudCamera;
        public Transform HudRoot => _visual?.HudRoot;

        public void SetCanvas(Canvas visualCanvas)
        {
            if (!IsReady)
            {
                Notebook.NoteError("Can't call Hud while its not ready");
                return;
            }

            if (visualCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                visualCanvas.worldCamera = HudCamera;
                visualCanvas.planeDistance = 1f;
            }
            
            visualCanvas.transform.SetParent(HudRoot);
        }

        public async UniTask AppLaunch()
        {
            await SetupVisual();
        }

        public async UniTask SetupVisual()
        {
            await CreateVisual();
            _visual.HudCamera = Camera.main;
            IsReady = true;
        }
    }
}