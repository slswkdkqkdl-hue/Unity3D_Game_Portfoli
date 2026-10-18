using System.Collections.Generic;
using UnityEngine;

public class BuildingDataManager : MonoBehaviour
{
    public static BuildingDataManager Instance { get; private set; }

    [Header("기본 건물 데이터 목록 (기본 4종 건물)")]
    [SerializeField] private List<BuildingDataSO> defaultBuildingList = new List<BuildingDataSO>();

    [Header("특수 건물 전체 맵/데이터베이스")]
    [SerializeField] private List<BuildingDataSO> specialBuildingList = new List<BuildingDataSO>();

    // 빠른 검색용 Dictionary
    private Dictionary<int, BuildingDataSO> buildingDataDict = new Dictionary<int, BuildingDataSO>();

    private void Awake()
    {
        Instance = this;
        InitDictionary();
    }

    private void InitDictionary()
    {
        foreach (var data in defaultBuildingList)
        {
            if (data != null && !buildingDataDict.ContainsKey(data.buildingId))
                buildingDataDict.Add(data.buildingId, data);
        }

        foreach (var data in specialBuildingList)
        {
            if (data != null && !buildingDataDict.ContainsKey(data.buildingId))
                buildingDataDict.Add(data.buildingId, data);
        }
    }

    /// <summary>
    /// 현재 건설 창에 표시할 사용 가능한 건물 목록 반환
    /// </summary>
    public List<BuildingDataSO> GetAvailableBuildings()
    {   


        // 기본 건물 + 현재 세션/영지에 맞춰 해금된 특수 건물을 합쳐서 전달
        return defaultBuildingList;
    }

    public BuildingDataSO GetBuildingData(int buildingId)
    {
        buildingDataDict.TryGetValue(buildingId, out var data);
        return data;
    }
}