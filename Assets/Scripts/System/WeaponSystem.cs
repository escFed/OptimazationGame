using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : IUpdateable
{
    private Player player;
    private ProjectileSystem projectileSystem;
    private ProjectileData projectileData;

    private List<ActivePattern> activePatterns = new();
    private List<Vector3> directions = new();

    private float cooldownTimer;

    public WeaponSystem(Player player, ProjectileSystem projectileSystem, ProjectileData projectileData, PatternData initialPattern)
    {
        this.player = player;
        this.projectileSystem = projectileSystem;
        this.projectileData = projectileData;

        AddPattern(initialPattern);
    }

    public IReadOnlyList<ActivePattern> ActivePatterns => activePatterns;

    public void Tick(float deltaTime)
    {
        if (!player.IsActive || deltaTime <= 0f)
        {
            return;
        }

        for (var i = 0; i < activePatterns.Count; i++)
        {
            var activePattern = activePatterns[i];

            activePattern.Tick(deltaTime);

            if (!activePattern.CanFire)
            {
                continue;
            }

            Shoot(activePattern.Pattern);
            activePattern.ResetCooldown(player.Stats.AttackSpeed);
        }
    }

    public void AddPattern(PatternData newPattern)
    {
        if (newPattern == null || HasPattern(newPattern))
        {
            return;
        }

        activePatterns.Add(new ActivePattern(newPattern));
    }

    private bool HasPattern(PatternData pattern)
    {
        for (var i = 0; i < activePatterns.Count; i++)
        {
            if (activePatterns[i].Pattern == pattern)
            {
                return true;
            }
        }

        return false;
    }

    private void Shoot(PatternData pattern)
    {
        directions.Clear();
        pattern.GetDirections(player.Transform, directions);

        for (var i = 0; i < directions.Count; i++)
        {
            var direction = directions[i];

            if (direction.sqrMagnitude <= 0f)
            {
                continue;
            }

            var normalizedDirection = direction.normalized;
            var spawnPosition = player.Position + normalizedDirection * projectileData.SpawnOffset;
            var damage = player.Stats.Damage + projectileData.Damage;

            projectileSystem.Spawn(projectileData, spawnPosition, normalizedDirection, player.Id, damage);

        }
    }
}
