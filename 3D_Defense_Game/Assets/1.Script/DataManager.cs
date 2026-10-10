using UnityEngine;
using System.IO;
using System.Threading.Tasks;

public partial class DataManager : MonoBehaviour
{
    public static DataManager I { get; private set; }

    [SerializeField] private int maxSlotCount = 3; //슬룻 수 

    [Header("현재 데이터")]
    public SaveData saveData;
    [field: SerializeField]public PlayerSessionData sessionData { get; private set; }

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
        SyncSessionToSaveData(); // 현재 세션데이터를 세이브데이터로 변환
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
        if (data == null) return;

        // 1. 새로운 세이브데이터 및 세션데이터 생성
        saveData = new SaveData();

        // 2. 영지 타입 및 영지 기본 정보 세팅
        saveData.domainType = data.domainType;

        // 3. 영웅 ID 저장 (data.heroDescDatas에서 ID 추출)
        if (data.heroDescDatas != null)
        {
            saveData.heroDates.Add(new HeroSaveData
            {
                HeroId = data.heroDescDatas.Hero_Id, // 인스펙터/SO 상의 Hero_Id 필드명 확인
                Level = 1,
                SP = 99,
                Equippedweapon_Id = 1,
                EquippedArmor_Id = 2
            });
            saveData.heroDates.Add(new HeroSaveData // 테스트 히어로
            {
                HeroId = 99, 
                Level = 1,
                SP = 30
            });
        }
        // 4. 유닛 ID들 저장 (data.unitDescDatas에서 ID 추출)
        if (data.unitDescDatas != null)
        {
            foreach (var unit in data.unitDescDatas)
            {
                if (unit == null) continue;

                saveData.unitDatas.Add(new UnitSaveData
                {
                    UnitId = unit.Unit_Id 
                });
            }
        }
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



}