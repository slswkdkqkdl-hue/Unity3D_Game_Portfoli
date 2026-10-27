using UnityEngine;

public class HeroMoveState : IState
{
    private HeroBase hero;
    private Vector3 targetPosition;

    public HeroMoveState(HeroBase hero)
    {
        this.hero = hero;
    }

    // 2. 우클릭 시 목적지만 가볍게 갱신하는 전용 함수
    public void SetTargetPosition(Vector3 destination)
    {
        this.targetPosition = destination;
        this.targetPosition.y = hero.transform.position.y; 
    }

    public void Enter()
    {
        // 뛰는 애니메이션 실행
        if (hero.Animator != null)
        {
            hero.Animator.SetFloat("Speed", 1f);
        }
    }

    public void Update()
    {

        hero.transform.position = Vector3.MoveTowards(hero.transform.position, targetPosition, hero.CurrentMoveSpeed * Time.deltaTime);

        // 회전 처리
        Vector3 dir = (targetPosition - hero.transform.position).normalized;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            hero.transform.rotation = Quaternion.Slerp(hero.transform.rotation, lookRot, Time.deltaTime * 10f);
        }

        // 목적지 도착 시 Idle로 전환
        if (Vector3.Distance(hero.transform.position, targetPosition) <= 0.1f)
        {
            hero.ChangeState(hero.IdleState);
        }
    }

    public void Exit() 
    {
        if (hero.Animator != null)
        {
            hero.Animator.SetFloat("Speed", 0f);
        }
    }
}