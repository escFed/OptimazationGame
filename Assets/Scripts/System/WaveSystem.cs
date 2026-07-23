using System;
using System.Collections.Generic;

public class WaveSystem : IUpdateable
{
    private readonly WaveData[] waves;
    private readonly SpawnSystem spawnSystem;
    private readonly EnemySystem enemySystem;
    private readonly List<EnemyData> availableEnemyTypes = new();

    private int currentWaveIndex;
    private float timeRemaining;
    private bool isRunning;
    private bool isWaitingForClear;

    public WaveSystem(
        WaveData[] waves,
        SpawnSystem spawnSystem,
        EnemySystem enemySystem
    )
    {
        this.waves = waves;
        this.spawnSystem = spawnSystem;
        this.enemySystem = enemySystem;
    }

    public event Action<WaveData> WaveStarted;
    public event Action<WaveData> WaveCompleted;
    public event Action<float, float> WaveTimeChanged;
    public event Action GameCompleted;

    public WaveData CurrentWave { get; private set; }
    public bool IsGameCompleted { get; private set; }
    public bool IsRunning => isRunning;
    public float TimeRemaining => timeRemaining;

    public void Start()
    {
        availableEnemyTypes.Clear();
        currentWaveIndex = 0;
        IsGameCompleted = false;
        StartCurrentWave();
    }

    public void ResetSession()
    {
        spawnSystem.ResetSession();

        currentWaveIndex = 0;
        timeRemaining = 0f;
        isRunning = false;
        isWaitingForClear = false;

        availableEnemyTypes.Clear();
        CurrentWave = null;
        IsGameCompleted = false;
    }

    public void Tick(float deltaTime)
    {
        if (!isRunning || CurrentWave == null)
        {
            return;
        }

        if (timeRemaining > 0f)
        {
            timeRemaining = Math.Max(0f, timeRemaining - deltaTime);
            WaveTimeChanged?.Invoke(
                timeRemaining,
                CurrentWave.Duration
            );

            if (timeRemaining <= 0f)
            {
                spawnSystem.StopSpawning();
                isWaitingForClear = true;
            }
        }

        if (isWaitingForClear && enemySystem.CountActive == 0)
        {
            CompleteCurrentWave();
        }
    }

    public void ContinueToNextWave()
    {
        if (isRunning || IsGameCompleted)
        {
            return;
        }

        currentWaveIndex++;
        StartCurrentWave();
    }

    private void StartCurrentWave()
    {
        if (waves == null || currentWaveIndex >= waves.Length)
        {
            CompleteGame();
            return;
        }

        CurrentWave = waves[currentWaveIndex];

        if (CurrentWave == null)
        {
            currentWaveIndex++;
            StartCurrentWave();
            return;
        }

        timeRemaining = CurrentWave.Duration;
        isRunning = true;
        isWaitingForClear = false;

        AddEnemyTypes(CurrentWave);
        spawnSystem.Configure(
            CurrentWave,
            availableEnemyTypes
        );
        spawnSystem.StartSpawning();

        WaveStarted?.Invoke(CurrentWave);
        WaveTimeChanged?.Invoke(
            timeRemaining,
            CurrentWave.Duration
        );
    }

    private void AddEnemyTypes(WaveData wave)
    {
        var enemyTypes = wave.EnemyTypes;

        if (enemyTypes == null)
        {
            return;
        }

        for (var i = 0; i < enemyTypes.Length; i++)
        {
            var enemyType = enemyTypes[i];

            if (
                enemyType != null &&
                !availableEnemyTypes.Contains(enemyType)
            )
            {
                availableEnemyTypes.Add(enemyType);
            }
        }
    }

    private void CompleteCurrentWave()
    {
        var completedWave = CurrentWave;

        spawnSystem.StopSpawning();

        if (completedWave.IsFinalWave)
        {
            WaveCompleted?.Invoke(completedWave);
            CompleteGame();
            return;
        }

        isRunning = false;
        WaveCompleted?.Invoke(completedWave);
    }

    private void CompleteGame()
    {
        isRunning = false;
        IsGameCompleted = true;
        CurrentWave = null;

        spawnSystem.StopSpawning();
        GameCompleted?.Invoke();
    }
}