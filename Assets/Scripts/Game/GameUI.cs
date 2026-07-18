using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameUIScreen screens;
    [SerializeField] private GameMainMenu mainMenu;
    [SerializeField] private GameUpgrade upgrades;
    [SerializeField] private GameHud hud;
    [SerializeField] private GameEndScreen endScreens;
    [SerializeField] private GamePause pause;

    private GameStateSystem gameStateSystem;
    private UpgradeSystem upgradeSystem;
    private Player player;
    private WaveSystem waveSystem;
    private EnemySystem enemySystem;
    private List<string> appliedUpgradeNames = new();

    private float previousTimeScale = 1f;
    private bool isUpgradeSelectionOpen;

    public void Initialize(GameStateSystem gameStateSystem, UpgradeSystem upgradeSystem, Player player, WaveSystem waveSystem, EnemySystem enemySystem)
    {
        this.gameStateSystem = gameStateSystem;
        this.upgradeSystem = upgradeSystem;
        this.player = player;
        this.waveSystem = waveSystem;
        this.enemySystem = enemySystem;

        gameStateSystem.StateChanged += ShowGameState;
        upgradeSystem.UpgradeSelectionStarted += ShowUpgrades;
        upgradeSystem.UpgradeApplied += OnUpgradeApplied;
        player.Health.Changed += UpdateHealth;
        waveSystem.WaveStarted += UpdateWave;
        waveSystem.WaveTimeChanged += UpdateWaveTime;
        enemySystem.EnemyKilled += UpdateKills;

        mainMenu.PlayButton.onClick.RemoveAllListeners();
        mainMenu.PlayButton.onClick.AddListener(gameStateSystem.StartGame);

        mainMenu.QuitButton.onClick.RemoveAllListeners();
        mainMenu.QuitButton.onClick.AddListener(QuitGame);

        endScreens.VictoryRestartButton.onClick.RemoveAllListeners();
        endScreens.VictoryRestartButton.onClick.AddListener(RestartScene);

        endScreens.DefeatRestartButton.onClick.RemoveAllListeners();
        endScreens.DefeatRestartButton.onClick.AddListener(RestartScene);

        endScreens.VictoryMainMenuButton.onClick.RemoveAllListeners();
        endScreens.VictoryMainMenuButton.onClick.AddListener(RestartScene);

        endScreens.DefeatMainMenuButton.onClick.RemoveAllListeners();
        endScreens.DefeatMainMenuButton.onClick.AddListener(RestartScene);

        pause.ResumeButton.onClick.RemoveAllListeners();
        pause.ResumeButton.onClick.AddListener(gameStateSystem.ResumeGame);

        pause.MainMenuButton.onClick.RemoveAllListeners();
        pause.MainMenuButton.onClick.AddListener(RestartScene);

        HideAll();
        UpdateHealth(player.Health.Current, player.Health.Max);
        UpdateKills(enemySystem.TotalEnemiesKilled);
        UpdateAppliedUpgrades();
        ShowGameState(gameStateSystem.CurrentState);
    }

    private void ShowGameState(GameState state)
    {
        screens.MainMenuPanel.SetActive(state == GameState.MainMenu);
        screens.HudPanel.SetActive(state == GameState.Playing || state == GameState.Paused);
        screens.PausePanel.SetActive(state == GameState.Paused);
        screens.VictoryPanel.SetActive(state == GameState.Victory);
        screens.DefeatPanel.SetActive(state == GameState.Defeat);

        if (state != GameState.Playing)
        {
            screens.UpgradePanel.SetActive(false);
        }
    }

    private void ShowUpgrades(IReadOnlyList<UpgradeData> choices)
    {
        if (isUpgradeSelectionOpen)
        {
            return;
        }

        isUpgradeSelectionOpen = true;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        screens.UpgradePanel.SetActive(true);
        screens.MainMenuPanel.SetActive(false);
        screens.PausePanel.SetActive(false);
        screens.VictoryPanel.SetActive(false);
        screens.DefeatPanel.SetActive(false);

        for (var i = 0; i < upgrades.Buttons.Length; i++)
        {
            var hasChoice = choices != null && i < choices.Count && choices[i] != null;

            upgrades.Buttons[i].gameObject.SetActive(hasChoice);

            if (!hasChoice)
            {
                continue;
            }

            var index = i;
            var upgrade = choices[i];

            upgrades.TitleTexts[i].text = upgrade.Title;
            upgrades.DescriptionTexts[i].text = upgrade.Description;

            upgrades.Buttons[i].onClick.RemoveAllListeners();
            upgrades.Buttons[i].onClick.AddListener(() => upgradeSystem.SelectUpgrade(index));
        }
    }

    private void OnUpgradeApplied(UpgradeData upgrade)
    {
        screens.UpgradePanel.SetActive(false);

        if (isUpgradeSelectionOpen)
        {
            isUpgradeSelectionOpen = false;
            Time.timeScale = previousTimeScale;
        }

        if (upgrade != null)
        {
            appliedUpgradeNames.Add(upgrade.Title);
            UpdateAppliedUpgrades();
        }
    }

    private void UpdateHealth(float current, float max)
    {
        hud.HealthSlider.maxValue = max;
        hud.HealthSlider.value = current;
        hud.HealthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }

    private void UpdateWave(WaveData wave)
    {
        hud.WaveText.text = $"Wave {wave.WaveNumber}";
    }

    private void UpdateWaveTime(float timeRemaining, float duration)
    {
        hud.WaveTimerText.text = $"{Mathf.CeilToInt(timeRemaining)}s";
    }

    private void UpdateKills(int kills)
    {
        hud.KillsText.text = $"Kills: {kills}";
    }

    private void UpdateAppliedUpgrades()
    {
        hud.UpgradesText.text = appliedUpgradeNames.Count == 0 ? "Upgrades: -" : "Upgrades: " + string.Join(", ", appliedUpgradeNames);
    }

    private void HideAll()
    {
        screens.MainMenuPanel.SetActive(false);
        screens.HudPanel.SetActive(false);
        screens.UpgradePanel.SetActive(false);
        screens.PausePanel.SetActive(false);
        screens.VictoryPanel.SetActive(false);
        screens.DefeatPanel.SetActive(false);
    }

    private void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitGame()
    {
        Application.Quit();
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
            upgradeSystem.UpgradeApplied -= OnUpgradeApplied;
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
