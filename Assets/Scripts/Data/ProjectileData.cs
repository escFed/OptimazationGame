using UnityEngine;

[CreateAssetMenu(menuName = "Projectiles/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float speed = 12f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private float spawnOffset = 0.8f;
    [SerializeField] private float collisionRadius = 0.25f;

    public GameObject Prefab => prefab;
    public float Speed => speed;
    public float Damage => damage;
    public float Lifetime => lifetime;
    public float SpawnOffset => spawnOffset;
    public float CollisionRadius => collisionRadius;
}
