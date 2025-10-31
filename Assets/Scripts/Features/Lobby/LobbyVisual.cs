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
        [SerializeField] private LobbySettings _settings;
        
        [SerializeField] private UnityEngine.UI.Button _newGameButton;
        [SerializeField] private UnityEngine.UI.Button _settingsButton;
        [SerializeField] private UnityEngine.UI.Button _exitButton;
        
        private void OnEnable()
        {
			_newGameButton.onClick.AddListener(OnNewGameClicked);
			_settingsButton.onClick.AddListener(OnSettingsClicked);
			_exitButton.onClick.AddListener(OnExitClicked);

            _settings.gameObject.SetActive(false);
        }

		private void OnDisable()
		{
			_newGameButton.onClick.RemoveListener(OnNewGameClicked);
			_settingsButton.onClick.RemoveListener(OnSettingsClicked);
			_exitButton.onClick.RemoveListener(OnExitClicked);
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

		private void OnSettingsClicked()
		{
			DJ.Play(DJ.Click_Sound);
            
            _settings.gameObject.SetActive(true);
            _options.gameObject.SetActive(false);
		}

		private void OnExitClicked()
		{
			DJ.Play(DJ.Click_Sound);
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

        public void ExitSettings()
        {
            _settings.gameObject.SetActive(false);
            _options.gameObject.SetActive(true);
        }
    }
}