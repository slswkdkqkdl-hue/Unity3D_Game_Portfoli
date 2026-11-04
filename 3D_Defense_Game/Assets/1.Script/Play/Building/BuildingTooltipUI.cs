using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingTooltipUI : MonoBehaviour
{
    [Header("UI Component References")]
    [SerializeField] private TextMeshProUGUI titleText; // 건물 이름 + 건설 (예: "병영 훈련소 건설")
    [SerializeField] private TextMeshProUGUI costText;  // 건물 가격 (예: ": 300 G")
    [SerializeField] private Image resourceIconImage;  // 재화/자원 아이콘 이미지

    [Header("골드 이미지")]
    [SerializeField] Sprite GoldImage;
    
    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            UpdateTooltipPosition();
        }
    }

    /// <summary>
    /// 툴팁 UI 정보를 설정하고 패널을 활성화합니다.
    /// </summary>
    public void SetupTooltip(string buildingName, int cost, Sprite iconSprite = null)
    {
        // 1. 건물 이름 설정
        if (titleText != null)
        {
            titleText.text = $"{buildingName} 건설";
        }

        // 2. 가격 설정
        if (costText != null)
        {
            costText.text = $": {cost:N0} G";
        }

        // 3. 이미지 설정
        if (resourceIconImage != null)
        {
            if (iconSprite != null)
            {
                resourceIconImage.sprite = iconSprite;
                resourceIconImage.gameObject.SetActive(true);
            }
            else
            {
                resourceIconImage.sprite = GoldImage;
            }
        }

        // 패널 활성화
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 툴팁 UI를 초기화하고 비활성화합니다.
    /// </summary>
    public void ClearAndHide()
    {
        if (titleText != null) titleText.text = string.Empty;
        if (costText != null) costText.text = string.Empty;
        if (resourceIconImage != null) resourceIconImage.sprite = null;

        // 패널 비활성화
        gameObject.SetActive(false);
    }

    public void UpdateTooltipPosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        rectTransform.position = mousePosition;
    }
    
}