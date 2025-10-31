using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Core
{
    public abstract class BaseFactory
    {
        public abstract UniTask<TypeVisual> Create<TypeVisual>()
            where TypeVisual : BaseVisual;
    }
}