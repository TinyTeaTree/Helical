using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class BattleUnitMover : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _moveDuration = 0.5f;
        [SerializeField] private Ease _moveEase = Ease.InOutQuad;
        [SerializeField] private float _jumpHeight = 0.5f;
        
        private Tween _currentMoveTween;
        
        /// <summary>
        /// Moves the unit from its current position to the target world position
        /// </summary>
        public void MoveToPosition(Vector3 targetPosition, System.Action onComplete = null)
        {
            // Kill any existing movement tween
            _currentMoveTween?.Kill();
            
            // Create jump movement using DOTween sequence
            _currentMoveTween = transform.DOJump(targetPosition, _jumpHeight, 1, _moveDuration)
                .SetEase(_moveEase)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                });
        }
        
        /// <summary>
        /// Instantly stops any ongoing movement
        /// </summary>
        public void StopMovement()
        {
            _currentMoveTween?.Kill();
        }
        
        private void OnDestroy()
        {
            _currentMoveTween?.Kill();
        }
    }
}

