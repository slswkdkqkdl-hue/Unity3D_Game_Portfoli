using UnityEngine;

public class UnitBase : MonoBehaviour
{
    [Header("유닛 데이터")]
    public UnitData MyUnitData { get; private set; }

    [Header("런타임 스탯 (적용된 값)")]
    public int currentHp;
    public int currentMp;
    public int currentAttack;
    public int currentDefense;
    public float currentAttackSpeed;
    public float currentMoveSpeed;

    private bool isDead = false; // 중복 반환 방지용 플래그
    
    // 초기화 함수: 데이터 통째로 받아서 스탯 적용하기
    public virtual void Init(UnitData data)
    {
        MyUnitData = data;

        if (data != null)
        {
            // UnitData의 스탯들을 런타임 변수에 그대로 복사해서 적용
            currentHp = data.hp;
            currentMp = data.mp;
            currentAttack = data.attack;
            currentDefense = data.defense;
            currentAttackSpeed = data.attackSpeed;
            currentMoveSpeed = data.moveSpeed;

            Debug.Log($"[{data.Unit_Name}] 스탯 적용 완료! (HP: {currentHp}, 공격력: {currentAttack}, 이속: {currentMoveSpeed})");
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
    protected virtual void OnDestroy()
    {
        // 씬이 언로드되는 도중에 매니저가 먼저 파괴되어 생기는 에러 방지
        if (!gameObject.scene.isLoaded) return;

        // 유닛 데이터가 존재하고, 아직 인구수가 반환되지 않았다면
        if (MyUnitData != null && !isDead)
        {
            isDead = true;

            if (ResourceManager.I != null)
            {
                // 유닛이 소모했던 인구수만큼 다시 감소(반환)시키기
                ResourceManager.I.DecreasePopulation(MyUnitData.population);
                Debug.Log($"[{MyUnitData.Unit_Name} 사망/파괴] 인구수 {MyUnitData.population} 반환 완료");
            }
        }
    }
}