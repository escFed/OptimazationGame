using System;
using UnityEngine;

[Serializable]
public class UpgradeSetupData
{
    [SerializeField] private UpgradeData[] availableUpgrades;
    [SerializeField] private int choiceCount = 3;

    public UpgradeData[] AvailableUpgrades => availableUpgrades;
    public int ChoiceCount => choiceCount;
}
