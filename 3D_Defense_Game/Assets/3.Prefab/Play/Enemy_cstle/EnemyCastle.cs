using System.Collections.Generic;
using UnityEngine;

public class EnemyCastle : MonoBehaviour, IDamageable
{   
    // 유닛의 FSM 상태 정의
    public enum UnitState
    {
        Idle,
        Chase,
        Attack,
        Die
    }

    // 싱글톤 구현
    public static EnemyCastle I { get; private set; }

    [Header("적 성 체력")]
    private float maxHp;
    private float currentHp;
    private CastleUI castleUI;

    [Header("적 성 데이터")]
    [SerializeField] private EnemyInfoSO enemyInfoSO;

    [Header("적 영웅 / 유닛 데이터 (BattleEntryData에서 받아옴)")]
    private List<HeroData> Heros = new List<HeroData>();
    private List<UnitData> Units = new List<UnitData>();

    [Header("팀 설정 (적 팀)")]
    public Team enemyTeam = Team.Enemy;

    [Header("적 유닛이 나올 출구 포인트들")]
    [SerializeField] private Transform[] exitPoints;

    [Header("공격 포인트")]
    [SerializeField] public Transform[] AttackPoint = new Transform[3];


    [Header("테스트 소환 설정")]
    [SerializeField] private int targetUnitId = 111; // 테스트할 유닛 ID
    [SerializeField]private float spawnTimer = 0f;                   // 2초 쿨타임 타이머
    [SerializeField]private float spawnInterval = 2f;                // 소환 간격 (2초)
    [SerializeField]private UnitData targetUnitData = null;          // 찾은 ID 111 유닛 데이터\
    [SerializeField]private bool isSpawning = false; // 소환여부
    [SerializeField]private int EnemyCount = 0;

    private void Awake()
    {
        if (I == null)
        {
            I = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!isSpawning || EnemyCount <= 0) return;

        // 타이머 누적
        spawnTimer += Time.deltaTime;

        // 2초가 지났다면 소환 실행 후 타이머 초기화
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f; 
            TrySpawnUnit();
        }
    }

    public void Init(EnemyInfoSO data)
    {
        enemyInfoSO = data;
        LoadDataFromEntry();

        if (enemyInfoSO != null)
        {
            EnemyCount = Random.Range(enemyInfoSO.minEnemyCount, enemyInfoSO.maxEnemyCount);
            spawnTimer = 0f;
            targetUnitData = Units.Find(u => u.Unit_Id == targetUnitId);
            isSpawning = true;
        }
    }
    
    /// <summary>
    /// BattleEntryData로부터 적 유닛과 영웅 데이터를 가져오는 함수
    /// </summary>
    private void LoadDataFromEntry()
    {
        // 1. 적 유닛 데이터 가져오기
        if (BattleEntryData.EnemyUnit != null && BattleEntryData.EnemyUnit.Count > 0)
        {
            Units = new List<UnitData>(BattleEntryData.EnemyUnit);
            Debug.Log($"[EnemyCastle] 적 유닛 데이터 {Units.Count}개 연동 완료!");
        }
        else
        {
            Debug.LogWarning("[EnemyCastle] BattleEntryData에 연동할 적 유닛 데이터가 없습니다!");
        }

        // 2. 적 영웅 데이터 가져오기
        if (BattleEntryData.EnemyHero != null && BattleEntryData.EnemyHero.Count > 0)
        {
            Heros = new List<HeroData>(BattleEntryData.EnemyHero);
            Debug.Log($"[EnemyCastle] 적 영웅 데이터 {Heros.Count}개 연동 완료!");
        }
        else
        {
            Debug.LogWarning("[EnemyCastle] BattleEntryData에 연동할 적 영웅 데이터가 없습니다!");
        }
    }

    /// <summary>
    /// 제한 횟수 체크 후 UnitPoolManager를 통해 유닛을 소환하는 함수
    /// </summary>
    private void TrySpawnUnit()
    {
        if (targetUnitData == null) return;

        // 1. 성에서 오프셋이 적용된 스폰 위치와 회전 데이터를 한 번에 받아옴
        var spawnData = GetRandomSpawnData();

        // 2. UnitPoolManager를 통해 유닛 가져오기 (풀링 사용)
        UnitBase spawnedUnit = UnitPoolManager.I.GetUnit(targetUnitData, spawnData.position, spawnData.rotation);

        if (spawnedUnit != null)
        {
            // 적 팀(Team.Enemy)으로 초기화 (BattlefieldManager에 자동 등록됨)
            spawnedUnit.Init(targetUnitData, enemyTeam);
            EnemyCount--;
            Debug.Log($"[EnemyCastle] 테스트 유닛 소환 완료: {targetUnitData.Unit_Name} (남은 횟수: {EnemyCount})");
        }
        else
        {
            Debug.LogWarning("[EnemyCastle] 유닛 풀에서 유닛을 가져오지 못했습니다!");
        }
    }
    /// <summary>
    /// 유닛의 현재 위치를 받아, 성의 AttackPoint 중 가장 가까운 포인트를 반환합니다.
    /// </summary>
    public Transform GetClosestAttackPoint(Vector3 currentPos)
    {
        if (AttackPoint == null || AttackPoint.Length == 0) return null;

        Transform closestPoint = AttackPoint[0];
        float minSqrDist = (closestPoint.position - currentPos).sqrMagnitude;

        for (int i = 1; i < AttackPoint.Length; i++)
        {
            if (AttackPoint[i] == null) continue;
            float sqrDist = (AttackPoint[i].position - currentPos).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                closestPoint = AttackPoint[i];
            }
        }

        return closestPoint;
    }
    /// <summary>
    /// 랜덤 출구 위치(Z축 오프셋 포함)와 해당 출구의 회전값을 세트로 반환합니다.
    /// </summary>
    public (Vector3 position, Quaternion rotation) GetRandomSpawnData()
    {
        if (exitPoints == null || exitPoints.Length == 0)
        {
            Debug.LogWarning("[EnemyCastle] 등록된 출구(exitPoints)가 없습니다!");
            return (transform.position, transform.rotation);
        }

        int randomIndex = Random.Range(0, exitPoints.Length);
        Transform point = exitPoints[randomIndex];

        // Z축에 +- 1.5 범위의 랜덤 값 부여하여 겹침 방지
        float randomZ = Random.Range(-1.5f, 1.5f);
        Vector3 spawnPos = point.position + new Vector3(0f, 0f, randomZ);

        return (spawnPos, point.rotation);
    }

    /// <summary>
    /// 적 성 체력 초기화 및 UI 연결
    /// </summary>
    public void HPInit(float maxHp, float currentHp, CastleUI ui)
    {
        this.maxHp = maxHp;
        this.currentHp = currentHp;
        this.castleUI = ui; // UI 연결 완료!
    }

    /// <summary>
    /// IDamageable 구현: 플레이어 유닛에게 공격받았을 때 호출
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (currentHp <= 0) return;

        currentHp -= damage;
        currentHp = Mathf.Max(0f, currentHp);

        // ⭐ 체력이 깎일 때 적 성 UI 동기화
        if (castleUI != null)
        {
            castleUI.SetHP(currentHp, maxHp);
        }

        Debug.Log($"[EnemyCastle] 적 성 피격! 남은 체력: {currentHp} / {maxHp}");

        if (currentHp <= 0)
        {
            OnCastleDestroyed();
        }
    }

    private void OnCastleDestroyed()
    {
        Debug.Log("적성 파괴");
        BattleResultManager.I.Open();
    }
}