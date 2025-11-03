using Agents;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class GridSelection : BaseVisualFeature<GridSelectionVisual>, IGridSelection, IBattleLaunchAgent
    {
        [Inject] public GridSelectionRecord Record { get; set; }
        [Inject] public IBattleUnits BattleUnits { get; set; }
        
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
            
            // Clear battle unit selection
            BattleUnits.UpdateUnitSelectionAtCoordinate(null);
        }

        public void SelectHex(HexOperator hexOperator)
        {
            Vector2Int coordinate = hexOperator.Coordinate;
            
            // Deselect previous hex if there is one
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
            
            Notebook.NoteData($"Selected hex at coordinate: {coordinate}");
        }
    }
}