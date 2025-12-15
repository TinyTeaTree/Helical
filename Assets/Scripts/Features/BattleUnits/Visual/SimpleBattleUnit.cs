using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Services;
using UnityEngine;

namespace Game
{
    public class SimpleBattleUnit : BaseBattleUnit
    {
        [SerializeField, CanBeNull] private BattleUnitGlow _glowComponent;
        [SerializeField] private AnimationClip _attackAnimationClip;

        [SerializeField, CanBeNull] private BaseSoundDesign _hitSound;
        
       // [SerializeField] private HexAreaPattern _pattern;

        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int GetHitTrigger = Animator.StringToHash("GetHit");
        private static readonly int IsMoveBool = Animator.StringToHash("IsWalk");
        private static readonly int DieTrigger = Animator.StringToHash("Die");

        public HexAreaPattern _pattern;

        private float AttackAnimationDuration => _attackAnimationClip.length; //TODO: Use this to set the Animator speed to adjust for Action Points

        protected override void OnInitialized(string unitId)
        {
            base.OnInitialized(unitId);
            // Simple battle unit initialization
        }

        public override async UniTask Attack(float duration, float interceptionTime, System.Action onInterception)
        {
            // Calculate animator speed to make animation take the desired duration
            float speedMultiplier = AttackAnimationDuration / duration;
            _animator.speed = speedMultiplier;

            _animator.SetTrigger(AttackTrigger);

            // Wait for interception time and trigger damage calculation
            if (interceptionTime > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(interceptionTime));
                onInterception?.Invoke();
            }

            // Wait for the remaining attack animation to complete
            float remainingTime = duration - interceptionTime;
            if (remainingTime > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(remainingTime));
            }

            // Always reset animator speed to 1
            _animator.speed = 1f;
        }

        public override void GetHit()
        {
            _animator.SetTrigger(GetHitTrigger);

            if (_hitSound != null)
                DJ.Play(_hitSound);
        }

        public override void SetGlow(bool isGlowing)
        {
            _glowComponent?.SetGlow(isGlowing);
        }

        public override void UpdateHealthBar(float healthPercentage)
        {
            _healthBarWidget.UpdateFill(healthPercentage);
        }

        public override void SetIsMove(bool isMoving)
        {
            _animator.SetBool(IsMoveBool, isMoving);
        }

        public override void SetIsDead(bool isDead)
        {
            _animator.SetTrigger(DieTrigger);
        }
    }
}


