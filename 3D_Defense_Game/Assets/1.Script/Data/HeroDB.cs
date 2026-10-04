using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;



[System.Serializable]
public struct HeroEntry
{
    public int heroId;
    public AssetReferenceT<RuntimeHeroData> heroRef; 
}


[CreateAssetMenu(fileName = "HeroDB", menuName = "Database/HeroDB")]
public class HeroDB : ScriptableObject
{
    [Header("영웅 DB 목록")]
    public List<HeroEntry> heroList = new List<HeroEntry>();


    /// <summary>
    /// ID 기반 영웅 어드레서블 참조 검색
    /// </summary>
    public AssetReferenceT<RuntimeHeroData> GetHeroRef(int heroId)
    {
        var entry = heroList.Find(x => x.heroId == heroId);

        if (entry.heroRef != null)
        {
            return entry.heroRef;
        }

        Debug.LogError($"[HeroDB] ID {heroId}에 해당하는 영웅 참조를 찾을 수 없습니다.");
        return null;
    }
}
