using System;
using System.Threading.Tasks;
using Core;
using UnityEngine;

namespace Services
{
    public interface ISummoningService : IService
    {
        void SetProvider(Type type, BaseAssetPackProvider provider);

        T CreateAsset<T>(T loadedAsset, Transform parent)
            where T : UnityEngine.Object;
        
        T LoadResource<T>(string resourcePath)
            where T : UnityEngine.Object;

        Task<TAssetPack> LoadAssetPack<TAssetPack>()
            where TAssetPack : BaseAssetPack;
    }
}