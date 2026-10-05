using UnityEngine;
using System.IO;
using System.Threading.Tasks;

public class DataManager : MonoBehaviour
{
    public static DataManager I { get; private set; }

    [SerializeField] private int maxSlotCount = 3; //슬룻 수 

    [Header("현재 데이터")]
    public SaveData saveData;
    public PlayerSessionData sessionData;

    [Header("DB")]
    [SerializeField] private HeroDB heroDB;
    [SerializeField] private UnitDB unitDB;
    [SerializeField] private ItemDB itemDB;


    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
    }

    // 해당 슬룻 경로 반환
    private string GetSavePath(int slot)
    {
        return Path.Combine(
            Application.persistentDataPath,
            $"SaveData{slot}.json"
        );
    }

    // 해당 경로에 데이터 있는지 확인
    public bool HasSaveData(int slot)
    {
        string path = GetSavePath(slot);

        return File.Exists(path);
    }
    // 저장 데이터가 '하나라도' 있는지 확인하는 함수
    public bool HasAnySaveData()
    {
        for (int i = 0; i < maxSlotCount; i++)
        {
            if (HasSaveData(i))
            {
                return true; // 하나라도 찾으면 즉시 true 반환 후 종료
            }
        }
        return false; // 끝까지 찾아도 없으면 false
    }

    // 게임 저장
    public void SaveGame(int slot)
    {
        string path = GetSavePath(slot);

        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(path, json);
    }
    /// 현재 게임 데이터에 영향을 주지 않고 특정 슬롯의 데이터만 읽어서 반환합니다.
    public SaveData GetSaveData(int slot)
    {
        string path = GetSavePath(slot);

        if (!File.Exists(path))
        {
            return null; // 파일이 없으면 null 반환
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    // 게임 불러오기
    public bool LoadGame(int slot)
    {
        string path = GetSavePath(slot);

        // 저장 데이터가 없으면 실패
        if (!File.Exists(path))
        {
            return false;
        }

        string json = File.ReadAllText(path);
        saveData = JsonUtility.FromJson<SaveData>(json);
        return true;
    }

    // 새로운 게임 시작 (선택씬에서 쓸거)
    public void NewGame(TerritoryData data)
    {
        // 새로운 세이브데이터 생성
        saveData = new SaveData();
        sessionData = new PlayerSessionData();
    }

    // 저장 데이터 삭제
    public void DeleteSave(int slot)
    {
        string path = GetSavePath(slot);

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }



    // 세이브 데이터를 런타임 데이터로 변환
    public async Task BuildSessionFromSaveDataAsync()
    {   
        Debug.Log("실행중2");
        if (sessionData == null) return;
        if (saveData == null) return;
           Debug.Log("실행중3");
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
                HeroId = hero.Hero_Id
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