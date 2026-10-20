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
        
        // (선택사항) 이때 하단 건물 슬롯 UI 패널을 화면에 켜주면 됨!
    }
    

    /// <summary>
    /// 하단 슬롯을 클릭했을 때 호출됨
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

        // TODO: 골드(돈) 부족한지 여기서 체크!
        // if (PlayerGold.Instance.Gold < slotData.buildCost) { Debug.Log("골드가 부족합니다!"); return; }

        // 3. 조건 만족! 기억하고 있던 그 땅(`selectedPlot`)에게 건물을 지으라고 명령함
        selectedPlot.SpawnBuilding(slotData);

        // 4. 건설이 끝났으니 선택된 땅 초기화 (다시 땅을 누를 때까지 대기)
        selectedPlot = null;
    }
}