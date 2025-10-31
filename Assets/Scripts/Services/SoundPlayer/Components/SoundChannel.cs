using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Services
{
    public class SoundChannel : MonoBehaviour
    {
        [SerializeField] private SoundPlayer _original;
        [SerializeField] private Transform _poolParent;
        private Stack<SoundPlayer> _pool = new();
        private List<SoundPlayer> _playing = new();

        private List<SoundPlayer> _musicStack = new();

        public void Play(IDesignSound soundDesign)
        {
            var player = GetSoundPlayer();

            _playing.Add(player);
            
#if UNITY_EDITOR
            player.name = $"{soundDesign.Clip.name} - {soundDesign.Type.ToString()}";
#endif
            
            player.Play(soundDesign, () =>
            {
                player.transform.SetParent(_poolParent);
                _playing.Remove(player);
                _pool.Push(player);
                player.Clean();
            });
        }

        public SoundPlayer GetPlayer(IDesignSound soundDesign)
        {
            foreach (var player in _playing)
            {
                if (player.Design == soundDesign)
                    return player;
            }

            return null;
        }

        private SoundPlayer GetSoundPlayer()
        {
            SoundPlayer player;
            if (_pool.Count == 0)
            {
                player = Instantiate(_original, transform);
            }
            else
            {
                player = _pool.Pop();
                player.transform.SetParent(transform);
            }

            return player;
        }

        public void PlayMusic(IDesignSound soundDesign)
        {
            foreach (var player in _musicStack)
            {
                if (player.Clip == soundDesign.Clip)
                {
                    return; //This Music is already in the stack
                }
            }

            var newPlayer = GetSoundPlayer();
            
#if UNITY_EDITOR
            newPlayer.name = $"{soundDesign.Clip.name} - {soundDesign.Type.ToString()}";
#endif
            
            foreach (var player in _musicStack)
            {
                player.FadeOut();
            }
            
            _musicStack.Add(newPlayer);
            newPlayer.Play(soundDesign, () =>
            {
                newPlayer.transform.SetParent(_poolParent);
                _pool.Push(newPlayer);
                newPlayer.Clean();

                if (_musicStack.Count > 1 && _musicStack.Last() == newPlayer)
                {
                    var returnPlayer = _musicStack[^2];
                    returnPlayer.FadeIn();
                }
                
                _musicStack.Remove(newPlayer);
            });
        }

        public void EndMusic(IDesignSound soundDesign)
        {
            var musicPlayer = _musicStack.FirstOrDefault(p => p.Clip == soundDesign.Clip);
            if (musicPlayer == null)
                return;
            
            musicPlayer.FadeOut();
            
            if (_musicStack.Count > 1 && _musicStack.Last() == musicPlayer)
            {
                var returnPlayer = _musicStack[^2];
                returnPlayer.FadeIn();
            }
            
            _musicStack.Remove(musicPlayer);
            musicPlayer.StopWhenFadedOut();
        }

        public void Stop(IDesignSound soundDesign)
        {
            var clipName = soundDesign.Clip.name; //TODO: Name based comparison is very slow, I suspect that reference based comparison might suit our project. So we should try it out sometimes
            for (int i = 0; i < _playing.Count; ++i)
            {
                if (_playing[i].Clip.name == clipName)
                {
                    if(soundDesign.FadeOut == 0)
                    {
                        _playing[i].Stop();
                    }                    
                    else
                    {
                        _playing[i].FadeOut();
                        _playing[i].StopWhenFadedOut();
                    }
                }
            }
        }
    }
}