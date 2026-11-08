using UnityEngine;

public class HeroBase : MonoBehaviour, ICombatEntity, IDamageable
{
    public static HeroBase ActiveHero { get; private set; }

    protected IState currentState;
    public HeroIdleState IdleState { get; private set; }
    public HeroMoveState MoveState { get; private set; }
    public HeroAttackState AttackState { get; private set; }

    [Header("영웅 런타임 데이터")]
    [SerializeField] private HeroRuntimeData runtimeData;
    public HeroRuntimeData RuntimeData => runtimeData;

    [Header("영웅 팀")]
    [SerializeField] private Team currentTeam; 
    public Team team => currentTeam;         

    [Header("애니메이션")]
    [SerializeField] private Animator animator;
    public Animator Animator => animator;

    private HeroSlot assignedSlot;

    [Header("영웅 런타임 스탯 (확인용)")]
    [field: SerializeField] public int CurrentHp { get; protected set; }
    [field: SerializeField] public int CurrentMp { get; protected set; }
    [field: SerializeField] public int CurrentAttack { get; protected set; }
    [field: SerializeField] public int CurrentDefense { get; protected set; }
    [field: SerializeField] public float CurrentAttackSpeed { get; protected set; }
    [field: SerializeField] public float CurrentMoveSpeed { get; protected set; }
    [field: SerializeField] public float detectionRange { get; protected set; }
    [field: SerializeField] public float attackRange { get; protected set; }



    protected virtual void Awake()
    {
        IdleState = new HeroIdleState(this);
        MoveState = new HeroMoveState(this);
        AttackState = new HeroAttackState(this);
    }

    protected virtual void OnEnable()
    {
        ActiveHero = this;
        if (BattlefieldManager.I != null)
        {
            BattlefieldManager.I.RegisterUnit(this, currentTeam);
        }
        ChangeState(IdleState);
    }

    protected virtual void OnDisable()
    {
        if (ActiveHero == this) ActiveHero = null;
        if (BattlefieldManager.I != null)
        {
            BattlefieldManager.I.UnregisterUnit(this, currentTeam);
        }
    }

    protected virtual void Update()
    {
        currentState?.Update();
    }
    /// <summary>
    /// 최초 한번 실행댈 초기화함수 
    /// </summary>
    public virtual void InitHero(HeroRuntimeData data, Team team, HeroSlot slot)
    {
        runtimeData = data;
        currentTeam = team; 
        assignedSlot = slot;

        CalculateStats();
        CalculateMaxExp();

        if (assignedSlot != null)
        {
            assignedSlot.SetUpHp(CurrentHp, CurrentHp); 
            assignedSlot.SetUpMp(CurrentMp, CurrentMp);
        }

    }

    /// <summary>
    /// 스테이스 갱신
    /// </summary>
    protected virtual void CalculateStats()
    {
        if (runtimeData == null || runtimeData.heroData == null) return;

        HeroData baseData = runtimeData.heroData;
        int levelFactor = Mathf.Max(0, runtimeData.level - 1);

        CurrentHp = baseData.hp + (baseData.hpPerLevel * levelFactor);
        CurrentMp = baseData.mp + (baseData.mpPerLevel * levelFactor);
        CurrentAttack = baseData.attack + (baseData.attackPerLevel * levelFactor);
        CurrentDefense = baseData.defense + (baseData.defensePerLevel * levelFactor);
        CurrentAttackSpeed = baseData.attackSpeed + (baseData.attackSpeedPerLevel * levelFactor);
        CurrentMoveSpeed = baseData.moveSpeed + (baseData.moveSpeedPerLevel * levelFactor);
        detectionRange = baseData.detectionRange;
        attackRange = baseData.attackRange;
    }
    /// <summary>
    /// 현재 레벨에 필요한 최대 경험치를 계산합니다.
    /// </summary>
    private void CalculateMaxExp()
    {
        runtimeData.maxExp = runtimeData.heroData.baseExp;

        for (int i = 1; i < runtimeData.level; i++)
        {
            runtimeData.maxExp *= runtimeData.heroData.expMultiplier;
        }
    }
    /// <summary>
    /// 캐릭터 생성시 팀에 따른 위치 선정
    /// </summary>
    public Transform GetClosestEnemyCastlePoint(Vector3 pos, float range, Team team)
    {
        Transform point = null;
        if (team == Team.Ally && EnemyCastle.I != null)
        {
            point = EnemyCastle.I.GetClosestAttackPoint(pos);
        }
        else if (team == Team.Enemy && MainCastle.I != null)
        {
            point = MainCastle.I.GetClosestAttackPoint(pos);
        }

        if (point != null)
        {
            float distSqr = (point.position - pos).sqrMagnitude;
            if (distSqr <= range * range) return point;
        }
        return null;
    }
    // 상태전환 
    public void ChangeState(IState newState)
    {
        if (currentState == newState) return;
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    // 이동명령시
    public void MoveTo(Vector3 destination)
    {
        MoveState.SetTargetPosition(destination);
        ChangeState(MoveState);
    }
    /// <summary>
    /// 애니메이션 이벤트에서 호출하는 공격 타격 함수 추후에 늘어나지만일딴 먼저
    /// </summary>
    public virtual void PerformAttack()
    {
        if (currentState is HeroAttackState attackState)
        {
            attackState.PerformAttack();
        }
    }

    /// <summary>
    /// 피해를 받아 체력을 감소시키고 사망 여부를 처리합니다.
    /// </summary>
    public virtual void TakeDamage(int damage , ICombatEntity attacker)
    {
        CurrentHp -= damage;
        CurrentHp = Mathf.Max(0, CurrentHp);

        if (assignedSlot != null)
        {
            assignedSlot.UpdatHP(CurrentHp);
        }

        if (CurrentHp <= 0)
        {
            Debug.Log("죽음");
        }
    }
    /// <summary>
    /// 경험치를 추가하고 레벨업을 처리합니다.
    /// </summary>
    public void GainExp(int amount)
    {
        if (runtimeData == null) return;

        runtimeData.currentExp += amount;
        while (runtimeData.currentExp >= runtimeData.maxExp)
        {
            if (runtimeData.level >= runtimeData.heroData.maxLevel)
            {
                runtimeData.currentExp = runtimeData.maxExp;
                break;
            }

            runtimeData.currentExp -= runtimeData.maxExp;

            runtimeData.level++;

            runtimeData.maxExp *= runtimeData.heroData.expMultiplier;

            BattleResultManager.I.AddLeveledUpHero(runtimeData);
        }

    }


    /// <summary>
    /// 영웅의 모든 스탯과 정보를 구조체 묶음으로 통째로 반환 .. 영웅정보 보기에사용댐
    /// </summary>
    public CombatEntityData GetEntityData()
    {
        return new CombatEntityData
        {
            entityName = runtimeData.heroData != null ? runtimeData.heroData.hero_PullName : "Unknown",
            currentHp = this.CurrentHp,
            currentMp = this.CurrentMp,
            currentAttack = this.CurrentAttack,
            currentDefense = this.CurrentDefense,
            currentAttackSpeed = this.CurrentAttackSpeed,
            currentMoveSpeed = this.CurrentMoveSpeed,
            detectRange = this.detectionRange,
            attackRange = this.attackRange,
            image = this.runtimeData.heroData.portrait
        };
    }
}