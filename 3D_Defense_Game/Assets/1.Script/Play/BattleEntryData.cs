using System.Collections.Generic;
using UnityEngine.AddressableAssets; // 어드레서블 네임스페이스 추가

public static class BattleEntryData
{
    // 선택한 적 거점
    public static EnemyInfoSO selectedEnemy;

    // 전투에 플레이어 출전할 캐릭터
    public static List<HeroRuntimeData> selectedHeroes = new List<HeroRuntimeData>();

    public static StageData currentStage;

    // 로드한 스테이지의 레퍼런스를 기억해 둠 (해제할 때 사용)
    private static AssetReference loadedStageRef;

    public static void EnemyInit(EnemyInfoSO currentSO)
    {
        // 데이터 완전히 초기화
        ReleaseCurrentStage();
        // 선택 적정보 넣기 
        selectedEnemy = currentSO;
    }

    public static void PlayerHerosInit(List<HeroRuntimeData> currentHeroes)
    {
        selectedHeroes.Clear();
        foreach (HeroRuntimeData result in currentHeroes)
        {
            selectedHeroes.Add(result);                
        }
    }

    // 스테이지 레퍼런스를 세팅하는 함수 (로딩 매니저에서 호출)
    public static void SetLoadedStage(AssetReference stageRef, StageData stageData)
    {
        loadedStageRef = stageRef;
        currentStage = stageData;
    }

    // 메모리 해제 + 데이터 초기화 
    public static void ReleaseCurrentStage()
    {
        if (loadedStageRef != null && loadedStageRef.IsValid())
        {
            loadedStageRef.ReleaseAsset();
        }
        loadedStageRef = null;
        currentStage = null;
    }
}