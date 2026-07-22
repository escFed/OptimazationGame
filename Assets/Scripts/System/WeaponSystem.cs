using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : IUpdateable
{
    private readonly Player player;
    private readonly ProjectileSystem projectileSystem;
    private readonly ProjectileData projectileData;
    private readonly PatternData initialPattern;

    private readonly List<ActivePattern> activePatterns = new();
    private readonly List<Vector3> directions = new();

    public WeaponSystem(Player player, ProjectileSystem projectileSystem, ProjectileData projectileData, PatternData initialPattern)
    {
        this.player = player;
        this.projectileSystem = projectileSystem;
        this.projectileData = projectileData;
        this.initialPattern = initialPattern;

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

    public void ResetSession()
    {
        if (activePatterns.Count > 1)
        {
            activePatterns.RemoveRange(1, activePatterns.Count - 1);
        }

        if (activePatterns.Count == 0 && initialPattern != null)
        {
            AddPattern(initialPattern);
        }

        if (activePatterns.Count > 0)
        {
            activePatterns[0].Reset();
        }

        directions.Clear();
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

            projectileSystem.Spawn(
                projectileData,
                spawnPosition,
                normalizedDirection,
                player.Id,
                damage
            );
        }
    }
}
