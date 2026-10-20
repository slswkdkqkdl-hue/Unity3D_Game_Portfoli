using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 로비 → 플레이씬으로 전투 진입 정보(적, 출전 영웅, 스테이지)를 넘기는 static 데이터 홀더.
/// 씬 간 전달용이라 생명주기가 필요 없어서 static 클래스로 구현.
/// </summary>
public static class BattleEntryData
{
    public static EnemyInfoSO selectedEnemy;
    public static List<HeroRuntimeData> selectedHeroes = new List<HeroRuntimeData>();
    public static StageData currentStage;
    private static AssetReference loadedStageRef;

    /// <summary>
    /// 선택한 적 정보를 저장한다.
    /// </summary>
    public static void EnemyInit(EnemyInfoSO currentSO)
    {
        ReleaseCurrentStage();
        selectedEnemy = currentSO;
    }

    /// <summary>
    /// 출전 영웅 목록을 저장한다.
    /// </summary>
    public static void PlayerHerosInit(List<HeroRuntimeData> currentHeroes)
    {
        selectedHeroes.Clear();
        selectedHeroes.AddRange(currentHeroes);
    }

    /// <summary>
    /// 로드가 끝난 스테이지의 어드레서블 참조와 데이터를 저장한다. (로딩 매니저에서 호출)
    /// </summary>
    public static void SetLoadedStage(AssetReference stageRef, StageData stageData)
    {
        loadedStageRef = stageRef;
        currentStage = stageData;
    }

    /// <summary>
    /// 로드해둔 스테이지 어드레서블 에셋을 메모리에서 해제하고 스테이지 참조를 비운다.
    /// </summary>
    public static void ReleaseCurrentStage()
    {
        if (loadedStageRef != null && loadedStageRef.IsValid())
        {
            loadedStageRef.ReleaseAsset();
        }
        loadedStageRef = null;
        currentStage = null;
    }

    /// <summary>
    /// 전투 종료 후 호출. 어드레서블 해제 + 전투 진입 데이터 전부 초기화.
    /// 얘는 게임플레이 종료후 호출할것
    /// </summary>
    public static void Clear()
    {
        ReleaseCurrentStage();
        selectedEnemy = null;
        selectedHeroes.Clear();
    }
}