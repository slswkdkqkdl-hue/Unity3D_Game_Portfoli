using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager I { get; private set; }

    [Header("씬에 미리 배치되어 있는 고정 패널 (적/대상 정보용)")]
    [SerializeField] private TargetInfoPanel targetInfoPanel; 

    [Header("건물 팝업이 생성될 부모 Transform")]
    [SerializeField] private Transform popupParent;

    // 건물 ID를 키로 하는 팝업 딕셔너리 (캐싱용)
    private Dictionary<int, GameObject> buildingPopupDict = new Dictionary<int, GameObject>();
    
    // 현재 화면에 켜져 있는 동적 건물 팝업 추적용
    private GameObject currentBuildingPopup;

    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        CloseAllPopups();
    }

    /// <summary>
    /// 모든 팝업(건물 팝업 + 타겟 정보 패널)을 한 번에 싹 닫는 공통 함수
    /// </summary>
    public void CloseAllPopups()
    {
        // 1. 동적 건물 팝업 끄기 (파괴하지 않고 비활성화로 캐싱 유지)
        if (currentBuildingPopup != null)
        {
            currentBuildingPopup.SetActive(false);
            currentBuildingPopup = null;
        }

        // 2. 고정 타겟 정보 패널 끄기
        if (targetInfoPanel != null)
        {
            targetInfoPanel.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// [건물 클릭 시 호출] 건물 데이터를 받아 팝업을 띄움
    /// </summary>
    public void OpenBuildingPopup(BuildingDataSO buildingData)
    {
        if (buildingData == null) return;

        // 1. 먼저 열려있는 모든 팝업 닫기
        CloseAllPopups();

        // 2. 딕셔너리에 이 건물의 ID에 해당하는 팝업이 있는지 확인
        if (!buildingPopupDict.TryGetValue(buildingData.buildingId, out GameObject popupObj))
        {
            // 없으면 새로 생성 후 딕셔너리에 등록
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

        // 3. 팝업 활성화 및 데이터 주입
        popupObj.SetActive(true);
        currentBuildingPopup = popupObj;

        // 예시: 팝업 내부 스크립트 가져와서 데이터 세팅
        // var popupScript = popupObj.GetComponent<BaseBuildingPopupUI>();
        // if (popupScript != null) popupScript.Setup(buildingData);

        Debug.Log($"[PopupManager] 건물 팝업 오픈: {buildingData.buildingName}");
    }

    /// <summary>
    /// [적/영웅 클릭 시 호출] 적 데이터를 받아 TargetInfoPanel을 켜고 데이터 주입
    /// (추후 EnemyDataSO 등 원하는 데이터 타입으로 매개변수 변경 가능)
    /// </summary>
    public void OpenTargetInfoPopup(/* EnemyDataSO enemyData */)
    {
        // 1. 먼저 열려있는 모든 팝업 닫기
        CloseAllPopups();

        // 2. 씬에 있는 TargetInfoPanel 켜기
        if (targetInfoPanel != null)
        {
            targetInfoPanel.gameObject.SetActive(true);
            
            // 데이터 주입 (나중에 적 데이터 스크립트 만들면 연동)
            // targetInfoPanel.Init(enemyData);
            
            Debug.Log("[PopupManager] TargetInfoPanel 오픈 완료!");
        }
    }
}