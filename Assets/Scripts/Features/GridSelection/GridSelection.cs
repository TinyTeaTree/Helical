using Agents;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class GridSelection : BaseVisualFeature<GridSelectionVisual>, IGridSelection, IBattleLaunchAgent
    {
        [Inject] public GridSelectionRecord Record { get; set; }
        
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
            if (camera == null)
            {
                Notebook.NoteError("GridSelection: Camera.main not found.");
                return;
            }
            
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
            if (_visual == null)
            {
                return;
            }

            Record.IsSelectionEnabled = false;
            _visual.gameObject.SetActive(false);
            
            // Deselect current hex when halting
            if (_currentlySelectedHex != null)
            {
                _currentlySelectedHex.SetNormalState();
                _currentlySelectedHex = null;
            }
            
            Record.ClearSelection();
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
            
            Notebook.NoteData($"Selected hex at coordinate: {coordinate}");
        }
    }
}