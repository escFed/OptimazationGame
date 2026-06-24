using System;
using System.Collections.Generic;

public class ServiceLocator
{
    private readonly Dictionary<Type, object> services = new();

    public void Register<T>(T service) where T : class
    {
        var type = typeof(T);
        if (!services.TryAdd(type, service))
        {
            throw new InvalidOperationException($"Service of type {type.Name} is already registered.");
        }
    }

    public T Get<T>() where T : class
    {
        var type = typeof(T);
        if (!services.TryGetValue(type, out var service))
        {
            throw new InvalidOperationException($"Service not found: {type.Name}");
        }
        return (T)service;
    }

    public bool Has<T>() where T : class
    {
        return services.ContainsKey(typeof(T));
    }
}
