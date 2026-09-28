using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager I { get; private set; }

    [SerializeField] private int maxSlotCount = 3; //슬룻 수 

    public SaveData saveData;

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

    // 새로운 게임 시작
    public void NewGame()
    {
        // 새로운 세이브데이터 생성
        saveData = new SaveData();
        // 런타임 플레이어 데이터 생성
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

    //세이브 데이터를 매개변수로 받고 그걸 런타임데이터에 변환하는 함수 
    //런타임데이터를 매개변수로 받고 그걸 세이브데이터로 변환해서 담는함수
}