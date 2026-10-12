using UnityEngine;

public static class FactionUtility 
{

    public static string FactionName(DomainType type)
    {
        switch (type)
        {   
            case DomainType.Greenhaven:
                return "그린헤이븐 영지";
            case DomainType.WhiteMoon:
                return "백월가문";
            case DomainType.None:
                return"";
        }
        return "";
    }

    public static string FactionTier(DomainTier type, int size)
    {
        if (size <= 5)
        {
            type = DomainTier.EmergingPower;
        }
        else if (size <= 10)
        {
            type = DomainTier.InfluentialPower;
        }
        else if (size <= 15)
        {
            type = DomainTier.RisingPower;
        }
        else if (size <= 20)
        {
            type = DomainTier.Hegemony;
        }
        else
        {
            type = DomainTier.ContinentalSupremacy;
        }

        // 결정된 티어에 따른 한글 명칭 반환
        return type switch
        {
            DomainTier.EmergingPower => "신흥 세력",
            DomainTier.InfluentialPower => "유력 세력",
            DomainTier.RisingPower => "강성 세력",
            DomainTier.Hegemony => "패권 세력",
            DomainTier.ContinentalSupremacy => "대륙의 패자",
            _ => "미지"
        };
    }

    public static string EnemyStageTier(EnemyFactionRank rank)
    {
        return rank switch
        {
            EnemyFactionRank.Barony => "남작령",
            EnemyFactionRank.Viscounty => "자작령",
            EnemyFactionRank.County => "백작령",
            EnemyFactionRank.Duchy => "공작령",
            EnemyFactionRank.Kingdom => "왕국",
            _ => "미지"
        };
    }
}
