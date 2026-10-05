using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RunHeroData", menuName = "Scriptable Objects/RunHeroData")]
public class RuntimeHeroData : ScriptableObject
{
    [Header("영웅 프리팹")]
    public GameObject prefab;
    [Header("표시용 값들")]
    public int Hero_Id;
    public int Current_Level;
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


    [Header("장착중인 무기/방어구")]
    public int Equippedweapon_Id = -1; // 장착중인 무기 Id
    public int EquippedArmor_Id = -1; // 장착중인 방어구 Id

    [Header("선택한 패시브스킬,액티브스킬")]
    public List<int> SelectPassiveSkill = new List<int>(); // 선택한 패시브스킬
    public List<int> SelectActiveSkill = new List<int>(); // 선택한 액티브스킬
}
