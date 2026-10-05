using System.Threading.Tasks;
using UnityEngine;

public partial class DataManager : MonoBehaviour
{
    [Header("DB")]
    public HeroDB heroDB;
    public UnitDB unitDB;
    public ItemDB itemDB;

    // 세이브 데이터를 런타임 데이터로 변환 (어드레서블 비동기 로드 방식)
    public async Task BuildSessionFromSaveDataAsync()
    {   
        if (sessionData == null) return;
        if (saveData == null) return;
        
        // 이미 데이터가 로드되어 있는지 체크 (오타 수정: runtimeHeroDatas 등 올바른 필드 참조)
        if (sessionData.runtimeHeroDatas.Count > 0 || sessionData.runtimeUnits.Count > 0 || sessionData.runtimeItemDatas.Count > 0)
        {
            Debug.Log("[DataManager] 이미 세션 데이터가 메모리에 로드되어 있어 복원을 스킵합니다.");
            return;
        }

            // ----------------------------------------------------
            // ① 영웅 복원
            // ----------------------------------------------------
            if (saveData.heorDatas != null && heroDB != null)
            {
                Debug.Log($"[DataManager] 영웅 복원 시작 (총 {saveData.heorDatas.Count}개)");
                foreach (HeroSaveData heroSave in saveData.heorDatas)
                {
                    var heroRef = heroDB.GetHeroRef(heroSave.HeroId);
                    Debug.Log($"[영웅 검색] ID: {heroSave.HeroId}, 결과: {(heroRef != null ? "성공" : "실패(Null)")}");
                    
                    if (heroRef != null)
                    {
                        var handle = heroRef.LoadAssetAsync();
                        RuntimeHeroData loadedHero = await handle.Task;
                        
                        if (loadedHero != null)
                        {
                            sessionData.AddHero(loadedHero);
                            Debug.Log($"[영웅 로드 완료] ID: {heroSave.HeroId}");
                        }
                    }
                }
            }

            // ----------------------------------------------------
            // ② 유닛 복원
            // ----------------------------------------------------
            if (saveData.unitDatas != null && unitDB != null)
            {
                Debug.Log($"[DataManager] 유닛 복원 시작 (총 {saveData.unitDatas.Count}개)");
                foreach (UnitSaveData unitSave in saveData.unitDatas)
                {
                    var unitRef = unitDB.GetUnitRef(unitSave.UnitId);
                    Debug.Log($"[유닛 검색] ID: {unitSave.UnitId}, 결과: {(unitRef != null ? "성공" : "실패(Null)")}");
                    
                    if (unitRef != null)
                    {
                        var handle = unitRef.LoadAssetAsync();
                        RuntimeUnitData loadedUnit = await handle.Task;
                        
                        if (loadedUnit != null)
                        {
                            sessionData.AddUnit(loadedUnit);
                            Debug.Log($"[유닛 로드 완료] ID: {unitSave.UnitId}");
                        }
                    }
                }
            }

            // ----------------------------------------------------
            // ③ 아이템 복원
            // ----------------------------------------------------
            if (saveData.itemDatas != null && itemDB != null)
            { 
                Debug.Log($"[DataManager] 아이템 복원 시작 (총 {saveData.itemDatas.Count}개)");
                foreach (ItemSaveData itemSave in saveData.itemDatas)
                {
                    var itemRef = itemDB.GetItemRef(itemSave.ItemId);
                    Debug.Log($"[아이템 검색] ID: {itemSave.ItemId}, 결과: {(itemRef != null ? "성공" : "실패(Null)")}");

                    if (itemRef != null)
                    {
                        var handle = itemRef.LoadAssetAsync();
                        RuntimeItemData loadedItem = await handle.Task;
                        
                        if (loadedItem != null)
                        {
                            sessionData.AddItem(loadedItem);
                            Debug.Log($"[아이템 로드 완료] ID: {itemSave.ItemId}");
                        }
                    }
                }
            }
            Debug.Log("[DataManager] 모든 세션 데이터 복원 완료!");

    }

    // 런타임 데이터를 세이브 데이터로 변환
    public void SyncSessionToSaveData()
    {
        if (saveData == null) saveData = new SaveData();
        if (sessionData == null) return;

        saveData.heorDatas.Clear();
        saveData.unitDatas.Clear();
        saveData.itemDatas.Clear();

        // ① 영웅 변환
        foreach (var hero in sessionData.runtimeHeroDatas)
        {
            saveData.heorDatas.Add(new HeroSaveData
            {
                HeroId = hero.Hero_Id,
                Level = hero.Current_Level,
            });
        }

        // ② 유닛 변환
        foreach (var unit in sessionData.runtimeUnits)
        {
            saveData.unitDatas.Add(new UnitSaveData
            {
                UnitId = unit.Unit_Id
            });
        }

        // ③ 아이템 변환
        foreach (var item in sessionData.runtimeItemDatas)
        {
            saveData.itemDatas.Add(new ItemSaveData
            {
                ItemId = item.item_Id
            });
        }
    }

    // 세션 해제 및 어드레서블 에셋 메모리 언로드
    public void ClearSessionAndReleaseAssets()
    {
        if (sessionData == null) return;

        // 1. 영웅 어드레서블 에셋 언로드
        if (sessionData.runtimeHeroDatas != null && heroDB != null)
        {
            foreach (var hero in sessionData.runtimeHeroDatas)
            {
                var heroRef = heroDB.GetHeroRef(hero.Hero_Id);
                if (heroRef != null && heroRef.RuntimeKeyIsValid())
                {
                    heroRef.ReleaseAsset();
                }
            }
            sessionData.runtimeHeroDatas.Clear();
        }

        // 2. 유닛 어드레서블 에셋 언로드
        if (sessionData.runtimeUnits != null && unitDB != null)
        {
            foreach (var unit in sessionData.runtimeUnits)
            {
                var unitRef = unitDB.GetUnitRef(unit.Unit_Id);
                if (unitRef != null && unitRef.RuntimeKeyIsValid())
                {
                    unitRef.ReleaseAsset();
                }
            }
            sessionData.runtimeUnits.Clear();
        }

        // 3. 아이템 어드레서블 에셋 언로드
        if (sessionData.runtimeItemDatas != null && itemDB != null)
        {
            foreach (var item in sessionData.runtimeItemDatas)
            {
                var itemRef = itemDB.GetItemRef(item.item_Id);
                if (itemRef != null && itemRef.RuntimeKeyIsValid())
                {
                    itemRef.ReleaseAsset();
                }
            }
            sessionData.runtimeItemDatas.Clear();
        }

        // 세션 데이터 자체 초기화
        sessionData = null;
        Debug.Log("[DataManager] 세션 데이터가 초기화되고 어드레서블 메모리가 완벽히 언로드되었습니다.");
    }
}