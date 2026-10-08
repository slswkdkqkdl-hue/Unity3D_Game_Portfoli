using UnityEngine;

public enum SkillType
{
    Active,
    Passive
}

[CreateAssetMenu(fileName = "NewSkillData", menuName = "GameData/Skill Data", order = 2)]
public class SkillData : ScriptableObject
{
    [Header("기본 정보")]
    public int skill_Id;
    public string skill_name;
    [TextArea(3, 5)] public string description; // 설명이 길어져도 편하게 적을 수 있는 텍스트 박스
    public Sprite icon;                         // 스킬 아이콘 이미지

    [Header("스킬 타입")]
    public SkillType skillType;                 // 액티브인지 패시브인지 구분

    [Header("소모값 / 쿨타임")]
    public int spCost;                          // 소모 SP (UI에 보이던 보유 SP와 연동용)
    public float coolTime;                      // 쿨타임 (초)
    public int MpCost;                          // 사용시 필요한 마나
}