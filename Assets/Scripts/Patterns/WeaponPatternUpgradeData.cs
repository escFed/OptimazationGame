using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Weapon Pattern Upgrade")]
public class WeaponPatternUpgradeData : UpgradeData
{
    [SerializeField] private PatternData pattern;

    public PatternData Pattern => pattern;

    public override void Apply(UpgradeContext context)
    {
        context.WeaponSystem.SetPattern(pattern);
    }
}