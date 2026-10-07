using UnityEngine;

public class LobbyManager : MonoBehaviour
{   

    public static LobbyManager I;

    public UIManager_Play uIManager_;

    public PlayerSessionData sessionData;


    private void Awake()
    {
        if (I == null)
        {
            I = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// DataManager로부터 플레이어 세션 데이터를 받아옵니다.
    /// </summary>
    private void LoadPlayerData()
    {
        if (DataManager.I != null)
        {
            sessionData = DataManager.I.sessionData; // DataManager의 실제 세션 데이터 반환 메서드명으로 맞춰주세요.
 
        }
        else
        {
            Debug.LogWarning("[LobbyManager] DataManager 싱글톤 인스턴스를 찾을 수 없습니다.");
        }
    }
    

    void Start()
    {
        uIManager_.InitPanels();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
