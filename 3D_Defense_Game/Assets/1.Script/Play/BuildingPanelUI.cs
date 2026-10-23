using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanelUI : MonoBehaviour
{
    [Header("패널 컴포넌트")]
    [SerializeField] private GameObject slotContentBase;
    [SerializeField] private GameObject slotContentSpecial;
    [SerializeField] private Button changeButton;
    [SerializeField] private TextMeshProUGUI buttonText;


    private bool isSpecialActive = false;

    public void Init()
    {
        // 1. 버튼 이벤트 연결
        if (changeButton != null)
        {
            changeButton.onClick.RemoveAllListeners();
            changeButton.onClick.AddListener(ToggleSlotPanel);
        }

        // 2. 초기 상태 설정 (Base 활성화, Special 비활성화)
        SetPanelState(false);

        // 3. 건물 슬롯 데이터 초기화
        InitBuildingSlots();
    }

    private void ToggleSlotPanel()
    {
        isSpecialActive = !isSpecialActive;
        SetPanelState(isSpecialActive);
    }

    private void SetPanelState(bool showSpecial)
    {
        if (slotContentBase != null) slotContentBase.SetActive(!showSpecial);
        if (slotContentSpecial != null) slotContentSpecial.SetActive(showSpecial);
        if (buttonText != null) buttonText.text = showSpecial ? "특수건물" : "일반건물";
    }

    /// <summary>
    /// BuildingDataManager 데이터를 받아와 각 슬롯 초기화
    /// </summary>
    private void InitBuildingSlots()
    {
        var data = BuildingDataManager.I;
        // 별도의 리스트 생성 없이 원본 리스트를 그대로 전달
        SetupSlotsInGroup(slotContentBase, data.GetAvailableBuildings());
        SetupSlotsInGroup(slotContentSpecial, data.GetSpecialBuildings());
    }
    /// <summary>
    /// 건물창 패널 슬룻들 초기화
    /// </summary>
    private void SetupSlotsInGroup(GameObject contentGroup, List<BuildingDataSO> rawDataList)
    {
        if (contentGroup == null) return;

        BuildingSlotUI[] slots = contentGroup.GetComponentsInChildren<BuildingSlotUI>(true);
        
        int slotIndex = 0;
        int dataCount = rawDataList != null ? rawDataList.Count : 0;

        // 1. MainCastle이 아닌 데이터만 슬롯에 채움
        for (int i = 0; i < dataCount; i++)
        {
            if (slotIndex >= slots.Length) break; // 슬롯 개수 초과 시 중단

            var data = rawDataList[i];
            if (data == null || data.buildingType == BuildingType.MainCastle) continue; // 메인 성 건너뛰기

            slots[slotIndex].InitSlot(data);
            slotIndex++;
        }

        // 2. 남은 슬롯들은 빈 슬롯으로 초기화
        while (slotIndex < slots.Length)
        {
            slots[slotIndex].InitSlot(null);
            slotIndex++;
        }
    }
}