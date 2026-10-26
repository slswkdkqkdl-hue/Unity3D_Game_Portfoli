using System.Collections.Generic;
using UnityEngine;

public class UnitPoolManager : MonoBehaviour
{
    public static UnitPoolManager I;

    // 유닛 ID별로 비활성화된 유닛들을 대기시켜 놓는 큐(Queue) 딕셔너리
    private Dictionary<int, Queue<UnitBase>> poolDictionary = new Dictionary<int, Queue<UnitBase>>();
    
    // 원본 프리팹을 보관해 두는 딕셔너리
    private Dictionary<int, UnitBase> prefabDictionary = new Dictionary<int, UnitBase>();

    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 풀에서 유닛을 가져오거나, 없으면 새로 생성(Instantiate)하는 함수
    /// </summary>
    public UnitBase GetUnit(UnitData unitData, Vector3 position, Quaternion rotation)
    {
        if (unitData == null || unitData.prefab_Play == null) return null;

        int id = unitData.Unit_Id;

        // 1. 해당 ID의 풀(Queue)이 아예 없다면 새로 만들어 줍니다.
        if (!poolDictionary.ContainsKey(id))
        {
            UnitBase unitPrefab = unitData.prefab_Play.GetComponent<UnitBase>();
            if (unitPrefab == null)
            {
                Debug.LogError($"[PoolManager] 프리팹에 UnitBase 컴포넌트가 없습니다: {unitData.Unit_Name}");
                return null;
            }

            prefabDictionary[id] = unitPrefab;
            poolDictionary[id] = new Queue<UnitBase>(); // 새로운 큐 생성
        }

        UnitBase unit = null;
        Queue<UnitBase> pool = poolDictionary[id];

        // 2. 풀(Queue) 안에 대기 중인(비활성화된) 유닛이 있다면 꺼내서 재사용 (Dequeue)
        if (pool.Count > 0)
        {
            unit = pool.Dequeue();
        }
        else
        {
            // 3. 풀이 텅 비어있다면 새로 생성 (Instantiate)
            unit = Instantiate(prefabDictionary[id], transform);
        }

        // 4. 위치, 회전 세팅 후 활성화
        if (unit != null)
        {
            unit.transform.position = position;
            unit.transform.rotation = rotation;
            unit.gameObject.SetActive(true);
        }

        return unit;
    }

    /// <summary>
    /// 사용이 끝난 유닛을 파괴하지 않고 비활성화한 뒤 다시 풀(Queue)에 집어넣는 함수
    /// </summary>
    public void ReturnToPool(UnitBase unit)
    {
        if (unit == null || unit.MyUnitData == null) return;

        int id = unit.MyUnitData.Unit_Id;

        // 유닛을 화면에서 보이지 않게 비활성화
        unit.gameObject.SetActive(false);

        // 해당 ID의 풀이 존재하면 큐에 다시 집어넣기 (Enqueue)
        if (poolDictionary.ContainsKey(id))
        {
            poolDictionary[id].Enqueue(unit);
        }
        else
        {
            // 혹시 모를 예외 상황: 관리되지 않는 유닛이라면 그냥 파괴
            Destroy(unit.gameObject);
        }
    }
}