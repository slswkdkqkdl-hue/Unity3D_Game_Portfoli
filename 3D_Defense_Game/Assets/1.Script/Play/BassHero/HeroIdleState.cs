using UnityEngine;

// 1. 대기 상태 (Idle)
public class HeroIdleState : IState
{
    private HeroBase hero;

    public HeroIdleState(HeroBase hero)
    {
        this.hero = hero;
    }

    public void Enter()
    {
        hero.StopMovement();
    }

    public void Update() { }

    public void Exit() { }
}