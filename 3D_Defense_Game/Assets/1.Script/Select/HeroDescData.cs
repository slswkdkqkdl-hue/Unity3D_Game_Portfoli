using UnityEngine;

public enum CharacterType
{
    Melee,      // 근접 
    Ranged,     // 원거리 
    Magic       // 마법
}

[CreateAssetMenu(fileName = "HeroDesc_", menuName = "Scriptable Objects/Hero Desc Data")]
public class HeroDescData : ScriptableObject
{   
    [Header("UI 보기용 영웅모델")]
    public GameObject prefab;
    [Header("UI 표시용 정보")]
    public int Hero_Id;
    public string hero_PullName;
    public string hero_name;
    public Sprite portrait;
    public CharacterType type; 
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