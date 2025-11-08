using System;
using Core;
using Services;
using UnityEngine;

namespace Game
{
    public class LoadingScreenVisual : BaseVisual<LoadingScreenFeature>
    {
        [SerializeField] private Canvas _canvas;
        public Canvas Canvas => _canvas;

        private LoadingScreenPage _currentPage;

        public void Close()
        {
            Destroy(_currentPage.gameObject);
            _currentPage = null;
        }

        public void UpdateProgress()
        {
            
        }

        internal void ShowPage(LoadingScreenPage page)
        {
            _currentPage = Summoner.CreateAsset(page, _canvas.transform);
        }
    }
}