using System.Collections.Generic;
using UnityEngine;

public class BattlefieldManager : MonoBehaviour
{
    public static BattlefieldManager I { get; private set; }

    // UnitBase 대신 ICombatEntity 인터페이스를 관리합니다.
    public List<ICombatEntity> playerUnits = new List<ICombatEntity>();
    public List<ICombatEntity> enemyUnits = new List<ICombatEntity>();
    private readonly List<ICombatEntity> allUnitsCache = new List<ICombatEntity>();

    [Header("유닛 뭉침 방지 (Separation) 공통 설정")]
    public float separationRadius = 1f;   
    public float separationStrength = 2.3f; 

    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    public void RegisterUnit(ICombatEntity entity, Team team)
    {
        if (team == Team.Ally)
        {
            if (!playerUnits.Contains(entity)) playerUnits.Add(entity);
        }
        else
        {
            if (!enemyUnits.Contains(entity)) enemyUnits.Add(entity);
        }

        if (!allUnitsCache.Contains(entity)) allUnitsCache.Add(entity);
    }

    public void UnregisterUnit(ICombatEntity entity, Team team)
    {
        if (team == Team.Ally)
        {
            playerUnits.Remove(entity);
        }
        else
        {
            enemyUnits.Remove(entity);
        }

        allUnitsCache.Remove(entity);
    }

    public Transform GetClosestUnitInRange(Team currentTeam, Vector3 position, float range)
    {   
        List<ICombatEntity> targetList = (currentTeam == Team.Ally) ? enemyUnits : playerUnits;

        Transform closestTransform = null;
        float minSqrDist = range * range;

        for (int i = 0; i < targetList.Count; i++)
        {
            ICombatEntity entity = targetList[i];
            if (entity == null || !entity.gameObject.activeInHierarchy) continue;

            float sqrDist = (entity.transform.position - position).sqrMagnitude;
            if (sqrDist <= minSqrDist)
            {
                minSqrDist = sqrDist;
                closestTransform = entity.transform;
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
            ICombatEntity other = allUnitsCache[i];
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