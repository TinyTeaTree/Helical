using Core;
using UnityEngine;

namespace Game
{
    public class GridSelectionVisual : BaseVisual<GridSelection>
    {
        private bool _isSelectionEnabled;
        private HexOperator _currentlySelectedHex;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                Notebook.NoteError("GridSelectionVisual: Camera.main not found.");
            }
        }

        private void Update()
        {
            if (!_isSelectionEnabled || _mainCamera == null)
            {
                return;
            }

            // Detect mouse click
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseClick();
            }
        }

        private void HandleMouseClick()
        {
            // Create raycast from camera through mouse position
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            
            // Perform raycast on "Hex" layer
            int hexLayerMask = LayerMask.GetMask("Hex");
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, hexLayerMask))
            {
                // Check if we hit a hex with detection script
                HexDetection hexDetection = hit.collider.GetComponent<HexDetection>();
                
                if (hexDetection != null && hexDetection.IsInitialized)
                {
                    SelectHex(hexDetection);
                }
            }
        }

        private void SelectHex(HexDetection hexDetection)
        {
            Vector2Int coordinate = hexDetection.Coordinate;
            
            // Deselect previous hex if there is one
            if (_currentlySelectedHex != null)
            {
                _currentlySelectedHex.SetNormalState();
            }
            
            // Select new hex
            HexOperator hexOperator = hexDetection.Operator;
            hexOperator.SetGlowingState();
            
            // Update tracking
            _currentlySelectedHex = hexOperator;
            Feature.Record.SelectedCoordinate = coordinate;
            
            Notebook.NoteData($"Selected hex at coordinate: {coordinate}");
        }

        public void StartSelection()
        {
            _isSelectionEnabled = true;
        }

        public void HaltSelection()
        {
            _isSelectionEnabled = false;
            
            // Deselect current hex when halting
            if (_currentlySelectedHex != null)
            {
                _currentlySelectedHex.SetNormalState();
                _currentlySelectedHex = null;
            }
            
            Feature.Record.ClearSelection();
        }
    }
}