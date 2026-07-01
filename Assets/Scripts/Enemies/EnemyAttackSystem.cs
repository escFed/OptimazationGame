using UnityEngine;

public class EnemyAttackSystem : IUpdateable
{
    private Player player;
    private EnemySystem enemySystem;
    private CombatSystem combatSystem;
    private GameStateSystem gameStateSystem;

    public EnemyAttackSystem(Player player, EnemySystem enemySystem, CombatSystem combatSystem, GameStateSystem gameStateSystem)
    {
        this.player = player;
        this.enemySystem = enemySystem;
        this.combatSystem = combatSystem;
        this.gameStateSystem = gameStateSystem;
    }

    public void Tick(float deltaTime)
    {
        if (!gameStateSystem.IsPlaying || player.IsDead)
        {
            return;
        }

        var enemies = enemySystem.ActiveEnemies;

        for (var i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i];

            if (!enemy.IsActive || enemy.IsDead)
            {
                continue;
            }

            enemy.TickAttackCooldown(deltaTime);

            if (!enemy.CanAttack)
            {
                continue;
            }

            var delta = player.Position - enemy.Position;
            delta.y = 0f;

            var attackDistance = enemy.Data.AttackRange + enemy.Data.CollisionRadius;

            if (delta.sqrMagnitude > attackDistance * attackDistance)
            {
                continue;
            }

            combatSystem.ApplyDamage(enemy, player, enemy.Data.Damage);
            enemy.ResetAttackCooldown();
        }
    }
}
