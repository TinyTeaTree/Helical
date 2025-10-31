using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Services
{
    public abstract class BaseAssetPackProvider
    {
        public abstract UniTask<TypeAssetPack> Load<TypeAssetPack>()
            where TypeAssetPack : BaseAssetPack;
    }
}