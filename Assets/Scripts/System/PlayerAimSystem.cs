using UnityEngine;

public class PlayerAimSystem : IUpdateable
{
    private Player player;
    private PlayerInput input;
    private Camera camera;

    public PlayerAimSystem(Player player, PlayerInput input, Camera camera)
    {
        this.player = player;
        this.input = input;
        this.camera = camera;
    }

    public void Tick(float deltaTime)
    {
        if (camera == null || !player.IsActive)
        {
            return;
        }

        var ray = camera.ScreenPointToRay(input.PointerScreenPosition);
        var groundPlane = new Plane(Vector3.up, player.Position);

        if (!groundPlane.Raycast(ray, out var enter))
        {
            return;
        }

        var targetPosition = ray.GetPoint(enter);
        var direction = targetPosition - player.Position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        player.Transform.forward = direction.normalized;
    }
}
