
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public TextMeshProUGUI item_name;
    public Button button;

    // 현재 슬롯에 표시하는 아이템
    [SerializeField] private ItemRuntimeData runtimeData;

    // EquipmentView에 슬롯 클릭을 전달
    private Action<EquipmentSlot> onSlotClicked;

    public ItemRuntimeData RuntimeData => runtimeData;


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


    // 클릭 이벤트 연결
    public void SetClickHandler(Action<EquipmentSlot> callback)
    {
        onSlotClicked = callback;
    }


    // 아이템 표시
    public void Init(ItemRuntimeData data)
    {
        runtimeData = data;

        bool hasItem = data != null && data.itemData != null;

        if (item_name != null)
        {
            item_name.text = hasItem? data.itemData.itemName: "no item";
        }
    }


    // 슬롯 비우기
    public void Clear()
    {
        runtimeData = null;

        if (item_name != null)
            item_name.text = "no item";
    }


    private void OnClickSlot()
    {
        onSlotClicked?.Invoke(this);
    }


}
