using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeroSaveData
{
    public int HeroId = -1; // 히어로 검색 ID
    public int Level = 1; // 히어로 레벨 (보통 1레벨부터 시작하므로 기본값 1 추천)
    public int SP = 0; // 성장포인트
    
    public int CurrentExp = 0; // 현재 쌓인 경험치
    public int MaxExp = 0;   // 다음 레벨업까지 필요한 경험치

    public int Equippedweapon_Id = -1; // 장착중인 무기 Id
    public int EquippedArmor_Id = -1; // 장착중인 방어구 Id
    public List<int> SelectPassiveSkill = new List<int>(); // 선택한 패시브스킬
    public List<int> SelectActiveSkill = new List<int>(); // 선택한 액티브스킬
}