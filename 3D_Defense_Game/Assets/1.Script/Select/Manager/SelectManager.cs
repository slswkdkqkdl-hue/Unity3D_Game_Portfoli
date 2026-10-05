using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{   
    [Header("영지 데이터 넣는곳")]
    public List<TerritoryData> Data = new List<TerritoryData>();

    void Awake()
    {
        InitTerritoryData();
    }
    void Start()
    {
        UIManager_Select.I.Init(Data);
    }
    private void OnDestroy()
    {   

        Debug.Log("메모리해재");
        ReleaseTerritoryData();
    }

    public void InitTerritoryData()
    {
        foreach (TerritoryData data in Data)
        {   
            // 1. 영웅 데이터 로드 (인스펙터에서 지정한 heroRef 사용)
            if (data.heroRef != null && data.heroRef.RuntimeKeyIsValid())
            {
                data.heroDescDatas = data.heroRef.LoadAssetAsync().WaitForCompletion();
            }

            // 2. 유닛 데이터 로드 (인스펙터에서 지정한 unitRefs 사용)
            data.unitDescDatas.Clear();
            foreach (var unitRef in data.unitRefs)
            {
                if (unitRef != null && unitRef.RuntimeKeyIsValid())
                {
                    RuntimeUnitData loadedUnit = unitRef.LoadAssetAsync().WaitForCompletion();
                    data.unitDescDatas.Add(loadedUnit);
                }
            }
        }
    }
    public void ReleaseTerritoryData()
    {
        foreach (TerritoryData data in Data)
        {
            if (data == null) continue;

            // 1. 영웅 에셋 메모리 해제
            if (data.heroRef != null && data.heroRef.IsValid())
            {
                data.heroRef.ReleaseAsset();
                data.heroDescDatas = null;
            }

            // 2. 유닛 에셋 메모리 해제
            if (data.unitRefs != null)
            {
                foreach (var unitRef in data.unitRefs)
                {
                    if (unitRef != null && unitRef.IsValid())
                    {
                        unitRef.ReleaseAsset();
                    }
                }
                data.unitDescDatas.Clear();
            }
        }
    }
}