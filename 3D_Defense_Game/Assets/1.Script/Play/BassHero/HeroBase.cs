using UnityEngine;

public class HeroBase : MonoBehaviour
{
    protected IState currentState;
    public IState IdleState { get; private set; }
    public HeroMoveState MoveState { get; private set; }

    [Header("영웅 런타임 데이터")]
    [SerializeField] private HeroRuntimeData runtimeData;
    public HeroRuntimeData RuntimeData => runtimeData;

    [Header("영웅 팀")]
    public Team team { get; protected set; }
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
        ChangeState(IdleState);
    }

    protected virtual void Update()
    {
        currentState?.Update();
    }

    /// <summary>
    /// 외부(Herocontroller 등)에서 영웅을 소환·초기화할 때 호출해 주는 메서드
    /// </summary>
    public virtual void InitHero(HeroRuntimeData data ,Team team)
    {
        runtimeData = data;
        this.team = team;
        CalculateStats(); // 레벨 반영 스탯 계산
    }

    /// <summary>
    /// 레벨당 증가량을 반영하여 최종 스탯을 계산하는 핵심 함수
    /// </summary>
    protected void CalculateStats()
    {
        if (runtimeData == null || runtimeData.heroData == null)
        {
            Debug.LogWarning($"[{gameObject.name}] 런타임 데이터나 HeroData가 비어있습니다!");
            return;
        }

        HeroData baseData = runtimeData.heroData;
        int lvl = runtimeData.level; // 0레벨 혹은 1레벨 기준 보정 필요시 확인

        // 기본 스탯 + (레벨당 증가 스탯 * 현재 레벨)
        CurrentHp = baseData.hp + (baseData.hpPerLevel * lvl);
        CurrentMp = baseData.mp + (baseData.mpPerLevel * lvl);
        CurrentAttack = baseData.attack + (baseData.attackPerLevel * lvl);
        CurrentDefense = baseData.defense + (baseData.defensePerLevel * lvl);
        CurrentAttackSpeed = baseData.attackSpeed + (baseData.attackSpeedPerLevel * lvl);
        CurrentMoveSpeed = baseData.moveSpeed + (baseData.moveSpeedPerLevel * lvl);

        Debug.Log($"[{baseData.hero_name}] 스탯 초기화 완료 (레벨: {lvl}) / 체력: {CurrentHp}, 공격력: {CurrentAttack}, 이속: {CurrentMoveSpeed}");
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
        MoveState = new HeroMoveState(this, destination, CurrentMoveSpeed); // 계산된 이동속도 전달 가능!
        ChangeState(MoveState);
    }

    public void StopMovement() { }
}