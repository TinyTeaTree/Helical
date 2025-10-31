using System.Threading.Tasks;
using Agents;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Services
{
    public class SoundPlayerService : BaseService, ISoundPlayerService, IAppLaunchAgent
    {
        private SoundPlayerGO _gb;
        
        public UniTask AppLaunch()
        {
            var summoner = GameInfra.Single.Services.Get<ISummoningService>();
            var soundPlayerResource = summoner.LoadResource<SoundPlayerGO>(Addresses.SoundPlayer);
            _gb = summoner.CreateAsset(soundPlayerResource, null);

            return UniTask.CompletedTask;
        }
        
        public void Play(IDesignSound soundDesign)
        {
            if (soundDesign?.Clip == null)
            {
                Notebook.NoteError("Missing sound or Clip");
                return;
            }
            
            _gb.PlaySound(new PlaySoundEvent { SoundDesign = soundDesign });
        }

        public void Play(IDesignSound soundDesign, Vector3 position)
        {
            //TODO: Add 3d
            Play(soundDesign);
        }

        public void Play(IDesignSound soundDesign, Transform transform)
        {
            //TODO: Add 3d
            Play(soundDesign);
        }

        private void Stop(AudioClip clip)
        {
            if(clip != null)
                Stop(clip.name);
        }

        private void Stop(string clipName)
        {
            if (string.IsNullOrEmpty(clipName))
            {
                Notebook.NoteError("Missing clip name to stop");
                return;
            }
            
            _gb.StopSound(new StopSoundEvent { ClipName = clipName });
        }

        public void Stop(IDesignSound soundDesign)
        {
            if (soundDesign?.Clip == null)
            {
                Notebook.NoteError("Missing clip name to stop");
                return;
            }
            
            _gb.StopSound(new StopSoundEvent { SoundDesign = soundDesign });
        }

        public SoundPlayer GetPlayer(IDesignSound soundDesign)
        {
            var getEvent = new GetPlayerEvent { SoundDesign = soundDesign };
            _gb.GetPlayer(getEvent);
            return getEvent.Player;
        }

        public void PlayMusic(IDesignSound soundDesign)
        {
            if (soundDesign?.Clip == null)
            {
                Notebook.NoteError("Missing sound or Clip");
                return;
            }
            
            _gb.PlayMusic(new PlayMusicEvent { SoundDesign = soundDesign });
        }

        public void EndMusic(IDesignSound soundDesign)
        {
            if (soundDesign?.Clip == null)
            {
                Notebook.NoteError("Missing sound or Clip");
                return;
            }
            
            _gb.EndMusic(new EndMusicEvent { SoundDesign = soundDesign });
        }
    }
}