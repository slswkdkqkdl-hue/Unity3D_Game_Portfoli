using System.Collections.Generic;
using UnityEngine;

public class BattlefieldManager : MonoBehaviour
{
    public static BattlefieldManager I { get; private set; }

    // 1. 팀별로 유닛 리스트 분리 관리
    public List<UnitBase> playerUnits = new List<UnitBase>();
    public List<UnitBase> enemyUnits = new List<UnitBase>();

    // 2. 분리(Push) 알고리즘용 통합 캐시 리스트 (등록/제거될 때만 동기화)
    private readonly List<UnitBase> allUnitsCache = new List<UnitBase>();

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

        // 통합 캐시에도 추가
        if (!allUnitsCache.Contains(unit)) allUnitsCache.Add(unit);
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

        // 통합 캐시에서도 제거
        allUnitsCache.Remove(unit);
    }

    /// <summary>
    /// 특정 위치(position)를 기준으로, 지정된 감지 범위(range) 내에서 
    /// 가장 가까운 상대 팀 유닛의 Transform을 반환합니다.
    /// </summary>
    public Transform GetClosestUnitInRange(Team currentTeam, Vector3 position, float range)
    {
        List<UnitBase> targetList = (currentTeam == Team.Ally) ? enemyUnits : playerUnits;

        Transform closestTransform = null;
        float minSqrDist = range * range; // 제곱 거리 비교로 성능 최적화

        for (int i = 0; i < targetList.Count; i++)
        {
            UnitBase unit = targetList[i];
            if (unit == null || !unit.gameObject.activeInHierarchy) continue;

            float sqrDist = (unit.transform.position - position).sqrMagnitude;
            if (sqrDist <= minSqrDist)
            {
                minSqrDist = sqrDist;
                closestTransform = unit.transform;
            }
        }

        return closestTransform;
    }

    /// <summary>
    /// 유닛들이 서로 뭉치지 않고 밀어내도록 하는 분리(Separation) 벡터를 계산합니다.
    /// </summary>
    public Vector3 GetSeparationVector(GameObject self, Vector3 myPos, float radius, float strength)
    {
        Vector3 separationSum = Vector3.zero;
        int count = 0;
        float radiusSqr = radius * radius;

        for (int i = 0; i < allUnitsCache.Count; i++)
        {
            UnitBase other = allUnitsCache[i];
            if (other == null || other.gameObject == self || !other.gameObject.activeInHierarchy) continue;

            Vector3 toOther = other.transform.position - myPos;
            toOther.y = 0f;
            float sqrDist = toOther.sqrMagnitude;

            if (sqrDist < radiusSqr && sqrDist > 0.0001f)
            {
                float dist = Mathf.Sqrt(sqrDist);
                Vector3 pushDir = -toOther.normalized;
                float pushFactor = (1.0f - (dist / radius));
                
                separationSum += pushDir * pushFactor;
                count++;
            }
        }

        if (count > 0)
        {
            separationSum /= count;
            separationSum *= strength;
        }

        return separationSum;
    }
}