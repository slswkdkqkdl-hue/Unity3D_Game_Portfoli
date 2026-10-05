using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSessionData
{
    [SerializeField] private List<RuntimeHeroData> runtimeHeroDatas;
    [SerializeField] private List<RuntimeUnitData> runtimeUnits;
    [SerializeField] private List<RuntimeItemData> runtimeItemDatas;

    // --- 외부 읽기용 프로퍼티 (SyncSessionToSaveData 등에서 사용) ---
    public IReadOnlyList<RuntimeHeroData> HeroDatas => runtimeHeroDatas;
    public IReadOnlyList<RuntimeUnitData> UnitDatas => runtimeUnits;
    public IReadOnlyList<RuntimeItemData> ItemDatas => runtimeItemDatas;

    public PlayerSessionData()
    {
        runtimeHeroDatas = new List<RuntimeHeroData>();
        runtimeUnits = new List<RuntimeUnitData>();
        runtimeItemDatas = new List<RuntimeItemData>();
    }

    // ==========================================
    // --- 1. 영웅 데이터 관리 ---
    // ==========================================
    public void AddHero(RuntimeHeroData hero)
    {
        if (hero != null && !runtimeHeroDatas.Contains(hero))
        {
            runtimeHeroDatas.Add(hero);
        }
    }

    // 영웅 삭제
    public bool RemoveHero(RuntimeHeroData hero)
    {
        return runtimeHeroDatas.Remove(hero);
    }

    // ID로 영웅 삭제
    public bool RemoveHeroById(int heroId)
    {
        RuntimeHeroData target = runtimeHeroDatas.Find(x => x.Hero_Id == heroId);
        if (target != null)
        {
            return runtimeHeroDatas.Remove(target);
        }
        return false;
    }


    // ==========================================
    // --- 2. 유닛 데이터 관리 ---
    // ==========================================
    public void AddUnit(RuntimeUnitData unit)
    {
        if (unit != null && !runtimeUnits.Contains(unit))
        {
            runtimeUnits.Add(unit);
        }
    }

    // 유닛 삭제
    public bool RemoveUnit(RuntimeUnitData unit)
    {
        return runtimeUnits.Remove(unit);
    }

    // ID로 유닛 삭제
    public bool RemoveUnitById(int unitId)
    {
        RuntimeUnitData target = runtimeUnits.Find(x => x.Unit_Id == unitId);
        if (target != null)
        {
            return runtimeUnits.Remove(target);
        }
        return false;
    }


    // ==========================================
    // --- 3. 아이템 데이터 관리 ---
    // ==========================================
    public void AddItem(RuntimeItemData item)
    {
        if (item != null)
        {
            runtimeItemDatas.Add(item);
        }
    }

    // 아이템 삭제 (가장 먼저 검색되는 1개만 제거)
    public bool RemoveItem(RuntimeItemData item)
    {
        return runtimeItemDatas.Remove(item);
    }

    // ID로 아이템 삭제
    public bool RemoveItemById(int itemId)
    {
        RuntimeItemData target = runtimeItemDatas.Find(x => x.item_Id == itemId);
        if (target != null)
        {
            return runtimeItemDatas.Remove(target);
        }
        return false;
    }
}