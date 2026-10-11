using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 1. 영지/국가 성장 단계 (티어)
public enum DomainTier
{
    EmergingPower,  // 신흥 세력
    InfluentialPower, // 유력 세력
    RisingPower,    // 강성 세력
    Hegemony,       // 패권 세력
    ContinentalSupremacy // 대륙의 패자
}

// 2. 선택 가능한 영지(세력) 종류
public enum DomainType
{   
    None,
    Greenhaven, // 영지 A
    WhiteMoon, // 영지 B
    FactionC  // 영지 C
}

// 3. 영지 데이터 클래스
[System.Serializable]
public class DomainData
{   
    public DomainType  domainType = DomainType.None;
    public Sprite domain_emblem;
    public DomainTier domainTier = DomainTier.EmergingPower;   // 현재 영지 티어 (소국 -> 강국 -> 대국)
    public List<UpgradeSaveData> upgradedata = new List<UpgradeSaveData>();
    public float Np = 0f; // 소유한 국가 포인트
}