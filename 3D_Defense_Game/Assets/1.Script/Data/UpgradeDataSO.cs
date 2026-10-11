using UnityEngine;

public enum UpgradeCategory
{
    Melee,      // 근접 전투 숙련
    Ranged,     // 사격 숙련
    Magic,      // 비전 연구
    Economy,    // 경제학 연구
    Tactics     // 전술 지휘
}




[CreateAssetMenu(fileName = "NewUpgradeData", menuName = "ScriptableObjects/UpgradeData")]
public class UpgradeDataSO : ScriptableObject
{
    [Header("=== 카테고리 분류 ===")]
    public UpgradeCategory category;      // 큰 틀 구분용 Enum (Melee, Ranged, Magic, Economy, Tactics)

    [Header("=== 세부 정보 ===")]
    public string upgradeId;              // 세부 구분용 고유 ID (예: "Melee_Sword_Atk")
    public string upgradeName;            // 강화 이름 (예: "검술 연마")
    [TextArea] 
    public string description;            // 설명

    [Header("=== 강화 레벨 및 설정 ===")]
    public int maxLevel = 5;              // 최대 레벨 제한

    [Tooltip("레벨별 소모 SP (인덱스 0 = 0->1레벨 업글 비용)")]
    [SerializeField] private int[] costSPPerLevel = { 3, 4, 5, 6, 7 };

    [Tooltip("레벨별 상승 스탯 수치 (인덱스 0 = 1레벨 달성 시 가산치)")]
    [SerializeField] private float[] statBonusPerLevel = { 5f, 10f, 15f, 25f, 40f };

}