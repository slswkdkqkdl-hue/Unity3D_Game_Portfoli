using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int itemSize = 20;
    public GameObject itemSlotPrefab;
    public Transform itemSlotParent;

    [SerializeField] private item_Popup item_Popup;

    private InventoryEquipmentService service;
    private readonly List<itemSlot> inventorySlots = new List<itemSlot>();

    public Action<ItemRuntimeData> OnItemSlotClicked;

    /// <summary>
    /// 인벤토리 초기화
    /// </summary>
    public void Init(
        List<ItemRuntimeData> items,
        InventoryEquipmentService sharedService)
    {
        service = sharedService;

        if (service == null)
        {
            Debug.LogError("InventoryEquipmentService가 전달되지 않았습니다.");
            return;
        }

        while (inventorySlots.Count < itemSize)
        {
            GameObject obj = Instantiate(
                itemSlotPrefab,
                itemSlotParent);

            itemSlot slot = obj.GetComponent<itemSlot>();
            slot.onSlotClicked = HandleSlotClicked;

            inventorySlots.Add(slot);
        }

        RefreshInventorySlots();
    }

    /// <summary>
    /// 인벤토리에 표시 가능한 아이템 수량 조회
    /// </summary>
    public int GetAvailableItemCount(int itemId)
    {
        if (service == null)
            return 0;

        return service.GetAvailableItemCount(itemId);
    }

    /// <summary>
    /// 보유 아이템 한 개 소비
    /// </summary>
    public bool TryRemoveOneItem(int itemId)
    {
        if (service == null)
            return false;

        bool success = service.TryRemoveOneItem(itemId);

        if (success)
            RefreshInventorySlots();

        return success;
    }

    /// <summary>
    /// 보유 아이템 한 개 추가
    /// </summary>
    public bool TryAddItem(ItemData itemData)
    {
        if (service == null)
            return false;

        bool success = service.TryAddItem(itemData);

        if (success)
            RefreshInventorySlots();

        return success;
    }

    /// <summary>
    /// 인벤토리 UI 갱신
    /// </summary>
    public void RefreshInventorySlots()
    {
        if (service == null)
            return;

        List<ItemRuntimeData> displayItems = service.GetDisplayItems();

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < displayItems.Count && i < itemSize)
            {
                inventorySlots[i].Init(displayItems[i], item_Popup);
            }
            else
            {
                inventorySlots[i].Init(null, item_Popup);
            }
        }
    }

    /// <summary>
    /// 인벤토리 슬롯 클릭 이벤트 전달
    /// </summary>
    private void HandleSlotClicked(ItemRuntimeData clickedItem)
    {
        OnItemSlotClicked?.Invoke(clickedItem);
    }
}
