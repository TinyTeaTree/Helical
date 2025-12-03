using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class SimpleBattleUnit : BaseBattleUnit
    {
        [SerializeField] private BattleUnitGlow _glowComponent;
        [SerializeField] private AnimationClip _attackAnimationClip;
        
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int GetHitTrigger = Animator.StringToHash("GetHit");
        private static readonly int IsMoveBool = Animator.StringToHash("IsWalk");
        private static readonly int IsDeadBool = Animator.StringToHash("IsDead");

        private float AttackAnimationDuration => _attackAnimationClip.length; //TODO: Use this to set the Animator speed to adjust for Action Points

        protected override void OnInitialized(string unitId)
        {
            base.OnInitialized(unitId);
            // Simple battle unit initialization
        }

        public override async UniTask Attack(float duration)
        {
            // Calculate animator speed to make animation take the desired duration
            float speedMultiplier = AttackAnimationDuration / duration;
            _animator.speed = speedMultiplier;

            _animator.SetTrigger(AttackTrigger);

            // Wait for the attack animation to complete
            await UniTask.Delay(TimeSpan.FromSeconds(duration));

            // Always reset animator speed to 1
            _animator.speed = 1f;
        }

        public override void GetHit()
        {
            _animator.SetTrigger(GetHitTrigger);
        }

        public override void SetIsMove(bool isMoving)
        {
            _animator.SetBool(IsMoveBool, isMoving);
        }

        public override void SetIsDead(bool isDead)
        {
            _animator.SetBool(IsDeadBool, isDead);
        }
        
        public override void SetGlow(bool isGlowing)
        {
            _glowComponent?.SetGlow(isGlowing);
        }

        public override void UpdateHealthBar(float healthPercentage)
        {
            if (_healthBarWidget != null)
            {
                _healthBarWidget.UpdateFill(healthPercentage);
            }
        }
    }
}
