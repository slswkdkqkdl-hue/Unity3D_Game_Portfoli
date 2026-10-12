using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectSlot2 : MonoBehaviour
{
    public bool isSelect = false; 
    public TextMeshProUGUI text;
    public Image image;
    public Button btn;

    public HeroRuntimeData currenthero;

    public Action<HeroSelectSlot2> OnClickEvent;

    public void Init(HeroRuntimeData data)
    {  
        currenthero = data;
        text.text = data.heroData.hero_name;
        image.sprite = data.heroData.portrait;
    }

    public void OnClick()
    {
        if (currenthero == null) return;
        OnClickEvent?.Invoke(this);
    }
}