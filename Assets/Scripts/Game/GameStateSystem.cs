using System;
using UnityEngine;

public sealed class GameStateSystem
{
    private readonly Player player;
    private readonly WaveSystem waveSystem;

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
