using UnityEngine;

public class UnitBase : MonoBehaviour
{
    [Header("유닛 데이터")]
    public UnitData MyUnitData { get; private set; }

    public Team currentTeam;

    [Header("런타임 스탯 (적용된 값)")]
    public int currentHp;
    public int currentMp;
    public int currentAttack;
    public int currentDefense;
    public float currentAttackSpeed;
    public float currentMoveSpeed;

    private bool isDead = false; // 중복 반환 방지용 플래그
    
    // 초기화 함수: 데이터 통째로 받아서 스탯 적용하기
    public virtual void Init(UnitData data , Team team)
    {
        MyUnitData = data;
        isDead = false;
        currentTeam = team;
        if (data != null)
        {
            // UnitData의 스탯들을 런타임 변수에 그대로 복사해서 적용
            currentHp = data.hp;
            currentMp = data.mp;
            currentAttack = data.attack;
            currentDefense = data.defense;
            currentAttackSpeed = data.attackSpeed;
            currentMoveSpeed = data.moveSpeed;

            Debug.Log($"[{data.Unit_Name}] 스탯 적용 완료! (HP: {currentHp}, 이속: {currentMoveSpeed})");
        }
    }

    protected virtual void Update()
    {
        // 태어나서 앞으로 걸어가는 테스트용 이동 로직
        MoveForward();
    }

    protected void MoveForward()
    {
        if (MyUnitData != null)
        {
            // 적용된 currentMoveSpeed를 사용해서 앞으로 이동
            transform.Translate(Vector3.forward * currentMoveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 유닛이 사망했을 때 호출하는 함수 (풀링 환경에 맞춤)
    /// </summary>
    public virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        // 1. 인구수 반환 처리
        if (MyUnitData != null && ResourceManager.I != null)
        {
            ResourceManager.I.DecreasePopulation(MyUnitData.population);
            Debug.Log($"[{MyUnitData.Unit_Name} 사망] 인구수 {MyUnitData.population} 반환 완료");
        }

        // 2. 삭제(Destroy)하는 대신 UnitPoolManager를 통해 풀로 돌려보내기
        if (UnitPoolManager.I != null)
        {
            UnitPoolManager.I.ReturnToPool(this);
        }
        else
        {
            Destroy(gameObject); // 혹시 매니저가 없다면 안전하게 파괴
        }
    }


}