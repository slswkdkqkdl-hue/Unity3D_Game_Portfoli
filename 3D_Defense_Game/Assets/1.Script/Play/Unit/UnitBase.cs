using UnityEngine;

public class UnitBase : MonoBehaviour, IDamageable
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

    [Header("팀 구분용 머테리얼")]
    [SerializeField] private Material PlayerMat;
    [SerializeField] private Material EnemyMat;

    [Header("본체 메시 렌더러")]
    public SkinnedMeshRenderer bodyRenderer;

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
    [SerializeField] protected float detectRange;  // 적을 감지하는 시야 범위
    [SerializeField] protected float attackRange;  // 공격을 시작할 수 있는 범위

    [Header("FSM 현재 상태")]
    [SerializeField] protected UnitState currentState;
    
    [Header("타겟 및 이동 목표")]
    [SerializeField] protected Transform objectiveTransform; // 찾아가야 할 최종 목표 (성 공격 포인트)
    protected Transform targetTransform;                    // 현재 추적/공격 중인 실제 타겟 (적 유닛 혹은 성)

    [Header("최종 이동 백터")]
    [SerializeField] Vector3 finalDir;

    [Header("최적화 및 쿨타임 타이머")]
    private float searchTimer = 0f;
    private float searchInterval = 0.3f; // 0.3초마다 적 탐색 (최적화)
    private float attackTimer = 0f; 

    private bool isDead = false;
    public bool IsDead => isDead;

    public virtual void Init(UnitData data, Team team)
    {   

        // 데이터 초기화 
        MyUnitData = data;
        isDead = false;
        currentTeam = team;
        objectiveTransform = null;
        targetTransform = null;
        attackTimer = 999f;

        if (data != null)
        {
            currentHp = data.hp;
            currentMp = data.mp;
            currentAttack = data.attack;
            currentDefense = data.defense;
            currentAttackSpeed = data.attackSpeed;
            currentMoveSpeed = data.moveSpeed;
            this.detectRange = data.detectRange;
            this.attackRange = data.attackRange;

            Debug.Log($"[{data.Unit_Name}] 스탯 적용 완료! (HP: {currentHp}, 이속: {currentMoveSpeed})");
        }

        // 100마리 이상일 때 탐색 연산 부하 분산을 위해 타이머에 랜덤 오차 부여
        searchTimer = Random.Range(0f, searchInterval);

        // 초기 성 공격 포인트 설정
        SetInitialObjective();
        targetTransform = objectiveTransform;

        // BattlefieldManager에 위치 등록
        if (BattlefieldManager.I != null)
        {
            BattlefieldManager.I.RegisterUnit(this, currentTeam);
        }

        ChangeState(UnitState.Chase); // 추격으로 시작 
    }
    /// <summary>
    /// 현재 타겟이 실제로 공격 가능한 대상인지 검사
    /// </summary>
    private bool IsValidTarget(Transform target)
    {
        if (target == null || !target.gameObject.activeInHierarchy)
            return false;

        // 유닛이면 사망 여부까지 검사
        UnitBase unit = target.GetComponentInParent<UnitBase>();

        if (unit != null && unit.IsDead)
            return false;

        // 성 등 IDamageable을 구현한 대상도 허용
        IDamageable damageable = target.GetComponent<IDamageable>();

        if (damageable == null)
            damageable = target.GetComponentInParent<IDamageable>();

        return damageable != null;
    }
    /// <summary>
    /// 가장 가까운 성 공격 포인트를 찾아 저장
    /// </summary>
    private void SetInitialObjective()
    {
        if (currentTeam == Team.Ally)
        {
            if (EnemyCastle.I != null)
            { 
                objectiveTransform = EnemyCastle.I.GetClosestAttackPoint(transform.position);
                bodyRenderer.material = PlayerMat;
            }
        }
        else
        {
            if (MainCastle.I != null)
            {
                objectiveTransform = MainCastle.I.GetClosestAttackPoint(transform.position);
                bodyRenderer.material = EnemyMat;
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
        if (objectiveTransform != null)
        {
            ChangeState(UnitState.Chase);
        }
    }

    /// <summary>
    /// [Chase 상태] 적 탐색 + 성/적 유닛을 향해 이동
    /// </summary>
    protected virtual void UpdateChase()
    {
        // 1. 주기적으로 적 탐색
        searchTimer += Time.deltaTime;

        if (searchTimer >= searchInterval)
        {
            searchTimer = 0f;
            CheckForNearbyEnemy();
        }

        // 2. 현재 타겟이 유효하지 않으면 재설정
        if (!IsValidTarget(targetTransform))
        {
            targetTransform = null;

            // 기존 성 목표도 유효한지 확인
            if (IsValidTarget(objectiveTransform))
            {
                targetTransform = objectiveTransform;
            }
            else
            {
                // 목표가 없다면 다음 탐색까지 대기
                return;
            }
        }

        // 3. 공격 범위 판정 (수평 거리)
        Vector3 offset = targetTransform.position - transform.position;
        offset.y = 0f;

        if (offset.sqrMagnitude <= attackRange * attackRange)
        {
            ChangeState(UnitState.Attack);
            return;
        }

        // 4. 추적 이동
        MoveTowardsTarget();
    }
    
    /// <summary>
    /// 시야 범위(detectRange) 내에 가장 가까운 적 유닛 탐색
    /// </summary>
    private void CheckForNearbyEnemy()
    {
        if (BattlefieldManager.I == null)
            return;

        Transform closestEnemy =BattlefieldManager.I.GetClosestUnitInRange(currentTeam,transform.position,detectRange);

        // 죽은 적이나 비활성 타겟은 선택하지 않음
        if (IsValidTarget(closestEnemy))
        {
            targetTransform = closestEnemy;
        }
        else
        {
            // 적이 없다면 성으로 복귀
            targetTransform = IsValidTarget(objectiveTransform)
                ? objectiveTransform
                : null;
        }
    }

    /// <summary>
    /// 물리 없이 순수 위치 이동 + 유닛 간 겹침 방지 반영
    /// </summary>
    private void MoveTowardsTarget()
    {
        Vector3 dir = targetTransform.position - transform.position;
        dir.y = 0f;
        Vector3 moveDir = dir.normalized;

        Vector3 separation = Vector3.zero;
        if (BattlefieldManager.I != null)
        {
            separation = BattlefieldManager.I.GetSeparationVector(gameObject, transform.position);
            separation.y = 0f;
        }

        finalDir = (moveDir + separation).normalized;

        if (finalDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(finalDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 15f * Time.deltaTime);
            
            transform.position += finalDir * currentMoveSpeed * Time.deltaTime;
        }
    }

    protected virtual void UpdateAttack()
    {
        // 1. 타겟이 죽었거나 없으면 즉시 추적 상태로 전환
        if (!IsValidTarget(targetTransform))
        {
            targetTransform = null;
            ChangeState(UnitState.Chase);
            return;
        }

        // 2. 타겟까지의 수평 거리 계산
        Vector3 offset = targetTransform.position - transform.position;
        offset.y = 0f;

        float sqrDist = offset.sqrMagnitude;
        
        // 공격 쿨타임 계산
        float attackInterval;
        if (currentAttackSpeed > 0f)
        {
            attackInterval = 1f / currentAttackSpeed;
        }
        else
        {
            attackInterval = 0.2f; 
        }

        // 만약 아직 공격 쿨타임이 차는 중(대기 중)이라면, 거리가 멀어졌을 때 추적으로 복귀 가능
        if (attackTimer < attackInterval)
        {
            float exitRange = attackRange * 1.5f; // 여유 범위를 좀 더 넉넉히(1.3~1.5배) 줍니다.
            if (sqrDist > exitRange * exitRange)
            {
                ChangeState(UnitState.Chase);
                return;
            }
        }

        // 4. 타겟을 바라보도록 회전
        if (offset.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(offset.normalized);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                15f * Time.deltaTime
            );
        }

        // 5. 공격 쿨타임 누적
        attackTimer += Time.deltaTime;

        if (attackTimer < attackInterval)
            return;

        // --- 공격 실행 구간 (이 순간부터는 데미지가 확정적으로 들어감) ---
        attackTimer = 0f;

        if (animator != null)
            animator.SetTrigger("Attack");

        IDamageable damageableTarget = targetTransform.GetComponent<IDamageable>();

        if (damageableTarget == null)
        {
            damageableTarget = targetTransform.GetComponentInParent<IDamageable>();
        }

        if (damageableTarget != null)
        {
            damageableTarget.TakeDamage(currentAttack);
        }
    }

    /// <summary>
    /// 공격 받았을시 호출
    /// </summary>
    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;
        currentHp = Mathf.Max(0, currentHp);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected void ChangeState(UnitState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        // 상태 전환 시 애니메이션 파라미터 제어
        switch (currentState)
        {
            case UnitState.Idle:
                if (animator != null) animator.SetFloat("Speed", 0f);
                break;
            case UnitState.Chase:
                if (animator != null) animator.SetFloat("Speed", 1f);
                break;
            case UnitState.Attack:
                if (animator != null) animator.SetFloat("Speed", 0f); // 멈춰서 공격
                attackTimer = 0f; // 공격 상태 진입 시 즉시 첫 타격 가능하도록 초기화
                break;
            case UnitState.Die:
                if (animator != null) animator.SetFloat("Speed", 0f);
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
        // 1. 파란색: 적 탐지 범위 (detectRange)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // 2. 빨간색: 공격 범위 (attackRange)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 3. ⭐ 노란색: 유닛 뭉침 방지 반경 (BattlefieldManager의 값 실시간 참조)
        if (BattlefieldManager.I != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, BattlefieldManager.I.separationRadius);
            Gizmos.DrawWireSphere(transform.position, BattlefieldManager.I.separationStrength);
        }
    }
}