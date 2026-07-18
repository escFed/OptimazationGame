public class PauseSystem : IUpdateable
{
    private PlayerInput input;
    private GameStateSystem gameStateSystem;

    public PauseSystem(PlayerInput input, GameStateSystem gameStateSystem)
    {
        this.input = input;
        this.gameStateSystem = gameStateSystem;
    }

    public void Tick(float deltaTime)
    { 
        if(input.PausePressed)
        {
            gameStateSystem.TogglePause();
        }
    }
}
