using UnityEngine;

public class HeroIdleState : IState
{
    private HeroBase hero;

    private float scanTimer = 0f;
    private float scanInterval = 0.3f; //

    public HeroIdleState(HeroBase hero)
    {
        this.hero = hero;
    }

    public void Enter()
    {
        hero.StopMovement();
        
        // 대기 시 Speed 0 (Idle 애니메이션)
        if (hero.Animator != null) hero.Animator.SetFloat("Speed", 0f);
    }

    public void Update()
    {
        if (BattlefieldManager.I != null)
        {
            scanTimer += Time.deltaTime;
            if (scanTimer > scanInterval)
            {
                Transform closestEnemyTransform = BattlefieldManager.I.GetClosestUnitInRange(hero.team, hero.transform.position, hero.detectionRange);
                if (closestEnemyTransform != null)
                {
                    ICombatEntity enemyEntity = closestEnemyTransform.GetComponent<ICombatEntity>();
                    if (enemyEntity != null)
                    {
                        hero.AttackTo(enemyEntity);
                    }
                }
                scanTimer = 0f;
            }
        }
    }

    public void Exit() { }
}