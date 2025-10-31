using UnityEngine;
using UnityEngine.Audio;

namespace Services
{
    public class SoundPlayerGO : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup _musicMixer;
        [SerializeField] private AudioMixerGroup _effectMixer;
        [SerializeField] private AudioMixerGroup _ambientMixer; 

        [SerializeField] private SoundChannel _effectChannel;
        [SerializeField] private SoundChannel _vocalChannel;
        [SerializeField] private SoundChannel _musicChannel;
        [SerializeField] private SoundChannel _ambientChannel;

        public void GetPlayer(GetPlayerEvent e)
        {
            var player = _effectChannel.GetPlayer(e.SoundDesign);
            if (player == null)
                player = _ambientChannel.GetPlayer(e.SoundDesign);
            if (player == null)
                player = _musicChannel.GetPlayer(e.SoundDesign);
            if (player == null)
                player = _vocalChannel.GetPlayer(e.SoundDesign);

            e.Player = player;
        }

        public void PlayMusic(PlayMusicEvent e)
        {
            _musicChannel.PlayMusic(e.SoundDesign);
        }

        public void EndMusic(EndMusicEvent e)
        {
            _musicChannel.EndMusic(e.SoundDesign);
        }

        public void StopSound(StopSoundEvent e)
        {
            _effectChannel.Stop(e.SoundDesign);
            _musicChannel.Stop(e.SoundDesign);
            _ambientChannel.Stop(e.SoundDesign);
            _vocalChannel.Stop(e.SoundDesign);
        }

        public void PlaySound(PlaySoundEvent e)
        {
            var soundDesign = e.SoundDesign;

            switch (soundDesign.Type)
            {
                case SoundType.Effect:
                    _effectChannel.Play(soundDesign);
                    break;
                case SoundType.Ambient:
                    _ambientChannel.Play(soundDesign);
                    break;
                case SoundType.Music:
                    _musicChannel.Play(soundDesign);
                    break;
                case SoundType.Vocal:
                    _vocalChannel.Play(soundDesign);
                    break;
            }
        }
    }
}