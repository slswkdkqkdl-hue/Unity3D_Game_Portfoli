using UnityEngine;

public class HeroAttackState : IState
{
    private HeroBase hero;
    private ICombatEntity targetUnit; 
    private float attackCooldown = 0f;

    public HeroAttackState(HeroBase hero)
    {
        this.hero = hero;
    }

    public void SetTarget(ICombatEntity target)
    {
        this.targetUnit = target;
    }

    public void Enter()
    {
        attackCooldown = 0f; // 진입하자마자 첫 타 공격 가능하도록 세팅
    }

    public void Update()
    {
        // 1. 타겟이 죽거나 사라졌다면 다시 Idle(스캔 대기) 상태로 복귀
        if (targetUnit == null || (targetUnit is MonoBehaviour mono && !mono.gameObject.activeInHierarchy))
        {
            hero.ChangeState(hero.IdleState);
            return;
        }

        Transform targetTransform = targetUnit.transform;
        Vector3 targetPos = targetTransform.position;
        targetPos.y = hero.transform.position.y;

        float sqrDist = (targetPos - hero.transform.position).sqrMagnitude;
        float sqrAttackRange = hero.attackRange * hero.attackRange;

        // 2. 사거리(attackRange)보다 멀리 있다면 적을 향해 이동 (추격)
        if (sqrDist > sqrAttackRange)
        {
            // 이동 중이므로 Run 애니메이션 (Speed = 1)
            if (hero.Animator != null) 
            {
                hero.Animator.SetFloat("Speed", 1f);
            }

            hero.transform.position = Vector3.MoveTowards(
                hero.transform.position, 
                targetPos, 
                hero.CurrentMoveSpeed * Time.deltaTime
            );

            // 적을 바라보며 이동
            Vector3 dir = (targetPos - hero.transform.position).normalized;
            if (dir != Vector3.zero)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir);
                hero.transform.rotation = Quaternion.Slerp(hero.transform.rotation, lookRot, Time.deltaTime * 15f);
            }
        }
        // 3. 사거리 안에 들어왔다면 멈춰서 공격!
        else
        {
            if (hero.Animator != null) 
            {
                hero.Animator.SetFloat("Speed", 0f);
            }

            // 적을 바라보기
            Vector3 dir = (targetPos - hero.transform.position).normalized;
            if (dir != Vector3.zero)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir);
                hero.transform.rotation = Quaternion.Slerp(hero.transform.rotation, lookRot, Time.deltaTime * 15f);
            }

            // 공격 쿨타임 계산
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0f)
            {
                PerformAttack();
                
                // 공속(CurrentAttackSpeed)에 맞춰 쿨타임 재설정
                attackCooldown = hero.CurrentAttackSpeed > 0f ? (1f / hero.CurrentAttackSpeed) : 1f;
            }
        }
    }

    private void PerformAttack()
    {
        if (hero.Animator != null)
        {
            hero.Animator.SetTrigger("Attack");
        }

        // 실제 데미지 적용
        if (targetUnit is IDamageable damageable)
        {
            damageable.TakeDamage(hero.CurrentAttack);
        }
    }

    public void Exit() 
    {
        targetUnit = null;
    }
}