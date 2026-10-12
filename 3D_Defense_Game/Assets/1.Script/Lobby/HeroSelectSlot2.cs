using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectSlot2 : MonoBehaviour
{

    public bool isClick = false; // 선택 되었다 여부 
    public TextMeshProUGUI text;
    public Image image;

    [SerializeField]private HeroRuntimeData currenthero;

    public void Init(HeroRuntimeData data)
    {   
        currenthero = data;
        text.text = data.heroData.hero_name;
        image.sprite = data.heroData.portrait;
    }

    // 버튼 이벤트용 
    public void OnClick()
    {
        if(currenthero == null || isClick == false) return;


    }
}
