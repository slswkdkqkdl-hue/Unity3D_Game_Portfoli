using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HeroPenl : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{   
    [SerializeField]private RuntimeHeroData heroDescData;
    [Header("UI 컴포넌트")]
    public Image Image;                     // 영웅 초상화 이미지
    public TextMeshProUGUI Hero_Name;       // 영웅 이름 (칭호 포함)
    public TextMeshProUGUI Hero_Description;// 영웅 설명


    public void Init(RuntimeHeroData heroData)
    {   
        if (heroData == null)
        {
            Clear();
            return;
        }
        this.heroDescData = heroData;
        // 1. 영웅 초상화 이미지 설정
        if (Image != null)
        {
            Image.sprite = heroData.portrait; 
            Image.gameObject.SetActive(Image.sprite != null);
        }

        // 2. 영웅 이름 설정
        if (Hero_Name != null)
        {
            Hero_Name.text = heroData.hero_PullName;
        }

        // 3. 영웅 설명 설정
        if (Hero_Description != null)
        {
            Hero_Description.text = heroData.description; 
        }
    }

    public void Clear()
    {
        if (Image != null)
        {
            Image.sprite = null;
            Image.gameObject.SetActive(false);
        }
        if (Hero_Name != null) Hero_Name.text = string.Empty;
        if (Hero_Description != null) Hero_Description.text = string.Empty;
    }
    // 마우스가 UI 요소 위로 올라왔을 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 여기서 툴팁 팝업을 켜고 데이터를 넣어줍니다.
        SelectPopUp.Instance.ShowInfoPopup(heroDescData);
    }

    // 마우스가 UI 요소에서 나갔을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        // 여기서 툴팁 팝업을 끄거나 초기화합니다.
        SelectPopUp.Instance.HideInfoPopup();
    }
}

