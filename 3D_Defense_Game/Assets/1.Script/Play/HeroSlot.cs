using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HeroSlot : MonoBehaviour
{
    [Header("UI Components")]
    public Image HeroImage;
    public Image HeroHpbar;
    public Image HeroMpbar;
    public Button Button;
    public TextMeshProUGUI Hp;
    public TextMeshProUGUI Mp;
    public TextMeshProUGUI Stage;

    private bool isDeployed = false; 
    public bool IsDeployed => isDeployed;

    private HeroRuntimeData heroRuntimeData;
    public HeroRuntimeData HeroRuntimeData => heroRuntimeData; 

    public event Action<HeroSlot, bool> OnDeployStateChanged;

    public void InitHeroSlot(HeroRuntimeData data)
    {
        heroRuntimeData = data;

        if (heroRuntimeData != null && heroRuntimeData.heroData != null)
        {
            Sprite heroSprite = heroRuntimeData.heroData.portrait;
            if (HeroImage != null)
            {
                HeroImage.sprite = heroSprite;
                HeroImage.gameObject.SetActive(heroSprite != null);
            }

            float maxHp = heroRuntimeData.heroData.hp;
            UpdateHp(maxHp, maxHp);

            float maxMp = heroRuntimeData.heroData.mp;
            UpdateMp(maxMp, maxMp);
        }

        isDeployed = false;
        if (Stage != null)
        {
            Stage.text = "출격"; 
        }
    }    

    public void UpdateHp(float currentHp, float maxHp)
    {
        float hpNormalized = maxHp > 0 ? Mathf.Clamp01(currentHp / maxHp) : 0f;
        if (HeroHpbar != null) HeroHpbar.fillAmount = hpNormalized;
        if (Hp != null) Hp.text = $"{Mathf.CeilToInt(currentHp)} / {Mathf.CeilToInt(maxHp)}";
    }

    public void UpdateMp(float currentMp, float maxMp)
    {
        float mpNormalized = maxMp > 0 ? Mathf.Clamp01(currentMp / maxMp) : 0f;
        if (HeroMpbar != null) HeroMpbar.fillAmount = mpNormalized;
        if (Mp != null) Mp.text = $"{Mathf.CeilToInt(currentMp)} / {Mathf.CeilToInt(maxMp)}";
    }

    public void OnClickDeployButton()
    {
        isDeployed = !isDeployed; 

        if (isDeployed)
        {    
            if (Stage != null) Stage.text = "후퇴"; 

        }
        else
        {    
            if (Stage != null) Stage.text = "출격"; 
        }

        // ⭐ 슬롯과 상태값만 쏙 넘겨줍니다.
        OnDeployStateChanged?.Invoke(this, isDeployed);
    }
}