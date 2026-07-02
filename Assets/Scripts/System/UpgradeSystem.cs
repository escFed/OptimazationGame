using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem
{
    private List<UpgradeData> availableUpgrades = new();
    private List<UpgradeData> selectedUpgrades = new();

    private int choiceCount;
    private UpgradeContext context;
    private WaveSystem waveSystem;

    private List<UpgradeData> currentChoices = new();
    private bool isSelectionOpen;

    public UpgradeSystem(UpgradeData[] availableUpgrades, int choiceCount, UpgradeContext context, WaveSystem waveSystem)
    {
        if (availableUpgrades != null)
        {
            for (var i = 0; i < availableUpgrades.Length; i++)
            {
                if (availableUpgrades[i] != null && !this.availableUpgrades.Contains(availableUpgrades[i]))
                {
                    this.availableUpgrades.Add(availableUpgrades[i]);
                }
            }
        }

        this.choiceCount = Mathf.Max(1, choiceCount);
        this.context = context;
        this.waveSystem = waveSystem;

        this.waveSystem.WaveCompleted += BeginSelection;
    }

    public event Action<IReadOnlyList<UpgradeData>> UpgradeSelectionStarted;
    public event Action<UpgradeData> UpgradeApplied;

    public IReadOnlyList<UpgradeData> CurrentChoices => currentChoices;
    public IReadOnlyList<UpgradeData> SelectedUpgrades => selectedUpgrades;
    public bool IsSelectionOpen => isSelectionOpen;

    public void SelectUpgrade(int choiceIndex)
    {
        if (!isSelectionOpen || choiceIndex < 0 || choiceIndex >= currentChoices.Count)
        {
            return;
        }

        var upgrade = currentChoices[choiceIndex];

        upgrade.Apply(context);
        selectedUpgrades.Add(upgrade);

        if (!upgrade.CanRepeat)
        {
            availableUpgrades.Remove(upgrade);
        }

        isSelectionOpen = false;
        currentChoices.Clear();

        UpgradeApplied?.Invoke(upgrade);
        waveSystem.ContinueToNextWave();
    }

    private void BeginSelection(WaveData completedWave)
    {
        if (completedWave.IsFinalWave)
        {
            return;
        }

        BuildChoices();

        if (currentChoices.Count == 0)
        {
            waveSystem.ContinueToNextWave();
            return;
        }

        isSelectionOpen = true;
        UpgradeSelectionStarted?.Invoke(currentChoices);
    }

    private void BuildChoices()
    {
        currentChoices.Clear();

        if (availableUpgrades.Count == 0)
        {
            return;
        }

        var tempPool = new List<UpgradeData>(availableUpgrades);
        var amount = Mathf.Min(choiceCount, tempPool.Count);

        for (var i = 0; i < amount; i++)
        {
            var randomIndex = UnityEngine.Random.Range(0, tempPool.Count);
            var upgrade = tempPool[randomIndex];

            currentChoices.Add(upgrade);
            tempPool.RemoveAt(randomIndex);
        }
    }
}
