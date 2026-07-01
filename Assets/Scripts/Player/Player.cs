using UnityEngine;
using System;

public class Player : IEntity, IDamageable
{
    private TransformView view;

    public Player(int id, TransformView view, PlayerBaseStats baseStats)
    {
        Id = id;
        this.view = view;

        Stats = new PlayerStats(baseStats);
        Health = new Health(Stats.MaxHealth);
        Health.Depleted += () => Died?.Invoke();
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

    private void HandleDeath()
    {
        Died?.Invoke();
    }
}
