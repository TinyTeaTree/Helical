using UnityEngine;

namespace Game
{
    public abstract class BaseBattleUnit : MonoBehaviour
    {
        [SerializeField] private string _id;
        public string Id => _id;
        
        [SerializeField] protected Animator _animator;

        public virtual void Initialize(string unitId)
        {
            OnInitialized(unitId);
        }
        
        public abstract void Attack();
        public abstract void SetIsMove(bool isMoving);
        public abstract void SetIsDead(bool isDead);
        public abstract void GetHit();
        
        public abstract void SetGlow(bool isGlowing);

        protected virtual void OnInitialized(string unitId)
        {
            // Override in derived classes for custom initialization
        }
    }
}
