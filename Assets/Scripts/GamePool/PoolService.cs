using System.Collections.Generic;
using UnityEngine;

public class PoolService
{
    private readonly Dictionary<
        GameObject,
        ObjectPool<PooledGameObject>
    > pools = new();

    private readonly Dictionary<
        GameObject,
        PooledGameObject
    > activeObjects = new();

    public GameObject Get(GameObject prefab)
    {
        var pool = GetOrCreatePool(prefab);
        var pooledObject = pool.Get();
        var instance = pooledObject.Instance;

        activeObjects.Add(
            instance,
            pooledObject
        );

        return instance;
    }

    public void Return(
        GameObject prefab,
        GameObject instance
    )
    {
        if (prefab == null || instance == null)
        {
            return;
        }

        if (
            !activeObjects.TryGetValue(
                instance,
                out var pooledObject
            )
        )
        {
            return;
        }

        var pool = GetOrCreatePool(prefab);

        activeObjects.Remove(instance);
        pool.Return(pooledObject);
    }

    public void Prewarm(
        GameObject prefab,
        int amount
    )
    {
        if (prefab == null || amount <= 0)
        {
            return;
        }

        var pool = GetOrCreatePool(prefab);
        pool.Prewarm(amount);
    }

    private ObjectPool<PooledGameObject>
        GetOrCreatePool(GameObject prefab)
    {
        if (pools.TryGetValue(prefab, out var pool))
        {
            return pool;
        }

        pool = new ObjectPool<PooledGameObject>(
            () =>
            {
                var instance =
                    Object.Instantiate(prefab);

                return new PooledGameObject(
                    instance
                );
            }
        );

        pools.Add(prefab, pool);

        return pool;
    }
}