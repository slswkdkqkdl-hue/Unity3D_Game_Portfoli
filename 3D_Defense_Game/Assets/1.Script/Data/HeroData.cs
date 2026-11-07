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
    public GameObject prefab_Play; // 실제 전투에서 사용할  영웅 프리팹

    [Header("영웅 프리팹 (UI)")]
    public GameObject prefab_UI; // UI 에서 보여줄 영웅 프리팹

    [Header("표시용 값들")]
    public int Hero_Id; // 그 영웅만에 고유 ID 검색용
    public int Current_Level; //현제 레벨
    public string hero_PullName; // 영웅 별명같은거
    public string hero_name; // 영웅 이름
    public Sprite portrait; // 영웅 초상화
    public CharacterType type; // 영웅 타입 

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

    [Header("사거리 및 탐색 설정")]
    public float detectionRange; // 감지범위
    public float attackRange; // 공격범위

    [Header("레벨업 설정")]
    public int maxLevel = 10;          // 최대 레벨
    public int baseExp = 100;          // 1레벨에서 2레벨로 갈 때 필요한 기본 경험치
    public float expMultiplier = 1.5f; // 레벨업할 때마다 필요 경험치 증가 배율  

    public List<SkillData> activeSkills = new List<SkillData>(); // 모든 액티브 스킬
    public List<SkillData> passiveSkills = new List<SkillData>();// 모든 패시브 스킬

}
