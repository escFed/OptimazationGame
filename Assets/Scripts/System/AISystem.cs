using UnityEngine;

public class AISystem : IUpdateable
{
    private Player player;
    private EnemySystem enemySystem;

    public AISystem(Player player, EnemySystem enemySystem)
    {
        this.player = player;
        this.enemySystem = enemySystem;
    }

    public void Tick(float deltaTime)
    {
        var enemies = enemySystem.ActiveEnemies;

        for (var i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i];

            if (!enemy.IsActive || enemy.IsDead)
            {
                continue;
            }

            var toPlayer = player.Position - enemy.Position;
            toPlayer.y = 0f;

            var stopDistance = enemy.Data.AttackRange;

            if (toPlayer.sqrMagnitude <= stopDistance * stopDistance)
            {
                enemy.FaceDirection(toPlayer);
                continue;
            }

            var direction = toPlayer.normalized;

            enemy.Position += direction * enemy.Data.Speed * deltaTime;
            enemy.FaceDirection(direction);
        }
    }
}
