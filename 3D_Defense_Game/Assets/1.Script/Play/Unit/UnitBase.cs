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

    public virtual void Init(UnitData data, Team team)
    {
        MyUnitData = data;
        isDead = false;
        currentTeam = team;
        objectiveTransform = null;
        targetTransform = null;
        attackTimer = 0f;

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

        ChangeState(UnitState.Chase);
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
        // 1. 주기적인 적 탐색
        searchTimer += Time.deltaTime;
        if (searchTimer >= searchInterval)
        {
            searchTimer = 0f;
            CheckForNearbyEnemy();
        }

        // 2. 타겟 유효성 검사
        if (targetTransform == null || !targetTransform.gameObject.activeInHierarchy)
        {
            targetTransform = objectiveTransform;
            if (targetTransform == null) return;
        }

        // 3. 공격 범위 내로 들어왔는지 체크
        float sqrDist = (targetTransform.position - transform.position).sqrMagnitude;
        float attackRangeSqr = attackRange * attackRange;

        if (sqrDist <= attackRangeSqr)
        {
            ChangeState(UnitState.Attack);
            return;
        }

        // 4. 타겟을 향한 이동 및 뭉침 방지(Separation) 적용
        MoveTowardsTarget();
    }

    /// <summary>
    /// 시야 범위(detectRange) 내에 가장 가까운 적 유닛 탐색
    /// </summary>
    private void CheckForNearbyEnemy()
    {
        if (BattlefieldManager.I == null) return;

        Transform closestEnemy = BattlefieldManager.I.GetClosestUnitInRange(currentTeam, transform.position, detectRange);

        if (closestEnemy != null)
        {
            // 적을 발견하면 타겟을 적 유닛으로 변경
            targetTransform = closestEnemy;
        }
        else
        {
            // 감지된 적이 없으면 본래 목표인 성 공격 포인트로 복귀
            if (targetTransform != objectiveTransform)
            {
                targetTransform = objectiveTransform;
            }
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

    /// <summary>
    /// [Attack 상태] 멈춰 서서 공격 쿨타임마다 공격 수행
    /// </summary>
    protected virtual void UpdateAttack()
    {
        // 1. 타겟이 사라졌거나 죽었는지 확인
        if (targetTransform == null || !targetTransform.gameObject.activeInHierarchy)
        {
            targetTransform = objectiveTransform;
            ChangeState(UnitState.Chase);
            return;
        }

        // 2. 타겟이 공격 범위를 벗어났는지 확인 (약간의 여유 Hysteresis 부여)
        float sqrDist = (targetTransform.position - transform.position).sqrMagnitude;
        float attackRangeSqr = attackRange * attackRange;
        if (sqrDist > attackRangeSqr * 1.3f)
        {
            ChangeState(UnitState.Chase);
            return;
        }

        // 3. 공격 속도(AttackSpeed)에 따른 쿨타임 계산
        attackTimer += Time.deltaTime;
        float attackInterval = (currentAttackSpeed > 0f) ? (1f / currentAttackSpeed) : 1f;

        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;

            // 애니메이션 Attack 트리거 작동
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            IDamageable damageableTarget = targetTransform.GetComponent<IDamageable>();
            
            if (damageableTarget == null)
            {
                damageableTarget = targetTransform.GetComponentInParent<IDamageable>();
            }

            if (damageableTarget != null)
            {
                damageableTarget.TakeDamage(currentAttack);
                Debug.Log($"[{MyUnitData.Unit_Name}] 성 공격 성공! 데미지: {currentAttack}");
            }
            else
            {
                Debug.LogWarning($"[{MyUnitData.Unit_Name}] 타겟({targetTransform.name})에서 IDamageable을 찾을 수 없습니다!");
            }
        }
    }

    /// <summary>
    /// IDamageable 구현: 공격받았을 때 호출
    /// </summary>
    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;
        currentHp = Mathf.Max(0, currentHp);

        Debug.Log($"[{MyUnitData?.Unit_Name ?? "Unit"} 피격] 남은 체력: {currentHp}");

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