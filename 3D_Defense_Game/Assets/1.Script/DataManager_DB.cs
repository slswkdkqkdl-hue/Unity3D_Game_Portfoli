using System.Threading.Tasks;
using UnityEngine;

public partial class DataManager : MonoBehaviour
{   
    // TODO: 추후 HeroDB, UnitDB, ItemDB도 Addressables로 관리
    //      DataManager에서는 AssetReferenceT<T>로 DB 참조를 보유하고,
    //      게임 시작 시 DB를 비동기로 로드한 뒤 세이브 데이터의 ID를 기준으로
    //      각 Runtime 데이터(SO)를 Addressables에서 필요한 만큼 로드하는 구조로 변경 예정.
    //      즉 HeroDB에 새로운 영웅넣을거면 새로운 RuntimeHeroData So 만들고 그걸 히어로 db에 넣고 어드레서블 업데이트하면 재 배포없이가능
    //      최종 구조:
    //      SaveData(ID)
    //          ↓
    //      DB(Addressables)
    //          ↓
    //      AssetReference
    //          ↓
    //      RuntimeHeroData / RuntimeUnitData / RuntimeItemData
    [Header("DB")] 
    public HeroDB heroDB;
    public UnitDB unitDB;
    public ItemDB itemDB;


    // 세이브 데이터를 런타임 데이터로 변환 (단순화된 어드레서블 비동기 로드)
    public async Task BuildSessionFromSaveDataAsync()
    {    
        if (sessionData == null || saveData == null) return;
        
        // 이미 데이터가 로드되어 있으면 스킵
        if (sessionData.runtimeHeroDatas.Count > 0 || sessionData.runtimeUnits.Count > 0 || sessionData.runtimeItemDatas.Count > 0)
        {
            Debug.Log("[DataManager] 이미 세션 데이터가 메모리에 로드되어 있어 복원을 스킵합니다.");
            return;
        }

        // ① 영웅 복원
        if (saveData.heroDates != null && heroDB != null)
        {
            Debug.Log($"[DataManager] 영웅 복원 시작 (총 {saveData.heroDates.Count}개)");
            foreach (HeroSaveData heroSave in saveData.heroDates)
            {
                var heroRef = heroDB.GetHeroRef(heroSave.HeroId);
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

        // ② 유닛 복원
        if (saveData.unitDatas != null && unitDB != null)
        {
            Debug.Log($"[DataManager] 유닛 복원 시작 (총 {saveData.unitDatas.Count}개)");
            foreach (UnitSaveData unitSave in saveData.unitDatas)
            {
                var unitRef = unitDB.GetUnitRef(unitSave.UnitId);
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

        // ③ 아이템 복원
        if (saveData.itemDatas != null && itemDB != null)
        { 
            Debug.Log($"[DataManager] 아이템 복원 시작 (총 {saveData.itemDatas.Count}개)");
            foreach (ItemSaveData itemSave in saveData.itemDatas)
            {
                var itemRef = itemDB.GetItemRef(itemSave.ItemId);
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

        saveData.heroDates.Clear();
        saveData.unitDatas.Clear();
        saveData.itemDatas.Clear();

        // ① 영웅 변환
        foreach (var hero in sessionData.runtimeHeroDatas)
        {
            saveData.heroDates.Add(new HeroSaveData
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
        saveData = null;
        Debug.Log("[DataManager] 세션 데이터가 초기화되고 어드레서블 메모리가 완벽히 언로드되었습니다.");
    }
}