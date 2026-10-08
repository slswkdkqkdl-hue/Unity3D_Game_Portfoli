using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI Skill_Name;
    public TextMeshProUGUI Skill_SP;
    public Button slotButton;
    public Image backgroundOrIconImage; 

    public SkillData boundSkill { get; private set; } 
    private Action<SkillData, SkillSlot> onClickCallback;

    public bool isEquipped { get; private set; } = false;

    // 기본 배경 색상 (#59B797)
    private readonly Color defaultColor = new Color32(0x59, 0xB7, 0x97, 255);

    /// <summary>
    /// 스킬 데이터 및 클릭 이벤트 초기화
    /// </summary>
    public void Init(SkillData data, Action<SkillData, SkillSlot> actionToPerform)
    {
        boundSkill = data;
        onClickCallback = actionToPerform;
        isEquipped = false;

        Skill_Name.text = data.skill_name;
        Skill_SP.text = $"{data.spCost}";

        SetSelectedVisual(false);

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(OnButtonClicked);
    }

    /// <summary>
    /// 슬롯 버튼 클릭 시 콜백 이벤트 호출
    /// </summary>
    private void OnButtonClicked()
    {
        onClickCallback?.Invoke(boundSkill, this);
    }

    /// <summary>
    /// 선택 상태에 따른 슬롯 배경 색상 변경
    /// </summary>
    public void SetSelectedVisual(bool isSelected)
    {
        if (backgroundOrIconImage != null)
        {
            backgroundOrIconImage.color = isSelected ? Color.red : defaultColor;
        }
    }

    /// <summary>
    /// 스킬 장착 상태 설정 및 비주얼 업데이트
    /// </summary>
    public void ToggleEquipState(bool state)
    {
        isEquipped = state;
        SetSelectedVisual(isEquipped);
    }
}