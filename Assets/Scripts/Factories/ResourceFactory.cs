using System.Threading.Tasks;
using Core;
using Cysharp.Threading.Tasks;

namespace Factories
{
    public class ResourceFactory : BaseFactory
    {
        private string _loadPath;

        public ResourceFactory(string path)
        {
            _loadPath = path;
        }
        
        public override UniTask<TypeVisual> Create<TypeVisual>()
        {
            var path = _loadPath.HasContent() ? _loadPath : typeof(TypeVisual).Name;
            var visual = UnityEngine.Object.Instantiate(UnityEngine.Resources.Load<TypeVisual>(path));
            return UniTask.FromResult(visual);
        }
    }
}