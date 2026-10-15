using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI item_name;
    public Button button;

    // EquipmentView에 슬롯 클릭을 전달
    private Action<EquipmentSlot> onSlotClicked;


    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveListener(OnClickSlot);
            button.onClick.AddListener(OnClickSlot);
        }
    }


    /// <summary>
    /// 클릭 이벤트 연결
    /// </summary>
    public void SetClickHandler(Action<EquipmentSlot> callback)
    {
        onSlotClicked = callback;
    }


    /// <summary>
    /// 아이템 표시
    /// 장착 상태는 저장하지 않고 이름만 표시한다.
    /// </summary>
    public void Init(ItemData itemData)
    {
        if (item_name == null)
            return;

        if (itemData == null)
        {
            item_name.text = "no item";
            return;
        }

        item_name.text = itemData.itemName;
    }


    /// <summary>
    /// 슬롯 비우기
    /// </summary>
    public void Clear()
    {
        if (item_name != null)
            item_name.text = "no item";
    }


    /// <summary>
    /// 슬롯 클릭
    /// </summary>
    private void OnClickSlot()
    {
        onSlotClicked?.Invoke(this);
    }


    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClickSlot);
    }
}