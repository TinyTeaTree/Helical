using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using UnityEngine;

namespace Services
{
    public class SummoningService : BaseService, ISummoningService
    {
        private Dictionary<Type, BaseAssetPackProvider> _providers = new();
        
        public void SetProvider(Type type, BaseAssetPackProvider provider)
        {
            _providers[type] = provider;
        }
        
        public T LoadResource<T>(string resourcePath)
            where T : UnityEngine.Object
        {
            return Resources.Load<T>(resourcePath);
        }

        public T CreateAsset<T>(T loadedAsset, Transform parent)
            where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(loadedAsset, parent);
        }

        public Task<TAssetPack> LoadAssetPack<TAssetPack>() 
            where TAssetPack : BaseAssetPack
        {
            if (_providers.TryGetValue(typeof(TAssetPack), out var provider))
            {
                return provider.Load<TAssetPack>();
            }
            else
            {
                Notebook.NoteError($"No provider for {typeof(TAssetPack)} found");
                return Task.FromResult((TAssetPack)null);
            }
        }
    }
}