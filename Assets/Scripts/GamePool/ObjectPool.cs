using System;
using System.Collections.Generic;

public class ObjectPool<T> : IPool<T>
    where T : class, IPoolable
{
    private readonly Func<T> factory;

    private readonly Stack<T> inactiveItems = new();
    private readonly HashSet<T> activeItems = new();

    public ObjectPool(Func<T> factory)
    {
        this.factory = factory ??
            throw new ArgumentNullException(
                nameof(factory)
            );
    }

    public int CountActive => activeItems.Count;
    public int CountInactive => inactiveItems.Count;

    public T Get()
    {
        var item = inactiveItems.Count > 0
            ? inactiveItems.Pop()
            : factory();

        activeItems.Add(item);
        item.OnSpawn();

        return item;
    }

    public void Return(T item)
    {
        if (!activeItems.Remove(item))
        {
            throw new InvalidOperationException(
                $"Item does not belong to this active pool: " +
                $"{typeof(T).Name}"
            );
        }

        item.OnDespawn();
        inactiveItems.Push(item);
    }

    public void Prewarm(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        var currentCapacity =
            inactiveItems.Count +
            activeItems.Count;

        var amountToCreate =
            amount - currentCapacity;

        for (var i = 0; i < amountToCreate; i++)
        {
            inactiveItems.Push(
                factory()
            );
        }
    }
}