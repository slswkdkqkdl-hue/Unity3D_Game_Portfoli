using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;



[System.Serializable]
public struct HeroEntry
{
    public int heroId;
    public AssetReferenceT<HeroData> heroRef; 
}


[CreateAssetMenu(fileName = "HeroDB", menuName = "Database/HeroDB")]
public class HeroDB : ScriptableObject
{
    [Header("영웅 DB 목록")]
    [SerializeField]private List<HeroEntry> heroList = new List<HeroEntry>();


    /// <summary>
    /// ID 기반 영웅 어드레서블 참조 검색
    /// </summary>
    public AssetReferenceT<HeroData> GetHeroRef(int heroId)
    {
        var entry = heroList.Find(x => x.heroId == heroId);
        return entry.heroRef; // 레퍼런스 주소만 반환
    }
}
