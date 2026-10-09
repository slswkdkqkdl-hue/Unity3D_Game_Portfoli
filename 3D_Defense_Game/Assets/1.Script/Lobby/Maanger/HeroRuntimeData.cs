using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeroRuntimeData
{
    // 원본 정의 데이터
    public HeroData heroData;

    // 현재 플레이어 상태
    public int level = 0;
    public int sp = 0;

    public int equippedWeaponId =1;
    public int equippedArmorId;

    public List<int> selectedPassiveSkill = new List<int>();
    public List<int> selectedActiveSkill = new  List<int>();
}