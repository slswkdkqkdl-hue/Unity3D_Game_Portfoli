using UnityEngine;

public enum BuildingType
{
    MainCastle,        // 메인 건물 (중심 성)
    Barracks_Basic,    // 기본 병영 (기본 3종 유닛)
    Barracks_Special,  // 특수 병영
    Infirmary,         // 치료소
    House              // 주민 집 (인구수/주거지 지원)
}

[CreateAssetMenu(fileName = "BuildingData", menuName = "Game/Building Data")]
public class BuildingDataSO : ScriptableObject
{
    [Header("기본 정보")]
    public int buildingId;
    public string buildingName;
    public BuildingType buildingType;
    public Sprite buildingIcon;       // 건설 UI에 띄울 아이콘

    [Header("비용 및 소환 유닛/영웅")]
    public int buildCost;

    [Header("3D 프리팹 및 UI 팝업 프리팹")]
    public GameObject buildingPrefab; // 실제 필드에 소환될 3D 건물 프리팹
    public GameObject popupUIPrefab;  // 건물 클릭 시 띄울 전용 UI 팝업 프리팹
}