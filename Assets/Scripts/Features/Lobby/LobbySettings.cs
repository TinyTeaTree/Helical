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
		public bool MusicOn => _musicToggle.isOn;
		public bool EffectOn => _audioToggle.isOn;

		private void OnEnable()
		{
			_musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);

			_audioToggle.onValueChanged.AddListener(OnAudioToggleChanged);

			_exitButton.onClick.AddListener(OnExitClicked);
		}

		private void OnDisable()
		{
			_musicToggle.onValueChanged.RemoveListener(OnMusicToggleChanged);

			_audioToggle.onValueChanged.RemoveListener(OnAudioToggleChanged);

			_exitButton.onClick.RemoveListener(OnExitClicked);
		}

		private void OnMusicToggleChanged(bool isOn)
		{
			DJ.Play(isOn ? DJ.ClickOn_Sound : DJ.ClickOff_Sound);
			_onMusicToggled?.Invoke(isOn);
		}

		private void OnAudioToggleChanged(bool isOn)
		{
			DJ.Play(isOn ? DJ.ClickOn_Sound : DJ.ClickOff_Sound);

			_onAudioToggled?.Invoke(isOn);
		}

		private void OnExitClicked()
		{
			DJ.Play(DJ.ClickOff_Sound);

			_visual.ExitSettings();
		}

		public void SetupSettings(PlayerSettingsRecord settingsRecord)
		{
			_musicToggle.SetIsOnWithoutNotify(settingsRecord.Music);
			_audioToggle.SetIsOnWithoutNotify(settingsRecord.Effects);
		}
	}
}


