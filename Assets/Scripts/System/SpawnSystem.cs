using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : IUpdateable
{
    private readonly EnemySystem enemySystem;
    private readonly Player player;

    private WaveData currentWave;
    private IReadOnlyList<EnemyData> availableEnemyTypes;
    private float timer;
    private bool isSpawning;

    public SpawnSystem(EnemySystem enemySystem, Player player)
    {
        this.enemySystem = enemySystem;
        this.player = player;
    }

    public void Configure(
        WaveData wave,
        IReadOnlyList<EnemyData> enemyTypes
    )
    {
        currentWave = wave;
        timer = 0f;
        availableEnemyTypes = enemyTypes;
    }

    public void StartSpawning()
    {
        isSpawning = true;
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    public void ResetSession()
    {
        isSpawning = false;
        currentWave = null;
        timer = 0f;
        availableEnemyTypes = null;
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
        var enemyPool = availableEnemyTypes;

        if (enemyPool == null || enemyPool.Count == 0)
        {
            return null;
        }

        return enemyPool[Random.Range(0, enemyPool.Count)];
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

        return new Vector3(
            playerPosition.x + point.x * currentWave.SpawnRadius,
            playerPosition.y,
            playerPosition.z + point.y * currentWave.SpawnRadius
        );
    }
}