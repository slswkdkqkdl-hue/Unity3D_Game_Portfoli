using UnityEngine;

public class Barracks : Building
{
    [System.Serializable]
    public class UnitSlotData
    {
        public string unitName;          // 유닛 이름
        public float spawnCooldown = 5f; // 유닛별 생산 쿨타임
        public GameObject unitPrefab;    // 생성될 유닛 프리팹
    }

    // 추후 UI 팝업에서 현재 진행 상황을 보여주기 위해 쓸 수 있는 프로퍼티들
    public int CurrentSelectedSlot => currentSelectedSlot;
    public float CurrentTimer => timer;
    public UnitSlotData[] UnitSlots => unitSlots;

    [Header("유닛 슬롯 (0번 ~ 9번)")]
    [SerializeField] private UnitSlotData[] unitSlots = new UnitSlotData[10];

    private int currentSelectedSlot = 0; // 현재 선택된 슬롯 (기본 0번)
    private float timer = 0f;           // 생산 쿨타임 타이머

    public override void Init(BuildingDataSO data, Team team = Team.Ally)
    {
        base.Init(data, team);
        buildingActivationType = BuildingActivationType.Passive;

        // 게임 시작 시(건설 시) 기본 0번 슬롯으로 초기화
        // SelectSlot(0);
    }

    private void Update()
    {
        if (buildingData == null) return;
        
        // 현재 선택된 슬롯의 유닛 데이터 유효성 검사
        if (unitSlots == null || currentSelectedSlot < 0 || currentSelectedSlot >= unitSlots.Length) return;
        var currentUnit = unitSlots[currentSelectedSlot];
        
        // 유닛 데이터나 프리팹이 비어있으면 쿨타임 정지
        if (currentUnit == null || currentUnit.unitPrefab == null) return;

        // 쿨타임 타이머 누적
        timer += Time.deltaTime;

        // 설정된 쿨타임에 도달하면 유닛 생성
        if (timer >= currentUnit.spawnCooldown)
        {
            SpawnUnit(currentUnit);
            timer = 0f; // 타이머 초기화 후 다음 유닛 생산 대기
        }
    }

    /// <summary>
    /// 팝업창 등에서 유저가 다른 슬롯을 선택했을 때 호출할 함수
    /// </summary>
    public void SelectSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= unitSlots.Length) return;

        currentSelectedSlot = slotIndex;
        timer = 0f; // ⭐ 핵심: 유닛을 바꾸는 순간 타이머를 즉시 초기화하고 새로 셈!
        
        Debug.Log($"[병영] 슬롯 변경됨 -> {slotIndex}번 슬롯 (유닛: {unitSlots[slotIndex].unitName}, 쿨타임 리셋)");
    }

    /// <summary>
    /// 실제 유닛을 생성하는 내부 함수
    /// </summary>
    private void SpawnUnit(UnitSlotData unitData)
    {
        // TODO: 유닛 생성(Instantiate) 및 출구로 내보내는 로직 구현
        Debug.Log($"[병영] 유닛 생산 완료! -> {unitData.unitName} 소환됨!");
    }


}