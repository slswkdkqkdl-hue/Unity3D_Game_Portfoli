using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSessionData
{   
    [Header("현재 데이터")]
    [SerializeField] public List<int> clearid;
    [SerializeField] public List<HeroRuntimeData> heroes;
    [SerializeField] public List<UnitRuntimeData> units;
    [SerializeField] public List<ItemRuntimeData> items;    
    [Header("세력정보")]
    public DomainData domainData;
    
    public PlayerSessionData()
    {
        heroes = new List<HeroRuntimeData>();
        units = new List<UnitRuntimeData>();
        items = new List<ItemRuntimeData>();
        clearid = new List<int>();
    }



    public void AddHero(HeroData hero)
    {
        if(hero == null || heroes.Exists(x => x.heroData.Hero_Id == hero.Hero_Id)) return;
        heroes.Add(new HeroRuntimeData { heroData = hero, level = 1 });
    }

    public void AddUnit(UnitData unit)
    {
        if (unit == null || units.Exists(u => u.unitData?.Unit_Id == unit.Unit_Id)) return;
        units.Add(new UnitRuntimeData { unitData = unit, level = 1 });
    }

    public void AddItem(ItemData item)
    {
        if (item == null) return;

        var existing = items.Find(i => i.itemData?.item_Id == item.item_Id);
        
        if (existing != null) existing.size++;
        else items.Add(new ItemRuntimeData { itemData = item, size = 1 });
    }

    public void AddClearId(int id)
    {
        if (clearid.Contains(id))
            return;
        clearid.Add(id);
    }

}