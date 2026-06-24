public struct UpgradeContext
{ 
    public UpgradeContext(Player player, WeaponSystem weaponSystem)
    {
        Player = player;
        WeaponSystem = weaponSystem;
    }

    public Player Player { get; }
    public WeaponSystem WeaponSystem { get; }
}

