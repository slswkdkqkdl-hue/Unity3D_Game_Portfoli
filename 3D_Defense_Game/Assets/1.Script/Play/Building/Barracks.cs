using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barracks : Building
{
    [Header("생산 가능한 유닛 데이터 리스트")]
    [SerializeField] private List<UnitData> unitDatas;

    private int currentSelectedSlot = 0; // 현재 선택된 슬롯 번호
    private float timer = 0f;            // 생산 쿨타임 타이머
    private bool hasLoggedPopulationWarning = false;
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
                if (ResourceManager.I.TryAddPopulation(currentUnit.population))
                { 
                    // 인구수 확보 성공 -> 유닛 소환 및 타이머 초기화
                    SpawnUnit(currentUnit);
                    timer = 0f;
                    
                    // 인구수가 해결되었으니 경고 플래그도 초기화
                    hasLoggedPopulationWarning = false;
                }
                else
                {
                    // 인구수 부족 실패 처리 (딱 한 번만 로그 출력)
                    if (!hasLoggedPopulationWarning)
                    {
                        hasLoggedPopulationWarning = true;
                        string warningMsg = $"인구수가 부족하여 [{currentUnit.Unit_Name}]을(를) 생산할 수 없습니다!";
                        if (InGameLog.I != null)
                        {
                            InGameLog.I.AddLog(warningMsg);
                        }
                    }
                    timer = currentUnit.CoolTime; 
                }
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
        if (castle != null)
        {   
            Transform point = castle.RandomPoint();
            Team team = castle.PlayerTeam;
            if (point == null)
            {
                Debug.LogWarning("[병영] 성의 출구(ExitPoint)가 없어 유닛 소환이 취소되었습니다.");
                if (ResourceManager.I != null)
                {
                    ResourceManager.I.DecreasePopulation(unitData.population);
                }
                return; 
            }
            UnitBase unit = UnitPoolManager.I.GetUnit(unitData, point.position, point.rotation);

            if (unit != null)
            {
                unit.Init(unitData,team);
            }
            else
            {
                Debug.LogWarning($"[병영] 유닛 소환 실패! -> {unitData.Unit_Name}");
            }
        }

        Debug.Log($"[병영] 유닛 생산 완료! -> {unitData.Unit_Name} 소환됨!");
    }
}