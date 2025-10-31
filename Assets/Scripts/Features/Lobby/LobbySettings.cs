using System;
using Services;
using UnityEngine;

namespace Game
{
	public class LobbySettings : MonoBehaviour
	{
		[SerializeField] private UnityEngine.UI.Toggle _musicToggle;
		[SerializeField] private UnityEngine.UI.Toggle _audioToggle;
		[SerializeField] private UnityEngine.UI.Button _exitButton;

		[SerializeField] private UnityEngine.Events.UnityEvent<bool> _onMusicToggled;
		[SerializeField] private UnityEngine.Events.UnityEvent<bool> _onAudioToggled;

		[SerializeField] private LobbyVisual _visual;

		private void OnEnable()
		{
			if (_musicToggle != null)
				_musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);

			if (_audioToggle != null)
				_audioToggle.onValueChanged.AddListener(OnAudioToggleChanged);

			if (_exitButton != null)
				_exitButton.onClick.AddListener(OnExitClicked);
		}

		private void OnDisable()
		{
			if (_musicToggle != null)
				_musicToggle.onValueChanged.RemoveListener(OnMusicToggleChanged);

			if (_audioToggle != null)
				_audioToggle.onValueChanged.RemoveListener(OnAudioToggleChanged);

			if (_exitButton != null)
				_exitButton.onClick.RemoveListener(OnExitClicked);
		}

		private void OnMusicToggleChanged(bool isOn)
		{
			DJ.Play(DJ.Click_Sound);
			_onMusicToggled?.Invoke(isOn);
		}

		private void OnAudioToggleChanged(bool isOn)
		{
			DJ.Play(DJ.Click_Sound);
			// Basic default behavior: toggle global audio. Projects with mixers can override via UnityEvent.
			AudioListener.pause = !isOn;
			_onAudioToggled?.Invoke(isOn);
		}

		private void OnExitClicked()
		{
			DJ.Play(DJ.Click_Sound);

			_visual.ExitSettings();

		}
	}
}


