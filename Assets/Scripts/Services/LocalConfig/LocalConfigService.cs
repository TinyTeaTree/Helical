using System.Linq;
using Core;

namespace Services
{
    public class LocalConfigService : BaseService, ILocalConfigService
    {
        private LocalConfigCollectionSO _so;

        public void SetConfigSO(LocalConfigCollectionSO so)
        {
            _so = so;
        }
        
        public T GetConfig<T>()
            where T : BaseConfig
        {
            var configs = _so.Configs;
            var configSO = configs.First(c => c.Config.GetType() == typeof(T));
            var result = (T)configSO.Config;
            return result;
        }
    }
}