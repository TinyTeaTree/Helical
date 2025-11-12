using Agents;
using Core;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class GridSelectionFeature : BaseFeature, IGridSelection, IBattleLaunchAgent
    {
        [Inject] public GridSelectionRecord Record { get; set; }
        [Inject] public IBattleUnits BattleUnits { get; set; }
        [Inject] public IBattleGUI BattleGUI { get; set; }
        [Inject] public ICameraMove CameraMove { get; set; }
        [Inject] public IGrid Grid { get; set; }
        [Inject] public ICastle Castle { get; set; }
        
        private HexOperator _currentlySelectedHex;
        private CastleOperator _currentlySelectedCastle;
        private Camera _camera;

        public async UniTask BattleLaunch()
        {
            _camera = Camera.main;
            await UniTask.CompletedTask;
        }

        public void Start()
        {
            Record.IsSelectionEnabled = true;
        }

        public void Halt()
        {
            Record.IsSelectionEnabled = false;
            
            // Deselect current hex when halting
            if (_currentlySelectedHex != null)
            {
                _currentlySelectedHex.SetNormalState();
                _currentlySelectedHex = null;
            }

            // Deselect current castle when halting
            if (_currentlySelectedCastle != null)
            {
                _currentlySelectedCastle.SetNormalState();
                _currentlySelectedCastle = null;
            }
            
            Record.ClearSelection();
            Record.ClearAbilityMode();
            
            // Clear battle unit selection
            BattleUnits.UpdateUnitSelection(null);
            
            // Hide the battle GUI
            BattleGUI.HideUnitSelection();
        }
        
        public void HandleLeftClick()
        {
            // Check if selection is enabled
            if (!Record.IsSelectionEnabled)
            {
                return;
            }

            // Don't process selection if mouse is over UI
            if (IsPointerOverUI())
            {
                return;
            }
            
            // Create raycast from camera through mouse position
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            // Single raycast to hit all layers and get the topmost object
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                // Check what layer the hit object is on
                int hitLayer = hit.collider.gameObject.layer;

                // Check for Castle layer first (since castles are on top of hexes)
                if (hitLayer == LayerMask.NameToLayer("Castle"))
                {
                    // Check if we hit a castle with CastleOperator
                    CastleOperator castleOperator = hit.collider.GetComponent<CastleOperator>();

                    if (castleOperator != null)
                    {
                        OnCastleClicked(castleOperator);
                        return;
                    }
                }
                // Check for Hex layer
                else if (hitLayer == LayerMask.NameToLayer("Hex"))
                {
                    // Check if we hit a hex with detection script
                    HexDetection hexDetection = hit.collider.GetComponent<HexDetection>();

                    if (hexDetection != null)
                    {
                        OnHexClicked(hexDetection.Operator);
                        return;
                    }
                }
                // Check for Bottom layer to deselect
                else if (hitLayer == LayerMask.NameToLayer("Bottom"))
                {
                    DeselectHex();
                    DeselectCastle();
                    return;
                }
            }

            // If no hit at all, deselect everything
            DeselectHex();
            DeselectCastle();
        }
        
        private bool IsPointerOverUI()
        {
            // Check if the pointer is over a UI element
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
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

        public void OnCastleClicked(CastleOperator castleOperator)
        {
            Vector2Int coordinate = castleOperator.Coordinate;

            // Check if we're in Attack mode
            if (Record.CurrentAbilityMode == AbilityMode.Attack)
            {
                HandleAttackMode(coordinate);
                return;
            }

            // Check if we're in Move mode
            if (Record.CurrentAbilityMode == AbilityMode.Move)
            {
                // Select the castle normally, but clear the ability mode to prevent bugs
                SelectCastle(castleOperator, coordinate);
                Record.ClearAbilityMode();

                // Lerp camera to castle coordinate
                CameraMove.LerpToCoordinate(coordinate);

                // Play select sound
                DJ.Play(DJ.SelectOn_Sound);

                Notebook.NoteData($"Selected castle at coordinate: {coordinate} (cleared Move mode)");
                return;
            }

            // Check if we're in Rotate mode
            if (Record.CurrentAbilityMode == AbilityMode.Rotate)
            {
                HandleRotateMode(coordinate);
                return;
            }

            // Normal selection mode
            SelectCastle(castleOperator, coordinate);

            // Lerp camera to castle coordinate
            CameraMove.LerpToCoordinate(coordinate);

            // Play select sound
            DJ.Play(DJ.SelectOn_Sound);

            Notebook.NoteData($"Selected castle at coordinate: {coordinate}");
        }

        private void SelectHex(HexOperator hexOperator, Vector2Int coordinate)
        {
            // Normal selection mode
            // Deselect previous hex if there is one (without playing sound - we're changing selection)
            if (_currentlySelectedHex != null)
            {
                _currentlySelectedHex.SetNormalState();
            }

            // Deselect castle if selecting hex
            if (_currentlySelectedCastle != null)
            {
                _currentlySelectedCastle.SetNormalState();
                _currentlySelectedCastle = null;
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

        private void SelectCastle(CastleOperator castleOperator, Vector2Int coordinate)
        {
            // Deselect previous castle if there is one
            if (_currentlySelectedCastle != null)
            {
                _currentlySelectedCastle.SetNormalState();
            }

            // Deselect hex if selecting castle
            if (_currentlySelectedHex != null)
            {
                _currentlySelectedHex.SetNormalState();
                _currentlySelectedHex = null;
            }

            // Select new castle
            castleOperator.SetGlowingState();

            // Update tracking
            _currentlySelectedCastle = castleOperator;
            Record.SelectedCoordinate = coordinate;

            // Clear battle unit selection since we selected a castle
            BattleUnits.UpdateUnitSelection(null);
            BattleGUI.HideUnitSelection();
        }

        private void DeselectCastle()
        {
            if (_currentlySelectedCastle != null)
            {
                _currentlySelectedCastle.SetNormalState();
                _currentlySelectedCastle = null;
            }
        }

        public void HandleRightClick()
        {
            // Check if selection is enabled
            if (!Record.IsSelectionEnabled)
            {
                return;
            }

            // Deselect everything and clear ability mode
            DeselectHex();
            DeselectCastle();
            Record.ClearSelection();
            Record.ClearAbilityMode();
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
            if (_currentlySelectedHex != null)
            {
                _currentlySelectedHex.SetNormalState();
                _currentlySelectedHex = null;
            }

            Record.ClearSelection();
            
            BattleUnits.UpdateUnitSelection(null);
            
            BattleGUI.HideUnitSelection();
            
            DJ.Play(DJ.SelectOff_Sound);
            
            Notebook.NoteData("Deselected hex");
        }
    }
}