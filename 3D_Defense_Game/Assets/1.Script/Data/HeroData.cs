using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    Warrior,
    Archer ,
    Mage 
}


[CreateAssetMenu(fileName = "RunHeroData", menuName = "Scriptable Objects/RunHeroData")]
public class HeroData : ScriptableObject
{
    [Header("영웅 프리팹 (전투)")]
    public GameObject prefab_Play;

    [Header("영웅 프리팹 (UI)")]
    public GameObject prefab_UI;

    [Header("표시용 값들")]
    public int Hero_Id;
    public int Current_Level;
    public string hero_PullName;
    public string hero_name;
    public Sprite portrait;
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

    [Header("레벨당 증가 스탯")]
    public int hpPerLevel;           // 레벨당 추가 체력
    public int mpPerLevel;           // 레벨당 추가 마나
    public int attackPerLevel;       // 레벨당 추가 공격력
    public int defensePerLevel;      // 레벨당 추가 방어력
    public float attackSpeedPerLevel; // 레벨당 추가 공격속도 
    public float moveSpeedPerLevel;  // 레벨당 추가 이동속도 

    public List<SkillData> activeSkills = new List<SkillData>(); // 모든 액티브 스킬
    public List<SkillData> passiveSkills = new List<SkillData>();// 모든 패시브 스킬

}
