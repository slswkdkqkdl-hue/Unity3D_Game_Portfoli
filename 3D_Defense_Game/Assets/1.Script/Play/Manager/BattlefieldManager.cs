using System.Collections.Generic;
using UnityEngine;

public class BattlefieldManager : MonoBehaviour
{
    public static BattlefieldManager I { get; private set; }

    public List<UnitBase> playerUnits = new List<UnitBase>();
    public List<UnitBase> enemyUnits = new List<UnitBase>();
    private readonly List<UnitBase> allUnitsCache = new List<UnitBase>();

    [Header("유닛 뭉침 방지 (Separation) 공통 설정")]
    public float separationRadius = 1f;   
    public float separationStrength = 2.3f; 

    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

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

        if (!allUnitsCache.Contains(unit)) allUnitsCache.Add(unit);
    }

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

        allUnitsCache.Remove(unit);
    }

    public Transform GetClosestUnitInRange(Team currentTeam, Vector3 position, float range)
    {   
        List<UnitBase> targetList = (currentTeam == Team.Ally) ? enemyUnits : playerUnits;

        Transform closestTransform = null;
        float minSqrDist = range * range;

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
    /// BattlefieldManager가 관리하는 공통 설정값으로 뭉침 방지 벡터를 계산합니다.
    /// </summary>
    public Vector3 GetSeparationVector(GameObject self, Vector3 myPos)
    {
        Vector3 separationSum = Vector3.zero;
        int count = 0;
        float radiusSqr = separationRadius * separationRadius;

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
                float pushFactor = (1.0f - (dist / separationRadius));
                
                separationSum += pushDir * pushFactor;
                count++;
            }
        }

        if (count > 0)
        {
            separationSum /= count;
            separationSum *= separationStrength;
        }

        return separationSum;
    }
}