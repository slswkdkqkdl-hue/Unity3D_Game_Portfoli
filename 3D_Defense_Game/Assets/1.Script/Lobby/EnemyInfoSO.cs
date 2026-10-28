
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum EnemyFactionRank
{
    Barony,      // 남작령
    Viscounty,   // 자작령
    County,      // 백작령
    Duchy,       // 공작령
    Kingdom      // 왕국
}

[System.Serializable]
public struct EnemyLootInfo
{
    public int item_id;
    public string item_name;
}

/// <summary>
/// 적 유닛별 소환 설정
/// unitId: 유닛 데이터 ID
/// spawnWeight: 출현 가중치. 값이 높을수록 자주 선택됨.
/// </summary>
[System.Serializable]
public class EnemyUnitSpawnSetting
{
    [Header("유닛 ID")]
    public int unitId;

    [Header("출현 가중치")]
    [Min(0)]
    public int spawnWeight = 100;
}

[CreateAssetMenu(fileName = "EnemyInfo", menuName = "Game/Enemy Info")]
public class EnemyInfoSO : ScriptableObject
{
    [Header("거점 정보")]
    public int Stage_id;
    public string settlementName;
    public EnemyFactionRank factionRank;
    public Sprite image;
    [Header("적 성")]
    public GameObject Enemy_castle;

    [Header("스테이지")]
    public AssetReferenceT<StageData> stageRef;

    [TextArea]
    public string specialNote;

    [Header("적 병력")]
    [Min(0)]
    public int minEnemyCount;

    [Min(0)]
    public int maxEnemyCount;

    [Header("성벽 체력용 세력 등급")]
    public DomainTier Tier = DomainTier.EmergingPower;

    [Header("주요 전리품")]
    public List<EnemyLootInfo> mainLoot = new List<EnemyLootInfo>();

    [Header("적 유닛 및 출현 비율")]
    [Tooltip("유닛 ID와 출현 가중치를 설정합니다. 가중치가 높을수록 더 자주 선택됩니다.")]
    public List<EnemyUnitSpawnSetting> unitSpawnSettings
        = new List<EnemyUnitSpawnSetting>();

    [Header("적 영웅")]
    [Tooltip("전투에 등장할 적 영웅 ID 목록입니다.")]
    public List<int> Enemy_Hero = new List<int>();

    [Header("기본 소환 설정")]
    [Tooltip("전투 시작 후 최초 소환까지 대기하는 시간(초)")]
    [Min(0f)]
    public float initialSpawnDelay = 0f;

    [Tooltip("일반 소환 사이의 대기 시간(초)")]
    [Min(0.1f)]
    public float spawnInterval = 2f;

    [Tooltip("한 번의 소환에서 등장할 수 있는 최대 인원")]
    [Min(1)]
    public int maxUnitsPerSpawn = 3;

    [Header("성 체력 감소 시 증원")]
    [Tooltip("성 체력이 이 비율 이하로 떨어지면 증원이 발동합니다.")]
    [Range(0f, 100f)]
    public float reinforcementHpPercent = 50f;

    [Tooltip("성 체력 기준에 처음 도달했을 때 추가 소환할 인원")]
    [Min(1)]
    public int reinforcementCount = 5;
    
}
