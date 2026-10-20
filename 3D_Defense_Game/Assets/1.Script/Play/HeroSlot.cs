using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    // 출격/후퇴 상태 관리를 위한 변수
    private bool isDeployed = false; 

    /// <summary>
    /// 영웅 슬롯 초기화 (최대 체력/마나를 받아 현재 값과 동시에 세팅)
    /// </summary>
    public void InitHeroSlot(Sprite heroSprite, float maxHp, float maxMp)
    {
        // 1. 영웅 이미지 설정
        if (HeroImage != null)
        {
            HeroImage.sprite = heroSprite;
            HeroImage.gameObject.SetActive(heroSprite != null);
        }

        // 2. 초기 체력 설정 (풀피)
        UpdateHp(maxHp, maxHp);

        // 3. 초기 마나 설정 (풀마나)
        UpdateMp(maxMp, maxMp);

        // 4. 출격/후퇴 상태 초기화 (기본: 미출격 상태)
        isDeployed = false;
        if (Stage != null)
        {
            Stage.text = "출격"; 
        }
        UpdateDeployButtonUI();
    }    

    /// <summary>
    /// 체력 갱신 및 UI 표시 함수 
    /// </summary>
    public void UpdateHp(float currentHp, float maxHp)
    {
        float hpNormalized = maxHp > 0 ? Mathf.Clamp01(currentHp / maxHp) : 0f;
        
        if (HeroHpbar != null)
        {
            HeroHpbar.fillAmount = hpNormalized;
        }
        if (Hp != null)
        {
            Hp.text = $"{Mathf.CeilToInt(currentHp)} / {Mathf.CeilToInt(maxHp)}";
        }
    }

    /// <summary>
    /// 마나 갱신 및 UI 표시 함수 
    /// </summary>
    public void UpdateMp(float currentMp, float maxMp)
    {
        float mpNormalized = maxMp > 0 ? Mathf.Clamp01(currentMp / maxMp) : 0f;
        
        if (HeroMpbar != null)
        {
            HeroMpbar.fillAmount = mpNormalized;
        }
        if (Mp != null)
        {
            Mp.text = $"{Mathf.CeilToInt(currentMp)} / {Mathf.CeilToInt(maxMp)}";
        }
    }

    /// <summary>
    /// 출격/후퇴 버튼 클릭 시 호출할 메서드
    /// </summary>
    public void OnClickDeployButton()
    {
        isDeployed = !isDeployed; // 상태 토글 (출격 <-> 후퇴)

        if (isDeployed)
        {    
            if (Stage != null) Stage.text = "후퇴"; 
            Debug.Log("영웅 출격!");
        }
        else
        {    
            if (Stage != null) Stage.text = "출격"; 
            Debug.Log("영웅 후퇴(귀환)!");
        }

        UpdateDeployButtonUI();
    }

    private void UpdateDeployButtonUI()
    {
        // 추가적인 버튼 그래픽 처리가 필요할 때 작성
    }
}