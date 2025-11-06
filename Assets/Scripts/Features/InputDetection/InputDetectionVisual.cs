using Core;
using UnityEngine;

namespace Game
{
    public class InputDetectionVisual : BaseVisual<InputDetection>
    {
        [Header("Detection Settings")]
        [SerializeField] private float _dragThreshold = 5f; // Pixels before it's considered a drag
        
        private bool _isMouseDown = false;
        private Vector2 _mouseDownPosition;
        private Vector2 _lastMousePosition;
        private bool _isDragging = false;
        
        private void Update()
        {
            if (!Feature.Record.IsInputEnabled)
            {
                return;
            }
            
            // Detect mouse button down
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseDown();
            }
            
            // Track mouse movement while button is held
            if (_isMouseDown)
            {
                Vector2 currentMousePosition = Input.mousePosition;
                
                // Check if we've moved enough to start dragging
                if (!_isDragging)
                {
                    float distanceMoved = Vector2.Distance(currentMousePosition, _mouseDownPosition);
                    if (distanceMoved > _dragThreshold)
                    {
                        _isDragging = true;
                    }
                }
                
                // If we're dragging, send drag delta
                if (_isDragging)
                {
                    Vector2 dragDelta = currentMousePosition - _lastMousePosition;
                    Feature.HandleDrag(dragDelta);
                }
                
                _lastMousePosition = currentMousePosition;
            }
            
            // Detect mouse button up
            if (Input.GetMouseButtonUp(0))
            {
                OnMouseUp();
            }
        }
        
        private void OnMouseDown()
        {
            _isMouseDown = true;
            _mouseDownPosition = Input.mousePosition;
            _lastMousePosition = Input.mousePosition;
            _isDragging = false;
        }
        
        private void OnMouseUp()
        {
            // Only trigger click if we weren't dragging
            if (!_isDragging)
            {
                Feature.HandleClick();
            }
            
            // Reset state
            _isMouseDown = false;
            _isDragging = false;
        }
    }
}