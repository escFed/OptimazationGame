using UnityEngine;

public class Projectile : IEntity, IPoolable
{
    private GameObject instance;
    private float lifetime;

    public int Id { get; private set; }
    public ProjectileData Data { get; private set; }
    public Vector3 Direction { get; set; }
    public int OwnerId { get; private set; }
    public bool IsActive => instance != null && instance.activeInHierarchy;
    public GameObject Instance => instance;

    public Vector3 Position
    {
        get => instance.transform.position;
        set => instance.transform.position = value;
    }

    public void Initialize(int id, ProjectileData data, GameObject instance, Vector3 position, Vector3 direction, int ownerId)
    {
        Id = id;
        Data = data;
        this.instance = instance;
        Direction = direction.normalized;
        OwnerId = ownerId;
        lifetime = 0f;
        Position = position;
        instance.transform.forward = Direction;
    }

    public bool TickLifetime(float deltaTime)
    {
        lifetime += deltaTime;
        return lifetime >= Data.Lifetime;
    }

    public void OnSpawn()
    {
    }

    public void OnDespawn()
    {
        Id = 0;
        Data = null;
        Direction = Vector3.zero;
        OwnerId = 0;
        instance = null;
        lifetime = 0f;
    }
}
