
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인벤토리와 장비의 데이터 처리를 담당한다.
/// UI 표시나 버튼 이벤트는 처리하지 않는다.
/// </summary>
public class InventoryEquipmentService
{
    private List<ItemRuntimeData> inventoryItems;
    private List<HeroRuntimeData> heroes;

    private int itemSize;

    /// <summary>
    /// 보유 아이템과 영웅 목록 설정
    /// </summary>
    public void Init(
        List<ItemRuntimeData> items,
        List<HeroRuntimeData> heroList,
        int maxItemSize)
    {
        inventoryItems = items ?? new List<ItemRuntimeData>();
        heroes = heroList ?? new List<HeroRuntimeData>();
        itemSize = Mathf.Max(0, maxItemSize);
    }

    /// <summary>
    /// 인벤토리에서 사용 가능한 아이템 수량
    /// </summary>
    public int GetAvailableItemCount(int itemId)
    {
        if (itemId <= 0 || inventoryItems == null)
            return 0;

        int totalCount = 0;

        foreach (ItemRuntimeData item in inventoryItems)
        {
            if (item == null || item.itemData == null)
                continue;

            if (item.itemData.item_Id == itemId)
                totalCount += Mathf.Max(0, item.size);
        }

        return Mathf.Max(0, totalCount - GetEquippedItemCount(itemId));
    }

    /// <summary>
    /// 모든 영웅이 장착한 해당 아이템 수량
    /// </summary>
    private int GetEquippedItemCount(int itemId)
    {
        if (heroes == null)
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
    /// 무기 또는 방어구 장착
    /// </summary>
    public bool TryEquipItem(
        HeroRuntimeData hero,
        ItemData itemData)
    {
        if (hero == null || itemData == null)
            return false;

        int itemId = itemData.item_Id;

        if (itemId <= 0)
            return false;

        bool isWeapon = itemData.itemType == ItemType.Weapon;
        bool isArmor = itemData.itemType == ItemType.Armor;

        if (!isWeapon && !isArmor)
            return false;

        int oldItemId = isWeapon
            ? hero.equippedWeaponId
            : hero.equippedArmorId;

        if (oldItemId == itemId)
            return false;

        if (GetAvailableItemCount(itemId) <= 0)
            return false;

        if (isWeapon)
            hero.equippedWeaponId = itemId;
        else
            hero.equippedArmorId = itemId;

        return true;
    }

    /// <summary>
    /// 장비 해제
    /// </summary>
    public bool TryUnequipItem(
        HeroRuntimeData hero,
        bool isWeapon)
    {
        if (hero == null)
            return false;

        int itemId = isWeapon
            ? hero.equippedWeaponId
            : hero.equippedArmorId;

        if (itemId <= 0)
            return false;

        if (isWeapon)
            hero.equippedWeaponId = -1;
        else
            hero.equippedArmorId = -1;

        return true;
    }

    /// <summary>
    /// 아이템 한 개 소비
    /// </summary>
    public bool TryRemoveOneItem(int itemId)
    {
        if (GetAvailableItemCount(itemId) <= 0)
            return false;

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            ItemRuntimeData item = inventoryItems[i];

            if (item == null || item.itemData == null)
                continue;

            if (item.itemData.item_Id != itemId || item.size <= 0)
                continue;

            item.size--;

            if (item.size <= 0)
                inventoryItems.RemoveAt(i);

            return true;
        }

        return false;
    }

    /// <summary>
    /// 아이템 한 개 획득
    /// </summary>
    public bool TryAddItem(ItemData itemData)
    {
        if (itemData == null || itemData.item_Id <= 0)
            return false;

        foreach (ItemRuntimeData item in inventoryItems)
        {
            if (item == null || item.itemData == null)
                continue;

            if (item.itemData.item_Id == itemData.item_Id)
            {
                item.size++;
                return true;
            }
        }

        if (GetVisibleItemTypeCount() >= itemSize)
            return false;

        inventoryItems.Add(new ItemRuntimeData
        {
            itemData = itemData,
            size = 1
        });

        return true;
    }

    /// <summary>
    /// 인벤토리에 표시되는 아이템 종류 수
    /// </summary>
    private int GetVisibleItemTypeCount()
    {
        HashSet<int> visibleItemIds = new HashSet<int>();

        foreach (ItemRuntimeData item in inventoryItems)
        {
            if (item == null || item.itemData == null)
                continue;

            int itemId = item.itemData.item_Id;

            if (GetAvailableItemCount(itemId) > 0)
                visibleItemIds.Add(itemId);
        }

        return visibleItemIds.Count;
    }

    /// <summary>
    /// 인벤토리에 표시할 아이템 목록
    /// 원본 수량은 변경하지 않는다.
    /// </summary>
    public List<ItemRuntimeData> GetDisplayItems()
    {
        List<ItemRuntimeData> displayItems =
            new List<ItemRuntimeData>();

        if (inventoryItems == null)
            return displayItems;

        HashSet<int> addedItemIds = new HashSet<int>();

        foreach (ItemRuntimeData item in inventoryItems)
        {
            if (item == null || item.itemData == null)
                continue;

            int itemId = item.itemData.item_Id;

            if (!addedItemIds.Add(itemId))
                continue;

            int availableCount = GetAvailableItemCount(itemId);

            if (availableCount <= 0)
                continue;

            displayItems.Add(new ItemRuntimeData
            {
                itemData = item.itemData,
                size = availableCount
            });
        }

        return displayItems;
    }
}
