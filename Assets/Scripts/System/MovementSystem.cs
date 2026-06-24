using UnityEngine;

public class MovementSystem : IUpdateable
{
    private Player player;
    private PlayerInput playerInput;

    public MovementSystem(Player player, PlayerInput playerInput)
    {
        this.player = player;
        this.playerInput = playerInput;
    }

    public void Tick(float deltaTime)
    {
        if (!player.IsActive)
        {
            return;
        }

        var moveInput = playerInput.MoveInput;
        var input = new Vector3(moveInput.x, 0f, moveInput.y);

        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        player.Position += input * player.Stats.MoveSpeed * deltaTime;
    }

}
