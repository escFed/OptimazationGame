using System;
using UnityEngine;

public class Health
{
    public event Action<float, float> Changed;
    public event Action Depleted;

    public Health(float maxHealth)
    {
        Max = Mathf.Max(1f, maxHealth);
        Current = Max;
    }

    public float Current { get; private set; }
    public float Max { get; private set; }
    public bool IsDead => Current <= 0f;

    public void TakeDamage(float amount)
    {
        if (IsDead)
        {
            return;
        }

        Current = Mathf.Max(0f, Current - Mathf.Max(0f, amount));
        Changed?.Invoke(Current, Max);

        if (IsDead)
        {
            Depleted?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead)
        {
            return;
        }

        Current = Mathf.Min(Max, Current + Mathf.Max(0f, amount));
        Changed?.Invoke(Current, Max);
    }

    public void SetMaxHealth(float maxHealth, bool refill)
    {
        Max = Mathf.Max(1f, maxHealth);
        Current = refill ? Max : Mathf.Min(Current, Max);
        Changed?.Invoke(Current, Max);
    }

    public void Reset(float maxHealth)
    {
        Max = Mathf.Max(1f, maxHealth);
        Current = Max;
        Changed?.Invoke(Current, Max);
    }
}
