using System.Threading.Tasks;
using UnityEngine;

public partial class DataManager : MonoBehaviour
{
    [Header("DB")]
    [SerializeField] private HeroDB heroDB;
    [SerializeField] private UnitDB unitDB;
    [SerializeField] private ItemDB itemDB;




        



    // 세이브 데이터를 런타임 데이터로 변환
    public async Task BuildSessionFromSaveDataAsync()
    {   

        if (sessionData == null) return;
        if (saveData == null) return;
        if (sessionData != null && (sessionData.HeroDatas.Count > 0 || sessionData.UnitDatas.Count > 0 || sessionData.ItemDatas.Count > 0))
        {
            Debug.Log("[DataManager] 이미 세션 데이터가 메모리에 로드되어 있어 복원을 스킵합니다.");
            return;
        }


        // ----------------------------------------------------
        // ① 영웅 복원 (HeroSaveData -> RuntimeHeroData)
        // ----------------------------------------------------
        if (saveData.heorDatas != null && heroDB != null)
        {
            foreach (HeroSaveData heroSave in saveData.heorDatas)
            {
                var heroRef = heroDB.GetHeroRef(heroSave.HeroId);
                if (heroRef != null)
                {
                    RuntimeHeroData loadedHero = await heroRef.LoadAssetAsync().Task;
                    sessionData.AddHero(loadedHero);
                }
            }
        }

        // ----------------------------------------------------
        // ② 유닛 복원 (UnitSaveData -> RuntimeUnitData)
        // ----------------------------------------------------
        if (saveData.unitDatas != null && unitDB != null)
        {
            foreach (UnitSaveData unitSave in saveData.unitDatas)
            {
                var unitRef = unitDB.GetUnitRef(unitSave.UnitId);
                if (unitRef != null)
                {
                    RuntimeUnitData loadedUnit = await unitRef.LoadAssetAsync().Task;
                    sessionData.AddUnit(loadedUnit);
                }
            }
        }

        // ----------------------------------------------------
        // ③ 아이템 복원 (ItemSaveData -> RuntimeItemData)
        // ----------------------------------------------------
        if (saveData.itemDatas != null && itemDB != null)
        { 
            Debug.Log(" 실행중 5");
            foreach (ItemSaveData itemSave in saveData.itemDatas)
            {
                var itemRef = itemDB.GetItemRef(itemSave.ItemId);
                Debug.Log($"[검색 요청] ItemId: {itemSave.ItemId} / 찾은 itemRef: {(itemRef != null ? itemRef.RuntimeKey : "null")}");

                if (itemRef != null)
                {
                    RuntimeItemData loadedItem = await itemRef.LoadAssetAsync().Task;
                    sessionData.AddItem(loadedItem);
                    Debug.Log("아이템 데이터 넣기완료");
                }
            }
        }
    }

    // 런타임 데이터를 세이브 데이터로 변환
    public void SyncSessionToSaveData()
    {
        if (saveData == null) saveData = new SaveData();
        if (sessionData == null) return;

        // 기존 저장 목록 비우기
        saveData.heorDatas.Clear();
        saveData.unitDatas.Clear();
        saveData.itemDatas.Clear();

        // ① 영웅 변환
        foreach (var hero in sessionData.HeroDatas)
        {
            saveData.heorDatas.Add(new HeroSaveData
            {
                HeroId = hero.Hero_Id,
                Level = hero.Current_Level,

                // 필요한 추가 가변 정보(레벨 등)가 있다면 여기에 대입
            });
        }

        // ② 유닛 변환
        foreach (var unit in sessionData.UnitDatas)
        {
            saveData.unitDatas.Add(new UnitSaveData
            {
                UnitId = unit.Unit_Id
                // 필요한 추가 가변 정보(레벨, 장비 ID 등)가 있다면 여기에 대입
            });
        }

        // ③ 아이템 변환
        foreach (var item in sessionData.ItemDatas)
        {
            saveData.itemDatas.Add(new ItemSaveData
            {
                ItemId = item.item_Id
               // 필요한 추가 가변 정보(강화 수치 등)가 있다면 여기에 대입
            });
        }
    }

    // 세션 해제 및 어드레서블 에셋 메모리 언로드
    // 얘는 게임종료하거나 시작씬 으로갔을때 세션데이터 초기화 
    public void ClearSessionAndReleaseAssets()
    {
        if (sessionData == null) return;

        // 1. 영웅 에셋 메모리 해제
        if (sessionData.HeroDatas != null)
        {
            foreach (var hero in sessionData.HeroDatas)
            {
                if (hero != null)
                {
                    var heroRef = heroDB.GetHeroRef(hero.Hero_Id);
                    if (heroRef != null && heroRef.IsValid())
                    {
                        heroRef.ReleaseAsset(); // 어드레서블 메모리 해제!
                    }
                }
            }
        }

        // 2. 유닛 에셋 메모리 해제
        if (sessionData.UnitDatas != null)
        {
            foreach (var unit in sessionData.UnitDatas)
            {
                if (unit != null)
                {
                    var unitRef = unitDB.GetUnitRef(unit.Unit_Id);
                    if (unitRef != null && unitRef.IsValid())
                    {
                        unitRef.ReleaseAsset();
                    }
                }
            }
        }

        // 3. 아이템 에셋 메모리 해제
        if (sessionData.ItemDatas != null)
        {
            foreach (var item in sessionData.ItemDatas)
            {
                if (item != null)
                {
                    var itemRef = itemDB.GetItemRef(item.item_Id);
                    if (itemRef != null && itemRef.IsValid())
                    {
                        itemRef.ReleaseAsset();
                    }
                }
            }
        }

        // 세션 데이터 초기화
        sessionData = null;
        Debug.Log("[DataManager] 세션 및 어드레서블 에셋 메모리가 정상적으로 해제되었습니다.");
    }

}
