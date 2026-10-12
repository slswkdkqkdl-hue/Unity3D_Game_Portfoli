using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamSlot : MonoBehaviour
{
    public TextMeshProUGUI hero_name;
    public Image hero_image;

    private HeroRuntimeData currentData;

    public void init()
    {
        hero_name.text = "";
        hero_image.sprite = null;
    }


    public void change(HeroRuntimeData data)
    {   
        currentData =data;
        hero_name.text = data.heroData.hero_name;
        hero_image.sprite = data.heroData.portrait;
    }

    // 버튼이벤트용
    public void OnClick()
    {
        if(currentData == null) return;


    }

}
