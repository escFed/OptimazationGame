using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : IUpdateable
{
    private Player player;
    private ProjectileSystem projectileSystem;
    private ProjectileData projectileData;
    private PatternData patternData;
    private List<Vector3> directions = new (8);
    private float cooldownTimer;

    public WeaponSystem(Player player, ProjectileSystem projectileSystem, ProjectileData projectileData, PatternData patternData)
    {
        this.player = player;
        this.projectileSystem = projectileSystem;
        this.projectileData = projectileData;
        this.patternData = patternData;
    }

    public void Tick(float deltaTime)
    {
        if (!player.IsActive) return;

        cooldownTimer -= deltaTime;
        if (cooldownTimer > 0f) return;

        Shoot();

        var shotsPerSecond = Mathf.Max(0.01f, player.Stats.AttackSpeed);
        cooldownTimer = 1f / shotsPerSecond;
    }

    public void SetPattern(PatternData newPattern)
    {
        if (newPattern != null)
        {
            patternData = newPattern;
        }
    }

    private void Shoot()
    {
        if (patternData == null)
        {
            return;
        }

        directions.Clear();
        patternData.GetDirections(player.Transform, directions);

        for (var i = 0; i < directions.Count; i++)
        {
            var direction = directions[i];

            if (direction.sqrMagnitude <= 0f)
            {
                continue;
            }

            var spawnPosition = player.Position + direction.normalized * projectileData.SpawnOffset;
            var damage = player.Stats.Damage + projectileData.Damage;
            projectileSystem.Spawn(projectileData, spawnPosition, direction, player.Id, damage);
        }
    }
}
