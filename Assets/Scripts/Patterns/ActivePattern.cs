using UnityEngine;

public class ActivePattern
{
    public ActivePattern(PatternData pattern)
    {
        Pattern = pattern;
        CooldownTimer = 0f;
    }

    public PatternData Pattern { get; }
    public float CooldownTimer { get; private set; }

    public bool CanFire => CooldownTimer <= 0f;

    public void Tick(float deltaTime)
    {
        if (CooldownTimer > 0f)
        {
            CooldownTimer -= deltaTime;
        }
    }

    public void ResetCooldown(float attackSpeed)
    {
        attackSpeed = Mathf.Max(0.01f, attackSpeed);
        CooldownTimer = Pattern.BaseAttackInterval / attackSpeed;
    }

    public void Reset()
    {
        CooldownTimer = 0f;
    }
}