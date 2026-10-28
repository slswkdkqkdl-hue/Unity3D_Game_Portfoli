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
        // 1. 타겟 체크 및 사거리 체크는 기존과 동일
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

        if (sqrDist > sqrAttackRange)
        {
            hero.MoveState.SetTargetUnit(targetUnit);
            hero.ChangeState(hero.MoveState);
            return;
        }

        // 적 바라보기
        Vector3 dir = (targetPos - hero.transform.position).normalized;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            hero.transform.rotation = Quaternion.Slerp(hero.transform.rotation, lookRot, Time.deltaTime * 15f);
        }

        // 2. 쿨타임 감소
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            // 쿨타임이 찼다면 공격 애니메이션 재생!
            // (애니메이션 안의 특정 프레임에 도달하면 애니메이션 이벤트가 PerformAttack()을 호출함)
            if (hero.Animator != null)
            {
                hero.Animator.SetTrigger("Attack");
            }

            // 다음 공격까지의 쿨타임 재설정 (공속에 따라)
            attackCooldown = hero.CurrentAttackSpeed > 0f ? (1f / hero.CurrentAttackSpeed) : 1f;
        }
    }

    // 애니멤이션 이벤트함수
    public void PerformAttack()
    {
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