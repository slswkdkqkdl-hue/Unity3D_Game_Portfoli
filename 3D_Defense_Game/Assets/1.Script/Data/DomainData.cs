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
    FactionA, // 영지 A
    FactionB, // 영지 B
    FactionC  // 영지 C
}

// 3. 영지 데이터 클래스
[System.Serializable]
public class DomainData
{
    public DomainType domainType = DomainType.FactionA; // 선택한 영지 종류
    public DomainTier domainTier = DomainTier.Small;   // 현재 영지 티어 (소국 -> 강국 -> 대국)
    public int growthPoint = 0;                         // 영지 성장 포인트
}