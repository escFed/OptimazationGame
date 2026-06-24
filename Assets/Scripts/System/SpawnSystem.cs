using UnityEngine;

public class SpawnSystem : IUpdateable
{
    private EnemySystem enemySystem;
    private Player player;
    private EnemyData enemyData;
    private WaveData currentWave;
    private float timer;
    private bool isSpawning;

    public SpawnSystem(EnemySystem enemySystem, Player player)
    {
        this.enemySystem = enemySystem;
        this.player = player;
    }

    public void Configure(WaveData wave)
    {
        currentWave = wave;
        timer = 0f;
    }

    public void StartSpawning()
    {
        isSpawning = true;
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    public void Tick(float deltaTime)
    {
        if (!isSpawning || currentWave == null)
        {
            return;
        }

        if (enemySystem.CountActive >= currentWave.MaxEnemies)
        {
            return;
        }

        timer -= deltaTime;

        if (timer > 0f)
        {
            return;
        }

        var enemyData = GetEnemyData();

        if (enemyData == null)
        {
            StopSpawning();
            return;
        }

        enemySystem.Spawn(enemyData, GetSpawnPosition());
        timer = currentWave.SpawnInterval;
    }

    private EnemyData GetEnemyData()
    {
        var enemyPool = currentWave.EnemyTypes;

        if (enemyPool == null || enemyPool.Length == 0)
        {
            return null;
        }

        return enemyPool[Random.Range(0, enemyPool.Length)];
    }

    private Vector3 GetSpawnPosition()
    {
        var point = Random.insideUnitCircle;

        if (point.sqrMagnitude <= 0f)
        {
            point = Vector2.up;
        }

        point.Normalize();

        var playerPosition = player.Position;
        return new Vector3(playerPosition.x + point.x * currentWave.SpawnRadius, playerPosition.y, playerPosition.z + point.y * currentWave.SpawnRadius);
    }
}
