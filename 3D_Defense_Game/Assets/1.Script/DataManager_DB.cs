using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
public partial class DataManager : MonoBehaviour
{
    // TODO: 추후 HeroDB, UnitDB, ItemDB도 Addressables로 관리
    //      DataManager에서는 AssetReferenceT<T>로 DB 참조를 보유하고,
    //      게임 시작 시 DB를 비동기로 로드한 뒤 세이브 데이터의 ID를 기준으로
    //      필요한 데이터를 로드하는 구조로 변경 예정.

    [Header("DB")]
    public HeroDB heroDB;
    public UnitDB unitDB;
    public ItemDB itemDB;


 // 세이브 데이터를 현재 세션 데이터로 복원
    public async Task BuildSessionFromSaveDataAsync()
    {
        if (sessionData == null || saveData == null)
            return;

        if (sessionData.heroes.Count > 0 ||
            sessionData.units.Count > 0 ||
            sessionData.items.Count > 0)
        {
            Debug.Log("[DataManager] 이미 세션 데이터가 메모리에 로드되어 있어 복원을 스킵합니다.");
            return;
        }

        // =========================
        // ① 영웅 복원
        // =========================
        if (saveData.heroDates != null && heroDB != null)
        {
            Debug.Log($"[DataManager] 영웅 복원 시작 (총 {saveData.heroDates.Count}개)");

            foreach (HeroSaveData heroSave in saveData.heroDates)
            {
                var heroRef = heroDB.GetHeroRef(heroSave.HeroId);

                if (heroRef == null)
                    continue;

                HeroData loadedHero = null;

                // 💡 이미 메모리에 로드되어 있다면 기존 Asset 재사용, 없으면 Addressables로 로드
                if (heroRef.IsValid())
                {
                    loadedHero = heroRef.Asset as HeroData;
                }
                else
                {
                    // heroRef.LoadAssetAsync() 대신 Addressables.LoadAssetAsync 사용
                    var handle = Addressables.LoadAssetAsync<HeroData>(heroRef);
                    loadedHero = await handle.Task;
                }

                if (loadedHero == null)
                    continue;

                HeroRuntimeData runtimeHero = new HeroRuntimeData
                {
                    heroData = loadedHero,
                    level = heroSave.Level,
                    sp = heroSave.SP,
                    equippedWeaponId = heroSave.Equippedweapon_Id,
                    equippedArmorId = heroSave.EquippedArmor_Id,
                    selectedPassiveSkill = new List<int>(heroSave.SelectPassiveSkill),
                    selectedActiveSkill = new List<int>(heroSave.SelectActiveSkill)
                };

                sessionData.heroes.Add(runtimeHero);
                Debug.Log($"[영웅 로드 완료] ID: {heroSave.HeroId}");
            }
        }

        // =========================
        // ② 유닛 복원
        // =========================
        if (saveData.unitDatas != null && unitDB != null)
        {
            Debug.Log($"[DataManager] 유닛 복원 시작 (총 {saveData.unitDatas.Count}개)");

            foreach (UnitSaveData unitSave in saveData.unitDatas)
            {
                var unitRef = unitDB.GetUnitRef(unitSave.UnitId);

                if (unitRef == null)
                    continue;

                UnitData loadedUnit = null;

                if (unitRef.IsValid())
                {
                    loadedUnit = unitRef.Asset as UnitData;
                }
                else
                {
                    var handle = Addressables.LoadAssetAsync<UnitData>(unitRef);
                    loadedUnit = await handle.Task;
                }

                if (loadedUnit == null)
                    continue;

                UnitRuntimeData runtimeUnit = new UnitRuntimeData
                {
                    unitData = loadedUnit,
                    level = unitSave.Level,
                    equippedWeaponId = unitSave.Equippedweapon_Id,
                    equippedArmorId = unitSave.EquippedArmor_Id
                };

                sessionData.units.Add(runtimeUnit);
                Debug.Log($"[유닛 로드 완료] ID: {unitSave.UnitId}");
            }

            sessionData.domainType = saveData.domainType;
            sessionData.domainData = saveData.nation;
        }

        // =========================
        // ③ 아이템 복원
        // =========================
        if (saveData.itemDatas != null && itemDB != null)
        {
            Debug.Log($"[DataManager] 아이템 복원 시작 (총 {saveData.itemDatas.Count}개)");

            foreach (ItemSaveData itemSave in saveData.itemDatas)
            {
                var itemRef = itemDB.GetItemRef(itemSave.ItemId);

                if (itemRef == null)
                    continue;

                ItemData loadedItem = null;

                if (itemRef.IsValid())
                {
                    loadedItem = itemRef.Asset as ItemData;
                }
                else
                {
                    var handle = Addressables.LoadAssetAsync<ItemData>(itemRef);
                    loadedItem = await handle.Task;
                }

                if (loadedItem == null)
                    continue;

                ItemRuntimeData runtimeItem = new ItemRuntimeData
                {
                    itemData = loadedItem,
                    size = itemSave.Size
                };

                sessionData.items.Add(runtimeItem);
                Debug.Log($"[아이템 로드 완료] ID: {itemSave.ItemId}, Size: {itemSave.Size}");
            }
        }

        Debug.Log("[DataManager] 모든 세션 데이터 복원 완료!");
    }


