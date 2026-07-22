using System;
using UnityEngine;

public class Player : IEntity, IDamageable
{
    private readonly TransformView view;
    private readonly PlayerBaseStats baseStats;

    public Player(int id, TransformView view, PlayerBaseStats baseStats)
    {
        Id = id;
        this.view = view;
        this.baseStats = baseStats;

        Stats = new PlayerStats(baseStats);
        Health = new Health(Stats.MaxHealth);
        Health.Depleted += HandleDeath;
    }

    public event Action Died;

    public int Id { get; }
    public PlayerStats Stats { get; }
    public Health Health { get; }

    public bool IsDead => Health.IsDead;
    public bool IsActive => view != null && view.gameObject.activeInHierarchy;
    public Transform Transform => view.Transform;

    public Vector3 Position
    {
        get => view.Transform.position;
        set => view.Transform.position = value;
    }

    public void TakeDamage(float amount, IEntity source)
    {
        if (IsDead)
        {
            return;
        }

        var finalDamage = Mathf.Max(1f, amount - Stats.Armor);
        Health.TakeDamage(finalDamage);
    }

    public void ResetSession(Vector3 spawnPosition)
    {
        Stats.Reset(baseStats);
        Health.Reset(Stats.MaxHealth);

        if (view != null)
        {
            view.gameObject.SetActive(true);
            Position = spawnPosition;
        }
    }

    private void HandleDeath()
    {
        Died?.Invoke();
    }
}
