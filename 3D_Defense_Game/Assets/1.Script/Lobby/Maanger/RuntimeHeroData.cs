using UnityEngine;


[CreateAssetMenu(fileName = "RunData", menuName = "Scriptable Objects/RunHeroData")]
public class RuntimeHeroData : ScriptableObject
{
    [Header("영웅 프리팹")]
    public GameObject prefab;
    [Header("표시용 값들")]
    public int Hero_Id;
    public string hero_PullName;
    public string hero_name;
    public Sprite portrait;
    public CharacterType type;

    [Header("기본스탯")]
    public int hp;
    public int mp;
    public int attack;
    public int defense;
    public float attackSpeed;
    public float moveSpeed;

    [Header("레벨당 증가 스탯")]
    public int hpPerLevel;           // 레벨당 추가 체력
    public int mpPerLevel;           // 레벨당 추가 마나
    public int attackPerLevel;       // 레벨당 추가 공격력
    public int defensePerLevel;      // 레벨당 추가 방어력
    public float attackSpeedPerLevel; // 레벨당 추가 공격속도 
    public float moveSpeedPerLevel;  // 레벨당 추가 이동속도 
}
