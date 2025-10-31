using System.Threading.Tasks;
using UnityEngine;

namespace Services
{
    public class ResourceAssetPackProvider : BaseAssetPackProvider
    {
        private string _path;
        
        public ResourceAssetPackProvider(string path)
        {
            _path = path;
        }
        
        public override Task<T> Load<T>()
        {
            var result = Resources.Load<T>(_path);
            return Task.FromResult(result);
        }
    }
}