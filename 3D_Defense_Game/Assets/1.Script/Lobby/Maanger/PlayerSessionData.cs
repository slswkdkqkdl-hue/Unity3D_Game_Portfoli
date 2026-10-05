using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSessionData
{
    public List<RuntimeHeroData> runtimeHeroDatas;
    public List<RuntimeUnitData> runtimeUnits;
    public List<RuntimeItemData> runtimeItemDatas;



    public PlayerSessionData()
    {
        runtimeHeroDatas = new List<RuntimeHeroData>();
        runtimeUnits = new List<RuntimeUnitData>();
        runtimeItemDatas = new List<RuntimeItemData>();
    }
    // 데이터 추가용 편의 메서드 (DataManager에서 사용)
    public void AddHero(RuntimeHeroData hero) { if (!runtimeHeroDatas.Contains(hero)) runtimeHeroDatas.Add(hero); }
    public void AddUnit(RuntimeUnitData unit) { if (!runtimeUnits.Contains(unit)) runtimeUnits.Add(unit); }
    public void AddItem(RuntimeItemData item) { if (!runtimeItemDatas.Contains(item)) runtimeItemDatas.Add(item); }

}