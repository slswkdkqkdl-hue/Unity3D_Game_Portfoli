using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{   
    public static SelectManager I;

    [Header("인스펙터 등록용 리스트 (SO 데이터를 여기에 드래그 앤 드롭)")]
    [SerializeField] private List<HeroDescData> heroDataList = new List<HeroDescData>();
    [SerializeField] private List<UnitDescData> unitDataList = new List<UnitDescData>();

    // ID 기반 데이터 사전 (런타임에 리스트를 통해 자동 채워짐)
    public Dictionary<int, HeroDescData> Hero = new Dictionary<int, HeroDescData>();
    public Dictionary<int, UnitDescData> Unit = new Dictionary<int, UnitDescData>();

    [Header("현재 선택된 데이터 (저장용)")]
    public HeroDescData selectedHero;
    public List<UnitDescData> selectedUnits = new List<UnitDescData>();

    void Awake()
    {
        I = this;

        // 게임 시작 시 리스트 데이터를 딕셔너리로 변환
        InitDictionaries();
    }

    /// <summary>
    /// 인스펙터의 List 데이터를 기반으로 Dictionary를 채우는 함수
    /// </summary>
    private void InitDictionaries()
    {
        Hero.Clear();
        foreach (var heroData in heroDataList)
        {
            if (heroData != null && !Hero.ContainsKey(heroData.Hero_Id))
            {
                Hero.Add(heroData.Hero_Id, heroData);
            }
        }

        Unit.Clear();
        foreach (var unitData in unitDataList)
        {
            if (unitData != null && !Unit.ContainsKey(unitData.Unit_Id))
            {
                Unit.Add(unitData.Unit_Id, unitData);
            }
        }
    }

    public void SelectHero(int Hero_id, List<int> Units)
    {
        // 1. 영웅 ID로 HeroDescData 검색 및 저장
        if (Hero.TryGetValue(Hero_id, out HeroDescData foundHero))
        {
            selectedHero = foundHero;
        }
        else
        {
            Debug.LogWarning($"ID [{Hero_id}]에 해당하는 HeroDescData가 딕셔너리에 없습니다.");
            selectedHero = null;
        }

        // 2. 전달받은 유닛 ID 리스트로 UnitDescData 목록 검색 및 저장
        selectedUnits.Clear();

        if (Units != null)
        {
            foreach (int unitId in Units)
            {
                if (Unit.TryGetValue(unitId, out UnitDescData foundUnit))
                {
                    selectedUnits.Add(foundUnit);
                }
                else
                {
                    Debug.LogWarning($"ID [{unitId}]에 해당하는 UnitDescData가 딕셔너리에 없습니다.");
                }
            }
        }

        // 3. UI 갱신 요청
        if (UIManager_Select.I != null)
        {
            UIManager_Select.I.SelectData(selectedHero, selectedUnits);
        }
    }
}