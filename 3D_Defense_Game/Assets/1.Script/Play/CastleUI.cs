using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CastleUI : MonoBehaviour
{
    [Header("UI 구성 요소")]
    [SerializeField] private Image emblemImage;      // 성 엠블럼/초상화
    [SerializeField] private Image hpBarImage;       // Fill Amount용 체력바 Image
    [SerializeField] private TextMeshProUGUI hpText; // 체력 수치 표기용 텍스트

    private float maxHp;
    private float currentHp;

    /// <summary>
    /// 1. 최초 체력 및 UI 설정 (게임 시작 시 호출)
    /// </summary>
    public void InitCastleUI(Sprite emblem, float initialHp, float maxHp)
    {
        if (emblemImage != null && emblem != null)
        {
            emblemImage.sprite = emblem;
        }

        this.maxHp = maxHp;
        SetHPInternal(initialHp);
    }

    /// <summary>
    /// 2. 체력 채우기 / 회복 (치료, 수리, 힐 시 호출)
    /// </summary>
    public void IncreaseHP(float amount)
    {
        if (amount <= 0) return;

        SetHPInternal(currentHp + amount);
    }

    /// <summary>
    /// 3. 체력 깎이기 / 피격 (데미지 입을 시 호출)
    /// </summary>
    public void DecreaseHP(float amount)
    {
        if (amount <= 0) return;

        SetHPInternal(currentHp - amount);
    }

    /// <summary>
    /// 4. 체력 직접 설정 (특수 상태 연출 또는 수치 동기화용)
    /// </summary>
    public void SetHP(float currentHp, float maxHp)
    {
        this.maxHp = maxHp;
        SetHPInternal(currentHp);
    }

    /// <summary>
    /// UI 체력바 FillAmount 및 텍스트 갱신 (내부 공통 처리)
    /// </summary>
    private void SetHPInternal(float targetHp)
    {
        if (maxHp <= 0) return;

        // 체력을 0 ~ maxHp 범위 내로 제한
        currentHp = Mathf.Clamp(targetHp, 0f, maxHp);

        // 0.0 ~ 1.0 비율 계산
        float fillRatio = currentHp / maxHp;

        if (hpBarImage != null)
        {
            hpBarImage.fillAmount = fillRatio;
        }

        if (hpText != null)
        {
            hpText.text = $"{Mathf.CeilToInt(currentHp)} / {Mathf.CeilToInt(maxHp)}";
        }
    }
}