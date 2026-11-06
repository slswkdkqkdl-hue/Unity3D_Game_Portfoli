using UnityEngine;

public class StageManager : MonoBehaviour
{

    [Header("맵이 생성될 위치 (Transform)")]
    [SerializeField] private Transform stageSpawnPoint;
    private StageController stageController;

    /// <summary>
    /// 스테이지 프리팹 생성
    /// </summary>
    public void CreateStage()
    {
        if (BattleEntryData.currentStage == null || BattleEntryData.currentStage.StagePrefab == null)
        {
            Debug.LogError("[StageManager] 스테이지 데이터가 없습니다.");
            return;
        }

        GameObject stagePrefab = BattleEntryData.currentStage.StagePrefab;
        GameObject stageInstance = Instantiate(stagePrefab, stageSpawnPoint);

        stageController = stageInstance.GetComponentInChildren<StageController>();

        if (stageController == null)
        {
            Debug.LogError("[StageManager] StageController를 찾지 못했습니다.");
        }
    }

    /// <summary>
    /// 건물 슬롯 및 스테이지 컨트롤러 초기화
    /// </summary>
    public void InitStage(PlayerSessionData sessionData, EnemyInfoSO enemyInfo)
    {
        if (stageController == null || sessionData?.clearid == null)
        {
            Debug.LogWarning("[StageManager] 초기화에 필요한 데이터가 없습니다.");
            return;
        }

        int clearCount = sessionData.clearid.Count;
        int castleLevel = FactionUtility.GetCastleLevelByClearCount(clearCount);

        stageController.Init(castleLevel, enemyInfo);
    }
}