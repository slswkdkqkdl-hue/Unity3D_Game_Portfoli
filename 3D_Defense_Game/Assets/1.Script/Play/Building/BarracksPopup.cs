using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarracksPopup : BaseBuildingPopupUI
{
    [Header("연결된 실제 건물 (런타임 주입됨)")]
    private Barracks targetBarracks;

    [Header("우측 3x3 슬롯 UI 배열")]
    [SerializeField] private UnitSlot[] unitSlotUIs;

    [Header("좌측 현재 생산 정보 UI")]
    [SerializeField] private Image currentUnitImage;
    [SerializeField] private TextMeshProUGUI timerText; // "30초" 등 표시용

    private int previousSelectedSlot = -1; // 이전에 선택되었던 슬롯 번호 기억용

    public override void Setup(Building targetBuilding)
    {
        base.Setup(targetBuilding);

        targetBarracks = targetBuilding as Barracks;
        if (targetBarracks == null)
        {
            Debug.LogError("[BarracksPopup] 전달된 건물이 Barracks 타입이 아닙니다!");
            return;
        }

        // 1. 우측 3x3 슬롯 세팅
        SetupSlots();

        // 2. 초기 선택된 슬롯 UI 갱신 (기본 0번)
        ChangeSelectedSlotUI(targetBarracks.CurrentSelectedSlot);
    }

    private void Update()
    {
        if (targetBarracks == null) return;
        // 추후 타이머 UI 갱신 로직 추가 가능
    }

    private void SetupSlots()
    {
        var datas = targetBarracks.UnitDatas;

        for (int i = 0; i < unitSlotUIs.Length; i++)
        {
            if (i < datas.Count)
            {
                unitSlotUIs[i].Init(i, datas[i], this);
            }
            else
            {
                unitSlotUIs[i].Init(i, null, this);
            }
        }
    }

    /// <summary>
    /// 슬롯 UI에서 클릭되었을 때 호출되는 콜백 함수
    /// </summary>
    public void OnSlotClicked(int slotIndex)
    {
        if (targetBarracks == null) return;

        // 1. 실제 건물에 슬롯 변경 통지 (쿨타임 리셋 등)
        targetBarracks.SelectSlot(slotIndex);

        // 2. UI 테두리 및 좌측 정보 갱신
        ChangeSelectedSlotUI(slotIndex);
    }

    /// <summary>
    /// 슬롯이 바뀔 때 테두리와 좌측 패널을 함께 바꿔주는 함수
    /// </summary>
    private void ChangeSelectedSlotUI(int newSlotIndex)
    {
        var datas = targetBarracks.UnitDatas;
        if (newSlotIndex < 0 || newSlotIndex >= datas.Count) return;

        // 1. 이전에 선택되어 있던 슬롯의 테두리 끄기
        if (previousSelectedSlot >= 0 && previousSelectedSlot < unitSlotUIs.Length)
        {
            unitSlotUIs[previousSelectedSlot].SetSelected(false);
        }

        // 2. 새로 선택된 슬롯의 테두리를 빨간색으로 켜기
        unitSlotUIs[newSlotIndex].SetSelected(true);

        // 3. 현재 선택된 슬롯 번호 기억 갱신
        previousSelectedSlot = newSlotIndex;

        // 4. 좌측 패널 이미지 갱신
        var selectedData = datas[newSlotIndex];
        if (currentUnitImage != null && selectedData.portrait != null)
        {
            currentUnitImage.gameObject.SetActive(true);
            currentUnitImage.sprite = selectedData.portrait;
        }
        if (timerText != null)
        {
            timerText.text = $"{selectedData.CoolTime}초"; 
            // 만약 변수명이 Cost가 아니라 다른 것이라면 (예: spawnCooldown) selectedData.spawnCooldown 로 변경해주세요!
        }
    }
}