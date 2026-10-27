using UnityEngine;

public class HeroMoveState : IState
{
    private HeroBase hero;
    private Vector3 targetPosition;
    private ICombatEntity targetUnit; // 추격할 적 타겟 (null이면 일반 지면 이동)
    private bool isPursuing = false;  // 적 추격 중인지 여부

    public HeroMoveState(HeroBase hero)
    {
        this.hero = hero;
    }

    // 1. 일반 지면 이동용
    public void SetTargetPosition(Vector3 destination)
    {
        this.targetPosition = destination;
        this.targetPosition.y = hero.transform.position.y;
        this.targetUnit = null;
        this.isPursuing = false;
    }

    // 2. 적 추격 이동용
    public void SetTargetUnit(ICombatEntity target)
    {
        this.targetUnit = target;
        this.isPursuing = true;
    }

    public void Enter()
    {
        if (hero.Animator != null)
        {
            hero.Animator.SetFloat("Speed", 1f); // 뛰는 애니메이션
        }
    }

    public void Update()
    {
        Vector3 destPos = targetPosition;

        // 만약 적을 추격하는 중이라면, 적의 현재 위치를 실시간으로 목적지로 삼음
        if (isPursuing)
        {
            if (targetUnit == null || (targetUnit is MonoBehaviour mono && !mono.gameObject.activeInHierarchy))
            {
                // 적이 사라졌으면 대기 상태로 복귀
                hero.ChangeState(hero.IdleState);
                return;
            }

            destPos = targetUnit.transform.position;
            destPos.y = hero.transform.position.y;

            // 사거리(attackRange) 안에 들어왔는지 체크!
            float sqrDist = (destPos - hero.transform.position).sqrMagnitude;
            float sqrAttackRange = hero.attackRange * hero.attackRange;

            if (sqrDist <= sqrAttackRange)
            {
                // 사거리 도달 완료! 공격 상태로 전환
                hero.AttackState.SetTarget(targetUnit);
                hero.ChangeState(hero.AttackState);
                return;
            }
        }

        // 이동 처리 (MoveTowards)
        hero.transform.position = Vector3.MoveTowards(
            hero.transform.position, 
            destPos, 
            hero.CurrentMoveSpeed * Time.deltaTime
        );

        // 회전 처리
        Vector3 dir = (destPos - hero.transform.position).normalized;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            hero.transform.rotation = Quaternion.Slerp(hero.transform.rotation, lookRot, Time.deltaTime * 10f);
        }

        // 일반 이동일 경우, 목적지 도착 시 Idle로 전환
        if (!isPursuing && Vector3.Distance(hero.transform.position, destPos) <= 0.1f)
        {
            hero.ChangeState(hero.IdleState);
        }
    }

    public void Exit() 
    {
        isPursuing = false;
        targetUnit = null;

        if (hero.Animator != null)
        {
            hero.Animator.SetFloat("Speed", 0f);
        }
    }
}