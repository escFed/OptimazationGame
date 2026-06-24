using System.Collections.Generic;
using UnityEngine;

public class CustomUpdateManager : MonoBehaviour
{
    private readonly List<IUpdateable> updateSystems = new();
    private readonly List<IUpdateable> fixedUpdateSystems = new();
    private readonly List<IUpdateable> lateUpdateSystems = new();

    public void Register(IUpdateable system)
    {
        RegisterUpdate(system);
    }

    public void Unregister(IUpdateable system)
    {
        updateSystems.Remove(system);
        fixedUpdateSystems.Remove(system);
        lateUpdateSystems.Remove(system);
    }

    public void RegisterUpdate(IUpdateable system)
    {
        RegisterSystem(updateSystems, system);
    }

    public void RegisterFixedUpdate(IUpdateable system)
    {
        RegisterSystem(fixedUpdateSystems, system);
    }

    public void RegisterLateUpdate(IUpdateable system)
    {
        RegisterSystem(lateUpdateSystems, system);
    }

    private void Update()
    {
        Tick(updateSystems, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Tick(fixedUpdateSystems, Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        Tick(lateUpdateSystems, Time.deltaTime);
    }

    private static void RegisterSystem(List<IUpdateable> systems, IUpdateable system)
    {
        if (!systems.Contains(system))
        {
            systems.Add(system);
        }
    }

    private static void Tick(List<IUpdateable> systems, float deltaTime)
    {
        for (var i = 0; i < systems.Count; i++)
        {
            systems[i].Tick(deltaTime);
        }
    }
}
