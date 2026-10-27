using UnityEngine;

[CreateAssetMenu(fileName = "RunUnitData", menuName = "Scriptable Objects/RunUnitData")]
public class UnitData : ScriptableObject
{
    [Header("전투 씬에서 생성될 실제 유닛 프리팹")]
    public GameObject prefab_Play;
    
    [Header("UI 용 유닛 프리팹")]
    public GameObject prefab_UI;

    [Space(10)]
    [Header("유닛 찾기용 고유 ID")]
    public int Unit_Id; 
    
    [Header("유닛 병종 이름 (예: 검병, 궁병 등)")]
    public string Unit_Name; 
    
    [Header("유닛 초상화 아이콘")]
    public Sprite portrait; 
    
    [Header("캐릭터 전투/직업 타입")]
    public CharacterType type;

    [Space(10)]
    [Header("생산시간")]
    public int CoolTime;
    [Header("소모 인구수")]
    public int population;

    [Space(10)]
    [Header("=== 캐릭터 설명 ===")]
    [TextArea(3, 5)]
    public string description; 

    [Header("=== 전투 기본 스탯 ===")]
    public int hp; // 체력
    public int mp; // 마나
    public int attack; // 공격력
    public int defense; // 방어력
    public float attackSpeed; // 공격속도
    public float moveSpeed; // 이동속도
    public float detectRange; // 감지범위
    public float attackRange; // 공격범위
}