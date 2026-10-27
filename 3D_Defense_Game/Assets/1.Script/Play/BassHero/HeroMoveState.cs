using UnityEngine;

public class HeroMoveState : IState
{
    private HeroBase hero;
    private Vector3 targetPosition;
    private float moveSpeed; 

    public HeroMoveState(HeroBase hero, Vector3 targetPosition, float moveSpeed)
    {
        this.hero = hero;
        this.targetPosition = targetPosition;
        this.moveSpeed = moveSpeed;
    }

    public void Enter()
    {
        // Y축 높이 차이 무시 (지면을 평평하게 이동하기 위함)
        targetPosition.y = hero.transform.position.y;
    }

    public void Update()
    {
        // 목표 지점으로 직접 이동 (받아온 moveSpeed 적용)
        hero.transform.position = Vector3.MoveTowards(
            hero.transform.position, 
            targetPosition, 
            moveSpeed * Time.deltaTime
        );

        // 바라보는 방향 회전
        Vector3 dir = (targetPosition - hero.transform.position).normalized;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            hero.transform.rotation = Quaternion.Slerp(hero.transform.rotation, lookRot, Time.deltaTime * 10f);
        }

        // 목적지에 거의 도달했으면 대기 상태로 전환
        if (Vector3.Distance(hero.transform.position, targetPosition) <= 0.1f)
        {
            hero.ChangeState(hero.IdleState);
        }
    }

    public void Exit() { }
}