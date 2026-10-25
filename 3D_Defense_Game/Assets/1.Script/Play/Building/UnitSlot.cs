using UnityEngine;
using UnityEngine.UI;

public class UnitSlot : MonoBehaviour
{
    public Image unitImage;
    public Image unitBorder; // 테두리 이미지
    
    [Header("버튼 컴포넌트")]
    [SerializeField] private Button slotButton; // 유니티 Button 컴포넌트 연결

    private int slotIndex;
    private UnitData assignedUnitData;
    private BarracksPopup parentPopup;

    private void Awake()
    {
        // 인스펙터에서 할당하지 않았다면 자동으로 같은 오브젝트에서 찾기
        if (slotButton == null)
        {
            slotButton = GetComponent<Button>();
        }
    }

    public void Init(int index, UnitData data, BarracksPopup popup)
    {
        slotIndex = index;
        assignedUnitData = data;
        parentPopup = popup;

        // 버튼 클릭 이벤트 설정
        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners(); // 기존에 등록된 이벤트 중복 방지
            slotButton.onClick.AddListener(OnBtnClicked);
        }

        if (data != null && data.portrait != null)
        {
            unitImage.gameObject.SetActive(true);
            unitImage.sprite = data.portrait;
        }
        else
        {
            unitImage.gameObject.SetActive(false);
            if (unitBorder != null) unitBorder.color = Color.white;
        }
    }

    /// <summary>
    /// 이 슬롯의 선택 상태를 변경하여 테두리 색상 조절
    /// </summary>
    public void SetSelected(bool isSelected)
    {
        if (unitBorder == null) return;

        if (isSelected)
        {
            unitBorder.color = Color.red; // 선택되면 빨간색으로 변경!
        }
        else
        {
            unitBorder.color = Color.white; // 해제되면 기본 색상
        }
    }

    /// <summary>
    /// 버튼이 클릭되었을 때 실행되는 함수
    /// </summary>
    private void OnBtnClicked()
    {
        if (assignedUnitData == null) return;
        
        if (parentPopup != null)
        {
            parentPopup.OnSlotClicked(slotIndex);
        }
    }
}