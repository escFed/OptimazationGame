using UnityEngine;

public class Player : IEntity
{
    private TransformView view;

    public Player(int id, TransformView view, PlayerBaseStats baseStats)
    {
        Id = id;
        this.view = view;
        Stats = new PlayerStats(baseStats);
    }

    public int Id { get; }
    public PlayerStats Stats { get; }
    public bool IsActive => view != null && view.gameObject.activeInHierarchy;
    public Transform Transform => view.Transform;

    public Vector3 Position
    {
        get => view.Transform.position;
        set => view.Transform.position = value;
    }
}
