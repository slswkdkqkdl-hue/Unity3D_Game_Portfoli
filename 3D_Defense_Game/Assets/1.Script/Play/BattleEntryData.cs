using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// 로비 → 플레이씬으로 전투 진입 정보(적, 출전 영웅, 스테이지, 적 유닛/영웅)를 넘기는 static 데이터 홀더.
/// </summary>
public static class BattleEntryData
{
    public static EnemyInfoSO selectedEnemy;
    public static List<HeroRuntimeData> selectedHeroes = new List<HeroRuntimeData>();
    
    // 새로 추가된 적 유닛 및 영웅 데이터 리스트
    public static List<UnitData> EnemyUnit = new List<UnitData>();
    public static List<HeroData> EnemyHero = new List<HeroData>();

    private static AssetReference loadedStageRef;
    
    // 어드레서블로 로드한 적 유닛/영웅 에셋들의 핸들을 모아두는 리스트 (메모리 해제용)
    private static List<AsyncOperationHandle> loadedEnemyAssetHandles = new List<AsyncOperationHandle>();

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
    /// 로드가 끝난 스테이지의 어드레서블 참조와 데이터를 저장한다.
    /// </summary>
    public static void SetLoadedStage(AssetReference stageRef, StageData stageData)
    {
        loadedStageRef = stageRef;
        currentStage = stageData;
    }

    public static StageData currentStage;

    /// <summary>
    /// 로드된 적 유닛 및 영웅 데이터와 핸들 목록을 등록한다.
    /// </summary>
    public static void SetLoadedEnemyData(List<UnitData> units, List<HeroData> heroes, List<AsyncOperationHandle> handles)
    {
        EnemyUnit.Clear();
        EnemyUnit.AddRange(units);

        EnemyHero.Clear();
        EnemyHero.AddRange(heroes);

        loadedEnemyAssetHandles = handles;
    }

    /// <summary>
    /// 로드해둔 스테이지 어드레서블 에셋을 메모리에서 해제한다.
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
    /// 로드된 적 유닛 및 영웅 에셋들을 메모리에서 해제한다.
    /// </summary>
    public static void ReleaseEnemyData()
    {
        foreach (var handle in loadedEnemyAssetHandles)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
        loadedEnemyAssetHandles.Clear();
        EnemyUnit.Clear();
        EnemyHero.Clear();
    }

    /// <summary>
    /// 전투 종료 후 호출. 모든 어드레서블 해제 및 진입 데이터 초기화.
    /// </summary>
    public static void Clear()
    {
        ReleaseCurrentStage();
        ReleaseEnemyData();
        selectedEnemy = null;
        selectedHeroes.Clear();
    }
}