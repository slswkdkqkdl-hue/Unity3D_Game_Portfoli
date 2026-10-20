using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{   
    [Header("팝업이 생성될 위치")]
    [SerializeField] private Transform popupParent;

    private Dictionary<int, GameObject> popupDictionary = new Dictionary<int, GameObject>();
    private GameObject currentOpenPopup;

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

        // TODO: 골드(돈) 부족한지 여기서 체크!
        // if (PlayerGold.Instance.Gold < slotData.buildCost) { Debug.Log("골드가 부족합니다!"); return; }

        // 3. 조건 만족! 기억하고 있던 그 땅(`selectedPlot`)에게 건물을 지으라고 명령함
        selectedPlot.SpawnBuilding(slotData);

        // 4. 건설이 끝났으니 선택된 땅 초기화 (다시 땅을 누를 때까지 대기)
        selectedPlot = null;
    }

    /// <summary>
    /// 필드에 지어진 건물을 클릭했을 때 
    /// </summary>
    public void OpenBuildingPopup(BuildingDataSO buildingData)
    {
        if (buildingData == null) return;

        // 1. 기존에 열려있는 팝업이 있다면 닫기
        if (currentOpenPopup != null)
        {
            currentOpenPopup.SetActive(false);
        }

        // 2. 딕셔너리에서 이 건물의 ID(buildingId)에 해당하는 팝업이 이미 만들어져 있는지 확인
        if (!popupDictionary.TryGetValue(buildingData.buildingId, out GameObject popupObj))
        {
            // 없다면? SO에 들어있는 팝업 프리팹을 그 순간에 생성!
            if (buildingData.popupUIPrefab != null)
            {
                popupObj = Instantiate(buildingData.popupUIPrefab, popupParent);
                
                // 딕셔너리에 이 ID의 팝업으로 등록 (캐싱)
                popupDictionary.Add(buildingData.buildingId, popupObj);
            }
            else
            {
                Debug.LogWarning($"{buildingData.buildingName} (ID: {buildingData.buildingId})에 팝업 프리팹이 없습니다!");
                return;
            }
        }

        // 3. 팝업 켜기
        popupObj.SetActive(true);
        currentOpenPopup = popupObj;

        // 4. (필요하다면) 팝업 내부 스크립트에 데이터 주입
        // var popupScript = popupObj.GetComponent<BaseBuildingPopupUI>();
        // if (popupScript != null) popupScript.Setup(buildingData);

        Debug.Log($"ID [{buildingData.buildingId}] {buildingData.buildingName} 팝업 오픈!");
    } 
}