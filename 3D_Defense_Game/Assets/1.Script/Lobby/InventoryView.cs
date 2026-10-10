using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int itemSize = 20;
    public GameObject itemSlotPrefab;
    public Transform itemSlotParent;

    private List<ItemRuntimeData> inventoryItems =new List<ItemRuntimeData>();

    private List<itemSlot> inventorySlots =new List<itemSlot>();

    public Action<ItemRuntimeData> OnItemSlotClicked;


    public void Init(List<ItemRuntimeData> items)
    {
        inventoryItems = items ?? new List<ItemRuntimeData>();

        while (inventorySlots.Count <= itemSize)
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


    // 아이템 한 개 제거
    public bool TryRemoveOneItem(int itemId)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            ItemRuntimeData item = inventoryItems[i];

            if (item == null || item.itemData == null)
                continue;

            if (item.itemData.item_Id != itemId)
                continue;

            if (item.size > 1)
            {
                item.size--;
            }
            else
            {
                inventoryItems.RemoveAt(i);
            }

            RefreshInventorySlots();
            return true;
        }

        return false;
    }


    // 아이템 한 개 추가
    public bool TryAddItem(ItemData itemData)
    {
        if (itemData == null)
            return false;

        // 같은 아이템이 있으면 수량만 증가
        foreach (ItemRuntimeData item in inventoryItems)
        {
            if (item == null || item.itemData == null)
                continue;

            if (item.itemData.item_Id == itemData.item_Id)
            {
                item.size++;
                RefreshInventorySlots();
                return true;
            }
        }

        // 새로운 아이템을 넣을 공간이 없으면 실패
        if (inventoryItems.Count >= itemSize)
            return false;

        inventoryItems.Add(new ItemRuntimeData
        {
            itemData = itemData,
            size = 1
        });

        RefreshInventorySlots();
        return true;
    }


    // 인벤토리 UI 갱신
    public void RefreshInventorySlots()
    {
        if (inventoryItems == null)
            return;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < inventoryItems.Count)
                inventorySlots[i].Init(inventoryItems[i]);
            else
                inventorySlots[i].Init(null);
        }
    }


    private void HandleSlotClicked(ItemRuntimeData clickedItem)
    {
        OnItemSlotClicked?.Invoke(clickedItem);
    }
}