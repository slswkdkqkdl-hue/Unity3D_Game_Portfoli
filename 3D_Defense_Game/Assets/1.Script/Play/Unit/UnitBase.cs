using UnityEngine;

public class UnitBase : MonoBehaviour
{
    // 유닛의 FSM 상태 정의
    public enum UnitState
    {
        Idle,
        Chase,
        Attack,
        Die
    }

    [Header("유닛 데이터")]
    public UnitData MyUnitData { get; private set; }
    private Team currentTeam;

    [Header("런타임 스탯 (적용된 값)")]
    [SerializeField] protected int currentHp;
    [SerializeField] protected int currentMp;
    [SerializeField] protected int currentAttack;
    [SerializeField] protected int currentDefense;
    [SerializeField] protected float currentAttackSpeed;
    [SerializeField] protected float currentMoveSpeed;

    [Header("애니메이션")]
    [SerializeField] private Animator animator;

    [Header("거리 및 감지")]
    [SerializeField] protected float detectRange = 5f;  // 적을 감지하는 시야 범위
    [SerializeField] protected float attackRange = 1f;  // 공격을 시작할 수 있는 범위

    [Header("FSM 현재 상태")]
    [SerializeField] protected UnitState currentState;
    
    [Header("타겟 및 이동 목표")]
    [SerializeField]protected Transform objectiveTransform; // 찾아가야 할 최종 목표

    [Header("최종 이동 백터")]
    [SerializeField]Vector3 finalDir;

    private bool isDead = false;

    public virtual void Init(UnitData data, Team team)
    {
        MyUnitData = data;
        isDead = false;
        currentTeam = team;
        objectiveTransform = null;

        if (data != null)
        {
            currentHp = data.hp;
            currentMp = data.mp;
            currentAttack = data.attack;
            currentDefense = data.defense;
            currentAttackSpeed = data.attackSpeed;
            currentMoveSpeed = data.moveSpeed;

            Debug.Log($"[{data.Unit_Name}] 스탯 적용 완료! (HP: {currentHp}, 이속: {currentMoveSpeed})");
        }

        // BattlefieldManager에 위치 등록
        if (BattlefieldManager.I != null)
        {
            BattlefieldManager.I.RegisterUnit(this, currentTeam);
        }
        SetInitialObjective();

        ChangeState(UnitState.Chase);
    }

    /// <summary>
    /// 가장 가까운 성 공격 포인트를 찾아 백터 저장
    /// </summary>
    private void SetInitialObjective()
    {
        if (currentTeam == Team.Ally)
        {
            if (EnemyCastle.I != null)
            {   
                objectiveTransform = EnemyCastle.I.GetClosestAttackPoint(transform.position);
            }
        }
        else
        {
            if (MainCastle.I != null)
            {
                objectiveTransform = MainCastle.I.GetClosestAttackPoint(transform.position);
            }
        }

        if (objectiveTransform == null)
        {
            Debug.LogWarning($"[{MyUnitData.Unit_Name}] 목표 성의 공격 포인트를 찾지 못했습니다!");
        }
    }

    protected virtual void Update()
    {
        if (isDead) return;

        // FSM 상태에 따른 매 프레임 행동 처리
        switch (currentState)
        {
            case UnitState.Idle:
                UpdateIdle();
                break;
            case UnitState.Chase:
                UpdateChase();
                break;
            case UnitState.Attack:
                UpdateAttack();
                break;
            case UnitState.Die:
                break;
        }
    }

    protected virtual void UpdateIdle()
    {
       

    }

    /// <summary>
    /// [Chase 상태] 목표 지점을 향해 이동하는 상태 (물리 없이 position 이동 + 뭉침 방지)
    /// </summary>
    protected virtual void UpdateChase()
    {
        if (objectiveTransform == null) return;

        // 1. 목표 지점 방향 계산
        Vector3 dir = objectiveTransform.position - transform.position;
        dir.y = 0f;
        Vector3 moveDir = dir.normalized;

        finalDir = moveDir; 

        if (finalDir != Vector3.zero)
        {
            // 부드러운 회전
            Quaternion targetRot = Quaternion.LookRotation(finalDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 15f * Time.deltaTime);
            
            // 물리 없이 순수 위치 이동
            transform.position += finalDir * currentMoveSpeed * Time.deltaTime;
        }
    }

    protected virtual void UpdateAttack()
    {
        
    }

    protected void ChangeState(UnitState newState)
    {
        if (currentState == newState) return;
        currentState = newState;


            // 애니메이션연출
        switch (currentState)
        {
            case UnitState.Idle:
                animator.SetFloat("Speed", 0f);
                break;
            case UnitState.Chase:
                animator.SetFloat("Speed", 1f);
                break;
            case UnitState.Attack:
            
                break;
            case UnitState.Die:
                break;
        }
    }

    public virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        ChangeState(UnitState.Die);

        if (BattlefieldManager.I != null)
        {
            BattlefieldManager.I.UnregisterUnit(this, currentTeam);
        }

        if (MyUnitData != null && currentTeam == Team.Ally && ResourceManager.I != null)
        {
            ResourceManager.I.DecreasePopulation(MyUnitData.population);
            Debug.Log($"[{MyUnitData.Unit_Name} 사망] 인구수 {MyUnitData.population} 반환 완료");
        }

        if (UnitPoolManager.I != null)
        {
            UnitPoolManager.I.ReturnToPool(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}