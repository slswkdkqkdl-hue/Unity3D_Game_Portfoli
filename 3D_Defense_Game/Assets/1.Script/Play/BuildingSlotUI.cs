using UnityEngine;
using UnityEngine.UI;

public class BuildingSlotUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Button slotButton;

    private BuildingDataSO currentBuildingData;

    /// <summary>
    /// 슬롯 데이터 초기화 및 UI 갱신
    /// </summary>
    public void InitSlot(BuildingDataSO data)
    {
        currentBuildingData = data;

        if (data != null)
        {
            if (iconImage != null)
            {
                iconImage.sprite = data.buildingIcon; // SO에 명시된 아이콘 적용
                iconImage.gameObject.SetActive(true);
            }

            if (slotButton != null)
            {
                slotButton.interactable = true;
                slotButton.onClick.RemoveAllListeners();
                slotButton.onClick.AddListener(OnSlotClicked);
            }
        }
        else
        {
            // 데이터가 없는 빈 슬롯 처리
            if (iconImage != null) iconImage.gameObject.SetActive(false);
            if (slotButton != null) slotButton.interactable = false;
        }
    }

    private void OnSlotClicked()
    {
        if (currentBuildingData == null) return;

        Debug.Log($"건물 선택됨: {currentBuildingData.buildingName}");
        BuildingManager.I.OnClickBuildingSlot(currentBuildingData);
    }
}