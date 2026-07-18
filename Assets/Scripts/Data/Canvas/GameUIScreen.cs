using System;
using UnityEngine;

[Serializable]
public class GameUIScreen
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    public GameObject MainMenuPanel => mainMenuPanel;
    public GameObject HudPanel => hudPanel;
    public GameObject UpgradePanel => upgradePanel;
    public GameObject PausePanel => pausePanel;
    public GameObject VictoryPanel => victoryPanel;
    public GameObject DefeatPanel => defeatPanel;
}