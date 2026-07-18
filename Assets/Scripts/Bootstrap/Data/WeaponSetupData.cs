using System;
using UnityEngine;

[Serializable]
public class WeaponSetupData
{
    [SerializeField] private ProjectileData projectileData;
    [SerializeField] private PatternData initialPattern;

    public ProjectileData ProjectileData => projectileData;
    public PatternData InitialPattern => initialPattern;
}
