using UnityEngine;

public class Play_manager : MonoBehaviour
{

    [Header("현재 스테이지 적 정보 데이터")]
    [SerializeField] private EnemyInfoSO currentEnemyInfo;

    [Header("플레이어 현재 데이터")]
    private PlayerSessionData sessionData;


    [Header("초기화 할것들 (Canvas UI 오브젝트에 붙어있는 스크립트)")]
    [SerializeField] private BuildingPanelUI buildingSlotUI; 
    [SerializeField] private CastleController castleController;
    [SerializeField] private Herocontroller herocontroller;
    [SerializeField] private TimerController timerController;
    [SerializeField] private ResourceUI resourceUI;


    // 생성된 스테이지의 컨트롤러
    [Header("스테이지 매니저")]
    [SerializeField]private StageManager StageManager;

    private void Start()
    {   
        InitPlayScene();
    }
    
    /// <summary>
    /// 전투 씬에 필요한 데이터를 불러오고 각 시스템을 초기화합니다.
    /// </summary>
    public void InitPlayScene()
    {   
        // 1-1 데이터 매니저에서 현재 플레이어 세션 데이터 가져오기
        sessionData = GameManager.I.currentSessionData;
        // 1-2 배틀엔티티 매니저 한테서 현재 적 데이터 가져오기 
        currentEnemyInfo = BattleEntryData.selectedEnemy;

        if(sessionData == null ||currentEnemyInfo == null)
        {   
            Debug.Log("데이터 없음");
            return;
        }
        // 2. 빌딩매니저한테 유닛 데이터 전달
        BuildingManager.I.InitUnits(sessionData.units);

        // 3. 결과창 초기화
        BattleResultManager.I.LoadFixedRewardItems(sessionData.heroes); 

        // 4. 플레이어 리소스(돈 , 인구수) 초기화
        resourceUI.Init(0,0,0);

        // 5. 선택한 스테이지 프리팹 생성 및 인스턴스 참조 저장
        StageManager.CreateStage();

        // 6. 플레이어 클리어스테이지 수에 따른 건물 슬룻 활성 + 플레이어/적 메인성 생성 및 초기화
        StageManager.InitStage(sessionData,currentEnemyInfo);

        // 7. 기본 건물 아이콘 초기화 + 특수건물은 아직미완성
        buildingSlotUI.Init();

        // 8. 각 성벽 체력 초기화
        castleController.Init(sessionData.domainData,currentEnemyInfo);

        // 9. 선택한 영웅들 초기화 + 팀슬룻 생성
        var result = BattleEntryData.selectedHeroes;
        herocontroller.Init(result);
        
        // 10. 게임 시작과 함께 플레이 타이머 초기화 및 시작
        timerController.Init(true);

        // 11. 게임시작 로그 메시지 
        InGameLog.I.AddLog("전투가 시작되었습니다.");
    }

}