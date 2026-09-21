using UnityEngine;

[CreateAssetMenu(fileName = "HeroDesc_", menuName = "Scriptable Objects/Hero Desc Data")]
public class HeroDescData : ScriptableObject
{
    [Header("UI 표시용 정보")]
    public int Hero_Id;
    public string heroName;
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