using UnityEngine;

public class HeroIdleState : IState
{
    private HeroBase hero;

    private float scanTimer = 0f;
    private float scanInterval = 0.3f; // 최적화용도

    public HeroIdleState(HeroBase hero)
    {
        this.hero = hero;
    }

    public void Enter()
    {        
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
                        hero.MoveState.SetTargetUnit(enemyEntity);
                        hero.ChangeState(hero.MoveState);
                        scanTimer = 0f;
                        return;
                    }
                }
                Transform closestCastlePoint = hero.GetClosestEnemyCastlePoint(hero.transform.position, hero.detectionRange,hero.team);
                if (closestCastlePoint != null)
                {

                    ICombatEntity castleTarget = closestCastlePoint.GetComponent<ICombatEntity>();
                    if (castleTarget == null) castleTarget = closestCastlePoint.GetComponentInChildren<ICombatEntity>();

                    if (castleTarget != null)
                    {
                        hero.MoveState.SetTargetUnit(castleTarget);
                        hero.ChangeState(hero.MoveState);
                    }
                }

                scanTimer = 0f;
            }

        }
    }

    public void Exit() { }


}