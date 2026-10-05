using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class PlayerSessionData
{   
    [Header("현재 있는 영웅/유닛/아이템 데이터")]
    public List<HeroRuntimeData> heroes;
    public List<UnitRuntimeData> units;
    public List<ItemRuntimeData> items;

    [Header("세력정보")]
    public DomainType domainType; 
    public DomainData domainData;


    public PlayerSessionData()
    {
        heroes = new List<HeroRuntimeData>();
        units = new List<UnitRuntimeData>();
        items = new List<ItemRuntimeData>();
    }

    // 데이터 추가용 편의 메서드
    public void AddHero(HeroData hero)
    {
        if (hero == null)
            return;

        foreach (var result in heroes)
        {
            if (result.heroData != null &&
                result.heroData.Hero_Id == hero.Hero_Id)
            {
                return;
            }
        }

        heroes.Add(new HeroRuntimeData
        {
            heroData = hero,
            level = 1
        });
    }

    public void AddUnit(UnitData unit)
    {
        if (unit == null)
            return;

        foreach (var result in units)
        {
            if (result.unitData != null &&
                result.unitData.Unit_Id == unit.Unit_Id)
            {
                return;
            }
        }

        units.Add(new UnitRuntimeData
        {
            unitData = unit,
            level = 1
        });
    }
    public void AddItem(ItemData item)
    {
        if (item == null)
            return;

        foreach (var result in items)
        {
            if (result.itemData != null &&
                result.itemData.item_Id == item.item_Id)
            {
                result.size++;
                return;
            }
        }

        items.Add(new ItemRuntimeData
        {
            itemData = item,
            size = 1
        });
    }
}