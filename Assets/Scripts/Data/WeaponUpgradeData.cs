using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Weapon Upgrade")]
public class WeaponUpgradeData : ScriptableObject
{
    [SerializeField] private PatternData pattern;

    public PatternData Pattern => pattern;

    public void Apply(UpgradeContext context)
    {
        context.WeaponSystem.SetPattern(pattern);
    }
}
