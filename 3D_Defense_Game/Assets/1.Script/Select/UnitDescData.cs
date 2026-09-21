using UnityEngine;

[CreateAssetMenu(fileName = "UnitDesc_", menuName = "Scriptable Objects/Unit Desc Data")]
public class UnitDescData : ScriptableObject
{
    [Header("UI 표시용 정보")]
    public int Unit_Id;
    public string unitClass;
    public Sprite portrait;
    [TextArea(3, 5)]
    public string description;

    [Header("UI 표시용 스탯")]
    public int hp;
    public int mp;
    public int attack;
    public int defense;
    public float attackSpeed;
    public float moveSpeed;
}