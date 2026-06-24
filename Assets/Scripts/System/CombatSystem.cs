using UnityEngine;

public class CombatSystem
{
    public void ApplyDamage(IEntity source, IDamageable target, float amount)
    {
        if (target.IsDead) return;

        target.TakeDamage(Mathf.Max(0f, amount), source);
    }
}
