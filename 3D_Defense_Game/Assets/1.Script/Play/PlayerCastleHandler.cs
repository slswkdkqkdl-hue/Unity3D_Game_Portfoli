using System.Collections.Generic;
using UnityEngine;

public class PlayerCastleHandler : MonoBehaviour
{
    public BuildingSlotManager buildingSlotManager;

    [SerializeField] private Transform MainCastlePos;
    [SerializeField] private Team team = Team.Ally;
    
    public void Init()
    {
        if (BuildingDataManager.I == null)
        {
            Debug.LogError("BuildingDataManager 인스턴스가 씬에 존재하지 않습니다!");
            return;
        }

        List<BuildingDataSO> result = BuildingDataManager.I.GetAvailableBuildings();

        foreach (BuildingDataSO data in result)
        {
            if (data.buildingType == BuildingType.MainCastle)
            {
                // 1. 성 프리팹 생성
                GameObject castleObj = Instantiate(data.buildingPrefab, MainCastlePos);
                
                // 2. MainCastle 컴포넌트 가져오기
                MainCastle castle = castleObj.GetComponent<MainCastle>();
                
                if (castle != null)
                {
                    // 3. 성 초기화 
                    castle.Init(data, team);
                }
            }
        }
    }
}