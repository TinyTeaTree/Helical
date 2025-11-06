using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class BattleUnitRotator : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private float _rotationDuration = 0.3f;
        [SerializeField] private Ease _rotationEase = Ease.OutQuad;
        
        private Tween _currentRotationTween;
        
        /// <summary>
        /// Rotates the unit to face towards a target coordinate.
        /// Calculates the nearest HexDirection and performs the rotation.
        /// </summary>
        /// <param name="fromCoordinate">The unit's current coordinate</param>
        /// <param name="toCoordinate">The target coordinate to face towards</param>
        /// <param name="currentDirection">The unit's current facing direction</param>
        /// <param name="onComplete">Callback when rotation is complete, receives the new target direction</param>
        public void RotateTowardsCoordinate(Vector2Int fromCoordinate, Vector2Int toCoordinate, HexDirection currentDirection, System.Action<HexDirection> onComplete = null)
        {
            // Calculate the nearest direction from current to target coordinate
            HexDirection targetDirection = HexDirectionExtensions.GetNearestDirection(fromCoordinate, toCoordinate);
            
            // Check if already facing that direction
            if (currentDirection == targetDirection)
            {
                onComplete?.Invoke(targetDirection);
                return;
            }
            
            // Calculate rotation direction
            int rotationDirection = currentDirection.GetRotationDirection(targetDirection);
            
            // Perform the rotation
            RotateToDirection(targetDirection, rotationDirection, () =>
            {
                onComplete?.Invoke(targetDirection);
            });
        }
        
        /// <summary>
        /// Rotates the unit to face the target direction
        /// </summary>
        /// <param name="targetDirection">The HexDirection to face</param>
        /// <param name="rotationDirection">+1 for clockwise, -1 for counter-clockwise</param>
        /// <param name="onComplete">Callback when rotation is complete</param>
        private void RotateToDirection(HexDirection targetDirection, int rotationDirection, System.Action onComplete = null)
        {
            // Kill any existing rotation tween
            _currentRotationTween?.Kill();
            
            // Get target rotation
            Quaternion targetRotation = targetDirection.ToRotation();
            
            // If rotation direction is specified, we need to determine the path
            if (rotationDirection != 0)
            {
                // Calculate the angle difference and ensure we rotate in the specified direction
                float currentY = transform.eulerAngles.y;
                float targetY = targetDirection.ToAngle();
                
                // Normalize current angle to [0, 360)
                while (currentY < 0f) currentY += 360f;
                while (currentY >= 360f) currentY -= 360f;
                
                // Calculate the total rotation needed
                float angleDiff = targetY - currentY;
                
                // Adjust based on rotation direction
                if (rotationDirection > 0) // Clockwise
                {
                    // If difference is negative, add 360 to make it positive (clockwise)
                    if (angleDiff < 0)
                        angleDiff += 360f;
                }
                else // Counter-clockwise
                {
                    // If difference is positive, subtract 360 to make it negative (counter-clockwise)
                    if (angleDiff > 0)
                        angleDiff -= 360f;
                }
                
                // Create rotation tween using the calculated angle
                _currentRotationTween = transform.DORotate(
                    new Vector3(0f, currentY + angleDiff, 0f),
                    _rotationDuration,
                    RotateMode.FastBeyond360
                )
                .SetEase(_rotationEase)
                .OnComplete(() =>
                {
                    // Ensure we end up at exactly the target rotation
                    transform.rotation = targetRotation;
                    onComplete?.Invoke();
                });
            }
            else
            {
                // No rotation needed, already facing target
                onComplete?.Invoke();
            }
        }
        
        /// <summary>
        /// Instantly stops any ongoing rotation
        /// </summary>
        public void StopRotation()
        {
            _currentRotationTween?.Kill();
        }
        
        private void OnDestroy()
        {
            _currentRotationTween?.Kill();
        }
    }
}

