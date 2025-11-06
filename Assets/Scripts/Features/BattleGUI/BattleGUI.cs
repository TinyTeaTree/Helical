using Agents;
using Core;
using Cysharp.Threading.Tasks;
using Services;

namespace Game
{
    public class BattleGUI : BaseVisualFeature<BattleGUIVisual>, IBattleGUI, IBattleLaunchAgent
    {
        [Inject] public IGridSelection GridSelection { get; set; }

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

        public void OnAttackButtonClicked()
        {
            GridSelection.SetAbilityMode(AbilityMode.Attack);
            DJ.Play(DJ.Tick_Sound);
            Notebook.NoteData("Attack mode activated");
        }
        
        public void OnMoveButtonClicked()
        {
            DJ.Play(DJ.Tick_Sound);
            GridSelection.SetAbilityMode(AbilityMode.Move);
            Notebook.NoteData("Move mode activated");
        }
        
        public void OnRotateButtonClicked()
        {
            DJ.Play(DJ.Tick_Sound);
            GridSelection.SetAbilityMode(AbilityMode.Rotate);
            Notebook.NoteData("Rotate mode activated");
        }
    }
}