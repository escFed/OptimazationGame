using UnityEngine;

public class CameraFollowSystem : IUpdateable
{
    private Camera camera;
    private Player player;
    private CameraFollowData data;
    private Vector3 velocity;

    public CameraFollowSystem(Camera camera, Player player, CameraFollowData data)
    {
        this.camera = camera;
        this.player = player;
        this.data = data;
    }

    public void Tick(float deltaTime)
    {
        if (camera == null || !player.IsActive || data == null)
        {
            return;
        }

        var targetPosition = player.Position + data.Offset;

        camera.transform.position = data.UseSmoothing? Vector3.SmoothDamp(camera.transform.position, targetPosition, ref velocity, data.SmoothTime, Mathf.Infinity, deltaTime): targetPosition;

        if (data.LookAtTarget)
        {
            camera.transform.LookAt(player.Position + data.LookAtOffset);
        }
    }
}
