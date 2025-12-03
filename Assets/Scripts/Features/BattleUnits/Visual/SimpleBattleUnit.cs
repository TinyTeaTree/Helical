using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class SimpleBattleUnit : BaseBattleUnit
    {
        [SerializeField] private BattleUnitGlow _glowComponent;
        
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int GetHitTrigger = Animator.StringToHash("GetHit");
        private static readonly int IsMoveBool = Animator.StringToHash("IsWalk");
        private static readonly int IsDeadBool = Animator.StringToHash("IsDead");

        protected override void OnInitialized(string unitId)
        {
            base.OnInitialized(unitId);
            // Simple battle unit initialization
        }

        public override async UniTask Attack()
        {
            _animator.SetTrigger(AttackTrigger);
            
            //TODO: actually delay action points
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
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
    }
}
