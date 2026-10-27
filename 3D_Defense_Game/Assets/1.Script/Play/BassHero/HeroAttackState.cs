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
        attackCooldown = 0f; 

        // 공격 상태 진입 시 이동 정지 애니메이션 (Speed = 0)
        if (hero.Animator != null) 
        {
            hero.Animator.SetFloat("Speed", 0f);
        }
    }

    public void Update()
    {
        // 1. 타겟이 죽거나 사라졌다면 Idle로 복귀
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

        // 2. 만약 적이 도망쳐서 사거리를 벗어났다면 -> 다시 MoveState의 추격 모드로 전환!
        if (sqrDist > sqrAttackRange)
        {
            hero.MoveState.SetTargetUnit(targetUnit);
            hero.ChangeState(hero.MoveState);
            return;
        }

        // 3. 사거리 내에 있으므로 적을 바라보며 공격에만 집중
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
            attackCooldown = hero.CurrentAttackSpeed > 0f ? (1f / hero.CurrentAttackSpeed) : 1f;
        }
    }

    private void PerformAttack()
    {
        if (hero.Animator != null)
        {
            hero.Animator.SetTrigger("Attack");
        }

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