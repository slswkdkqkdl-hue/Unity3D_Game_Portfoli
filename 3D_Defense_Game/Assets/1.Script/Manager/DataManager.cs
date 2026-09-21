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
    public void Setpath()
    {
        savePath = Path.Combine(Application.dataPath, "SaveData.json");
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
        // 씬이동 
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