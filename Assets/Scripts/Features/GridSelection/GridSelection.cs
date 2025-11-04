using Agents;
using Core;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;

namespace Game
{
    public class GridSelection : BaseVisualFeature<GridSelectionVisual>, IGridSelection, IBattleLaunchAgent
    {
        [Inject] public GridSelectionRecord Record { get; set; }
        [Inject] public IBattleUnits BattleUnits { get; set; }
        [Inject] public IBattleGUI BattleGUI { get; set; }
        [Inject] public ICameraMove CameraMove { get; set; }
        
        private HexOperator _currentlySelectedHex;

        public async UniTask BattleLaunch()
        {
            await SetupVisual();
        }

        private async UniTask SetupVisual()
        {
            if (_visual != null)
            {
                Notebook.NoteError($"Visual already exists for {typeof(GridSelectionVisual)}");
                return;
            }

            await CreateVisual();
            
            // Initialize visual with camera
            var camera = Camera.main;

            _visual.Initialize(camera);
            _visual.gameObject.SetActive(false);
        }

        public void Start()
        {
            Record.IsSelectionEnabled = true;
            _visual.gameObject.SetActive(true);
        }

        public void Halt()
        {
            Record.IsSelectionEnabled = false;
            _visual.gameObject.SetActive(false);
            
            // Deselect current hex when halting
            if (_currentlySelectedHex != null)
            {
                _currentlySelectedHex.SetNormalState();
                _currentlySelectedHex = null;
            }
            
            Record.ClearSelection();
            Record.ClearAbilityMode();
            
            // Clear battle unit selection
            BattleUnits.UpdateUnitSelectionAtCoordinate(null);
            
            // Hide the battle GUI
            BattleGUI.HideUnitSelection();
        }

        public void SetAbilityMode(AbilityMode mode)
        {
            Record.SetAbilityMode(mode);
            Notebook.NoteData($"Ability mode set to: {mode}");
        }

        public void SelectHex(HexOperator hexOperator)
        {
            Vector2Int coordinate = hexOperator.Coordinate;
            
            // Check if we're in Attack mode
            if (Record.CurrentAbilityMode == AbilityMode.Attack)
            {
                HandleAttackMode(coordinate);
                return;
            }
            
            // Normal selection mode
            // Deselect previous hex if there is one (without playing sound - we're changing selection)
            if (_currentlySelectedHex != null)
            {
                _currentlySelectedHex.SetNormalState();
            }
            
            // Select new hex
            hexOperator.SetGlowingState();
            
            // Update tracking
            _currentlySelectedHex = hexOperator;
            Record.SelectedCoordinate = coordinate;
            
            // Update battle unit selection based on the selected coordinate
            BattleUnits.UpdateUnitSelectionAtCoordinate(coordinate);
            
            // Check if there's a unit at this coordinate via BattleUnits API
            var unitData = BattleUnits.GetUnitDataAtCoordinate(coordinate);
            if (unitData != null)
            {
                BattleGUI.ShowUnitSelection();

                CameraMove.LerpToCoordinate(coordinate);
            }
            else
            {
                BattleGUI.HideUnitSelection();
            }
            
            // Play select sound
            DJ.Play(DJ.SelectOn_Sound);
            
            Notebook.NoteData($"Selected hex at coordinate: {coordinate}");
        }
        
        private void HandleAttackMode(Vector2Int targetCoordinate)
        {
            // Check if there's a selected unit (the attacker)
            if (!Record.HasSelection)
            {
                Notebook.NoteWarning("No unit selected to perform attack");
                return;
            }
            
            Vector2Int attackerCoordinate = Record.SelectedCoordinate.Value;
            
            // Execute the attack
            BattleUnits.ExecuteAttack(attackerCoordinate, targetCoordinate);
            
            // Clear ability mode after attack
            Record.ClearAbilityMode();
            
            Notebook.NoteData($"Attack executed from {attackerCoordinate} to {targetCoordinate}");
        }
        
        public void DeselectHex()
        {
            // Only deselect if something is actually selected
            if (_currentlySelectedHex == null)
                return;
            
            // Deselect the hex
            _currentlySelectedHex.SetNormalState();
            _currentlySelectedHex = null;
            
            // Clear selection data
            Record.ClearSelection();
            
            // Clear battle unit selection
            BattleUnits.UpdateUnitSelectionAtCoordinate(null);
            
            // Hide the battle GUI
            BattleGUI.HideUnitSelection();
            
            // Play deselect sound
            DJ.Play(DJ.SelectOff_Sound);
            
            Notebook.NoteData("Deselected hex");
        }
    }
}