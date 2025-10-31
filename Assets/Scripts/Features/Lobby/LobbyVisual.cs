using System;
using Core;
using DG.Tweening;
using Services;
using UnityEngine;

namespace Game
{
    public class LobbyVisual : BaseVisual<Lobby>
    {
        [SerializeField] private CanvasGroup _cg;
        [SerializeField] private GameObject _options;
        
        [SerializeField] private UnityEngine.UI.Button _newGameButton;
        [SerializeField] private UnityEngine.UI.Button _settingsButton;
        [SerializeField] private UnityEngine.UI.Button _exitButton;
        
        private void OnEnable()
        {
            _newGameButton.onClick.AddListener(OnNewGameClicked);
        }

        public void Hide(bool immediate = false)
        {
            if (immediate)
            {
                HideImmediately();
            }
            else
            {
                HideAnimate();
            }


        }

        private void HideAnimate()
        {
            gameObject.SetActive(true);
            _cg.alpha = 1;
            _cg.interactable = _cg.blocksRaycasts = false;
            _cg.DOFade(0f, 0.5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
            _options.SetActive(false);
        }

        private void HideImmediately()
        {
            _cg.alpha = 0;
            _cg.interactable = _cg.blocksRaycasts = false;
            gameObject.SetActive(false);
            _options.SetActive(false);
        }

        public void Show()
        {
            _cg.alpha = 0;
            _cg.interactable = _cg.blocksRaycasts = false;
            gameObject.SetActive(true);
            
            _cg.DOFade(1f, 0.5f).OnComplete(() =>
            {
                _cg.interactable = _cg.blocksRaycasts = true;
            });
        }

        public void DisplayOptions()
        {
            _options.SetActive(true);
        }

        private void OnNewGameClicked()
        {
            DJ.Play(DJ.Click_Sound);
        }
    }
}