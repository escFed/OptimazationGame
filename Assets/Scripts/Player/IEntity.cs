using UnityEngine;

public interface IEntity
{
    int Id { get; }
    Vector3 Position { get; set; }
    bool IsActive { get; }
}
