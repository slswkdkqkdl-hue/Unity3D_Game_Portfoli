using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Domain Data", menuName = "Scriptable Objects/Domain Data")]
public class TerritoryData : ScriptableObject
{
    public string territoryName;        // 영지 이름 
    public Sprite territoryEmblem;      // 영지 엠블럼 이미지
    public Sprite territoryBackGround; // 영지 배경이미지
    public DomainType domainType; // 영지 구분용 어떤게 선택했냐 맵씬에서쓸거임
    [TextArea] public string DomainDescription; // 영지 설명
    [TextArea] public string passiveDescription; // 영지 패시브 설명

    [Header("Hero Info")]
    public HeroDescData heroDescDatas; // 영웅 정보

    [Header("Exclusive Units")]
    public List<UnitDescData> unitDescDatas = new List<UnitDescData>(); // 유닛 정보 
}