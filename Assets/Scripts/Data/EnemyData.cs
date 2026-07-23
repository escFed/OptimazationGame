using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float health = 30f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float collisionRadius = 0.5f;
    [SerializeField] private int introducedAtWave = 1;
    [SerializeField] private string idleAnimationState = "Idle";
    [SerializeField] private string moveAnimationState = "Run";

    public GameObject Prefab => prefab;
    public float Health => health;
    public float Speed => speed;
    public float Damage => damage;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    public float CollisionRadius => collisionRadius;
    public int IntroducedAtWave => introducedAtWave;
    public string IdleAnimationState => idleAnimationState;
    public string MoveAnimationState => moveAnimationState;
}
