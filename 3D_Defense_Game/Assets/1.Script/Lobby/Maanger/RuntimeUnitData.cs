using UnityEngine;


[CreateAssetMenu(fileName = "RunUnitData", menuName = "Scriptable Objects/RunUnitData")]
public class RuntimeUnitData : ScriptableObject
{
    [Header("유닛 프리팹")]
    public GameObject prefab;
    [Header("표시용 값들")]
    public int Unit_Id; // 유닛 찾기용 고유id
    public string Unit_Name; // 유닛 병종
    public Sprite portrait; // 유닛 초상화
    public CharacterType type; 
    [Header("기본스탯")]
    public int hp;
    public int mp;
    public int attack;
    public int defense;
    public float attackSpeed;
    public float moveSpeed;
}
