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

    private float cachedMaxHp;
    private float cachedMaxMp;

    public event Action<HeroSlot, bool> OnDeployStateChanged;

    /// <summary>
    /// 영웅 슬롯 최초 초기화 
    /// </summary>
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

            // 최대 체력/마나 계산 후 캐싱
            cachedMaxHp = heroRuntimeData.heroData.hp + (heroRuntimeData.heroData.hpPerLevel * heroRuntimeData.level);
            cachedMaxMp = heroRuntimeData.heroData.mp + (heroRuntimeData.heroData.mpPerLevel * heroRuntimeData.level);
            
            // 처음엔 풀피/풀마나로 셋팅
            SetUpHp(cachedMaxHp, cachedMaxHp);
            SetUpMp(cachedMaxMp, cachedMaxMp);
        }

        isDeployed = false;
        if (Stage != null) Stage.text = "출격";
    }    

    /// <summary>
    /// 최초 판 깔 때 쓰는 초기화용 세팅 함수
    /// </summary>
    public void SetUpHp(float currentHp, float maxHp)
    {
        cachedMaxHp = maxHp; // 최대치 갱신 필요시 대비
        float hpNormalized = maxHp > 0 ? Mathf.Clamp01(currentHp / maxHp) : 0f;
        if (HeroHpbar != null) HeroHpbar.fillAmount = hpNormalized;
        if (Hp != null) Hp.text = $"{(int)currentHp} / {(int)maxHp}";
    }

    public void SetUpMp(float currentMp, float maxMp)
    {
        cachedMaxMp = maxMp;
        float mpNormalized = maxMp > 0 ? Mathf.Clamp01(currentMp / maxMp) : 0f;
        if (HeroMpbar != null) HeroMpbar.fillAmount = mpNormalized;
        if (Mp != null) Mp.text = $"{(int)currentMp} / {(int)maxMp}";
    }

    /// <summary>
    /// 전투 중 피격/회복 시 숫자 하나만 던져서 갱신하는 함수 (체력)
    /// </summary>
    public void UpdatHP(float currentHp)
    {
        float hpNormalized = cachedMaxHp > 0 ? Mathf.Clamp01(currentHp / cachedMaxHp) : 0f;
        if (HeroHpbar != null) HeroHpbar.fillAmount = hpNormalized;
        if (Hp != null) Hp.text = $"{(int)currentHp} / {(int)cachedMaxHp}";
    }

    /// <summary>
    /// 전투 중 마나 사용/회복 시 숫자 하나만 던져서 갱신하는 함수 (마나)
    /// </summary>
    public void UpdatMP(float currentMp)
    {
        float mpNormalized = cachedMaxMp > 0 ? Mathf.Clamp01(currentMp / cachedMaxMp) : 0f;
        if (HeroMpbar != null) HeroMpbar.fillAmount = mpNormalized;
        if (Mp != null) Mp.text = $"{(int)currentMp} / {(int)cachedMaxMp}";
    }

    public void OnClickDeployButton()
    {
        isDeployed = !isDeployed; 

        if (Stage != null)
        {
            Stage.text = isDeployed ? "후퇴" : "출격";
        }

        OnDeployStateChanged?.Invoke(this, isDeployed);
    }
}