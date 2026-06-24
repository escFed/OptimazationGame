using UnityEngine;
using System.Collections.Generic;
public class ProjectileSystem :IUpdateable
{
    private PoolService poolService;
    private EnemySystem enemySystem;
    private CombatSystem combatSystem;
    private ObjectPool<Projectile> projectilePool;
    private List<Projectile> activeProjectiles = new();
    private int nextId = 1;

    public ProjectileSystem(PoolService poolService, EnemySystem enemySystem, CombatSystem combatSystem)
    {
        this.poolService = poolService;
        this.enemySystem = enemySystem;
        this.combatSystem = combatSystem;
        projectilePool = new ObjectPool<Projectile>(() => new Projectile());
    }

    public void Spawn(ProjectileData data, Vector3 position, Vector3 direction, int ownerId,float damage)
    {
        var instance = poolService.Get(data.Prefab);
        var projectile = projectilePool.Get();

        projectile.Initialize(nextId, data, instance, position, direction, ownerId);
        nextId++;

        activeProjectiles.Add(projectile);
    }

    public void Tick(float deltaTime)
    {
        for (var i = activeProjectiles.Count - 1; i >= 0; i--)
        {
            var projectile = activeProjectiles[i];

            projectile.Position += projectile.Direction * projectile.Data.Speed * deltaTime;

            if (HitEnemy(projectile))
            {
                Despawn(i);
                continue;
            }

            if (projectile.TickLifetime(deltaTime))
            {
                Despawn(i);
            }
        }
    }

    private bool HitEnemy(Projectile projectile)
    {
        var enemies = enemySystem.ActiveEnemies;

        for (var i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i];

            if (!enemy.IsActive || enemy.IsDead)
            {
                continue;
            }

            var delta = enemy.Position - projectile.Position;
            delta.y = 0f;

            var collisionDistance = enemy.Data.CollisionRadius + projectile.Data.CollisionRadius;

            if (delta.sqrMagnitude > collisionDistance * collisionDistance)
            {
                continue;
            }

            combatSystem.ApplyDamage(projectile, enemy, projectile.Data.Damage);
            return true;
        }

        return false;
    }

    private void Despawn(int index)
    {
        var projectile = activeProjectiles[index];

        poolService.Return(projectile.Data.Prefab, projectile.Instance);
        projectilePool.Return(projectile);
        activeProjectiles.RemoveAt(index);
    }
}
