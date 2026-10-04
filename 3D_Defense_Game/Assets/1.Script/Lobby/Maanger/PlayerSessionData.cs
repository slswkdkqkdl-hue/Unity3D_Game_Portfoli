using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSessionData
{
    // 외부 접근을 위해 public 리스트로 선언하거나 프로퍼티 사용
    [SerializeField]private List<RuntimeHeroData> HeroDatas;
    [SerializeField]private List<RuntimeUnitData> RuntimeUnits;
    [SerializeField]private List<RuntimeItemData> ItemDatas;

    public PlayerSessionData()
    {
        HeroDatas = new List<RuntimeHeroData>();
        RuntimeUnits = new List<RuntimeUnitData>();
        ItemDatas = new List<RuntimeItemData>();
    }

    // --- 데이터 추가용 함수들 ---

    // 1. 영웅 데이터 추가
    public void AddHero(RuntimeHeroData hero)
    {
        if (hero != null && !HeroDatas.Contains(hero))
        {
            HeroDatas.Add(hero);
        }
    }

    // 2. 유닛 데이터 추가
    public void AddUnit(RuntimeUnitData unit)
    {
        if (unit != null && !RuntimeUnits.Contains(unit))
        {
            RuntimeUnits.Add(unit);
        }
    }

    // 3. 아이템 데이터 추가
    public void AddItem(RuntimeItemData item)
    {
        if (item != null)
        {
            ItemDatas.Add(item);
        }
    }
}