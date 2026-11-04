using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager I { get; private set; }

    [Header("1. 고정 패널 (적/대상 정보용)")]
    [SerializeField] private TargetInfoPanel targetInfoPanel; 

    [Header("2. 건물 툴팁 패널 (마우스 오버용)")]
    [SerializeField] private BuildingTooltipUI buildingTooltipUI;

    [Header("3. 동적 건물 팝업 설정 (클릭용)")]
    [SerializeField] private Transform popupParent;
    private Dictionary<int, GameObject> buildingPopupDict = new Dictionary<int, GameObject>();
    private GameObject currentBuildingPopup;

    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        CloseBuildingPopup();
        CloseTargetInfoPopup();
        CloseBuildingTooltip();
    }


    /// <summary>
    /// 현재 건물 상세정보 팝업창 닫기 
    /// </summary>
    public void CloseBuildingPopup()
    {
        if (currentBuildingPopup != null)
        {
            currentBuildingPopup.SetActive(false);
            currentBuildingPopup = null;
        }
    }

    /// <summary>
    /// (영웅 , 유닛) 팝업창 닫기 
    /// </summary>
    public void CloseTargetInfoPopup()
    {
        if (targetInfoPanel != null)
        {
            targetInfoPanel.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 건물 툴팁창 닫기 
    /// </summary>
    public void CloseBuildingTooltip()
    {
        if (buildingTooltipUI != null)
        {
            buildingTooltipUI.ClearAndHide();
        }
    }


    /// <summary>
    /// [건물 클릭 시 실행] 건물 상세정보 팝업 열기 
    /// </summary>
    public void OpenBuildingPopup(Building targetBuilding)
    {
        if (targetBuilding == null || targetBuilding.Data == null) return;

        BuildingDataSO buildingData = targetBuilding.Data;

        // 다른 메인 팝업들은 닫기
        CloseBuildingPopup();
        CloseTargetInfoPopup();

        if (!buildingPopupDict.TryGetValue(buildingData.buildingId, out GameObject popupObj))
        {
            if (buildingData.popupUIPrefab != null)
            {
                popupObj = Instantiate(buildingData.popupUIPrefab, popupParent);
                buildingPopupDict.Add(buildingData.buildingId, popupObj);
            }
            else
            {
                Debug.LogWarning($"{buildingData.buildingName}의 팝업 프리팹이 없습니다!");
                return;
            }
        }

        popupObj.SetActive(true);
        currentBuildingPopup = popupObj;

        BaseBuildingPopupUI popupScript = popupObj.GetComponent<BaseBuildingPopupUI>();
        if (popupScript != null)
        {
            popupScript.Setup(targetBuilding);
        }

        Debug.Log($"[PopupManager] 건물 상세 팝업 오픈: {buildingData.buildingName}");
    }
    /// <summary>
    /// [건물 아이콘에 마우스를 올렸을 때 실행] 건물 툴팁 창 열기 
    /// </summary>
    public void OpenBuildingTooltip(BuildingDataSO data)
    {
        if (data == null) return;

        if (buildingTooltipUI != null)
        {
            buildingTooltipUI.SetupTooltip(data.buildingName, data.buildCost);
        }
    }
    /// <summary>
    /// [적 또는 영웅 클릭 시 실행] 캐릭터 정보 팝업 열기 
    /// </summary>
    public void OpenTargetInfoPopup(ICombatEntity targetEntity)
    {
        CloseBuildingPopup();
        CloseTargetInfoPopup();

        if (targetInfoPanel != null)
        {
            targetInfoPanel.gameObject.SetActive(true);
            targetInfoPanel.Init(targetEntity);
            
            Debug.Log("[PopupManager] TargetInfoPanel 오픈 완료!");
        }
    }

}