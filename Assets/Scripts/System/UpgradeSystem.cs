using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem
{
    private readonly List<UpgradeData> initialUpgrades = new();
    private readonly List<UpgradeData> availableUpgrades = new();
    private readonly List<UpgradeData> selectedUpgrades = new();
    private readonly List<UpgradeData> currentChoices = new();
    private readonly List<UpgradeData> choicePool = new();

    private readonly int choiceCount;
    private readonly UpgradeContext context;
    private readonly WaveSystem waveSystem;

    private bool isSelectionOpen;

    public UpgradeSystem(
        UpgradeData[] availableUpgrades,
        int choiceCount,
        UpgradeContext context,
        WaveSystem waveSystem
    )
    {
        if (availableUpgrades != null)
        {
            for (var i = 0; i < availableUpgrades.Length; i++)
            {
                var upgrade = availableUpgrades[i];

                if (
                    upgrade == null ||
                    initialUpgrades.Contains(upgrade)
                )
                {
                    continue;
                }

                initialUpgrades.Add(upgrade);
                this.availableUpgrades.Add(upgrade);
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
        if (
            !isSelectionOpen ||
            choiceIndex < 0 ||
            choiceIndex >= currentChoices.Count
        )
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

    public void ResetSession()
    {
        isSelectionOpen = false;

        selectedUpgrades.Clear();
        currentChoices.Clear();
        choicePool.Clear();

        availableUpgrades.Clear();
        availableUpgrades.AddRange(initialUpgrades);
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
        choicePool.Clear();

        if (availableUpgrades.Count == 0)
        {
            return;
        }

        choicePool.AddRange(availableUpgrades);

        var amount = Mathf.Min(choiceCount, choicePool.Count);

        for (var i = 0; i < amount; i++)
        {
            var randomIndex = UnityEngine.Random.Range(
                0,
                choicePool.Count
            );

            var upgrade = choicePool[randomIndex];

            currentChoices.Add(upgrade);
            choicePool.RemoveAt(randomIndex);
        }
    }
}