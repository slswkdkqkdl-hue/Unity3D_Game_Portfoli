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
                GameObject caslte = Instantiate(data.buildingPrefab, MainCastlePos);
                caslte.GetComponent<Building>().Init(data, team);
            }
        }
    }

}
