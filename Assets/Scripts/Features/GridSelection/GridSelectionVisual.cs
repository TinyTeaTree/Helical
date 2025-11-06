using Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class GridSelectionVisual : BaseVisual<GridSelection>
    {
        private Camera _camera;

        public void Initialize(Camera camera)
        {
            _camera = camera;
        }

        private void Update()
        {
            if (_camera == null)
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
            // Don't process selection if mouse is over UI
            if (IsPointerOverUI())
            {
                return;
            }
            
            // Create raycast from camera through mouse position
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            
            // Check for Hex layer first
            int hexLayerMask = LayerMask.GetMask("Hex");
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, hexLayerMask))
            {
                // Check if we hit a hex with detection script
                HexDetection hexDetection = hit.collider.GetComponent<HexDetection>();
                
                if (hexDetection != null)
                {
                    Feature.OnHexClicked(hexDetection.Operator);
                    return;
                }
            }
            
            // Check for Bottom layer to deselect
            int bottomLayerMask = LayerMask.GetMask("Bottom");
            
            if (Physics.Raycast(ray, out RaycastHit bottomHit, Mathf.Infinity, bottomLayerMask))
            {
                Feature.DeselectHex();
            }
        }
        
        private bool IsPointerOverUI()
        {
            // Check if the pointer is over a UI element
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}