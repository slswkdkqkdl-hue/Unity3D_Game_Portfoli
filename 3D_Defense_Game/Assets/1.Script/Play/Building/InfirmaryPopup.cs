using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfirmaryPopup : BaseBuildingPopupUI
{
    [SerializeField] private TextMeshProUGUI CoolTimeText;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;

    private Infirmary infirmary;

    public override void Setup(Building building)
    {
        base.Setup(building); // 부모의 기본 정보(이름, 설명, 아이콘) 세팅


        if (infirmary != null)
        {
            infirmary.OnStateChanged -= UpdateUIState;
        }

        if (building is Infirmary targetInfirmary)
        {
            infirmary = targetInfirmary;
            infirmary.OnStateChanged += UpdateUIState;
            UpdateUIState(infirmary.isReady);
        }
    }

    private void OnDisable()
    {
        if (infirmary != null)
        {
            infirmary.OnStateChanged -= UpdateUIState;
        }
    }

    /// <summary>
    /// 상태가 변할 때(쿨타임 완료 등) UI(텍스트, 버튼 활성화)를 갱신해주는 함수
    /// </summary>
    private void UpdateUIState(bool isReady)
    {
        if (infirmary == null) return;

        if (isReady)
        {
            buttonText.text = "회복";
            button.interactable = true; 
        }
        else
        {
            buttonText.text = "준비 중..";
            button.interactable = false; 
        }
    }

    /// <summary>
    /// 유저가 팝업창의 '회복 버튼'을 직접 눌렀을 때 호출
    /// </summary>
    public void OnClickRecovery()
    {
        if (infirmary == null) return;
        infirmary.OnClickHealButton();            
        UpdateUIState(infirmary.isReady);
        Debug.Log("회복 버튼 눌림 및 치료 발동 완료!");
    }
}