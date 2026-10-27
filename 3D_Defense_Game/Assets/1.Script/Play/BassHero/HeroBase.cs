using UnityEngine;

public class HeroBase : MonoBehaviour, ICombatEntity, IDamageable
{
    public static HeroBase ActiveHero { get; private set; }

    protected IState currentState;
    public HeroIdleState IdleState { get; private set; }
    public HeroMoveState MoveState { get; private set; }

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

    public int CurrentHp { get; protected set; }
    public int CurrentMp { get; protected set; }
    public int CurrentAttack { get; protected set; }
    public int CurrentDefense { get; protected set; }
    public float CurrentAttackSpeed { get; protected set; }
    public float CurrentMoveSpeed { get; protected set; }

    protected virtual void Awake()
    {
        IdleState = new HeroIdleState(this);
        MoveState = new HeroMoveState(this);
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

    protected void CalculateStats()
    {
        if (runtimeData == null || runtimeData.heroData == null) return;

        HeroData baseData = runtimeData.heroData;
        int lvl = runtimeData.level;

        CurrentHp = baseData.hp + (baseData.hpPerLevel * lvl);
        CurrentMp = baseData.mp + (baseData.mpPerLevel * lvl);
        CurrentAttack = baseData.attack + (baseData.attackPerLevel * lvl);
        CurrentDefense = baseData.defense + (baseData.defensePerLevel * lvl);
        CurrentAttackSpeed = baseData.attackSpeed + (baseData.attackSpeedPerLevel * lvl);
        CurrentMoveSpeed = baseData.moveSpeed + (baseData.moveSpeedPerLevel * lvl);
    }

    public void ChangeState(IState newState)
    {
        if (currentState == newState) return;
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    // ⭐ 목적지 좌표만 틱 던져주고 상태 전환!
    public void MoveTo(Vector3 destination)
    {
        MoveState.SetTargetPosition(destination);
        ChangeState(MoveState);
    }

    public void StopMovement() { }

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