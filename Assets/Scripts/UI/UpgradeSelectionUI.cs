using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeSelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button[] buttons;
    [SerializeField] private TextMeshProUGUI[] titleTexts;
    [SerializeField] private TextMeshProUGUI[] descriptionTexts;

    private UpgradeSystem upgradeSystem;
    private float previousTimeScale = 1f;

    public void Initialize(UpgradeSystem upgradeSystem)
    {
        this.upgradeSystem = upgradeSystem;

        upgradeSystem.UpgradeSelectionStarted += Show;
        upgradeSystem.UpgradeApplied += HideUpgrade;

        Hide();
    }

    private void Show(IReadOnlyList<UpgradeData> upgrades)
    {
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        panel.SetActive(true);

        for (var i = 0; i < buttons.Length; i++)
        {
            var hasUpgrade = i < upgrades.Count;

            buttons[i].gameObject.SetActive(hasUpgrade);

            if (!hasUpgrade)
            {
                continue;
            }

            var index = i;
            var upgrade = upgrades[i];

            titleTexts[i].text = upgrade.Title;
            descriptionTexts[i].text = upgrade.Description;

            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => upgradeSystem.SelectUpgrade(index));
        }
    }

    private void HideUpgrade(UpgradeData upgrade)
    {
        Hide();
    }

    private void Hide()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        Time.timeScale = previousTimeScale;
    }
}
