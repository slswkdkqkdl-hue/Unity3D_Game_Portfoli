using UnityEngine;

public class CastleController : MonoBehaviour
{
    [Header("각 성벽 UI 데이터")]
    [SerializeField] private CastleUI playerCastleData;
    [SerializeField] private CastleUI enemyCastleData;

    public void Init(DomainData playerData, EnemyInfoSO enemyData)
    {
        // 1. 플레이어 성벽 UI 초기화
        SetupPlayerCastle(playerData);

        // 2. 적 성벽 UI 초기화
        SetupEnemyCastle(enemyData);
    }

    /// <summary>
    /// 플레이어 성벽 UI 초기화
    /// </summary>
    private void SetupPlayerCastle(DomainData playerData)
    {
        if (playerData == null || playerCastleData == null) return;

        Sprite emblem = playerData.domain_emblem;
        float maxHp = FactionUtility.GetDomainTierRequirement(playerData.domainTier);
        float currentHp = maxHp; // 초기화 시점에는 풀피로 설정

        playerCastleData.InitCastleUI(emblem, maxHp, currentHp);
    }

    /// <summary>
    /// 적 성벽 UI 초기화
    /// </summary>
    private void SetupEnemyCastle(EnemyInfoSO enemyData)
    {
        if (enemyData == null || enemyCastleData == null) return;

        Sprite emblem = enemyData.image; // 적 영지 아이콘
        float maxHp = FactionUtility.GetEnemyCastleHp(enemyData.factionRank);
        float currentHp = maxHp;

        enemyCastleData.InitCastleUI(emblem, maxHp, currentHp);
    }
}