    // 현재 세션 데이터를 세이브 데이터로 변환
    public void SyncSessionToSaveData()
    {
        if (saveData == null)
            saveData = new SaveData();

        if (sessionData == null)
            return;


        saveData.heroDates.Clear();
        saveData.unitDatas.Clear();
        saveData.itemDatas.Clear();


        // =========================
        // ① 영웅
        // =========================
        foreach (HeroRuntimeData hero in sessionData.heroes)
        {
            if (hero == null || hero.heroData == null)
                continue;

            saveData.heroDates.Add(new HeroSaveData
            {
                HeroId = hero.heroData.Hero_Id,

                Level = hero.level,
                SP = hero.sp,

                Equippedweapon_Id = hero.equippedWeaponId,
                EquippedArmor_Id = hero.equippedArmorId,

                SelectPassiveSkill =
                    new List<int>(hero.selectedPassiveSkill),

                SelectActiveSkill =
                    new List<int>(hero.selectedActiveSkill)
            });
        }


        // =========================
        // ② 유닛
        // =========================
        foreach (UnitRuntimeData unit in sessionData.units)
        {
            if (unit == null || unit.unitData == null)
                continue;

            saveData.unitDatas.Add(new UnitSaveData
            {
                UnitId = unit.unitData.Unit_Id,

                Level = unit.level,

                Equippedweapon_Id = unit.equippedWeaponId,
                EquippedArmor_Id = unit.equippedArmorId
            });
        }


        // =========================
        // ③ 아이템
        // =========================
        foreach (ItemRuntimeData item in sessionData.items)
        {
            if (item == null || item.itemData == null)
                continue;

            saveData.itemDatas.Add(new ItemSaveData
            {
                ItemId = item.itemData.item_Id,
                Size = item.size
            });
        }

        Debug.Log("[DataManager] 세션 데이터 → 세이브 데이터 변환 완료");
    }


    // 세션 해제 및 Addressables 에셋 메모리 해제
    public void ClearSessionAndReleaseAssets()
    {
        if (sessionData == null)
            return;

        if (sessionData.heroes != null && heroDB != null)
        {
            foreach (HeroRuntimeData hero in sessionData.heroes)
            {
                if (hero == null || hero.heroData == null)
                    continue;

                var heroRef = heroDB.GetHeroRef(hero.heroData.Hero_Id);

                
                if (heroRef != null && heroRef.IsValid())
                {
                    heroRef.ReleaseAsset();
                }
            }
            sessionData.heroes.Clear();
        }

        if (sessionData.units != null && unitDB != null)
        {
            foreach (UnitRuntimeData unit in sessionData.units)
            {
                if (unit == null || unit.unitData == null)
                    continue;

                var unitRef = unitDB.GetUnitRef(unit.unitData.Unit_Id);

                if (unitRef != null && unitRef.IsValid())
                {
                    unitRef.ReleaseAsset();
                }
            }
            sessionData.units.Clear();
        }

        if (sessionData.items != null && itemDB != null)
        {
            foreach (ItemRuntimeData item in sessionData.items)
            {
                if (item == null || item.itemData == null)
                    continue;

                var itemRef = itemDB.GetItemRef(item.itemData.item_Id);

                if (itemRef != null && itemRef.IsValid())
                {
                    itemRef.ReleaseAsset();
                }
            }
            sessionData.items.Clear();
        }

        sessionData = null;
        saveData = null;

        Debug.Log("[DataManager] 세션 데이터 초기화 및 Addressables 에셋 해제 완료");
    }

}