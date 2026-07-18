using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Bootstrap Data")]
public class GameBootstrapData : ScriptableObject
{
    [SerializeField] private PlayerSetupData player;
    [SerializeField] private WeaponSetupData weapon;
    [SerializeField] private WaveSetupData waves;
    [SerializeField] private UpgradeSetupData upgrades;
    [SerializeField] private CameraFollowData cameraFollowData;

    public PlayerSetupData Player => player;
    public WeaponSetupData Weapon => weapon;
    public WaveSetupData Waves => waves;
    public UpgradeSetupData Upgrades => upgrades;
    public CameraFollowData CameraFollowData => cameraFollowData;
}
