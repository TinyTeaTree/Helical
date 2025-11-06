using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// A visual based Feature Script
    /// </summary>
    /// <typeparam name="VisualType"></typeparam>
    public class BaseVisualFeature<VisualType> : BaseFeature
        where VisualType : BaseVisual
    {
        protected VisualType _visual;

        protected BaseFactory _factory;

        public override void Bootstrap(IBootstrap bootstrap)
        {
            base.Bootstrap(bootstrap);
            
            if(bootstrap.Factories.TryGetValue(typeof(VisualType), out var factory))
            {
                _factory = factory;
            }
        }

        protected async UniTask CreateVisual()
        {
            if (_factory == null)
            {
                Notebook.NoteError($"No factory set in {this.GetType()}");
                return;
            }
            
            if(_visual != null)
            {
                Notebook.NoteError($"Visual already exists for {typeof(VisualType)}");
                return;
            }
            
            _visual = await _factory.Create<VisualType>();
            
            _visual.SetFeature(this);
        }
    }
}