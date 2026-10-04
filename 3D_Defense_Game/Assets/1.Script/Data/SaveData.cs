using System.Collections.Generic;
using UnityEngine;




[System.Serializable]
public class SaveData
{   
    public float playTime = 0f;// 플레이 타임

    public DomainType domainType = DomainType.None; // 선택한 영지 종류
    public DomainData nation = new DomainData(); //영지 정보 

    public List<int> clearedStageIDs = new List<int>(); // 클리어한 스테이지 정보

    public List<HeroSaveData> heorDatas = new List<HeroSaveData>(); // 소유한 영웅
    public List<UnitSaveData> unitDatas = new List<UnitSaveData>(); // 소유한 유닛
    public List<ItemSaveData> itemDatas = new List<ItemSaveData>(); // 소유한 아이템


    public List<ItemSaveData> StartingItems = new List<ItemSaveData>()
    {
        new ItemSaveData { ItemId = 1, Size = 1 }, // 시작 무기
        new ItemSaveData { ItemId = 1, Size = 1 } // 시작 방어구
    };
}
