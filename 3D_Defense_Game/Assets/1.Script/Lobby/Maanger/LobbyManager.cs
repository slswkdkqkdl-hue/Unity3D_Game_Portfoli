using UnityEngine;

public class LobbyManager : MonoBehaviour
{   

    public static LobbyManager I;

    public UIManager_Play uIManager_;

    private PlayerSessionData sessionData;
    
    [Header("맨처음초기화")]
    [SerializeField]private HeroSelectController heroSelect; // 영웅창 관리
    [SerializeField]private UpgradeController upgrade; // 업글레이드 관리
    [SerializeField]private MapManager mapManager;


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
        LoadPlayerData();
    }


    /// <summary>
    /// 데이터매니저한테 데이터 받은후 각 UI 및 초기화
    /// </summary>
    private void LoadPlayerData()
    {
        if (DataManager.I != null && DataManager.I.sessionData != null)
        {
            sessionData = DataManager.I.sessionData;
            Debug.Log($"[LobbyManager] 세션 데이터 로드 성공. 영웅 수: {sessionData.heroes?.Count ?? 0}");
        }
        else
        {
            Debug.LogWarning("[LobbyManager] DataManager 또는 sessionData가 아직 초기화되지 않았습니다.");
            return;
        }

        heroSelect.Init(sessionData.heroes,sessionData.items); // 영웅 데이터 받아서 각 슬룻생성 
        upgrade.Init(sessionData.domainData); // 업그레이드 데이터 받아서 각 업글 초기화
        mapManager.Init(sessionData.clearid);
        uIManager_.Init(sessionData.domainData); // 세력 데이터를 받아서 각 UI에 초기화 
        uIManager_.InitPanels();  // 모든 패널 초기화 
    }

    
    


}
