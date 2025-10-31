using System.Threading.Tasks;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Factories
{
    public class AsyncResourceFactory : BaseFactory
    {
        private string _loadPath;

        public AsyncResourceFactory(string path)
        {
            _loadPath = path;
        }
        
        public override async UniTask<TypeVisual> Create<TypeVisual>()
        {
            var loadedVisualAsync = Resources.LoadAsync<TypeVisual>(_loadPath);
            await UniTask.WaitUntil(() => loadedVisualAsync.isDone);
            var loadedVisual = (TypeVisual)loadedVisualAsync.asset;
            var visual = Object.Instantiate(loadedVisual);
            return visual;
        }
    }
}