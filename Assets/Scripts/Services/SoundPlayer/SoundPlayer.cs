using System.Collections;
using Core;
using UnityEngine;

namespace Services
{
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _source;

        public AudioClip Clip => _source.clip;

        public IDesignSound Design { get; private set; }
        public bool IsFadingOut { get; private set; }
        public bool IsFadingIn { get; private set; }

        public void Play(IDesignSound soundDesign, System.Action onDone) //We dont use tasks to maximise optimization, this is super internal and should not be made with tasks.
        {
            if (soundDesign.Clip == null)
            {
                onDone.Invoke();
                Notebook.NoteWarning("No clip was provided to play sound");
                return;
            }

            Design = soundDesign;
            
            _source.clip = soundDesign.Clip;
            _source.pitch = soundDesign.Pitch;
            _source.loop = soundDesign.Loop;
            
            IsFadingOut = false;
            IsFadingIn = false;

            if (soundDesign.FadeIn > 0f)
            {
                _source.volume = 0f;
                FadeIn();
            }
            else
            {
                _source.volume = soundDesign.Volume;
            }
            
            _source.Play();

            StartCoroutine(PlayRoutine(onDone));
        }

        public void FadeOut()
        {
            if (IsFadingOut)
                return;

            IsFadingOut = true;
            IsFadingIn = false;

            //TODO add fade
            _source.volume = 0f;
        }

        public void FadeIn()
        {
            if (IsFadingIn)
                return;

            IsFadingOut = false;
            IsFadingIn = true;
            
            //TODO: Add Fade
            _source.volume = Design.Volume;
        }

        public void SetPitch(float pitch)
        {
            _source.pitch = pitch;
        }
        
        private IEnumerator PlayRoutine(System.Action onDone)
        {
            while (_source.isPlaying)
                yield return null;

            onDone.Invoke();
        }

        public void Stop()
        {
            _source.Stop();
        }

        public void StopWhenFadedOut()
        {
            StartCoroutine(StopWhenFadedOutRoutine());
        }

        private IEnumerator StopWhenFadedOutRoutine()
        {
            while (_source.volume > 0.001f)
                yield return null;

            Stop();
        }

        public void Clean()
        {
            _source.clip = null;
            Design = null;
        }
    }
}