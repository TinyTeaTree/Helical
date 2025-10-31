using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class WagAudioManager : WagMonoton<WagAudioManager>
{
    [SerializeField] SoundPlayer _effectPlayerOriginal;
    [SerializeField] SoundPlayer _musicPlayerOriginal;

    [SerializeField] AudioMixer _masterMixer;

    List<SoundPlayer> _musicStack = new List<SoundPlayer>();

    public void SetMute(bool mute)
    {
        if (mute)
        {
            _masterMixer.SetFloat("Volume", -80f);
        }
        else
        {
            _masterMixer.SetFloat("Volume", 0f);
        }
    }

    public void Play(ISoundConfig soundConfig)
    {
        if(soundConfig.Type == AudioType.Effects)
        {
            PlayEffect(soundConfig);
        }
        else if(soundConfig.Type == AudioType.Music)
        {
            PlayMusic(soundConfig);
        }
    }

    public void Stop(ISoundConfig instruction)
    {
        if (instruction.Type == AudioType.Effects)
        {
            Debug.LogWarning("Can't stop an effect, no support");
        }
        else if (instruction.Type == AudioType.Music)
        {
            StopMusic(instruction);
        }
    }

    void PlayEffect(ISoundConfig instruction)
    {
        var effectPlayer = Instantiate(_effectPlayerOriginal, transform);
        effectPlayer.Play(instruction.Clip, instruction.Volume, instruction.Pitch, false, true);
        effectPlayer.name = $"{instruction.Type} {instruction.Clip}";
    }

    void PlayMusic(ISoundConfig instruction)
    {
        var musicPlayer = Instantiate(_musicPlayerOriginal, transform);
        musicPlayer.Play(instruction.Clip, instruction.Volume, instruction.Pitch, true, false);
        musicPlayer.name = $"{instruction.Type} {instruction.Clip}";


        if (_musicStack.Any())
        {
            var lastMusic = _musicStack.Last();

            lastMusic.Pause();
        }
        
        _musicStack.Add(musicPlayer);
    }

    void StopMusic(ISoundConfig instruction)
    {
        if (!_musicStack.Any())
        {
            return;
        }

        for(int i = _musicStack.Count - 1; i>= 0; --i)
        {
            var musicPlayer = _musicStack[i];

            if(musicPlayer.ClipPlaying == instruction.Clip.name)
            {
                musicPlayer.Finish();
                _musicStack.RemoveAt(i);
                break;
            }
        }
    }
}