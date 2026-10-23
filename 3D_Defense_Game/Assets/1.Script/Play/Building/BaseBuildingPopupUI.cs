using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaseBuildingPopupUI : MonoBehaviour
{
    [Header("공통 UI 요소")]
    [SerializeField] protected TextMeshProUGUI nameText;
    [SerializeField] protected TextMeshProUGUI descriptionText;
    [SerializeField] protected Image iconImage;

    protected Building targetBuilding;

    public virtual void Setup(Building building)
    {
        targetBuilding = building;
        BuildingDataSO data = building.Data;

        if (data != null)
        {
            if (nameText != null) nameText.text = data.buildingName;
            if (descriptionText != null) descriptionText.text = data.buildingDescription;
            if (iconImage != null) iconImage.sprite = data.buildingIcon;
        }
    }
}