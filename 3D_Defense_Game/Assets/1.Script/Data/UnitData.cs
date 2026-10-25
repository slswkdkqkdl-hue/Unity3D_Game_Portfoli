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
    [Header("유닛 생산 또는 소환에 소모되는 비용")]
    public int Cost;

    [Space(10)]
    [Header("=== 캐릭터 설명 ===")]
    [TextArea(3, 5)]
    public string description; 

    [Header("=== 전투 기본 스탯 ===")]
    public int hp;
    public int mp;
    public int attack;
    public int defense;
    public float attackSpeed;
    public float moveSpeed;
}