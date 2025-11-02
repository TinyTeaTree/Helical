using Core;
using UnityEngine;

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
            // Create raycast from camera through mouse position
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            
            // Perform raycast on "Hex" layer
            int hexLayerMask = LayerMask.GetMask("Hex");
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, hexLayerMask))
            {
                // Check if we hit a hex with detection script
                HexDetection hexDetection = hit.collider.GetComponent<HexDetection>();
                
                if (hexDetection != null)
                {
                    Feature.SelectHex(hexDetection.Operator);
                }
            }
        }
    }
}