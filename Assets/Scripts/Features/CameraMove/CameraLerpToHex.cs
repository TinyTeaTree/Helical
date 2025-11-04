using Core;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class CameraLerpToHex : MonoBehaviour
    {
        [Header("Lerp Settings")]
        [SerializeField] private float _lerpDuration = 0.5f;
        [SerializeField] private Ease _lerpEase = Ease.OutQuad;
        
        private Tween _activeTween;
        
        public void LerpToWorldPosition(Vector3 targetWorldPosition, Transform cameraTransform)
        {
            // Cancel any existing tween
            CancelLerp();
            
            // The target position should be where the camera center looks at
            // We need to position the camera so it looks at the target world position
            Vector3 targetCameraPosition = CalculateCameraPositionForTarget(targetWorldPosition, cameraTransform);
            
            // Start the tween
            _activeTween = cameraTransform.DOMove(targetCameraPosition, _lerpDuration)
                .SetEase(_lerpEase)
                .OnComplete(() => _activeTween = null);
        }
        
        public void CancelLerp()
        {
            if (_activeTween != null && _activeTween.IsActive())
            {
                _activeTween.Kill();
                _activeTween = null;
            }
        }
        
        public bool IsLerping()
        {
            return _activeTween != null && _activeTween.IsActive();
        }
        
        private Vector3 CalculateCameraPositionForTarget(Vector3 targetWorldPosition, Transform cameraTransform)
        {
            // Calculate where the camera's forward vector intersects the XZ plane
            Vector3 cameraPos = cameraTransform.position;
            Vector3 forward = cameraTransform.forward;
            
            if (Mathf.Approximately(forward.y, 0f))
            {
                Notebook.NoteError("CameraLerpToHex: Camera forward is parallel to ground plane.");
                return cameraPos;
            }
            
            // Calculate the offset from camera to its current center point
            float t = -cameraPos.y / forward.y;
            Vector3 currentCenterPoint = cameraPos + t * forward;
            Vector3 offset = currentCenterPoint - cameraPos;
            
            // Calculate the camera position that would center on the target
            Vector3 newCameraPos = new Vector3(
                targetWorldPosition.x - offset.x,
                cameraPos.y, // Keep the same height
                targetWorldPosition.z - offset.z
            );
            
            return newCameraPos;
        }
    }
}

