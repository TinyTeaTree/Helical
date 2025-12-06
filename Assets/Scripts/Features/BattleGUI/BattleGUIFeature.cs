using System.Linq;
using Agents;
using Core;
using Cysharp.Threading.Tasks;
using Services;

namespace Game
{
    public class BattleGUIFeature : BaseVisualFeature<BattleGUIVisual>, IBattleGUI, IBattleLaunchAgent
    {
        [Inject] public IGridSelection GridSelection { get; set; }
        [Inject] public ILocalConfigService ConfigService { get; set; }
        [Inject] public IPlayerAccount PlayerAccount { get; set; }
        [Inject] public ITurn Turn { get; set; }
        [Inject] public IBattleUnits BattleUnits { get; set; }

        private BattleUnitsConfig _config;
        private BattleUnitsAssetPack _assetPack;

        public async UniTask BattleLaunch()
        {
            _config = ConfigService.GetConfig<BattleUnitsConfig>();
            _assetPack = await Summoner.SummoningService.LoadAssetPack<BattleUnitsAssetPack>();
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
            HideUnitSelection();
        }

        public void ShowUnitSelection(BattleUnitData unitData)
        {
            // Get unit name from config
            var unitConfig = _config.GetBattleUnit(unitData.BattleUnitId);
            string displayName = unitConfig != null ? unitConfig.DisplayName : unitData.BattleUnitId;

            // Get unit photo from asset pack
            var photo = _assetPack.GetUnitPhoto(unitData.BattleUnitId);

            // Update the visual with name, level, and photo
            _visual.UpdateUnitInfo(displayName, unitData.Level, photo);

            bool canSelectUnit = CanSelectUnit(unitData);

            _visual.ShowUnitSelection(canSelectUnit);
            _visual.SetUnitActions(unitConfig.Actions.Select(a => a.Ability));

            if (canSelectUnit)
            {
                Turn.SelectedMyUnit();
            }
        }

        private bool CanSelectUnit(BattleUnitData unitData)
        {
            // Allow selection if it's the player's unit
            if (unitData.PlayerId == PlayerAccount.PlayerId)
            {
                return true;
            }

            // Allow selection if debug mode is enabled on the unit
            return unitData.DebugAllowTurnOrdering;
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

        public void OnWaitButtonClicked()
        {
            var selectedCoordinate = GridSelection.GetSelectedCoordinate();
            Turn.OrderTurn(selectedCoordinate, default, AbilityMode.Wait); // Target is ignored for Wait
            DJ.Play(DJ.Tick_Sound);
            Notebook.NoteData("Wait action ordered");
        }
    }
}