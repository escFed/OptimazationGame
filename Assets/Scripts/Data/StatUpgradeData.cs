using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Stat Upgrade")]
public class StatUpgradeData : UpgradeData
{
    [SerializeField] private StatType stat;
    [SerializeField] private StatModifierMode mode;
    [SerializeField] private float value;

    public StatType Stat => stat;
    public StatModifierMode Mode => mode;
    public float Value => value;

    public override void Apply(UpgradeContext context)
    {
        context.Player.Stats.ApplyModifier(stat, mode, value);
    }
}
