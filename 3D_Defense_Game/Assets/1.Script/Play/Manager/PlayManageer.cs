using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Play_manager : MonoBehaviour
{

    [Header("현재 스테이지 적 정보 데이터")]
    [SerializeField] private EnemyInfoSO currentEnemyInfo;

    [Header("맵이 생성될 위치 (Transform)")]
    [SerializeField] private Transform stageSpawnPoint;

    [Header("플레이어 현재 데이터")]
    private PlayerSessionData sessionData;


    [Header("초기화 할것들 (Canvas UI 오브젝트에 붙어있는 스크립트)")]
    [SerializeField] private BuildingPanelUI buildingSlotUI; 
    [SerializeField] private CastleController castleController;
    [SerializeField] private Herocontroller herocontroller;
    [SerializeField] private TimerController timerController;
    [SerializeField] private ResourceUI resourceUI;

    // 프로젝트 프리팹이 아닌, 씬에 실제 생성된 맵 게임오브젝트 저장

    // 생성된 스테이지의 컨트롤러
    [Header("현재 스테이지 컨트롤러")]
    private StageController stageController;

    private void Start()
    {
        InitPlayScene();
    }

    public void InitPlayScene()
    {   
        // 1-1 데이터 매니저에서 현재 플레이어 세션 데이터 가져오기
        sessionData = DataManager.I.sessionData;
        // 1-2 배틀엔티티 매니저 한테서 현재 적 데이터 가져오기 
        currentEnemyInfo = BattleEntryData.selectedEnemy;

        if(sessionData == null ||currentEnemyInfo == null)
        {   
            Debug.Log("데이터 없음");
            return;
        }

        // 2. 플레이어 리소스(돈 , 인구수) 초기화
        resourceUI.Init(0,0,0);

        // 3. 선택한 스테이지 프리팹 생성 및 인스턴스 참조 저장
        StageCreate();

        // 4. 플레이어 클리어스테이지 수에 따른 건물 슬룻 활성 + 플레이어/적 메인성 생성 및 초기화
        SetupBuildingSlots();

        // 빌딩매니저한테 유닛 데이터 전달
        BuildingManager.I.InitUnits(sessionData.units);

        // 5. 기본 건물 아이콘 초기화 + 특수건물은 아직미완성
        buildingSlotUI.Init();

        // 6. 각 성벽 체력 초기화
        castleController.Init(sessionData.domainData,currentEnemyInfo);

        // 7. 선택한 영웅들 초기화 + 팀슬룻 생성
        var result = BattleEntryData.selectedHeroes;
        herocontroller.Init(result);
        
        // 9. 게임시작 로그 메시지 
        InGameLog.I.AddLog("전투가 시작되었습니다.");

        // 10. 게임 시작과 함께 플레이 타이머 초기화 및 시작
        timerController.Init(true);


        BattleResultManager.I.LoadFixedRewardItems(sessionData.heroes);
    }

    /// <summary>
    /// 클리어한 스테이지 수에 맞춰 건물 슬롯 해금 레벨을 계산하고 활성화합니다.
    /// </summary>
    private void SetupBuildingSlots()
    {
        if (stageController == null || sessionData?.clearid == null)
        {
            Debug.LogWarning("[Play_manager] 건물 슬롯 초기화에 필요한 데이터가 없습니다.");
            return;
        }

        int clearCount = sessionData.clearid.Count;
        int castleLevel = FactionUtility.GetCastleLevelByClearCount(clearCount);

        stageController.Init(castleLevel,currentEnemyInfo);
    }
    private void StageCreate()
    {
        if (BattleEntryData.currentStage == null ||
            BattleEntryData.currentStage.StagePrefab == null)
        {
            Debug.LogError("[Play_manager] 스테이지 데이터가 없습니다.");
            return;
        }

        GameObject stagePrefab = BattleEntryData.currentStage.StagePrefab;
        GameObject stageInstance = Instantiate(stagePrefab, stageSpawnPoint);

        stageController = stageInstance.GetComponentInChildren<StageController>();

        if (stageController == null)
        {
            Debug.LogError("[Play_manager] StageController를 찾지 못했습니다.");
        }
    }

    private void SetupGameElements()
    {
        // UI 및 적 AI 스폰 초기화
    }
}