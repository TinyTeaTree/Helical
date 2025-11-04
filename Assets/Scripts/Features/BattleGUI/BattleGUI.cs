using Agents;
using Core;
using Cysharp.Threading.Tasks;

namespace Game
{
    public class BattleGUI : BaseVisualFeature<BattleGUIVisual>, IBattleGUI, IBattleLaunchAgent
    {
        public async UniTask BattleLaunch()
        {
            await SetupVisual();
        }

        private async UniTask SetupVisual()
        {
            if (_visual != null)
            {
                Notebook.NoteError($"Visual already exists for {typeof(BattleGUIVisual)}");
                return;
            }

            await CreateVisual();
            _visual.gameObject.SetActive(true);
        }

        public void ShowUnitSelection()
        {
            _visual.ShowUnitSelection();
        }

        public void HideUnitSelection()
        {
            _visual.HideUnitSelection();
        }
    }
}