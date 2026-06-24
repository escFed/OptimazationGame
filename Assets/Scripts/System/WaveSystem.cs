using System;

public class WaveSystem : IUpdateable
{
    private WaveData[] waves;
    private SpawnSystem spawnSystem;
    private EnemySystem enemySystem;
    private int currentWaveIndex;
    private float timeRemaining;
    private bool isRunning;
    private bool isWaitingForClear;

    public WaveSystem(WaveData[] waves, SpawnSystem spawnSystem, EnemySystem enemySystem)
    {
        this.waves = waves;
        this.spawnSystem = spawnSystem;
        this.enemySystem = enemySystem;
    }

    public event Action<WaveData> WaveStarted;
    public event Action<WaveData> WaveCompleted;
    public event Action GameCompleted;

    public WaveData CurrentWave { get; private set; }
    public bool IsGameCompleted { get; private set; }
    public float TimeRemaining => timeRemaining;

    public void Start()
    {
        currentWaveIndex = 0;
        IsGameCompleted = false;
        StartCurrentWave();
    }

    public void Tick(float deltaTime)
    {
        if (!isRunning || CurrentWave == null)
        {
            return;
        }

        if (timeRemaining > 0f)
        {
            timeRemaining -= deltaTime;

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

        spawnSystem.Configure(CurrentWave);
        spawnSystem.StartSpawning();
        WaveStarted?.Invoke(CurrentWave);
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

    public void ContinueToNextWave()
    {
        if (isRunning || IsGameCompleted)
        {
            return;
        }

        currentWaveIndex++;
        StartCurrentWave();
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
