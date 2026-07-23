using System.Collections.Generic;
using UnityEngine;

public class EffectSystem : IUpdateable
{
    private class ActiveEffect
    {
        public GameObject Prefab;
        public GameObject Instance;
        public ParticleSystem[] ParticleSystems;
    }

    private PoolService poolService;

    private List<ActiveEffect>activeEffects = new();

    private readonly Dictionary<GameObject, ParticleSystem[] > particleSystemsByInstance = new();

    public EffectSystem(PoolService poolService)
    {
        this.poolService = poolService;
    }

    public void Play(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            return;
        }

        var instance = poolService.Get(prefab);

        instance.transform.SetPositionAndRotation(position, rotation);

        var particleSystems = GetParticleSystems(instance);

        for (var i = 0; i < particleSystems.Length;i++)
        {
            particleSystems[i].Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

            particleSystems[i].Play(false);
        }

        activeEffects.Add(new ActiveEffect{Prefab = prefab, Instance = instance, ParticleSystems = particleSystems});
    }

    public void Tick(float deltaTime)
    {
        for (var i = activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = activeEffects[i];

            if (IsAlive(effect))
            {
                continue;
            }

            ReturnEffect(i);
        }
    }

    public void ResetSession()
    {
        for (var i = activeEffects.Count - 1; i >= 0; i--)
        {
            ReturnEffect(i);
        }
    }

    private ParticleSystem[] GetParticleSystems(GameObject instance)
    {
        if (particleSystemsByInstance.TryGetValue(instance, out var particleSystems))
        {
            return particleSystems;
        }

        particleSystems = instance.GetComponentsInChildren<ParticleSystem>(true);

        particleSystemsByInstance.Add(instance, particleSystems);

        return particleSystems;
    }

    private static bool IsAlive(ActiveEffect effect)
    {
        for (var i = 0; i < effect.ParticleSystems.Length; i++)
        {
            if (effect.ParticleSystems[i].IsAlive(false))
            {
                return true;
            }
        }

        return false;
    }

    private void ReturnEffect(int index)
    {
        var effect = activeEffects[index];

        poolService.Return(effect.Prefab, effect.Instance);

        activeEffects.RemoveAt(index);
    }
}
