using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barracks : Building
{
    [Header("생산 가능한 유닛 데이터 리스트")]
    [SerializeField] private List<UnitData> unitDatas;

    private int currentSelectedSlot = 0; // 현재 선택된 슬롯 번호
    private float timer = 0f;            // 생산 쿨타임 타이머

    public int CurrentSelectedSlot => currentSelectedSlot;
    public List<UnitData> UnitDatas => unitDatas;

    private MainCastle castle;

    [Header("쿨타임 UI")]
    [SerializeField] private Image fillImage;


    public override void Init(BuildingDataSO data, Team team = Team.Ally)
    {
        base.Init(data, team);
        buildingActivationType = BuildingActivationType.Passive;
        unitDatas = new List<UnitData>(BuildingManager.I.unitDatas);

        if (MainCastle.I != null)
        {
            castle = MainCastle.I;
        }

        // 게임 시작 시 기본 0번 슬롯 선택 상태로 시작
        if (unitDatas.Count > 0)
        {
            SelectSlot(0);
        }
    }

    private void Update()
    {
        if (buildingData == null) return;
        if (unitDatas == null || unitDatas.Count == 0) return;

        // 현재 선택된 유닛 데이터 가져오기
        var currentUnit = unitDatas[currentSelectedSlot];
        if (currentUnit == null) return;

        // 쿨타임 타이머 누적
        timer += Time.deltaTime;
        fillImage.fillAmount = Mathf.Clamp01(timer / currentUnit.CoolTime);

        // 설정된 쿨타임에 도달하면 유닛 생성
        if (timer >= currentUnit.CoolTime)
        {
            SpawnUnit(currentUnit);
            timer = 0f; // 타이머 초기화
        }
    }

    /// <summary>
    /// 유저가 다른 슬롯을 선택했을 때 호출
    /// </summary>
    public void SelectSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= unitDatas.Count) return;

        currentSelectedSlot = slotIndex;
        timer = 0f; // 슬롯이 바뀌면 쿨타임 초기화
    }

    private void SpawnUnit(UnitData unitData)
    {
        // TODO: 유닛 생성(Instantiate) 및 출구로 내보내는 로직 구현
        Debug.Log($"[병영] 유닛 생산 완료! -> {unitData.Unit_Name} 소환됨!");
    }
}