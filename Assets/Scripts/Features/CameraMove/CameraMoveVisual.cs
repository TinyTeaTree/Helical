using Core;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CameraMoveVisual : BaseVisual<CameraMove>
    {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] [Range(0.01f, 0.1f)] private float _edgeScrollThreshold = 0.01f; // 1% of screen
        
        private bool _isMovementEnabled = false;
        private Vector3 _centerOffset;
        private bool _boundsInitialized = false;
        private Vector2 _clampMin;
        private Vector2 _clampMax;

        private void Update()
        {
            if (!_isMovementEnabled)
            {
                return;
            }

            MoveCamera();
        }

        private void MoveCamera()
        {
            Vector3 moveDirection = Vector3.zero;

            // WASD keyboard input
            if (Input.GetKey(KeyCode.W))
            {
                moveDirection += Vector3.forward;
            }

            if (Input.GetKey(KeyCode.A))
            {
                moveDirection += Vector3.left;
            }

            if (Input.GetKey(KeyCode.S))
            {
                moveDirection += Vector3.back;
            }

            if (Input.GetKey(KeyCode.D))
            {
                moveDirection += Vector3.right;
            }

            // Edge scrolling with mouse
            Vector3 edgeScrollDirection = CalculateEdgeScrollDirection();
            moveDirection += edgeScrollDirection;

            if (moveDirection != Vector3.zero)
            {
                // Move along XZ plane only
                moveDirection.y = 0f;
                moveDirection = moveDirection.normalized;
                Vector3 newPosition = transform.position + moveDirection * _moveSpeed * Time.deltaTime;
                
                // Apply clamping if bounds are initialized
                if (_boundsInitialized)
                {
                    newPosition = ApplyClamping(newPosition);
                }
                
                transform.position = newPosition;
            }
        }

        private Vector3 CalculateEdgeScrollDirection()
        {
            Vector3 direction = Vector3.zero;
            Vector3 mousePos = Input.mousePosition;
            
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            float edgeThresholdPixelsX = screenWidth * _edgeScrollThreshold;
            float edgeThresholdPixelsY = screenHeight * _edgeScrollThreshold;
            
            // Check left edge
            if (mousePos.x < edgeThresholdPixelsX)
            {
                float normalizedDistance = 1f - (mousePos.x / edgeThresholdPixelsX);
                direction += Vector3.left * normalizedDistance;
            }
            // Check right edge
            else if (mousePos.x > screenWidth - edgeThresholdPixelsX)
            {
                float normalizedDistance = (mousePos.x - (screenWidth - edgeThresholdPixelsX)) / edgeThresholdPixelsX;
                direction += Vector3.right * normalizedDistance;
            }
            
            // Check bottom edge
            if (mousePos.y < edgeThresholdPixelsY)
            {
                float normalizedDistance = 1f - (mousePos.y / edgeThresholdPixelsY);
                direction += Vector3.back * normalizedDistance;
            }
            // Check top edge
            else if (mousePos.y > screenHeight - edgeThresholdPixelsY)
            {
                float normalizedDistance = (mousePos.y - (screenHeight - edgeThresholdPixelsY)) / edgeThresholdPixelsY;
                direction += Vector3.forward * normalizedDistance;
            }
            
            return direction;
        }

        private Vector3 ApplyClamping(Vector3 position)
        {
            // Calculate what the center of the screen would see on the XZ plane
            Vector2 centerWorldPos = GetCenterWorldPosition(position);
            
            // Clamp the center position
            centerWorldPos.x = Mathf.Clamp(centerWorldPos.x, _clampMin.x, _clampMax.x);
            centerWorldPos.y = Mathf.Clamp(centerWorldPos.y, _clampMin.y, _clampMax.y);
            
            // Calculate the camera position that would show this clamped center
            return PositionFromCenterWorldPosition(centerWorldPos);
        }

        private Vector2 GetCenterWorldPosition(Vector3 cameraPosition)
        {
            // Use the cached offset to get the center world position
            return new Vector2(cameraPosition.x + _centerOffset.x, cameraPosition.z + _centerOffset.z);
        }

        private Vector3 PositionFromCenterWorldPosition(Vector2 centerWorldPos)
        {
            // Reverse the offset to get the camera position
            return new Vector3(centerWorldPos.x - _centerOffset.x, transform.position.y, centerWorldPos.y - _centerOffset.z);
        }

        public void CalculateAndSetBounds(IGrid grid)
        {
            // First, calculate the center offset by raycasting
            CalculateCenterOffset();
            
            // Then, calculate grid bounds
            CalculateGridBounds(grid);
            
            _boundsInitialized = true;
        }

        private void CalculateCenterOffset()
        {
            // Calculate where the camera's forward vector intersects the XZ plane at Y=0
            // Using parametric equation: P(t) = cameraPos + t * forward
            // Solve for t where Y = 0: cameraPos.y + t * forward.y = 0
            // t = -cameraPos.y / forward.y
            
            Vector3 cameraPos = transform.position;
            Vector3 forward = transform.forward;
            
            if (Mathf.Approximately(forward.y, 0f))
            {
                Notebook.NoteError("CameraMoveVisual: Camera forward is parallel to ground plane.");
                return;
            }
            
            float t = -cameraPos.y / forward.y;
            Vector3 hitPoint = cameraPos + t * forward;
            
            // Calculate offset from camera position to hit point
            _centerOffset = hitPoint - cameraPos;
        }

        private void CalculateGridBounds(IGrid grid)
        {
            if (grid is Grid gridFeature)
            {
                var hexCache = gridFeature.Visual.GetHexCache();
                
                if (hexCache == null || hexCache.Count == 0)
                {
                    Notebook.NoteError("CameraMoveVisual: Grid cache is empty. Cannot calculate bounds.");
                    return;
                }

                float minX = float.MaxValue;
                float maxX = float.MinValue;
                float minZ = float.MaxValue;
                float maxZ = float.MinValue;

                // Find the extreme positions of all hexes
                foreach (var kvp in hexCache)
                {
                    var (hexData, instance) = kvp.Value;
                    Vector3 worldPos = instance.transform.position;
                    
                    minX = Mathf.Min(minX, worldPos.x);
                    maxX = Mathf.Max(maxX, worldPos.x);
                    minZ = Mathf.Min(minZ, worldPos.z);
                    maxZ = Mathf.Max(maxZ, worldPos.z);
                }

                // Set clamp bounds
                _clampMin = new Vector2(minX, minZ);
                _clampMax = new Vector2(maxX, maxZ);
            }
            else
            {
                Notebook.NoteError("CameraMoveVisual: Could not access Grid feature visual.");
            }
        }

        public void StartMovement()
        {
            _isMovementEnabled = true;
        }

        public void HaltMovement()
        {
            _isMovementEnabled = false;
        }
    }
}