using UnityEngine;
public class ParticleSystemEffect : EffectEmitBase
{
    [SerializeField] ParticleSystem[] _particleSystem;
    [SerializeField] private float randomPercent = 10;

    public override void Emit(Vector3 vector3)
    {
        var audio = GetComponent<AudioSource>();
        audio.pitch *= 1 + Random.Range(-randomPercent / 100, randomPercent / 100);
        audio.Play();
        foreach (var particle in _particleSystem)
        {
            particle.Play();
        }
    }
}
