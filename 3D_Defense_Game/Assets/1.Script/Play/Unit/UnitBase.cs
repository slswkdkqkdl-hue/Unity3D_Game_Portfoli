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
    protected Transform targetTransform; // 현재 추적중인것

    private bool isDead = false;

    public virtual void Init(UnitData data, Team team)
    {
        MyUnitData = data;
        isDead = false;
        currentTeam = team;
        currentState = UnitState.Chase; // 생성 시 기본은 Idle로 시작
        targetTransform = null;

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
                // 사망 상태에서는 별도 처리 없음 (Die() 함수에서 풀 반환 등 처리)
                break;
        }
    }

    /// <summary>
    /// [Idle 상태] 적을 탐색하거나 전진할 타겟을 찾는 상태
    /// </summary>
    protected virtual void UpdateIdle()
    {
        // TODO: 자식 클래스(SwordsMan 등)나 여기서 범위 내 적 탐지 로직 수행
        // 적을 찾으면 targetTransform에 넣고 ChangeState(UnitState.Chase)로 전환

    }

    /// <summary>
    /// [Chase 상태] 타겟을 향해 이동하는 상태 (물리 없이 position 이동)
    /// </summary>
    protected virtual void UpdateChase()
    {
        // TODO: 타겟과의 거리를 계산하여 attackRange 이내면 Attack으로, 
        // 타겟이 사라졌거나 멀어지면 Idle로 전환
    }

    /// <summary>
    /// [Attack 상태] 멈춰 서서 공격을 수행하는 상태
    /// </summary>
    protected virtual void UpdateAttack()
    {
        // TODO: 공격 쿨타임에 맞춰 데미지 주기
        // 타겟이 범위 밖으로 도망치거나 죽으면 다시 Chase나 Idle로 전환
    }

    /// <summary>
    /// 상태를 안전하게 전환하는 함수
    /// </summary>
    protected void ChangeState(UnitState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        
        // 상태가 바뀔 때 필요한 초기화 작업이 있다면 여기서 수행 가능
    }

    /// <summary>
    /// BattlefieldManager에게 요청하여 감지 범위 내 가장 가까운 적을 탐색
    /// </summary>
    // protected Transform FindClosestEnemy()
    // {
    //     if (BattlefieldManager.I == null) return null;
    //     return BattlefieldManager.I.GetClosestUnitInRange(currentTeam, transform.position, detectRange);
    // }

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
        // 에디터에서 감지 범위(파란색)와 공격 범위(빨간색) 시각화
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}