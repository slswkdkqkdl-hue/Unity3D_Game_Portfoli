using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Play_manager : MonoBehaviour
{
    public static Play_manager Instance { get; private set; }

    [Header("현재 스테이지 적 정보 데이터")]
    [SerializeField] private EnemyInfoSO currentEnemyInfo;

    [Header("맵이 생성될 위치 (Transform)")]
    [SerializeField] private Transform stageSpawnPoint;

    [Header("플레이어 현재 데이터")]
    private PlayerSessionData sessionData;


    [Header("초기화 할것들")]
    [SerializeField] private BuildingPanelUI buildingSlotUI; 
    [SerializeField] private CastleController castleController;
    [SerializeField] private Herocontroller herocontroller;

    // 프로젝트 프리팹이 아닌, 씬에 실제 생성된 맵 게임오브젝트 저장
    private GameObject spawnedStageInstance;

    private void Awake()
    {
        Instance = this;
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
        // 2. 선택한 스테이지 프리팹 생성 및 인스턴스 참조 저장
        if (BattleEntryData.currentStage != null && BattleEntryData.currentStage.StagePrefab != null)
        {
            GameObject stagePrefab = BattleEntryData.currentStage.StagePrefab;
            spawnedStageInstance = Instantiate(stagePrefab, stageSpawnPoint);
        }

        // 3. 플레이어 클리어 기록에 따른 건물 슬롯 레벨 해금
        SetupBuildingSlots();

        // 4. 기본 건물 아이콘 초기화 + 특수건물은 아직미완성
        buildingSlotUI.Init();

        // 5. 각 성벽 체력 초기화
        castleController.Init(sessionData.domainData,currentEnemyInfo);

        // 6. 선택한 영웅들 초기화 + 팀슬룻 생성
        var result = BattleEntryData.selectedHeroes;
        herocontroller.Init(result);
    }

    /// <summary>
    /// 클리어한 스테이지 수에 맞춰 건물 슬롯 해금 레벨을 계산하고 활성화합니다.
    /// </summary>
    private void SetupBuildingSlots()
    {
        if (spawnedStageInstance == null || sessionData?.clearid == null) return;

        int clearCount = sessionData.clearid.Count;
        int castleLevel = FactionUtility.GetCastleLevelByClearCount(clearCount); 

        if (spawnedStageInstance.TryGetComponent<BuildingSlotManager>(out var buildingSlot) ||
            (buildingSlot = spawnedStageInstance.GetComponentInChildren<BuildingSlotManager>()) != null)
        {
            buildingSlot.InitializeSlots(castleLevel);
        }
        else
        {
            Debug.LogWarning("[Play_manager] 생성된 맵에서 BuildingSlotManager를 찾지 못했습니다.");
        }
    }

    private void SetupGameElements()
    {
        // UI 및 적 AI 스폰 초기화
    }
}