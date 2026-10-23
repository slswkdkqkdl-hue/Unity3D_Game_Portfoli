using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{   

    public static BuildingManager I { get; private set; }
    
    // 현재 플레이어가 클릭해서 선택한 건설 부지 (핵심 데이터!)
    private BuildingPlot selectedPlot;

    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 플레인이 클릭되었을 때 매니저가 호출받아서 선택된 땅을 저장함
    /// </summary>
    public void SetSelectedPlot(BuildingPlot plot)
    {
        selectedPlot = plot;
        Debug.Log("BuildingManager: 선택된 땅을 기억했습니다. 이제 하단 슬롯에서 건물을 고르세요!");
    }
    

    /// <summary>
    /// 건물 슬롯창을 클릭했을 때 호출됨
    /// </summary>
    public void OnClickBuildingSlot(BuildingDataSO slotData)
    {
        // 1. 선택된 땅이 아예 없는 경우
        if (selectedPlot == null)
        {
            Debug.LogWarning("먼저 필드의 회색 플레인(땅)을 클릭해주세요!");
            return;
        }

        // 2. 이미 건물이 지어진 땅인 경우 (이중 안전장치)
        if (selectedPlot.isBuilding)
        {
            Debug.LogWarning("이미 건물이 있는 땅입니다!");
            return;
        }

        // 3. [골드 체크 및 차감]
        if (ResourceManager.I != null)
        {
            // 내 돈이 건설 비용보다 적다면?
            if (ResourceManager.I.Money < slotData.buildCost)
            {
                InGameLog.I.AddLog($"골드가 부족합니다! [{slotData.buildingName}] 건설에 {slotData.buildCost}G가 필요합니다.");
                return;
            }
            else
            {
                InGameLog.I.AddLog($"{slotData.buildingName} 건설 완료 ");
            }

            // 돈이 충분하므로 비용만큼 차감
            ResourceManager.I.ModifyMoney(-slotData.buildCost);   
        }

        // 3. 조건 만족! 기억하고 있던 그 땅(`selectedPlot`)에게 건물을 지으라고 명령함
        selectedPlot.SpawnBuilding(slotData);

        // 4. 건설이 끝났으니 선택된 땅 초기화 (다시 땅을 누를 때까지 대기)
        selectedPlot = null;
    }

}