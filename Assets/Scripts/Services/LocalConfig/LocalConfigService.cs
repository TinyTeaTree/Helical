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
            var configSO = configs.FirstOrDefault(c => c.Config.GetType() == typeof(T));
            if (configSO == null)
            {
                Notebook.NoteError("You might have forgotten to add your Config SO to the Local Config group SO for config type " + typeof(T));
                return null;
            }
            var result = (T)configSO.Config;
            return result;
        }
    }
}