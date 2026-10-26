using System.Collections.Generic;
using UnityEngine;

public class EnemyCastle : MonoBehaviour
{
    // 싱글톤 구현
    public static EnemyCastle I { get; private set; }

    [Header("적 성 데이터")]
    [SerializeField] private EnemyInfoSO enemyInfoSO;

    [Header("적 영웅 / 유닛 데이터 (BattleEntryData에서 받아옴)")]
    private List<HeroData> Heros = new List<HeroData>();
    private List<UnitData> Units = new List<UnitData>();

    [Header("팀 설정 (적 팀)")]
    public Team enemyTeam = Team.Enemy;

    [Header("적 유닛이 나올 출구 포인트들")]
    [SerializeField] private Transform[] exitPoints;

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


    public void Init(EnemyInfoSO data)
    {
        enemyInfoSO = data;
        LoadDataFromEntry();
        if (enemyInfoSO != null)
        {
            int EnemyCount = Random.Range(enemyInfoSO.minEnemyCount, enemyInfoSO.maxEnemyCount);
            // 필요하다면 여기서 적 수에 따른 추가 초기화 진행
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
    /// 적 유닛이 소환될 때 랜덤한 출구(Transform)를 반환하는 함수
    /// </summary>
    public Transform RandomPoint()
    {
        if (exitPoints == null || exitPoints.Length == 0)
        {
            Debug.LogWarning("[EnemyCastle] 등록된 출구(ExitPoint)가 없습니다!");
            return null;
        }

        int randomIndex = Random.Range(0, exitPoints.Length);
        return exitPoints[randomIndex];
    }
}