
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인벤토리와 장비의 데이터 처리를 담당한다.
/// </summary>
public class InventoryEquipmentService
{
    private List<ItemRuntimeData> inventoryItems;
    private List<HeroRuntimeData> heroes;

    private int itemSize;

    /// <summary>
    /// 인벤토리와 영웅 데이터 연결
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
    /// 인벤토리에 표시할 수량 반환
    /// 보유 수량에서 장착 중인 수량을 뺀다.
    /// </summary>
    public int GetAvailableItemCount(int itemId)
    {
        if (inventoryItems == null || itemId <= 0)
            return 0;

        int totalCount = 0;

        foreach (ItemRuntimeData item in inventoryItems)
        {
            if (item == null || item.itemData == null)
                continue;

            if (item.itemData.item_Id == itemId)
                totalCount += item.size;
        }

        int equippedCount = 0;

        if (heroes != null)
        {
            foreach (HeroRuntimeData hero in heroes)
            {
                if (hero == null)
                    continue;

                if (hero.equippedWeaponId == itemId)
                    equippedCount++;

                if (hero.equippedArmorId == itemId)
                    equippedCount++;
            }
        }

        return Mathf.Max(0, totalCount - equippedCount);
    }

    /// <summary>
    /// 무기 또는 방어구 장착
    /// </summary>
    public bool TryEquipItem(HeroRuntimeData hero, ItemData itemData)
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

        int currentItemId = isWeapon
            ? hero.equippedWeaponId
            : hero.equippedArmorId;

        // 이미 같은 아이템을 장착했다면 중단
        if (currentItemId == itemId)
            return false;

        // 인벤토리에 표시 가능한 수량이 없으면 중단
        if (GetAvailableItemCount(itemId) <= 0)
            return false;

        // 해당 장비 슬롯에 아이템 ID 저장
        if (isWeapon)
            hero.equippedWeaponId = itemId;
        else
            hero.equippedArmorId = itemId;

        return true;
    }

    /// <summary>
    /// 무기 또는 방어구 장착 해제
    /// </summary>
    public bool TryUnequipItem(HeroRuntimeData hero, bool isWeapon)
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

            // 수량이 0이면 목록에서 제거
            if (item.size == 0)
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
        if (inventoryItems == null ||
            itemData == null ||
            itemData.item_Id <= 0)
            return false;

        // 이미 보유한 아이템이면 수량만 증가
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

        // 새로운 아이템이면 인벤토리 빈 슬롯 확인
        if (GetDisplayItems().Count >= itemSize)
            return false;

        inventoryItems.Add(new ItemRuntimeData
        {
            itemData = itemData,
            size = 1
        });

        return true;
    }

    /// <summary>
    /// 인벤토리 UI에 표시할 아이템 목록 생성
    /// 원본 데이터는 변경하지 않는다.
    /// </summary>
    public List<ItemRuntimeData> GetDisplayItems()
    {
        List<ItemRuntimeData> displayItems =
            new List<ItemRuntimeData>();

        if (inventoryItems == null)
            return displayItems;

        foreach (ItemRuntimeData item in inventoryItems)
        {
            if (item == null || item.itemData == null)
                continue;

            int itemId = item.itemData.item_Id;

            if (itemId <= 0)
                continue;

            // 이미 같은 ID가 표시 목록에 있는지 확인
            bool alreadyAdded = false;

            foreach (ItemRuntimeData displayedItem in displayItems)
            {
                if (displayedItem.itemData.item_Id == itemId)
                {
                    alreadyAdded = true;
                    break;
                }
            }

            if (alreadyAdded)
                continue;

            int availableCount = GetAvailableItemCount(itemId);

            // 장착 중이라 표시할 수량이 없으면 생략
            if (availableCount <= 0)
                continue;

            // UI에 사용할 별도의 데이터 생성
            displayItems.Add(new ItemRuntimeData
            {
                itemData = item.itemData,
                size = availableCount
            });
        }

        return displayItems;
    }
}
