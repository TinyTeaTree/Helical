using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class BattleUnitMover : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private Ease _moveEase = Ease.InOutQuad;
        [SerializeField] private float _jumpHeight = 0.5f;
        [SerializeField] private AnimationCurve _moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private bool _useAnimationCurve = false;
        
        private Tween _currentMoveTween;
        
        /// <summary>
        /// Moves the unit from its current position to the target world position
        /// </summary>
        public async UniTask MoveToPosition(Vector3 targetPosition, float duration)
        {
            // Kill any existing movement tween
            _currentMoveTween?.Kill();
            
            // Create jump movement using DOTween sequence
            _currentMoveTween = transform.DOJump(targetPosition, _jumpHeight, 1, duration);

            // Apply easing based on selection
            if (_useAnimationCurve)
            {
                _currentMoveTween.SetEase(_moveCurve);
            }
            else
            {
                _currentMoveTween.SetEase(_moveEase);
            }

            await _currentMoveTween.AsyncWaitForCompletion();
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

