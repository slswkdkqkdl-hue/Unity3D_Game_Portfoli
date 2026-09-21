using UnityEngine;
using System.IO;
public class DataManager : MonoBehaviour
{
    public static DataManager I;


    // 저장 경로 
    private string savePath;
    public SaveData saveData;

    void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(this.gameObject);
            InitSavePath(); // 세이브경로 설정
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // 경로에 데이터 있는지 확인
    public bool HasSaveData()
    {
        return File.Exists(savePath);
    }
    // 처음 경로 생성
    public void InitSavePath()
    {
        savePath = Path.Combine(Application.persistentDataPath, "SaveData.json");
    }
    // 게임 저장
    public void SaveGame()
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
    }
    // 게임 불러오기
    public void LoadGame()
    {
        string json = File.ReadAllText(savePath);
        saveData = JsonUtility.FromJson<SaveData>(json);
        //
    }

    // 새로운 게임
    public void NewGame()
    {
        saveData = new SaveData();

        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        SaveGame();
    }


}