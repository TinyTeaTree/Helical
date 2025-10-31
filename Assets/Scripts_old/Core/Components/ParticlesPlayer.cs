using UnityEngine;

public class ParticlesPlayer : MonoBehaviour
{
    [SerializeField] ParticleSystem _particleSystem;

    public void Play()
    {
        _particleSystem.Play();
    }

    public void Stop()
    {
        _particleSystem.Stop();
    }
}