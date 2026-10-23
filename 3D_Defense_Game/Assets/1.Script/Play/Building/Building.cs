using UnityEngine;

public enum Team
{
    Ally,   // 아군
    Enemy   // 적군
}
// 건물 작동 방식을 구분하는 Enum
public enum BuildingActivationType
{
    Passive,    // 지속형: 가만히 두어도 자동으로 효과가 발동하는 건물 (예: 자동 자원 생산, 지속 버프 타워 등)
    Active,     // 발동형: 플레이어가 클릭 후 UI/팝업창에서 조작해야 실행되는 건물 (예: 상점, 용병 고용소 등)
    Hybrid      // 혼합형: 패시브 효과와 액티브(클릭 조작) 기능이 모두 있는 건물
}

public class Building : MonoBehaviour
{
    [SerializeField]protected BuildingDataSO buildingData;
    [SerializeField]protected Team currentTeam;
    [SerializeField]protected BuildingActivationType buildingActivationType;
    // 공통 초기화 함수
    public virtual void Init(BuildingDataSO data, Team team = Team.Ally)
    {
        buildingData = data;
        currentTeam = team;
    }

    // 공통 클릭 감지 (건물을 누르면 매니저에게 팝업 오픈 요청)
    protected virtual void OnMouseDown()
    {
        if (buildingData == null) return;

        Debug.Log($"공통 건물 클릭: {buildingData.buildingName}");
        PopupManager.I.OpenBuildingPopup(buildingData);
    }
}