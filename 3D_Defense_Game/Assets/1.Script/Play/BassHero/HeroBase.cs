using UnityEngine;

public class HeroBase : MonoBehaviour, ICombatEntity, IDamageable
{
    public static HeroBase ActiveHero { get; private set; }

    protected IState currentState;
    public IState IdleState { get; private set; }
    public HeroMoveState MoveState { get; private set; }

    [Header("영웅 런타임 데이터")]
    [SerializeField] private HeroRuntimeData runtimeData;
    public HeroRuntimeData RuntimeData => runtimeData;

    [Header("영웅 팀")]
    [SerializeField] private Team currentTeam; // ICombatEntity용 팀 변수
    public Team team => currentTeam;         // 인터페이스 구현

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
    }

    protected virtual void OnEnable()
    {
        ActiveHero = this;

        // ⭐ BattlefieldManager에 영웅 등록! (이제 적들이 영웅을 인식하고 밀어내기도 함)
        if (BattlefieldManager.I != null)
        {
            BattlefieldManager.I.RegisterUnit(this, currentTeam);
        }

        ChangeState(IdleState);
    }

    protected virtual void OnDisable()
    {
        if (ActiveHero == this)
        {
            ActiveHero = null;
        }

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

    public void MoveTo(Vector3 destination)
    {
        MoveState = new HeroMoveState(this, destination, CurrentMoveSpeed);
        ChangeState(MoveState);
    }

    public void StopMovement() { }


    public virtual void TakeDamage(int damage)
    {
        CurrentHp -= damage;
        CurrentHp = Mathf.Max(0, CurrentHp);

        // ⭐ 방금 만든 UpdatHP 함수에 현재 체력 숫자만 툭 던지기!
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