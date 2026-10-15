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
    /// 인벤토리 데이터와 영웅 목록 초기화
    /// </summary>
    public void Init(List<ItemRuntimeData> items,List<HeroRuntimeData> heroList,int maxItemSize)
    {
        inventoryItems = items ?? new List<ItemRuntimeData>();
        heroes = heroList ?? new List<HeroRuntimeData>();
        itemSize = Mathf.Max(0, maxItemSize);
    }

    /// <summary>
    /// 인벤토리에서 사용 가능한 아이템 수량 반환
    /// 전체 보유 수량에서 장착 중인 수량을 제외한다.
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

            if (item.itemData.item_Id != itemId)
                continue;

            totalCount += Mathf.Max(0, item.size);
        }

        int equippedCount = GetEquippedItemCount(itemId);

        return Mathf.Max(0, totalCount - equippedCount);
    }

    /// <summary>
    /// 모든 영웅이 장착한 해당 아이템 수량 반환
    /// </summary>
    private int GetEquippedItemCount(int itemId)
    {
        if (heroes == null || itemId <= 0)
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
    public bool TryEquipItem(HeroRuntimeData hero,ItemData itemData)
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

        int currentItemId = isWeapon? hero.equippedWeaponId: hero.equippedArmorId;

        if (currentItemId == itemId)
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
    /// 무기 또는 방어구 장착 해제
    /// </summary>
    public bool TryUnequipItem(HeroRuntimeData hero,bool isWeapon)
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
    /// 사용 가능한 아이템 한 개 소비
    /// </summary>
    public bool TryRemoveOneItem(int itemId)
    {
        if (inventoryItems == null || itemId <= 0)
            return false;

        if (GetAvailableItemCount(itemId) <= 0)
            return false;

        foreach (ItemRuntimeData item in inventoryItems)
        {
            if (item == null || item.itemData == null)
                continue;

            if (item.itemData.item_Id != itemId || item.size <= 0)
                continue;

            item.size--;

            if (item.size == 0)
                inventoryItems.Remove(item);

            return true;
        }

        return false;
    }

    /// <summary>
    /// 아이템 한 개 획득
    /// 기존 아이템이면 수량을 증가시키고,
    /// 새 아이템이면 빈 슬롯이 있을 때 추가한다.
    /// </summary>
    public bool TryAddItem(ItemData itemData)
    {
        if (inventoryItems == null)
            return false;

        if (itemData == null || itemData.item_Id <= 0)
            return false;

        foreach (ItemRuntimeData item in inventoryItems)
        {
            if (item == null || item.itemData == null)
                continue;

            if (item.itemData.item_Id != itemData.item_Id)
                continue;

            item.size++;
            return true;
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
    /// 인벤토리에 현재 표시되는 아이템 종류 수 반환
    /// </summary>
    private int GetVisibleItemTypeCount()
    {
        if (inventoryItems == null)
            return 0;

        int visibleCount = 0;

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            ItemRuntimeData item = inventoryItems[i];

            if (item == null || item.itemData == null)
                continue;

            int itemId = item.itemData.item_Id;

            if (itemId <= 0)
                continue;

            // 같은 ID가 앞에 이미 존재하면 중복 계산하지 않는다.
            bool alreadyCounted = false;

            for (int j = 0; j < i; j++)
            {
                ItemRuntimeData previousItem = inventoryItems[j];

                if (previousItem == null || previousItem.itemData == null)
                    continue;

                if (previousItem.itemData.item_Id == itemId)
                {
                    alreadyCounted = true;
                    break;
                }
            }

            if (alreadyCounted)
                continue;

            if (GetAvailableItemCount(itemId) > 0)
                visibleCount++;
        }

        return visibleCount;
    }

    /// <summary>
    /// 인벤토리 UI에 표시할 아이템 목록 생성
    /// 원본 아이템 수량은 변경하지 않는다.
    /// 아이템 ID마다 한 항목만 표시한다.
    /// </summary>
    public List<ItemRuntimeData> GetDisplayItems()
    {
        List<ItemRuntimeData> displayItems =
            new List<ItemRuntimeData>();

        if (inventoryItems == null)
            return displayItems;

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            ItemRuntimeData item = inventoryItems[i];

            if (item == null || item.itemData == null)
                continue;

            int itemId = item.itemData.item_Id;

            if (itemId <= 0)
                continue;

            // 같은 ID가 앞에 이미 존재하면 중복 표시하지 않는다.
            bool alreadyAdded = false;

            for (int j = 0; j < i; j++)
            {
                ItemRuntimeData previousItem = inventoryItems[j];

                if (previousItem == null || previousItem.itemData == null)
                    continue;

                if (previousItem.itemData.item_Id == itemId)
                {
                    alreadyAdded = true;
                    break;
                }
            }

            if (alreadyAdded)
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
