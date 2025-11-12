using Core;
using UnityEngine;

namespace Game
{
    public class InputDetectionVisual : BaseVisual<InputDetectionFeature>
    {
        [Header("Detection Settings")]
        [SerializeField] private float _dragThreshold = 5f; // Pixels before it's considered a drag

        private bool _isLeftMouseDown = false;
        private Vector2 _mouseDownPosition;
        private Vector2 _lastMousePosition;
        private bool _isDragging = false;
        
        private void Update()
        {
            if (!Feature.Record.IsInputEnabled)
            {
                return;
            }

            // Detect left mouse button down (for selection and dragging)
            if (Input.GetMouseButtonDown(0))
            {
                OnLeftMouseDown();
            }

            // Track mouse movement while left button is held (for dragging)
            if (_isLeftMouseDown)
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

            // Detect left mouse button up
            if (Input.GetMouseButtonUp(0))
            {
                OnLeftMouseUp();
            }

            // Detect right mouse button up
            if (Input.GetMouseButtonUp(1))
            {
                OnRightMouseUp();
            }
        }
        
        private void OnLeftMouseDown()
        {
            _isLeftMouseDown = true;
            _mouseDownPosition = Input.mousePosition;
            _lastMousePosition = Input.mousePosition;
            _isDragging = false;
        }

        private void OnLeftMouseUp()
        {
            // Only trigger click if we weren't dragging
            if (!_isDragging)
            {
                Feature.HandleLeftClick();
            }

            // Reset state
            _isLeftMouseDown = false;
            _isDragging = false;
        }

        private void OnRightMouseUp()
        {
            // Right click is immediate - no drag consideration needed
            Feature.HandleRightClick();
        }
    }
}