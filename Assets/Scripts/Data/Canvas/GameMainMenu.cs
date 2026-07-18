using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GameMainMenu
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    public Button PlayButton => playButton;
    public Button QuitButton => quitButton;
}
