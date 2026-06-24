using UnityEngine;
using System.Collections.Generic;

public class PoolService
{
    private Dictionary<GameObject, ObjectPool<PooledGameObject>> pools = new();
    private Dictionary<GameObject, PooledGameObject> activeObjects = new();

    public GameObject Get(GameObject prefab)
    {
        var pool = GetOrCreatePool(prefab);
        var pooledObject = pool.Get();
        var instance = pooledObject.Instance;

        activeObjects.Add(instance, pooledObject);

        return instance;
    }

    public void Return(GameObject prefab, GameObject instance)
    {
        var pool = GetOrCreatePool(prefab);
        var pooledObject = activeObjects[instance];

        activeObjects.Remove(instance);
        pool.Return(pooledObject);
    }

    private ObjectPool<PooledGameObject> GetOrCreatePool(GameObject prefab)
    {
        if (pools.TryGetValue(prefab, out var pool))
        {
            return pool;
        }

        pool = new ObjectPool<PooledGameObject>(() =>
        {
            var instance = Object.Instantiate(prefab);
            return new PooledGameObject(instance);
        });

        pools.Add(prefab, pool);
        return pool;
    }
}
