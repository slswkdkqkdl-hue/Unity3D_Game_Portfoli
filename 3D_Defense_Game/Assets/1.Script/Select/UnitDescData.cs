using UnityEngine;

[CreateAssetMenu(fileName = "UnitDesc_", menuName = "Scriptable Objects/Unit Desc Data")]
public class UnitDescData : ScriptableObject
{
    [Header("UI 표시용 정보")]
    public int Unit_Id; // 유닛 찾기용 고유id
    public string unitClass; // 유닛 병종
    public Sprite portrait; // 유닛 초상화
    [TextArea(3, 5)]
    public string description; // 유닛설명

    [Header("UI 표시용 스탯")]
    public int hp;
    public int mp;
    public int attack;
    public int defense;
    public float attackSpeed;
    public float moveSpeed;
}
