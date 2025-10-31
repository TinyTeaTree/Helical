using System.Threading.Tasks;
using Core;
using Cysharp.Threading.Tasks;

namespace Factories
{
    public class SelfProvidedFactory : BaseFactory
    {
        private BaseVisual _providedVisual;
        
        public void ProvideVisual(BaseVisual visual)
        {
            _providedVisual = visual;
        }
        
        public override UniTask<TypeVisual> Create<TypeVisual>()
        {
            return UniTask.FromResult((TypeVisual)_providedVisual);
        }
    }
}