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
        [Inject] public IGrid Grid { get; set; }
        
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
            BattleUnits.UpdateUnitSelection(null);
            
            // Hide the battle GUI
            BattleGUI.HideUnitSelection();
        }

        public void SetAbilityMode(AbilityMode mode)
        {
            Record.SetAbilityMode(mode);
            Notebook.NoteData($"Ability mode set to: {mode}");
        }
        
        public void UpdateSelectedCoordinate(Vector2Int newCoordinate)
        {
            // Get the HexOperator at the new coordinate
            var hexOperator = Grid.GetHexOperatorAtCoordinate(newCoordinate);
            
            if (hexOperator != null)
            {
                // Properly update the selection with all visual effects
                SelectHex(hexOperator, newCoordinate);
            }
            else
            {
                // Fallback: just update the coordinate if hex operator not found
                Record.SelectedCoordinate = newCoordinate;
            }
        }
        
        public bool IsCoordinateSelected(Vector2Int coordinate)
        {
            return Record.SelectedCoordinate.HasValue && Record.SelectedCoordinate.Value == coordinate;
        }

        public void OnHexClicked(HexOperator hexOperator)
        {
            Vector2Int coordinate = hexOperator.Coordinate;
            
            // Check if we're in Attack mode
            if (Record.CurrentAbilityMode == AbilityMode.Attack)
            {
                HandleAttackMode(coordinate);
                return;
            }
            
            // Check if we're in Move mode
            if (Record.CurrentAbilityMode == AbilityMode.Move)
            {
                HandleMoveMode(coordinate);
                return;
            }
            
            // Check if we're in Rotate mode
            if (Record.CurrentAbilityMode == AbilityMode.Rotate)
            {
                HandleRotateMode(coordinate);
                return;
            }
            
            SelectHex(hexOperator, coordinate);
            
            var unitData = BattleUnits.GetUnitData(coordinate);
            if (unitData != null)
            {
                CameraMove.LerpToCoordinate(coordinate);
            }

            // Play select sound
            DJ.Play(DJ.SelectOn_Sound);
            
            Notebook.NoteData($"Selected hex at coordinate: {coordinate}");
        }

        private void SelectHex(HexOperator hexOperator, Vector2Int coordinate)
        {
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
            BattleUnits.UpdateUnitSelection(coordinate);
            
            // Check if there's a unit at this coordinate via BattleUnits API
            var unitData = BattleUnits.GetUnitData(coordinate);
            if (unitData != null)
            {
                BattleGUI.ShowUnitSelection(unitData);
            }
            else
            {
                BattleGUI.HideUnitSelection();
            }
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
        
        private void HandleMoveMode(Vector2Int targetCoordinate)
        {
            // Check if there's a selected unit (the unit to move)
            if (!Record.HasSelection)
            {
                Notebook.NoteWarning("No unit selected to move");
                return;
            }
            
            Vector2Int unitCoordinate = Record.SelectedCoordinate.Value;
            
            // Check if the target coordinate is different from the unit's current coordinate
            if (unitCoordinate == targetCoordinate)
            {
                Notebook.NoteWarning("Unit is already at that location");
                return;
            }
            
            // Execute the move
            BattleUnits.ExecuteMove(unitCoordinate, targetCoordinate);
            
            // Clear ability mode after move
            Record.ClearAbilityMode();
            
            Notebook.NoteData($"Move executed from {unitCoordinate} to {targetCoordinate}");
        }
        
        private void HandleRotateMode(Vector2Int targetCoordinate)
        {
            // Check if there's a selected unit (the unit to rotate)
            if (!Record.HasSelection)
            {
                Notebook.NoteWarning("No unit selected to rotate");
                return;
            }
            
            Vector2Int unitCoordinate = Record.SelectedCoordinate.Value;
            
            // Execute the rotation
            BattleUnits.ExecuteRotate(unitCoordinate, targetCoordinate);
            
            // Clear ability mode after rotation
            Record.ClearAbilityMode();
            
            Notebook.NoteData($"Rotate executed for unit at {unitCoordinate} towards {targetCoordinate}");
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
            BattleUnits.UpdateUnitSelection(null);
            
            // Hide the battle GUI
            BattleGUI.HideUnitSelection();
            
            // Play deselect sound
            DJ.Play(DJ.SelectOff_Sound);
            
            Notebook.NoteData("Deselected hex");
        }
    }
}