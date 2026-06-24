using UnityEngine;

public class Enemy : IEntity, IDamageable, IPoolable
{
    private GameObject instance;
    private float currentHealth;

    public int Id { get; private set; }
    public EnemyData Data { get; private set; }
    public bool IsDead => currentHealth <= 0f;
    public bool IsActive => instance != null && instance.activeInHierarchy;
    public GameObject Instance => instance;

    public Vector3 Position
    {
        get => instance.transform.position;
        set => instance.transform.position = value;
    }

    public void Initialize(int id, EnemyData data, GameObject instance, Vector3 position)
    {
        Id = id;
        Data = data;
        this.instance = instance;
        currentHealth = data.Health;
        Position = position;
    }

    public void TakeDamage(float amount, IEntity source)
    {
        currentHealth = Mathf.Max(0f, currentHealth - amount);
    }

    public void FaceDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0f)
        {
            instance.transform.forward = direction.normalized;
        }
    }

    public void OnSpawn()
    {
    }

    public void OnDespawn()
    {
        Id = 0;
        Data = null;
        instance = null;
        currentHealth = 0f;
    }
}
