using UnityEngine;
using System.Collections.Generic;

public class EnemySystem : IUpdateable
{
    private PoolService poolService;
    private ObjectPool<Enemy> enemyPool;
    private List<Enemy> activeEnemies = new();
    private int nextEnemyId = 1;

    public EnemySystem(PoolService poolService)
    {
        this.poolService = poolService;
        enemyPool = new ObjectPool<Enemy>(() => new Enemy());
    }

    public int CountActive => activeEnemies.Count;
    public IReadOnlyList<Enemy> ActiveEnemies => activeEnemies;

    public Enemy Spawn(EnemyData data, Vector3 position)
    {
        var instance = poolService.Get(data.Prefab);
        var enemy = enemyPool.Get();

        enemy.Initialize(nextEnemyId, data, instance, position);
        nextEnemyId++;

        activeEnemies.Add(enemy);
        return enemy;
    }

    public void Tick(float deltaTime)
    {
        for (var i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i].IsDead)
            {
                Despawn(i);
            }
        }
    }

    private void Despawn(int index)
    {
        var enemy = activeEnemies[index];

        poolService.Return(enemy.Data.Prefab, enemy.Instance);
        enemyPool.Return(enemy);
        activeEnemies.RemoveAt(index);
    }
}
