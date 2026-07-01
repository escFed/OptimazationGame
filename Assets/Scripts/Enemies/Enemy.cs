using UnityEngine;

public class Enemy : IEntity, IDamageable, IPoolable
{
    private GameObject instance;
    private float attackCooldownTimer;

    public int Id { get; private set; }
    public EnemyData Data { get; private set; }
    public Health Health { get; private set; }

    public bool IsDead => Health == null || Health.IsDead;
    public bool IsActive => instance != null && instance.activeInHierarchy;
    public bool CanAttack => attackCooldownTimer <= 0f;
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

        if (Health == null)
        {
            Health = new Health(data.Health);
        }
        else
        {
            Health.Reset(data.Health);
        }

        attackCooldownTimer = 0f;
        Position = position;
    }

    public void TakeDamage(float amount, IEntity source)
    {
        Health.TakeDamage(amount);
    }

    public void FaceDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0f)
        {
            instance.transform.forward = direction.normalized;
        }
    }

    public void TickAttackCooldown(float deltaTime)
    {
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= deltaTime;
        }
    }

    public void ResetAttackCooldown()
    {
        attackCooldownTimer = Data.AttackCooldown;
    }

    public void OnSpawn()
    {
    }

    public void OnDespawn()
    {
        Id = 0;
        Data = null;
        instance = null;
        attackCooldownTimer = 0f;
    }
}
