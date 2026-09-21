using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{   
    public static SelectManager I;

    // private이지만 [SerializeField]를 붙이면 유니티 인스펙터에서 눈으로 확인 가능
    [Header("현재 선택된 데이터 (확인용)")]
    [SerializeField] private HeroDescData selectedHero;
    [SerializeField] private List<UnitDescData> selectedUnits = new List<UnitDescData>();

    void Awake()
    {
        I = this;
    }

    /// <summary>
    /// 영웅 및 유닛 데이터 선택 (외부에서 데이터 설정 함수)
    /// </summary>
    public void SelectHero(HeroDescData hero, List<UnitDescData> units)
    {
        selectedHero = hero;

        selectedUnits.Clear();
        if (units != null)
        {
            selectedUnits.AddRange(units);
        }

        // UI 갱신 요청
        if (UIManager_Select.I != null)
        {
            UIManager_Select.I.SelectData(selectedHero, selectedUnits);
        }
    }

    // ==========================================
    // 외부에서 데이터를 안전하게 가져가는 함수들 (Getters)
    // ==========================================

    /// <summary>
    /// 현재 선택된 영웅 데이터 가져오기
    /// </summary>
    public HeroDescData GetSelectedHero()
    {
        return selectedHero;
    }

    /// <summary>
    /// 현재 선택된 유닛 리스트 가져오기
    /// </summary>
    public List<UnitDescData> GetSelectedUnits()
    {
        return new List<UnitDescData>(selectedUnits);
    }
}