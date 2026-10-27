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

    public virtual void InitHero(HeroRuntimeData data, Team team, HeroSlot slot)
    {
        runtimeData = data;
        currentTeam = team; 
        assignedSlot = slot;

        CalculateStats();

        if (assignedSlot != null)
        {
            assignedSlot.SetUpHp(CurrentHp, CurrentHp); 
            assignedSlot.SetUpMp(CurrentMp, CurrentMp);
        }
    }

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


    public virtual void TakeDamage(int damage)
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
}