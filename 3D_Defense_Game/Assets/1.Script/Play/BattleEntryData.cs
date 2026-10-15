using System.Collections.Generic;

public static class BattleEntryData
{
    // 선택한 적 거점
    public static EnemyInfoSO selectedEnemy;

    // 전투에 출전할 캐릭터
    public static List<HeroRuntimeData> selectedHeroes
        = new List<HeroRuntimeData>();
}