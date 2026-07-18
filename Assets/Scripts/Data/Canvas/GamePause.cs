using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GamePause
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    public Button ResumeButton => resumeButton;
    public Button MainMenuButton => mainMenuButton;
}
