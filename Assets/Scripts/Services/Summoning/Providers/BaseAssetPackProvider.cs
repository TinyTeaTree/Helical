using System.Threading.Tasks;

namespace Services
{
    public abstract class BaseAssetPackProvider
    {
        public abstract Task<TypeAssetPack> Load<TypeAssetPack>()
            where TypeAssetPack : BaseAssetPack;
    }
}