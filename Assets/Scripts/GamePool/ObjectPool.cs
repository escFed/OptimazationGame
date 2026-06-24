using System;
using System.Collections.Generic;

public class ObjectPool<T> : IPool<T> where T : class, IPoolable
{
    private Func<T> factory;
    private Stack<T> inactiveItems = new();
    private HashSet<T> activeItems = new();

    public ObjectPool(Func<T> factory)
    {
        this.factory = factory;
    }

    public int CountActive => activeItems.Count;
    public int CountInactive => inactiveItems.Count;

    public T Get()
    {
        var item = inactiveItems.Count > 0 ? inactiveItems.Pop() : factory();

        activeItems.Add(item);
        item.OnSpawn();

        return item;
    }

    public void Return(T item)
    {
        if (!activeItems.Remove(item))
        {
            throw new InvalidOperationException($"Item does not belong to this active pool: {typeof(T).Name}");
        }

        item.OnDespawn();
        inactiveItems.Push(item);
    }
}