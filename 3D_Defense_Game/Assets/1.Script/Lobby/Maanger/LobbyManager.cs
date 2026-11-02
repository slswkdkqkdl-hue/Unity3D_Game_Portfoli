using UnityEngine;

public class LobbyManager : MonoBehaviour
{   

    public static LobbyManager I;


    private PlayerSessionData sessionData;
    
    [Header("맨처음초기화")]
    [SerializeField]private HeroSelectController heroSelect; // 영웅창 관리
    [SerializeField]private UpgradeController upgrade; // 업글레이드 관리
    [SerializeField]private MapManager mapManager; // 각 스테이지 활성화
    [SerializeField]private TeamCompositionController infocontroller; // 적 정보패널에 에서 캐릭터 슬룻 생성
    [SerializeField]private UIManager_Lobsby uIManager_;

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

    void Start()
    {   
        var data = GameManager.I.currentSessionData;
        LoadPlayerData(data);
    }

    /// <summary>
    /// 데이터매니저한테 데이터 받은후 각 UI 및 초기화
    /// </summary>
    private void LoadPlayerData(PlayerSessionData sessionData)
    {   
        if (sessionData == null)
        {
            Debug.LogError("[LobbyManager] 전달받은 세션 데이터가 null입니다!");
            return;
        }
        Debug.Log($"[LobbyManager] 세션 데이터 주입 성공. 영웅 수: {sessionData.heroes?.Count ?? 0}");
        heroSelect.Init(sessionData.heroes,sessionData.items); // 영웅 데이터 받아서 각 슬룻생성 
        upgrade.Init(sessionData.domainData); // 업그레이드 데이터 받아서 각 업글 초기화
        mapManager.Init(sessionData.clearid); // 클리어 id 값 주입 
        infocontroller.Init(sessionData.heroes); // 영웅데이터 전달 후 슬룻 생성 
        uIManager_.Init(sessionData.domainData); // 세력 데이터를 받아서 각 UI에 초기화 
        uIManager_.InitPanels();  // 모든 패널 초기화 
    }

    
    


}
