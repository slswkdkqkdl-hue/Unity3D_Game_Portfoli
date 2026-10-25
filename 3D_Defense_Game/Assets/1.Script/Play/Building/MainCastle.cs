using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCastle : Building
{
    [Header("메인 성 보상 설정")]
    public string Castle_Name;
    [SerializeField] private int populationBonus;      // 건물 생성 시 증가할 최대 인구수
    [SerializeField] private int goldPerInterval;   // 주기마다 지급할 골드 양
    [SerializeField] private float goldInterval;    // 골드 지급 주기 (초 단위)

    public int PopulationBonus => populationBonus;
    public int GoldPerInterval => goldPerInterval;
    public float GoldInterval => goldInterval;

    public List<Building> buildings = new List<Building>();

    private float timer = 0f;    // 골드 생산 주기 체크용 타이머

    [Header("쿨타임 UI")]
    [SerializeField] private Image fillImage;
    
    public override void Init(BuildingDataSO data, Team team = Team.Ally)
    {
        base.Init(data, team);
        buildingActivationType = BuildingActivationType.Passive;

        // 아군일 경우, 생성 즉시 최대 인구수 증가
        if (currentTeam == Team.Ally && ResourceManager.I != null)
        {   
            ResourceManager.I.ChangeMaxPopulation(populationBonus);
        }
    }
    
    /// <summary>
    /// 다른 건물들이 생성될 때 자신을 이 리스트에 등록하는 함수
    /// </summary>
    public void RegisterBuilding(Building newBuilding)
    {
        if (newBuilding != null && !buildings.Contains(newBuilding))
        {
            buildings.Add(newBuilding);
            Debug.Log($"[MainCastle] 새 건물 등록됨: {newBuilding.Data.buildingName} (총 건물 수: {buildings.Count})");
        }
    }

    private void Update()
    {
        if (buildingData == null) return;

        timer += Time.deltaTime;

        // 2. 0부터 1까지의 비율(현재 시간 / 전체 주기)을 계산해서 UI 게이지에 반영
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Clamp01(timer / goldInterval);
        }

        if (timer >= goldInterval)
        {
            timer = 0f;

            // 주기적 골드 지급
            if (ResourceManager.I != null)
            {
                ResourceManager.I.ModifyMoney(goldPerInterval);
                Debug.Log($"[MainCastle] {goldInterval}초 경과: {goldPerInterval} 골드 획득!");
            }
        }
    }
}