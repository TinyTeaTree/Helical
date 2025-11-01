using UnityEngine;

namespace Game
{
    public class SimpleBattleUnit : BaseBattleUnit
    {
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int GetHitTrigger = Animator.StringToHash("GetHit");
        private static readonly int IsMoveBool = Animator.StringToHash("IsMove");
        private static readonly int IsDeadBool = Animator.StringToHash("IsDead");

        protected override void OnInitialized(string unitId)
        {
            base.OnInitialized(unitId);
            // Simple battle unit initialization
        }

        public override void Attack()
        {
            _animator.SetTrigger(AttackTrigger);
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
    }
}
