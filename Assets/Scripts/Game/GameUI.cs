using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private Action restartGameRequested;
    private Action returnToMainMenuRequested;

    private readonly List<string> appliedUpgradeNames = new();

    private Canvas hudCanvas;
    private Canvas upgradeCanvas;
    private GraphicRaycaster upgradeRaycaster;

    private float previousTimeScale = 1f;
    private bool isUpgradeSelectionOpen;
    private int lastDisplayedWaveSecond = int.MinValue;

    public void Initialize(
        GameStateSystem gameStateSystem,
        UpgradeSystem upgradeSystem,
        Player player,
        WaveSystem waveSystem,
        EnemySystem enemySystem,
        Action restartGameRequested,
        Action returnToMainMenuRequested
    )
    {
        this.gameStateSystem = gameStateSystem;
        this.upgradeSystem = upgradeSystem;
        this.player = player;
        this.waveSystem = waveSystem;
        this.enemySystem = enemySystem;
        this.restartGameRequested = restartGameRequested;
        this.returnToMainMenuRequested = returnToMainMenuRequested;

        gameStateSystem.StateChanged += ShowGameState;
        upgradeSystem.UpgradeSelectionStarted += ShowUpgrades;
        upgradeSystem.UpgradeApplied += OnUpgradeApplied;
        player.Health.Changed += UpdateHealth;
        waveSystem.WaveStarted += UpdateWave;
        waveSystem.WaveTimeChanged += UpdateWaveTime;
        enemySystem.EnemyKilled += UpdateKills;

        BindMainMenuButtons();
        BindEndScreenButtons();
        BindPauseButtons();
        BindUpgradeButtons();

        /*
         * Se establecen los textos antes del precalentamiento para
         * que TextMeshPro genere los meshes de los valores reales
         * usados al comenzar la partida.
         */
        UpdateHealth(
            player.Health.Current,
            player.Health.Max
        );

        UpdateKills(
            enemySystem.TotalEnemiesKilled
        );

        UpdateAppliedUpgrades();

        hud.WaveText.text = "Wave -";
        UpdateWaveTime(0f, 0f);

        PrepareHudPanel();
        PrepareUpgradePanel();

        HideAll();

        ShowGameState(
            gameStateSystem.CurrentState
        );
    }

    public void ResetSession()
    {
        isUpgradeSelectionOpen = false;
        previousTimeScale = 1f;
        lastDisplayedWaveSecond = int.MinValue;

        appliedUpgradeNames.Clear();

        SetUpgradePanelVisible(false);

        UpdateHealth(
            player.Health.Current,
            player.Health.Max
        );

        UpdateKills(
            enemySystem.TotalEnemiesKilled
        );

        UpdateAppliedUpgrades();

        hud.WaveText.text = "Wave -";
        UpdateWaveTime(0f, 0f);
    }

    private void BindMainMenuButtons()
    {
        mainMenu.PlayButton.onClick.RemoveAllListeners();
        mainMenu.PlayButton.onClick.AddListener(
            gameStateSystem.StartGame
        );

        mainMenu.QuitButton.onClick.RemoveAllListeners();
        mainMenu.QuitButton.onClick.AddListener(QuitGame);
    }

    private void BindEndScreenButtons()
    {
        endScreens.VictoryRestartButton.onClick.RemoveAllListeners();
        endScreens.VictoryRestartButton.onClick.AddListener(
            RequestRestartGame
        );

        endScreens.DefeatRestartButton.onClick.RemoveAllListeners();
        endScreens.DefeatRestartButton.onClick.AddListener(
            RequestRestartGame
        );

        endScreens.VictoryMainMenuButton.onClick.RemoveAllListeners();
        endScreens.VictoryMainMenuButton.onClick.AddListener(
            RequestMainMenu
        );

        endScreens.DefeatMainMenuButton.onClick.RemoveAllListeners();
        endScreens.DefeatMainMenuButton.onClick.AddListener(
            RequestMainMenu
        );
    }

    private void BindPauseButtons()
    {
        pause.ResumeButton.onClick.RemoveAllListeners();
        pause.ResumeButton.onClick.AddListener(
            gameStateSystem.ResumeGame
        );

        pause.MainMenuButton.onClick.RemoveAllListeners();
        pause.MainMenuButton.onClick.AddListener(
            RequestMainMenu
        );
    }

    private void BindUpgradeButtons()
    {
        for (var i = 0; i < upgrades.Buttons.Length; i++)
        {
            var button = upgrades.Buttons[i];

            if (button == null)
            {
                continue;
            }

            var index = i;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(
                () => upgradeSystem.SelectUpgrade(index)
            );
        }
    }

    private void PrepareHudPanel()
    {
        var panel = screens.HudPanel;

        if (panel == null)
        {
            Debug.LogError(
                "GameUI: no se asignó el HudPanel."
            );

            return;
        }

        /*
         * El GameObject queda activo durante toda la aplicación.
         * Su Canvas se desactiva cuando el HUD debe estar oculto.
         */
        panel.SetActive(true);

        hudCanvas = panel.GetComponent<Canvas>();

        if (hudCanvas == null)
        {
            Debug.LogError(
                "GameUI: agregá un componente Canvas al HudPanel."
            );

            return;
        }

        hudCanvas.enabled = true;

        ForceHudMeshUpdate();
        Canvas.ForceUpdateCanvases();

        SetHudPanelVisible(false);
    }

    private void ForceHudMeshUpdate()
    {
        if (hud.HealthText != null)
        {
            hud.HealthText.ForceMeshUpdate(
                true,
                true
            );
        }

        if (hud.WaveText != null)
        {
            hud.WaveText.ForceMeshUpdate(
                true,
                true
            );
        }

        if (hud.WaveTimerText != null)
        {
            hud.WaveTimerText.ForceMeshUpdate(
                true,
                true
            );
        }

        if (hud.KillsText != null)
        {
            hud.KillsText.ForceMeshUpdate(
                true,
                true
            );
        }

        if (hud.UpgradesText != null)
        {
            hud.UpgradesText.ForceMeshUpdate(
                true,
                true
            );
        }
    }

    private void SetHudPanelVisible(bool visible)
    {
        var panel = screens.HudPanel;

        if (panel == null)
        {
            return;
        }

        if (!panel.activeSelf)
        {
            panel.SetActive(true);
        }

        if (hudCanvas != null)
        {
            hudCanvas.enabled = visible;
            return;
        }

        /*
         * Respaldo temporal. Si se ejecuta este bloque, falta
         * agregar el Canvas propio al HudPanel.
         */
        panel.SetActive(visible);
    }

    private void PrepareUpgradePanel()
    {
        var panel = screens.UpgradePanel;

        if (panel == null)
        {
            Debug.LogError(
                "GameUI: no se asignó el UpgradePanel."
            );

            return;
        }

        panel.SetActive(true);

        upgradeCanvas = panel.GetComponent<Canvas>();
        upgradeRaycaster =
            panel.GetComponent<GraphicRaycaster>();

        if (upgradeCanvas == null)
        {
            Debug.LogError(
                "GameUI: agregá un componente Canvas al UpgradePanel."
            );

            return;
        }

        upgradeCanvas.enabled = true;

        if (upgradeRaycaster != null)
        {
            upgradeRaycaster.enabled = false;
        }

        for (var i = 0; i < upgrades.Buttons.Length; i++)
        {
            var button = upgrades.Buttons[i];

            if (button == null)
            {
                continue;
            }

            if (!button.gameObject.activeSelf)
            {
                button.gameObject.SetActive(true);
            }
        }

        for (var i = 0; i < upgrades.TitleTexts.Length; i++)
        {
            var title = upgrades.TitleTexts[i];

            if (title == null)
            {
                continue;
            }

            title.ForceMeshUpdate(
                true,
                true
            );
        }

        for (var i = 0; i < upgrades.DescriptionTexts.Length; i++)
        {
            var description =
                upgrades.DescriptionTexts[i];

            if (description == null)
            {
                continue;
            }

            description.ForceMeshUpdate(
                true,
                true
            );
        }

        Canvas.ForceUpdateCanvases();

        SetUpgradePanelVisible(false);
    }

    private void SetUpgradePanelVisible(bool visible)
    {
        var panel = screens.UpgradePanel;

        if (panel == null)
        {
            return;
        }

        if (!panel.activeSelf)
        {
            panel.SetActive(true);
        }

        if (upgradeRaycaster != null)
        {
            upgradeRaycaster.enabled = visible;
        }

        if (upgradeCanvas != null)
        {
            upgradeCanvas.enabled = visible;
            return;
        }

        /*
         * Respaldo temporal. Si se ejecuta este bloque, falta
         * agregar el Canvas propio al UpgradePanel.
         */
        panel.SetActive(visible);
    }

    private void ShowGameState(GameState state)
    {
        screens.MainMenuPanel.SetActive(
            state == GameState.MainMenu
        );

        SetHudPanelVisible(
            state == GameState.Playing ||
            state == GameState.Paused
        );

        screens.PausePanel.SetActive(
            state == GameState.Paused
        );

        screens.VictoryPanel.SetActive(
            state == GameState.Victory
        );

        screens.DefeatPanel.SetActive(
            state == GameState.Defeat
        );

        if (state != GameState.Playing)
        {
            SetUpgradePanelVisible(false);
        }
    }

    private void ShowUpgrades(
        IReadOnlyList<UpgradeData> choices
    )
    {
        if (isUpgradeSelectionOpen)
        {
            return;
        }

        isUpgradeSelectionOpen = true;

        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        screens.MainMenuPanel.SetActive(false);
        screens.PausePanel.SetActive(false);
        screens.VictoryPanel.SetActive(false);
        screens.DefeatPanel.SetActive(false);

        /*
         * Se actualizan los textos antes de volver visible
         * el Canvas del panel.
         */
        for (var i = 0; i < upgrades.Buttons.Length; i++)
        {
            var button = upgrades.Buttons[i];

            if (button == null)
            {
                continue;
            }

            var hasChoice =
                choices != null &&
                i < choices.Count &&
                choices[i] != null;

            if (button.gameObject.activeSelf != hasChoice)
            {
                button.gameObject.SetActive(hasChoice);
            }

            if (!hasChoice)
            {
                continue;
            }

            var upgrade = choices[i];

            if (
                i < upgrades.TitleTexts.Length &&
                upgrades.TitleTexts[i] != null &&
                upgrades.TitleTexts[i].text != upgrade.Title
            )
            {
                upgrades.TitleTexts[i].text =
                    upgrade.Title;
            }

            if (
                i < upgrades.DescriptionTexts.Length &&
                upgrades.DescriptionTexts[i] != null &&
                upgrades.DescriptionTexts[i].text !=
                upgrade.Description
            )
            {
                upgrades.DescriptionTexts[i].text =
                    upgrade.Description;
            }
        }

        SetUpgradePanelVisible(true);
    }

    private void OnUpgradeApplied(
        UpgradeData upgrade
    )
    {
        SetUpgradePanelVisible(false);

        if (isUpgradeSelectionOpen)
        {
            isUpgradeSelectionOpen = false;
            Time.timeScale = previousTimeScale;
        }

        if (upgrade == null)
        {
            return;
        }

        appliedUpgradeNames.Add(
            upgrade.Title
        );

        UpdateAppliedUpgrades();
    }

    private void UpdateHealth(
        float current,
        float max
    )
    {
        hud.HealthSlider.maxValue = max;
        hud.HealthSlider.value = current;

        hud.HealthText.SetText(
            "{0:0} / {1:0}",
            current,
            max
        );
    }

    private void UpdateWave(
        WaveData wave
    )
    {
        if (wave == null)
        {
            hud.WaveText.text = "Wave -";
            return;
        }

        hud.WaveText.SetText(
            "Wave {0}",
            wave.WaveNumber
        );

        lastDisplayedWaveSecond =
            int.MinValue;
    }

    private void UpdateWaveTime(
        float timeRemaining,
        float duration
    )
    {
        var displayedSecond =
            Mathf.CeilToInt(timeRemaining);

        if (
            displayedSecond ==
            lastDisplayedWaveSecond
        )
        {
            return;
        }

        lastDisplayedWaveSecond =
            displayedSecond;

        hud.WaveTimerText.SetText(
            "{0}s",
            displayedSecond
        );
    }

    private void UpdateKills(int kills)
    {
        hud.KillsText.SetText(
            "Kills: {0}",
            kills
        );
    }

    private void UpdateAppliedUpgrades()
    {
        hud.UpgradesText.text =
            appliedUpgradeNames.Count == 0
                ? "Upgrades: -"
                : "Upgrades: " + string.Join(
                    ", ",
                    appliedUpgradeNames
                );
    }

    private void HideAll()
    {
        screens.MainMenuPanel.SetActive(false);

        SetHudPanelVisible(false);
        SetUpgradePanelVisible(false);

        screens.PausePanel.SetActive(false);
        screens.VictoryPanel.SetActive(false);
        screens.DefeatPanel.SetActive(false);
    }

    private void RequestRestartGame()
    {
        restartGameRequested?.Invoke();
    }

    private void RequestMainMenu()
    {
        returnToMainMenuRequested?.Invoke();
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        if (gameStateSystem != null)
        {
            gameStateSystem.StateChanged -=
                ShowGameState;
        }

        if (upgradeSystem != null)
        {
            upgradeSystem.UpgradeSelectionStarted -=
                ShowUpgrades;

            upgradeSystem.UpgradeApplied -=
                OnUpgradeApplied;
        }

        if (player != null)
        {
            player.Health.Changed -=
                UpdateHealth;
        }

        if (waveSystem != null)
        {
            waveSystem.WaveStarted -=
                UpdateWave;

            waveSystem.WaveTimeChanged -=
                UpdateWaveTime;
        }

        if (enemySystem != null)
        {
            enemySystem.EnemyKilled -=
                UpdateKills;
        }
    }
}