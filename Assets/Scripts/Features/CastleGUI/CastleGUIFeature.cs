using Agents;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class CastleGUIFeature : BaseVisualFeature<CastleGUIVisual>, ICastleGUI, IBattleLaunchAgent
    {
        public async UniTask BattleLaunch()
        {
            await SetupVisual();
        }

        private async UniTask SetupVisual()
        {
            if (_visual != null)
            {
                Notebook.NoteError($"Visual already exists for {typeof(CastleGUIVisual)}");
                return;
            }

            await CreateVisual();
            _visual.gameObject.SetActive(true);
            HideCastleSelection();
        }

        public void ShowCastleSelection(Vector2Int coordinate)
        {
            // For now, just show basic castle info
            // TODO: Add more detailed castle information (type, location, etc.)
            _visual.ShowCastleSelection(coordinate);
        }

        public void HideCastleSelection()
        {
            _visual.HideCastleSelection();
        }
    }
}