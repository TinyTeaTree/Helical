using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SoundPlayer : WagBehaviour
{
    const float FadeTime = 2.00f;

    [SerializeField] AudioSource _audioSource;

    public bool IsPlaying => _audioSource.isPlaying;
    public string ClipPlaying => _audioSource.clip.name;
    float _volume;

    public void Play(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false, bool killOnEnd = true)
    {
        _volume = volume;

        _audioSource.clip = clip;
        _audioSource.volume = volume;
        _audioSource.pitch = pitch;
        _audioSource.loop = loop;

        _audioSource.Play();

        if (killOnEnd)
        {
            WaitForEnd().Forget();
        }
    }

    public void Pause()
    {
        if(IsPlaying == false)
        {
            return;
        }

        _audioSource.DOFade(0f, FadeTime);
        PauseOnEnd().Forget();
    }

    public void Finish()
    {
        if (IsPlaying)
        {
            _audioSource.DOFade(0f, FadeTime);
        }

        WaitForEnd().Forget();
    }

    public void Resume()
    {
        if(IsPlaying)
        {
            return;
        }

        _audioSource.UnPause();
        _audioSource.DOFade(_volume, FadeTime);
    }

    async UniTaskVoid WaitForEnd()
    {
        await UniTask.WaitWhile(() => IsDead || (_audioSource.isPlaying && _audioSource.volume > 0f));

        if (IsDead)
        {
            return;
        }
        
        Destroy(gameObject);
    }

    async UniTaskVoid PauseOnEnd()
    {
        await UniTask.WaitWhile(() => (_audioSource.isPlaying && _audioSource.volume > 0) || IsDead);

        if (IsDead)
        {
            return;
        }

        _audioSource.Pause();
    }
}