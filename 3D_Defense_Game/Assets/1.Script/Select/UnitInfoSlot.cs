using TMPro;
using UnityEngine;
using UnityEngine.UI; // Image 컴포넌트 사용을 위해 추가

public class UnitInfoSlot : MonoBehaviour
{
    [SerializeField]private UnitDescData unitData;

    [Header("UI 컴포넌트")]
    public Image Image;                    // 유닛 초상화 또는 클래스 아이콘
    public TextMeshProUGUI Unit_Name;        // 유닛 클래스 이름 (예: 도사, 무사)
    public TextMeshProUGUI Unit_Description; // 유닛 설명


    /// <summary>
    /// UnitData를 받아와 유닛 정보 패널의 UI를 초기화하는 메서드
    /// </summary>
    public void Init(UnitDescData unitData)
    {
        if (unitData == null)
        {
            Clear();
            return;
        }
        this.unitData = unitData;

        // 1. 유닛 이미지 설정 (데이터 내 스프라이트 변수명에 맞게 수정하세요)
        if (Image != null)
        {
            Image.sprite = unitData.portrait; // 예: unitData.unitImage 또는 unitData.portrait
            Image.gameObject.SetActive(Image.sprite != null);
        }

        // 2. 유닛 이름/클래스 설정
        if (Unit_Name != null)
        {
            Unit_Name.text = unitData.Unit_Name; // 예: unitData.unitName 또는 unitData.unitClass
        }

        // 3. 유닛 설명 설정
        if (Unit_Description != null)
        {
            Unit_Description.text = unitData.description; // 예: unitData.description
        }
    }

    /// <summary>
    /// 패널의 UI 데이터를 비우고 초기 상태로 만드는 메서드
    /// </summary>
    public void Clear()
    {
        if (Image != null)
        {
            Image.sprite = null;
            Image.gameObject.SetActive(false);
        }

        if (Unit_Name != null)
        {
            Unit_Name.text = string.Empty;
        }

        if (Unit_Description != null)
        {
            Unit_Description.text = string.Empty;
        }
    }
}