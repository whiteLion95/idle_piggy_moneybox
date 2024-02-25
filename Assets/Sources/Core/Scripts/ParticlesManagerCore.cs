using EZ_Pooling;
using System.Collections;
using UnityEngine;

/// <summary>
/// A base class for particles managers
/// </summary>
public class ParticlesManagerCore : MonoBehaviour
{
    [SerializeField] protected ParticlesData particlesData;

    public ParticleSystem PlayParticleOnce(Particles particleName, Vector3 position, Quaternion rotation, float duration = 0f)
    {
        ParticleSystem playingParticle = PlayParticle(particleName, position, rotation);
        StartCoroutine(DespawnParticleRoutine(playingParticle, duration));

        return playingParticle;
    }

    public void PlayParticle(Particles particleName, Vector3 position, Quaternion rotation, Color color)
    {
        ParticleSystem playingParticle = PlayParticleOnce(particleName, position, rotation);

        ParticleSystem.MainModule settings = playingParticle.main;
        settings.startColor = new ParticleSystem.MinMaxGradient(color);
    }

    public ParticleSystem PlayParticle(Particles particleName, Vector3 position, Quaternion rotation)
    {
        ParticleData particleData = particlesData[particleName];
        ParticleSystem playingParticle = EZ_PoolManager.Spawn(particleData.Prefab.transform, position + particleData.Offset, rotation).GetComponentInChildren<ParticleSystem>();

        playingParticle.transform.localScale = particleData.Scale;
        playingParticle.Play();

        return playingParticle;
    }

    protected IEnumerator DespawnParticleRoutine(ParticleSystem particle, float duration = 0f)
    {
        float despawnDelay = duration == 0f ? particle.main.duration + 0.5f : duration;

        yield return new WaitForSeconds(despawnDelay);

        if (particle != null)
            EZ_PoolManager.Despawn(particle.transform.parent);
    }

    public void DespawnParticle(ParticleSystem particle)
    {
        EZ_PoolManager.Despawn(particle.transform);
    }
}