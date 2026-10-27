using UnityEngine;


public class CastleAttackPoint : MonoBehaviour, ICombatEntity, IDamageable
{
    [Header("어떤 팀의 공격 포인트인지")]
    [SerializeField] private Team pointTeam;
    public Team team => pointTeam;

    [Header("실제 데미지를 받을 부모 성 연결")]
    [SerializeField] private MonoBehaviour ownerCastle; 

    public void TakeDamage(int damage)
    {
        if (ownerCastle is IDamageable damageable)
        {
            damageable.TakeDamage(damage);
        }
    }
}