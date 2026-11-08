public interface IDamageable
{
    void TakeDamage(int damage, ICombatEntity attacker = null);
}