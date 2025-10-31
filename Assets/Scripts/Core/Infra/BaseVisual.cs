using UnityEngine;

namespace Core
{
    public class BaseVisual : MonoBehaviour, IAmDestructible
    {
        protected BaseFeature _baseFeature;
        
        public void SetFeature<TFeature>(TFeature feature)
            where TFeature : BaseFeature
        {
            _baseFeature = feature;
        }
        
        public void SelfDestroy()
        {
            Destroy(gameObject);
        }
    }
    public class BaseVisual<TFeature> : BaseVisual
        where TFeature : BaseFeature
    {
        protected TFeature Feature => _baseFeature as TFeature;
    }
}