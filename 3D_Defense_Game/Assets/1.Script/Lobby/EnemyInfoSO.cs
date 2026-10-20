
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

[CreateAssetMenu(fileName = "EnemyInfo", menuName = "Game/Enemy Info")]
public class EnemyInfoSO : ScriptableObject
{
    [Header("거점 정보")]
    public int Stage_id;  // 검색용 id
    public string settlementName;       // 거점명
    public EnemyFactionRank factionRank;     // 세력 등급
    public Sprite image; // 적 영지 아이콘 

    [Header("스테이지")]
    public AssetReferenceT<StageData> stageRef;

    [TextArea]
    public string specialNote;          // 특이사항

    [Header("적 병력")]
    public int minEnemyCount;           // 최소 예상 병력
    public int maxEnemyCount;           // 최대 예상 병력

    [Header("성벽체력용 세력등급")]
    public DomainTier Tier = DomainTier.EmergingPower;

    [Header("주요 전리품")]
    public List<EnemyLootInfo> mainLoot = new List<EnemyLootInfo>();

    [Header("적 유닛")]
    public List<int> Enemy_Unit = new List<int>();
    
    [Header("적 영웅")]
    public List<int> Enemy_Hero = new List<int>();
}
