using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GameEndScreen
{
    [SerializeField] private Button victoryRestartButton;
    [SerializeField] private Button victoryMainMenuButton;
    [SerializeField] private Button defeatRestartButton;
    [SerializeField] private Button defeatMainMenuButton;

    public Button VictoryRestartButton => victoryRestartButton;
    public Button VictoryMainMenuButton => victoryMainMenuButton;
    public Button DefeatRestartButton => defeatRestartButton;
    public Button DefeatMainMenuButton => defeatMainMenuButton;
}
