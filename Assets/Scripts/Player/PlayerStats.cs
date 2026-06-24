using UnityEngine;

public class PlayerStats
{
    public PlayerStats (PlayerBaseStats baseStats)
    {
        MoveSpeed = baseStats.MoveSpeed;
        AttackSpeed = baseStats.AttackSpeed;
        Damage = baseStats.Damage;
        MaxHealth = baseStats.MaxHealth;
        Armor = baseStats.Armor;
    }

    public float MoveSpeed { get; private set; }
    public float AttackSpeed { get; private set; }
    public float Damage { get; private set; }
    public float MaxHealth { get; private set; }
    public float Armor { get; private set; }

    public void ApplyModifier(StatType stat, StatModifierMode mode, float value)
    {
        var currentValue = GetValue(stat);
        var modifiedValue = mode == StatModifierMode.Percent? currentValue * (1f + value): currentValue + value;

        SetValue(stat, Mathf.Max(0f, modifiedValue));
    }

    private float GetValue(StatType stat)
    {
        return stat switch
        {
            StatType.MoveSpeed => MoveSpeed,
            StatType.AttackSpeed => AttackSpeed,
            StatType.Damage => Damage,
            StatType.MaxHealth => MaxHealth,
            StatType.Armor => Armor,
            _=> 0f
        };
    }

    private void SetValue(StatType stat, float value)
    {
        switch (stat)
        {
            case StatType.MoveSpeed:
                MoveSpeed = value;
                break;
            case StatType.AttackSpeed:
                AttackSpeed = Mathf.Max(0.01f, value);
                break;
            case StatType.Damage:
                Damage = value;
                break;
            case StatType.MaxHealth:
                MaxHealth = Mathf.Max(1f, value);
                break;
            case StatType.Armor:
                Armor = value;
                break;
        }
    }
}
