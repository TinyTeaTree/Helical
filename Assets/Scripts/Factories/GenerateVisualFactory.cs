using Core;
using Cysharp.Threading.Tasks;

namespace Factories
{
    //When you dont really need anything on the Visual, so it can be created on the fly, no need for a prefab
    public class GenerateVisualFactory : BaseFactory
    {
        public override UniTask<TypeVisual> Create<TypeVisual>()
        {
            var go = new UnityEngine.GameObject($"{typeof(TypeVisual)} (Generated)");
            var visual = go.AddComponent<TypeVisual>();
            
            return UniTask.FromResult(visual);
        }
    }
}