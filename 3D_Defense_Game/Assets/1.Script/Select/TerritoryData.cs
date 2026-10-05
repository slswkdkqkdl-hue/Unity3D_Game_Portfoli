using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "Domain Data", menuName = "Scriptable Objects/Domain Data")]
public class TerritoryData : ScriptableObject
{
    public string territoryName;        // 영지 이름 
    public Sprite territoryEmblem;      // 영지 엠블럼 이미지
    public Sprite territoryBackGround; // 영지 배경이미지
    public DomainType domainType; // 영지 구분용 어떤게 선택했냐 맵씬에서쓸거임
    [TextArea] public string DomainDescription; // 영지 설명
    [TextArea] public string passiveDescription; // 영지 패시브 설명

    [Header("Hero Addressable Reference")]
    public AssetReferenceT<HeroData> heroRef; 

    [Header("Unit Addressable References")]
    public List<AssetReferenceT<UnitData>> unitRefs = new List<AssetReferenceT<UnitData>>();

    // 얘는 게임실행중에 런타임용을 잔깐 사용할것 담아두기 데이터  에셋파일 저장하면안댐 
    [System.NonSerialized] public HeroData heroDescDatas;
    [System.NonSerialized] public List<UnitData> unitDescDatas = new List<UnitData>();
}