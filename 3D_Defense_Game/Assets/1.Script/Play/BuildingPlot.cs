using UnityEngine;

public class BuildingPlot : MonoBehaviour
{
    public bool isBuilding = false; // 이미 건물이 지어졌는지 여부

    // 마우스나 레이캐스트로 이 플레인을 클릭했을 때 호출
    private void OnMouseDown()
    {
        if (isBuilding) return; // 이미 건물이 있다면 무시

        Debug.Log("건설 부지 선택됨!");

        // BuildingManager에게 "내가 선택되었어!"라고 자신을 넘겨줌
        BuildingManager.I.SetSelectedPlot(this);

        
    }

    // 슬롯을 눌렀을 때, 매니저를 통해 최종적으로 건물이 생성되는 함수
    public void SpawnBuilding(BuildingDataSO data)
    {
        if (data == null || data.buildingPrefab == null) return;

        isBuilding = true; // 이제 빈 땅이 아님

        // 내 위치(transform.position)에 3D 건물 프리팹 생성
        GameObject buildingObj = Instantiate(data.buildingPrefab, transform.position, transform.rotation);

        // 생성된 건물에 데이터 주입
        // Building building = buildingObj.GetComponent<Building>();
        // if (building != null)
        // {
        //     building.Init(data);
        // }

        Debug.Log($"{data.buildingName} 건설 완료!");
    }
}