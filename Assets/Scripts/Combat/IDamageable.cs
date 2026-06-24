public interface IDamageable
{
    bool IsDead { get; }
    void TakeDamage(float amount, IEntity source);
}
