using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject hudPanel;

    [Header("Main Menu")]
    [SerializeField] private Button playButton;

    [Header("Upgrades")]
    [SerializeField] private Button[] upgradeButtons;
    [SerializeField] private TextMeshProUGUI[] upgradeTitleTexts;
    [SerializeField] private TextMeshProUGUI[] upgradeDescriptionTexts;

    [Header("HUD")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI upgradesText;
    [SerializeField] private TextMeshProUGUI waveTimerText;

    [Header("End Screens")]
    [SerializeField] private Button victoryRestartButton;
    [SerializeField] private Button defeatRestartButton;

    private GameStateSystem gameStateSystem;
    private UpgradeSystem upgradeSystem;
    private Player player;
    private WaveSystem waveSystem;
    private EnemySystem enemySystem;

    private readonly List<string> appliedUpgradeNames = new();
    private float previousTimeScale = 1f;

    public void Initialize(GameStateSystem gameStateSystem, UpgradeSystem upgradeSystem, Player player, WaveSystem waveSystem, EnemySystem enemySystem)
    {
        this.gameStateSystem = gameStateSystem;
        this.upgradeSystem = upgradeSystem;
        this.player = player;
        this.waveSystem = waveSystem;
        this.enemySystem = enemySystem;

        gameStateSystem.StateChanged += ShowGameState;

        upgradeSystem.UpgradeSelectionStarted += ShowUpgrades;
        upgradeSystem.UpgradeApplied += HideUpgrades;
        upgradeSystem.UpgradeApplied += AddUpgrade;

        player.Health.Changed += UpdateHealth;

        waveSystem.WaveStarted += UpdateWave;
        waveSystem.WaveTimeChanged += UpdateWaveTime;

        enemySystem.EnemyKilled += UpdateKills;

        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(gameStateSystem.StartGame);

        victoryRestartButton.onClick.RemoveAllListeners();
        victoryRestartButton.onClick.AddListener(RestartScene);

        defeatRestartButton.onClick.RemoveAllListeners();
        defeatRestartButton.onClick.AddListener(RestartScene);

        appliedUpgradeNames.Clear();

        UpdateHealth(player.Health.Current, player.Health.Max);
        UpdateKills(enemySystem.TotalEnemiesKilled);
        UpdateUpgradesText();

        HideAll();
        ShowGameState(gameStateSystem.CurrentState);
    }

    private void ShowGameState(GameState state)
    {
        mainMenuPanel.SetActive(state == GameState.MainMenu);
        victoryPanel.SetActive(state == GameState.Victory);
        defeatPanel.SetActive(state == GameState.Defeat);
        hudPanel.SetActive(state == GameState.Playing);

        if (state == GameState.MainMenu || state == GameState.Victory || state == GameState.Defeat)
        {
            upgradePanel.SetActive(false);
        }
    }

    private void ShowUpgrades(IReadOnlyList<UpgradeData> upgrades)
    {
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        mainMenuPanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        hudPanel.SetActive(false);
        upgradePanel.SetActive(true);

        for (var i = 0; i < upgradeButtons.Length; i++)
        {
            var hasUpgrade = i < upgrades.Count;

            upgradeButtons[i].gameObject.SetActive(hasUpgrade);

            if (!hasUpgrade)
            {
                continue;
            }

            var index = i;
            var upgrade = upgrades[i];

            upgradeTitleTexts[i].text = upgrade.Title;
            upgradeDescriptionTexts[i].text = upgrade.Description;

            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() => upgradeSystem.SelectUpgrade(index));
        }
    }

    private void HideUpgrades(UpgradeData upgrade)
    {
        upgradePanel.SetActive(false);
        hudPanel.SetActive(true);
        Time.timeScale = previousTimeScale;
    }

    private void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        }
    }

    private void UpdateWave(WaveData wave)
    {
        if (waveText != null)
        {
            waveText.text = $"Wave {wave.WaveNumber}";
        }
    }

    private void UpdateWaveTime(float remainingTime, float totalTime)
    {
        if (waveTimerText == null)
        {
            return;
        }

        var seconds = Mathf.CeilToInt(Mathf.Max(0f, remainingTime));
        waveTimerText.text = $"Tiempo: {seconds}s";
    }

    private void UpdateKills(int totalKills)
    {
        if (killsText != null)
        {
            killsText.text = $"Kills: {totalKills}";
        }
    }

    private void AddUpgrade(UpgradeData upgrade)
    {
        var upgradeName = string.IsNullOrWhiteSpace(upgrade.Title)? upgrade.name: upgrade.Title;

        appliedUpgradeNames.Add(upgradeName);
        UpdateUpgradesText();
    }

    private void UpdateUpgradesText()
    {
        if (upgradesText == null)
        {
            return;
        }

        upgradesText.text = appliedUpgradeNames.Count == 0? "Upgrades: Ninguna": $"Upgrades: {string.Join(", ", appliedUpgradeNames)}";
    }

    private void HideAll()
    {
        mainMenuPanel.SetActive(false);
        upgradePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        hudPanel.SetActive(false);
    }

    private void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        if (gameStateSystem != null)
        {
            gameStateSystem.StateChanged -= ShowGameState;
        }

        if (upgradeSystem != null)
        {
            upgradeSystem.UpgradeSelectionStarted -= ShowUpgrades;
            upgradeSystem.UpgradeApplied -= HideUpgrades;
            upgradeSystem.UpgradeApplied -= AddUpgrade;
        }

        if (player != null)
        {
            player.Health.Changed -= UpdateHealth;
        }

        if (waveSystem != null)
        {
            waveSystem.WaveStarted -= UpdateWave;
            waveSystem.WaveTimeChanged -= UpdateWaveTime;
        }

        if (enemySystem != null)
        {
            enemySystem.EnemyKilled -= UpdateKills;
        }
    }
}
