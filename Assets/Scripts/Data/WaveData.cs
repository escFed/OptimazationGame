using UnityEngine;

[CreateAssetMenu(menuName = "Wave/Wave Data")]
public class WaveData : ScriptableObject
{
    [SerializeField] private int waveNumber = 1;
    [SerializeField] private float duration = 45f;
    [SerializeField] private EnemyData[] enemyTypes;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnRadius = 12f;
    [SerializeField] private int maxEnemies = 20;
    [SerializeField] private bool isFinalWave;

    public int WaveNumber => waveNumber;
    public float Duration => duration;
    public EnemyData[] EnemyTypes => enemyTypes;
    public float SpawnInterval => spawnInterval;
    public float SpawnRadius => spawnRadius;
    public int MaxEnemies => maxEnemies;
    public bool IsFinalWave => isFinalWave;
}
