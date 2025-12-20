using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
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
        [SerializeField, CanBeNull] protected BattleUnitShooter _shooter;
        [SerializeField] protected Transform _healthBarAnchor;
        [SerializeField] protected Transform _damageAnchor;

        protected BattleUnitHealthBar _healthBarWidget;
        protected IBattleUnits _battleUnitsFeature;
        protected string _instanceGuid;

        public BattleUnitHealthBar HealthBarWidget => _healthBarWidget;

        public Transform HealthBarAnchor => _healthBarAnchor;
        public Transform DamageAnchor => _damageAnchor;
        public string InstanceGuid { get => _instanceGuid; set => _instanceGuid = value; }

        public virtual void Initialize(string unitId)
        {
            OnInitialized(unitId);
        }
        
        public abstract UniTask Attack(float duration, float interceptionTime, System.Action onInterception);
        public abstract void SetIsMove(bool isMoving);
        public abstract void SetIsDead(bool isDead);
        public abstract void GetHit();
        
        public abstract void SetGlow(bool isGlowing);

        public abstract void UpdateHealthBar(float healthPercentage);

        public void SetHealthBarWidget(BattleUnitHealthBar widget)
        {
            _healthBarWidget = widget;
        }

        public void SetBattleUnitsFeature(IBattleUnits feature)
        {
            _battleUnitsFeature = feature;
        }

        public void ShowDamageText(string damageText)
        {
            if (_battleUnitsFeature != null)
            {
                _battleUnitsFeature.ShowDamageText(damageText, _damageAnchor);
            }
        }

        /// <summary>
        /// Moves the unit to the target world position
        /// </summary>
        public virtual async UniTask Move(Vector3 targetPosition, float duration)
        {
            // Set the Move animation to true
            SetIsMove(true);

            // Move to the target position with specified duration
            await _mover.MoveToPosition(targetPosition, duration);

            // Set the Move animation to false when movement is complete
            SetIsMove(false);
        }
        
        /// <summary>
        /// Rotates the unit to face towards a target coordinate.
        /// The rotator calculates and performs the rotation, returning the new direction.
        /// </summary>
        /// <param name="fromCoordinate">The unit's current coordinate</param>
        /// <param name="toCoordinate">The target coordinate to face towards</param>
        /// <param name="currentDirection">The unit's current facing direction</param>
        /// <param name="duration">Duration in seconds for the rotation</param>
        public virtual UniTask<HexDirection> Rotate(Vector2Int fromCoordinate, Vector2Int toCoordinate, HexDirection currentDirection, float duration)
        {
            return _rotator.RotateTowardsCoordinate(fromCoordinate, toCoordinate, currentDirection, duration);
        }

        /// <summary>
        /// Shoots a projectile towards a target coordinate with the specified duration.
        /// Only works if a shooter component is attached.
        /// </summary>
        /// <param name="fromCoordinate">The unit's current coordinate</param>
        /// <param name="toCoordinate">The target coordinate to shoot towards</param>
        /// <param name="duration">How long in seconds the projectile flight should take</param>
        public virtual async UniTask Shoot(Vector2Int fromCoordinate, Vector2Int toCoordinate, float duration)
        {
            if (_shooter != null)
            {
                await _shooter.ShootTowardsCoordinate(fromCoordinate, toCoordinate, duration);
            }
        }


        protected virtual void OnInitialized(string unitId)
        {
            // Override in derived classes for custom initialization
        }
    }
}
