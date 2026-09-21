using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectButton : MonoBehaviour
{   
    [Header("영웅 & 유닛 SO 데이터 직접 연결")]
    public HeroDescData heroData;
    public List<UnitDescData> unitDatas = new List<UnitDescData>();

    [Header("UI 연결")]
    [SerializeField] private TextMeshProUGUI classNameText;
    [SerializeField] private Button button;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        bool isReady = heroData != null;

        if (classNameText != null)
        {
            classNameText.text = isReady ? heroData.heroName : "준비 중";
        }

        if (button != null)
        {
            button.interactable = isReady;
        }
    }

    public void OnClickSelectHero()
    {
        if (heroData == null) return;

        // ID 탐색 없이 SO 데이터 통째로 전달
        if (SelectManager.I != null)
        {
            SelectManager.I.SelectHero(heroData, unitDatas);
        }
    }
}