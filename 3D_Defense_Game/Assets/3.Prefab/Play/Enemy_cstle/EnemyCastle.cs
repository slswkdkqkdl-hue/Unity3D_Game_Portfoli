
using System.Collections.Generic;
using UnityEngine;

public class EnemyCastle : MonoBehaviour, IDamageable
{
    public static EnemyCastle I { get; private set; }

    [Header("적 성 체력")]
    private float maxHp;
    private float currentHp;
    private CastleUI castleUI;

    [Header("적 성 데이터")]
    [SerializeField] private EnemyInfoSO enemyInfoSO;

    [Header("로드된 적 유닛 데이터")]
    private readonly List<UnitData> units = new List<UnitData>();

    [Header("팀 설정")]
    public Team enemyTeam = Team.Enemy;

    [Header("적 유닛 출구")]
    [SerializeField] private Transform[] exitPoints;

    [Header("공격 포인트")]
    public Transform[] AttackPoint = new Transform[3];

    // 소환 관리
    private float spawnTimer;
    private bool isSpawning;
    private bool initialDelayComplete;
    private bool reinforcementTriggered;

    // 앞으로 소환할 유닛 수
    private int enemyCount;

    private void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (I == this)
            I = null;
    }

    private void Update()
    {
        if (!isSpawning || enemyCount <= 0 || enemyInfoSO == null)
            return;

        spawnTimer += Time.deltaTime;

        // 최초 소환 대기
        if (!initialDelayComplete)
        {
            if (spawnTimer < enemyInfoSO.initialSpawnDelay)
                return;

            initialDelayComplete = true;
            spawnTimer = 0f;

            TrySpawnBatch();
            return;
        }

        // 이후에는 설정된 간격마다 소환
        if (spawnTimer >= enemyInfoSO.spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnBatch();
        }
    }

    /// <summary>
    /// 적 성 초기화
    /// </summary>
    public void Init(EnemyInfoSO data)
    {
        if (data == null)
        {
            Debug.LogError("[EnemyCastle] EnemyInfoSO가 없습니다!");
            isSpawning = false;
            return;
        }

        enemyInfoSO = data;

        units.Clear();
        enemyCount = 0;
        spawnTimer = 0f;
        initialDelayComplete = false;
        reinforcementTriggered = false;
        isSpawning = false;

        LoadDataFromEntry();

        if (units.Count == 0)
        {
            Debug.LogError("[EnemyCastle] 사용할 수 있는 적 유닛이 없습니다!");
            return;
        }

        if (!HasValidSpawnWeight())
        {
            Debug.LogError("[EnemyCastle] 출현 가중치가 유효한 유닛이 없습니다!");
            return;
        }

        int minCount = Mathf.Max(0, data.minEnemyCount);
        int maxCount = Mathf.Max(0, data.maxEnemyCount);

        if (minCount > maxCount)
        {
            Debug.LogWarning("[EnemyCastle] 최소 병력이 최대 병력보다 큽니다. 값을 교환합니다.");

            int temp = minCount;
            minCount = maxCount;
            maxCount = temp;
        }

        // 최대값도 포함하여 전체 소환 병력 결정
        enemyCount = Random.Range(minCount, maxCount + 1);
        isSpawning = enemyCount > 0;

        Debug.Log(
            $"[EnemyCastle] 초기화 완료 / 예정 병력: {enemyCount} / " +
            $"사용 가능한 유닛 종류: {units.Count}"
        );
    }

    /// <summary>
    /// EnemyInfoSO에 설정된 ID와 일치하는 유닛 데이터를 가져온다.
    /// </summary>
    private void LoadDataFromEntry()
    {
        if (BattleEntryData.EnemyUnit == null)
        {
            Debug.LogWarning("[EnemyCastle] 로드된 적 유닛 목록이 없습니다!");
            return;
        }

        if (enemyInfoSO.unitSpawnSettings == null)
            return;

        foreach (EnemyUnitSpawnSetting setting in enemyInfoSO.unitSpawnSettings)
        {
            if (setting == null)
                continue;

            UnitData unit = BattleEntryData.EnemyUnit.Find(
                u => u != null && u.Unit_Id == setting.unitId
            );

            if (unit == null)
            {
                Debug.LogWarning(
                    $"[EnemyCastle] 유닛 ID {setting.unitId}의 데이터를 찾지 못했습니다."
                );
                continue;
            }

            if (!units.Contains(unit))
                units.Add(unit);
        }

        Debug.Log($"[EnemyCastle] 사용할 적 유닛 {units.Count}종 로드 완료");
    }

    /// <summary>
    /// 실제 로드된 유닛 중 출현 가중치가 양수인 유닛이 있는지 확인한다.
    /// </summary>
    private bool HasValidSpawnWeight()
    {
        foreach (EnemyUnitSpawnSetting setting in enemyInfoSO.unitSpawnSettings)
        {
            if (setting == null || setting.spawnWeight <= 0)
                continue;

            if (units.Exists(u => u != null && u.Unit_Id == setting.unitId))
                return true;
        }

        return false;
    }

    /// <summary>
    /// 출현 가중치에 따라 소환할 유닛을 선택한다.
    /// </summary>
    private UnitData SelectRandomUnit()
    {
        int totalWeight = 0;

        foreach (EnemyUnitSpawnSetting setting in enemyInfoSO.unitSpawnSettings)
        {
            if (setting == null || setting.spawnWeight <= 0)
                continue;

            if (units.Exists(u => u != null && u.Unit_Id == setting.unitId))
                totalWeight += setting.spawnWeight;
        }

        if (totalWeight <= 0)
            return null;

        int ticket = Random.Range(0, totalWeight);
        int accumulatedWeight = 0;

        foreach (EnemyUnitSpawnSetting setting in enemyInfoSO.unitSpawnSettings)
        {
            if (setting == null || setting.spawnWeight <= 0)
                continue;

            UnitData unit = units.Find(
                u => u != null && u.Unit_Id == setting.unitId
            );

            if (unit == null)
                continue;

            accumulatedWeight += setting.spawnWeight;

            if (ticket < accumulatedWeight)
                return unit;
        }

        return null;
    }

    /// <summary>
    /// 설정된 인원만큼 적 유닛을 소환한다.
    /// </summary>
    private void TrySpawnBatch()
    {
        if (enemyInfoSO == null || enemyCount <= 0)
        {
            isSpawning = false;
            return;
        }

        if (UnitPoolManager.I == null)
        {
            Debug.LogError("[EnemyCastle] UnitPoolManager가 없습니다!");
            return;
        }

        int batchCount = Mathf.Min(
            Mathf.Max(1, enemyInfoSO.maxUnitsPerSpawn),
            enemyCount
        );

        for (int i = 0; i < batchCount; i++)
        {
            UnitData selectedUnit = SelectRandomUnit();

            if (selectedUnit == null)
            {
                Debug.LogWarning("[EnemyCastle] 소환할 유닛을 선택하지 못했습니다.");
                break;
            }

            var spawnData = GetRandomSpawnData();

            UnitBase spawnedUnit = UnitPoolManager.I.GetUnit(
                selectedUnit,
                spawnData.position,
                spawnData.rotation
            );

            if (spawnedUnit == null)
            {
                Debug.LogWarning(
                    $"[EnemyCastle] 유닛 소환 실패: {selectedUnit.Unit_Name}"
                );
                break;
            }

            spawnedUnit.Init(selectedUnit, enemyTeam);
            enemyCount--;

            Debug.Log(
                $"[EnemyCastle] {selectedUnit.Unit_Name} 소환 완료 / " +
                $"남은 예정 병력: {enemyCount}"
            );
        }

        if (enemyCount <= 0)
        {
            isSpawning = false;
            Debug.Log("[EnemyCastle] 예정된 일반 유닛 소환 완료");
        }
    }

    /// <summary>
    /// 성 체력이 설정된 비율 이하로 내려가면 추가 병력을 한 번 소환한다.
    /// </summary>
    private void CheckReinforcement()
    {
        if (enemyInfoSO == null || reinforcementTriggered || maxHp <= 0f)
            return;

        float hpPercent = currentHp / maxHp * 100f;

        if (hpPercent > enemyInfoSO.reinforcementHpPercent)
            return;

        reinforcementTriggered = true;

        int additionalCount = Mathf.Max(0, enemyInfoSO.reinforcementCount);
        enemyCount += additionalCount;

        if (enemyCount > 0)
        {
            isSpawning = true;

            // 증원에는 최초 소환 대기를 다시 적용하지 않는다.
            initialDelayComplete = true;
        }

        Debug.Log(
            $"[EnemyCastle] 성 체력 {hpPercent:F1}% 도달 / " +
            $"추가 소환 예정: {additionalCount}명"
        );
    }

    /// <summary>
    /// 가장 가까운 공격 포인트를 반환한다.
    /// </summary>
    public Transform GetClosestAttackPoint(Vector3 currentPos)
    {
        if (AttackPoint == null || AttackPoint.Length == 0)
            return null;

        Transform closestPoint = null;
        float minSqrDist = float.MaxValue;

        foreach (Transform point in AttackPoint)
        {
            if (point == null)
                continue;

            float sqrDist = (point.position - currentPos).sqrMagnitude;

            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                closestPoint = point;
            }
        }

        return closestPoint;
    }

    /// <summary>
    /// 랜덤 출구 위치와 회전값을 반환한다.
    /// </summary>
    public (Vector3 position, Quaternion rotation) GetRandomSpawnData()
    {
        if (exitPoints == null || exitPoints.Length == 0)
        {
            Debug.LogWarning("[EnemyCastle] 등록된 출구가 없습니다!");
            return (transform.position, transform.rotation);
        }

        int validCount = 0;

        foreach (Transform point in exitPoints)
        {
            if (point != null)
                validCount++;
        }

        if (validCount == 0)
            return (transform.position, transform.rotation);

        int randomIndex = Random.Range(0, validCount);
        Transform selectedPoint = null;

        foreach (Transform point in exitPoints)
        {
            if (point == null)
                continue;

            if (randomIndex-- == 0)
            {
                selectedPoint = point;
                break;
            }
        }

        float randomZ = Random.Range(-1.5f, 1.5f);

        Vector3 spawnPos = selectedPoint.position
                         + new Vector3(0f, 0f, randomZ);

        return (spawnPos, selectedPoint.rotation);
    }

    /// <summary>
    /// 적 성 체력 초기화 및 UI 연결
    /// </summary>
    public void HPInit(float maxHp, float currentHp, CastleUI ui)
    {
        this.maxHp = Mathf.Max(0f, maxHp);
        this.currentHp = Mathf.Clamp(currentHp, 0f, this.maxHp);
        castleUI = ui;

        if (castleUI != null)
            castleUI.SetHP(this.currentHp, this.maxHp);

        CheckReinforcement();
    }

    /// <summary>
    /// 성 피격
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (currentHp <= 0f || damage <= 0)
            return;

        currentHp = Mathf.Max(0f, currentHp - damage);

        if (castleUI != null)
            castleUI.SetHP(currentHp, maxHp);

        CheckReinforcement();

        Debug.Log(
            $"[EnemyCastle] 성 피격 / 남은 체력: {currentHp} / {maxHp}"
        );

        if (currentHp <= 0f)
            OnCastleDestroyed();
    }

    private void OnCastleDestroyed()
    {
        isSpawning = false;

        Debug.Log("[EnemyCastle] 적 성 파괴");

        if (BattleResultManager.I != null)
            BattleResultManager.I.Open();
    }
}
