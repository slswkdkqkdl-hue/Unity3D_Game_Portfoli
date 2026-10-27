using UnityEngine;

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
        // 대기 시 Speed 0 (Idle)
        if (hero.Animator != null) hero.Animator.SetFloat("Speed", 0f);
    }

    public void Update() { }
    public void Exit() { }
}