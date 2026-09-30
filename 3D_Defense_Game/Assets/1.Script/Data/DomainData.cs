using UnityEngine;

// 1. 영지/국가 성장 단계 (티어)
public enum DomainTier
{
    Small,   // 소국
    Strong,  // 강국
    Great    // 대국
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
    public DomainTier domainTier = DomainTier.Small;   // 현재 영지 티어 (소국 -> 강국 -> 대국)
    public float Np = 0f; // 소유한 국가 포인트
}