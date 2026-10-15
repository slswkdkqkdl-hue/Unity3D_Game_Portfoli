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

    // 전체 보유 아이템 데이터 
    // 원본가져옴 요거는 변하지않는다 
    private List<ItemRuntimeData> inventoryItems = new List<ItemRuntimeData>();

    // 전체 영웅 목록
    // 장착 중인 아이템 수량을 계산할 때 사용한다.
    private List<HeroRuntimeData> heroes = new List<HeroRuntimeData>();

    private List<itemSlot> inventorySlots = new List<itemSlot>();

    public Action<ItemRuntimeData> OnItemSlotClicked;


    /// <summary>
    /// 인벤토리 초기화
    /// </summary>
    public void Init(List<ItemRuntimeData> items,List<HeroRuntimeData> heroDataList)
    {
        inventoryItems = items ?? new List<ItemRuntimeData>();
        heroes = heroDataList ?? new List<HeroRuntimeData>();

        // 슬롯 개수만큼 생성
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
    /// 해당 아이템의 전체 보유 수량에서
    /// 장착 중인 수량을 제외한 개수를 반환한다.
    /// </summary>
    public int GetAvailableItemCount(int itemId)
    {
        if (itemId <= 0)
            return 0;

        int totalCount = 0;

        // 실제 보유 수량
        foreach (ItemRuntimeData item in inventoryItems)
        {
            if (item == null || item.itemData == null)
                continue;

            if (item.itemData.item_Id != itemId)
                continue;

            totalCount += Mathf.Max(0, item.size);
        }

        // 장착 중인 수량
        int equippedCount = GetEquippedItemCount(itemId);

        return Mathf.Max(0, totalCount - equippedCount);
    }


    /// <summary>
    /// 모든 영웅이 장착 중인 해당 아이템의 개수
    /// </summary>
    private int GetEquippedItemCount(int itemId)
    {
        if (itemId <= 0 || heroes == null)
            return 0;

        int count = 0;

        foreach (HeroRuntimeData hero in heroes)
        {
            if (hero == null)
                continue;

            if (hero.equippedWeaponId == itemId)
                count++;

            if (hero.equippedArmorId == itemId)
                count++;
        }

        return count;
    }


    /// <summary>
    /// 실제 보유 아이템 한 개 제거
    /// 아이템 소비 등에 사용한다.
    /// 장착 처리에서는 호출하지 않는다.
    /// </summary>
    public bool TryRemoveOneItem(int itemId)
    {
        // 사용 가능한 수량이 없다면 제거 불가
        if (GetAvailableItemCount(itemId) <= 0)
            return false;

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            ItemRuntimeData item = inventoryItems[i];

            if (item == null || item.itemData == null)
                continue;

            if (item.itemData.item_Id != itemId)
                continue;

            if (item.size <= 0)
                continue;

            item.size--;

            if (item.size <= 0)
                inventoryItems.RemoveAt(i);

            RefreshInventorySlots();
            return true;
        }

        return false;
    }


    /// <summary>
    /// 실제 보유 아이템 한 개 추가
    /// 아이템 획득 등에 사용한다.
    /// 장비 해제에서는 호출하지 않는다.
    /// </summary>
    public bool TryAddItem(ItemData itemData)
    {
        if (itemData == null)
            return false;

        // 이미 보유 중인 아이템이면 수량만 증가
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            ItemRuntimeData item = inventoryItems[i];

            if (item == null || item.itemData == null)
                continue;

            if (item.itemData.item_Id != itemData.item_Id)
                continue;

            item.size++;
            RefreshInventorySlots();

            return true;
        }

        // 새로운 아이템 종류를 추가할 때만 공간 확인
        if (GetVisibleItemTypeCount() >= itemSize)
            return false;

        inventoryItems.Add(new ItemRuntimeData
        {
            itemData = itemData,
            size = 1
        });

        RefreshInventorySlots();

        return true;
    }


    /// <summary>
    /// 인벤토리에 표시할 아이템 종류 수
    /// 중복 ID는 한 번만 계산한다.
    /// </summary>
    private int GetVisibleItemTypeCount()
    {
        int count = 0;

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            ItemRuntimeData item = inventoryItems[i];

            if (item == null || item.itemData == null)
                continue;

            int itemId = item.itemData.item_Id;

            // 앞쪽에 같은 ID가 있으면 중복 계산하지 않는다.
            if (HasSameItemBeforeIndex(i, itemId))
                continue;

            // 장착 중이라 표시 가능한 수량이 없다면 제외
            if (GetAvailableItemCount(itemId) <= 0)
                continue;

            count++;
        }

        return count;
    }


    /// <summary>
    /// 현재 인덱스보다 앞에 같은 아이템 ID가 있는지 확인
    /// </summary>
    private bool HasSameItemBeforeIndex(int index, int itemId)
    {
        for (int i = 0; i < index; i++)
        {
            ItemRuntimeData item = inventoryItems[i];

            if (item == null || item.itemData == null)
                continue;

            if (item.itemData.item_Id == itemId)
                return true;
        }

        return false;
    }


    /// <summary>
    /// 인벤토리 UI 갱신
    /// 실제 보유 데이터는 수정하지 않고
    /// 표시용 ItemRuntimeData를 생성한다.
    /// </summary>
    public void RefreshInventorySlots()
    {
        if (inventoryItems == null)
            return;

        List<ItemRuntimeData> displayItems = new List<ItemRuntimeData>();

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            ItemRuntimeData item = inventoryItems[i];

            if (item == null || item.itemData == null)
                continue;

            int itemId = item.itemData.item_Id; 

            // 같은 ID를 가진 아이템은 슬롯 하나에만 표시
            if (HasSameItemBeforeIndex(i, itemId))
                continue;

            int availableCount = GetAvailableItemCount(itemId);

            // 전부 장착 중이면 표시하지 않는다.
            if (availableCount <= 0)
                continue;

            // UI 표시용 데이터
            // 원본 ItemRuntimeData의 size는 변경하지 않는다.
            displayItems.Add(new ItemRuntimeData
            {
                itemData = item.itemData,
                size = availableCount
            });
        }

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < displayItems.Count && i < itemSize)
            {
                inventorySlots[i].Init(
                    displayItems[i],
                    item_Popup);
            }
            else
            {
                inventorySlots[i].Init(
                    null,
                    item_Popup);
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