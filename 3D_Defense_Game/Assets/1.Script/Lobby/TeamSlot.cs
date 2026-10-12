using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamSlot : MonoBehaviour
{
    public TextMeshProUGUI hero_name;
    public Image hero_image;

    public HeroRuntimeData currentData;
    public Action<TeamSlot> onTeamSlotClicked;

    public void init()
    {
        currentData = null;
        if (hero_name != null) hero_name.text = "";
        if (hero_image != null) hero_image.sprite = null;
    }

    public void change(HeroRuntimeData data)
    {  
        currentData = data;
        if (hero_name != null) hero_name.text = data.heroData.hero_name;
        if (hero_image != null) hero_image.sprite = data.heroData.portrait;
    }

    public void OnClick()
    {
        onTeamSlotClicked?.Invoke(this);
    }
}