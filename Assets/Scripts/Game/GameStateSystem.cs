using System;
using UnityEngine;

public class GameStateSystem
{
    private Player player;
    private WaveSystem waveSystem;

    public GameStateSystem(Player player, WaveSystem waveSystem)
    {
        this.player = player;
        this.waveSystem = waveSystem;

        this.player.Died += LoseGame;
        this.waveSystem.GameCompleted += WinGame;
    }

    public event Action<GameState> StateChanged;

    public GameState CurrentState { get; private set; } = GameState.MainMenu;
    public bool IsPlaying => CurrentState == GameState.Playing;

    public void ShowMainMenu()
    {
        SetState(GameState.MainMenu);
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SetState(GameState.Playing);
        waveSystem.Start();
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        Time.timeScale = 0f;
        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused)
        {
            return;
        }

        Time.timeScale = 1f;
        SetState(GameState.Playing);
    }

    public void TogglePause()
    {
        if (CurrentState == GameState.Playing)
        {
            PauseGame();
            return;
        }

        if (CurrentState == GameState.Paused)
        {
            ResumeGame();
        }
    }


    private void WinGame()
    {
        Time.timeScale = 0f;
        SetState(GameState.Victory);
    }

    private void LoseGame()
    {
        Time.timeScale = 0f;
        SetState(GameState.Defeat);
    }

    private void SetState(GameState state)
    {
        if (CurrentState == state)
        {
            return;
        }

        CurrentState = state;
        StateChanged?.Invoke(CurrentState);
    }
}
