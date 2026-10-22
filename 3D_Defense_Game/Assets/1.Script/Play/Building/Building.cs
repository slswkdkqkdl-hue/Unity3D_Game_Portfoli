using UnityEngine;

public class Building : MonoBehaviour
{
    protected BuildingDataSO buildingData;

    // 공통 초기화 함수
    public virtual void Init(BuildingDataSO data)
    {
        buildingData = data;
        Debug.Log("데이터주입완료2");
    }

    // 공통 클릭 감지 (건물을 누르면 매니저에게 팝업 오픈 요청)
    protected virtual void OnMouseDown()
    {
        if (buildingData == null) return;

        Debug.Log($"공통 건물 클릭: {buildingData.buildingName}");
        PopupManager.I.OpenBuildingPopup(buildingData);
    }
}