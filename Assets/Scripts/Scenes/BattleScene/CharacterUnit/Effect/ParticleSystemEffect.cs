using UnityEngine;
public class ParticleSystemEffect : EffectEmitBase
{
    [SerializeField] ParticleSystem[] _particleSystem;
    [SerializeField] private float randomPercent = 10;

    public override void Emit(Vector3 vector3)
    {
        foreach (var particle in _particleSystem)
        {
            particle.Play();
        }
        var audio = GetComponent<AudioSource>();
        if (audio == null) return;
        audio.volume = Config.seVolume;
        audio.pitch *= 1 + Random.Range(-randomPercent / 100, randomPercent / 100);
        audio.Play();
    }
}
