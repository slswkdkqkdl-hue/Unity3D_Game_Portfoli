using System.Collections.Generic;
using UnityEngine;




[System.Serializable]
public class SaveData
{   
    public float playTime = 0f;// 플레이 타임
    public float Np = 0f; // 소유한 국가 포인트
    public DomainData nation = new DomainData(); //영지 정보 

    public List<int> clearedStageIDs = new List<int>(); // 클리어한 스테이지 정보

    public List<HeorData> heorDatas = new List<HeorData>(); // 소유한 영웅
    public List<UnitData> unitDatas = new List<UnitData>(); // 소유한 유닛
    public List<ItemData> itemDatas = new List<ItemData>(); // 소유한 아이템


    public List<ItemData> StartingItems = new List<ItemData>()
    {
        new ItemData { ItemId = 1, Size = 1 }, // 시작 무기
        new ItemData { ItemId = 1, Size = 1 } // 시작 방어구
    };
}
