using System.Collections.Generic;
using UnityEngine;

public class BattlefieldManager : MonoBehaviour
{
    public static BattlefieldManager I { get; private set; }

    // 1. 팀별로 유닛 리스트 분리 관리
    public List<UnitBase> playerUnits = new List<UnitBase>();
    public List<UnitBase> enemyUnits = new List<UnitBase>();

    // 2. 분리(Push) 알고리즘용 통합 캐시 리스트 (매 프레임 갱신)
    private readonly List<UnitBase> allUnitsCache = new List<UnitBase>();
    private int cachedFrame = -1;

    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    // 유닛이 생성될 때 등록
    public void RegisterUnit(UnitBase unit, Team team)
    {
        if (team == Team.Ally)
        {
            if (!playerUnits.Contains(unit)) playerUnits.Add(unit);
        }
        else
        {
            if (!enemyUnits.Contains(unit)) enemyUnits.Add(unit);
        }
    }

    // 유닛이 죽거나 사라질 때 제거
    public void UnregisterUnit(UnitBase unit, Team team)
    {
        if (team == Team.Ally)
        {
            playerUnits.Remove(unit);
        }
        else
        {
            enemyUnits.Remove(unit);
        }
    }
}