using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroPenl : MonoBehaviour
{
    [Header("UI 컴포넌트")]
    public Image Image;                     // 영웅 초상화 이미지
    public TextMeshProUGUI Hero_Name;       // 영웅 이름 (칭호 포함)
    public TextMeshProUGUI Hero_Description;// 영웅 설명


    public void Init(HeroDescData heroData)
    {
        if (heroData == null)
        {
            Clear();
            return;
        }

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
}