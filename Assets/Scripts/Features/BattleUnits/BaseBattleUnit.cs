using UnityEngine;

namespace Game
{
    public abstract class BaseBattleUnit : MonoBehaviour
    {
        [SerializeField] private string _id;
        public string Id => _id;
        
        [SerializeField] protected Animator _animator;
        [SerializeField] protected BattleUnitMover _mover;
        [SerializeField] protected BattleUnitRotator _rotator;

        public virtual void Initialize(string unitId)
        {
            OnInitialized(unitId);
        }
        
        public abstract void Attack();
        public abstract void SetIsMove(bool isMoving);
        public abstract void SetIsDead(bool isDead);
        public abstract void GetHit();
        
        public abstract void SetGlow(bool isGlowing);
        
        /// <summary>
        /// Moves the unit to the target world position
        /// </summary>
        public virtual void Move(Vector3 targetPosition, System.Action onComplete = null)
        {
            // Set the Move animation to true
            SetIsMove(true);
            
            // Move to the target position
            _mover.MoveToPosition(targetPosition, () =>
            {
                // Set the Move animation to false when movement is complete
                SetIsMove(false);
                onComplete?.Invoke();
            });
        }
        
        /// <summary>
        /// Rotates the unit to face towards a target coordinate.
        /// The rotator calculates and performs the rotation, returning the new direction.
        /// </summary>
        /// <param name="fromCoordinate">The unit's current coordinate</param>
        /// <param name="toCoordinate">The target coordinate to face towards</param>
        /// <param name="currentDirection">The unit's current facing direction</param>
        /// <param name="onComplete">Callback when rotation is complete, receives the new target direction</param>
        public virtual void Rotate(Vector2Int fromCoordinate, Vector2Int toCoordinate, HexDirection currentDirection, System.Action<HexDirection> onComplete = null)
        {
            _rotator.RotateTowardsCoordinate(fromCoordinate, toCoordinate, currentDirection, onComplete);
        }

        protected virtual void OnInitialized(string unitId)
        {
            // Override in derived classes for custom initialization
        }
    }
}
