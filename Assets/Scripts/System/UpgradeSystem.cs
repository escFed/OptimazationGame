using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem
{
    private UpgradeData[] availableUpgrades;
    private int choiceCount;
    private UpgradeContext context;
    private WaveSystem waveSystem;
    private List<UpgradeData> currentChoices = new();
    private bool isSelectionOpen;

    public UpgradeSystem(UpgradeData[] availableUpgrades, int choiceCount, UpgradeContext context, WaveSystem waveSystem)
    {
        this.availableUpgrades = availableUpgrades;
        this.choiceCount = Mathf.Max(1, choiceCount);
        this.context = context;
        this.waveSystem = waveSystem;
        this.waveSystem.WaveCompleted += BeginSelection;
    }

    public event Action<IReadOnlyList<UpgradeData>> UpgradeSelectionStarted;
    public event Action<UpgradeData> UpgradeApplied;

    public IReadOnlyList<UpgradeData> CurrentChoices => currentChoices;
    public bool IsSelectionOpen => isSelectionOpen;

    public void SelectUpgrade(int choiceIndex)
    {
        if (!isSelectionOpen || choiceIndex < 0 || choiceIndex >= currentChoices.Count)
        {
            return;
        }

        var upgrade = currentChoices[choiceIndex];

        upgrade.Apply(context);
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

        if (availableUpgrades == null || availableUpgrades.Length == 0)
        {
            return;
        }

        var attempts = 0;
        var maxAttempts = availableUpgrades.Length * 4;

        while (currentChoices.Count < choiceCount && attempts < maxAttempts)
        {
            attempts++;

            var upgrade = availableUpgrades[UnityEngine.Random.Range(0, availableUpgrades.Length)];

            if (upgrade != null && !currentChoices.Contains(upgrade))
            {
                currentChoices.Add(upgrade);
            }
        }
    }
}
