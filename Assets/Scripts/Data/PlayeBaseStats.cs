using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Player/Stats")]
public class PlayerBaseStats : ScriptableObject
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float armor = 0f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackSpeed = 1f;

    public float MoveSpeed => moveSpeed;
    public float MaxHealth => maxHealth;
    public float Armor => armor;
    public float Damage => damage;
    public float AttackSpeed => attackSpeed;
}
