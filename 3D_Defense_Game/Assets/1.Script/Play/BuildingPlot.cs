using UnityEngine;

public class BuildingPlot : MonoBehaviour
{
    public bool isBuilding = false; // 이미 건물이 지어졌는지 여부

    private Collider plotCollider; // 플레인 자신의 콜라이더 캐싱용

    private void Awake()
    {
        plotCollider = GetComponent<Collider>();
    }

    // 마우스나 레이캐스트로 이 플레인을 클릭했을 때 호출
    private void OnMouseDown()
    {
        if (isBuilding) return; // 이미 건물이 있다면 무시

        Debug.Log("건설 부지 선택됨!");
        BuildingManager.I.SetSelectedPlot(this);
    }

    // 슬롯을 눌렀을 때, 매니저를 통해 최종적으로 건물이 생성되는 함수
    public void SpawnBuilding(BuildingDataSO data)
    {
        if (data == null || data.buildingPrefab == null) return;

        isBuilding = true; // 이제 빈 땅이 아님


        GameObject buildingObj = Instantiate(data.buildingPrefab, transform.position, transform.rotation);
        buildingObj.transform.SetParent(transform);

        // 2. 생성된 건물에 BuildingDataSO 데이터 주입 
        Building building = buildingObj.GetComponent<Building>();
        if (building != null)
        {
            building.Init(data);
        }

        if (plotCollider != null)
        {
            plotCollider.enabled = false;
        }

        Debug.Log($"{data.buildingName} 건설 완료! (플레인 콜라이더 비활성화)");
    }
}