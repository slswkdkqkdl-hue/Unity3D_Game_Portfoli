using UnityEngine;


[CreateAssetMenu(fileName = "RunUnitData", menuName = "Scriptable Objects/RunUnitData")]
public class UnitData : ScriptableObject
{
    [Header("유닛 프리팹 (전투)")]
    public GameObject prefab_Play;

    [Header("유닛 프리팹 (UI)")]
    public GameObject prefab_UI;

    [Header("표시용 값들")]
    public int Unit_Id; // 유닛 찾기용 고유id
    public string Unit_Name; // 유닛 병종
    public Sprite portrait; // 유닛 초상화
    public CharacterType type;    
    [Header("캐릭터 설명용")]
    [TextArea(3, 5)]
    public string description; 
    [Header("기본스탯")]
    public int hp;
    public int mp;
    public int attack;
    public int defense;
    public float attackSpeed;
    public float moveSpeed;
}
  