using UnityEngine;

public enum NationTier
{
    SmallNation,  // 소국
    StrongNation, // 강국
    GreatNation   // 대국
}

public class NationData
{
    public NationTier nationTier = NationTier.SmallNation; // 국가 티어
    public int NationPoint = 0;// 국가 성장 포인트 
}
