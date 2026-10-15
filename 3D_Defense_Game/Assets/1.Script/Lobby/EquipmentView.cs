
using System.Collections.Generic;
using UnityEngine;

// 장비 UI 표시 및 클릭 이벤트 처리
public class EquipmentView : MonoBehaviour
{
    [Header("장비 슬롯")]
    public EquipmentSlot weapon;
    public EquipmentSlot amore;

    private HeroRuntimeData currentHero;
    private InventoryView inventoryView;
    private HeroSelectController heroSelectController;

    private InventoryEquipmentService service;

    // 아이템 ID로 ItemData 검색
    private Dictionary<int, ItemData> itemDataById =
        new Dictionary<int, ItemData>();

    /// <summary>
    /// 장비 UI 초기화
    /// </summary>
    public void Init(
    List<HeroRuntimeData> heroDataList,
    List<ItemRuntimeData> items,
    InventoryView inventory,
    HeroSelectController controller,
    InventoryEquipmentService sharedService)
    {
        inventoryView = inventory;
        heroSelectController = controller;

        itemDataById.Clear();

        if (items != null)
        {
            foreach (ItemRuntimeData item in items)
            {
                if (item == null || item.itemData == null)
                    continue;

                int itemId = item.itemData.item_Id;

                if (!itemDataById.ContainsKey(itemId))
                    itemDataById.Add(itemId, item.itemData);
            }
        }
        service = sharedService;

        if (service == null)
        {
            Debug.LogError("InventoryEquipmentService가 전달되지 않았습니다.");
            return;
        }

        weapon?.SetClickHandler(HandleEquipmentSlotClicked);
        amore?.SetClickHandler(HandleEquipmentSlotClicked);

        if (inventoryView != null)
        {
            inventoryView.OnItemSlotClicked -= HandleInventoryItemClicked;
            inventoryView.OnItemSlotClicked += HandleInventoryItemClicked;
        }

        weapon?.Clear();
        amore?.Clear();
    }

    /// <summary>
    /// 선택한 영웅의 장비 표시
    /// </summary>
    public void ShowHero(HeroRuntimeData hero)
    {
        currentHero = hero;

        if (currentHero == null)
        {
            weapon?.Clear();
            amore?.Clear();
            return;
        }

        ItemData weaponData = GetItemData(currentHero.equippedWeaponId);
        ItemData armorData = GetItemData(currentHero.equippedArmorId);

        weapon?.Init(weaponData);
        amore?.Init(armorData);
    }

    /// <summary>
    /// ID로 ItemData 검색
    /// </summary>
    private ItemData GetItemData(int itemId)
    {
        if (itemId <= 0)
            return null;

        if (itemDataById.TryGetValue(itemId, out ItemData data))
            return data;

        return null;
    }

    /// <summary>
    /// 인벤토리 아이템 클릭
    /// </summary>
    private void HandleInventoryItemClicked(ItemRuntimeData clickedItem)
    {
        if (currentHero == null ||
            clickedItem == null ||
            clickedItem.itemData == null ||
            service == null)
            return;

        ItemData data = clickedItem.itemData;

        bool success = service.TryEquipItem(currentHero, data);

        if (!success)
            return;

        itemDataById[data.item_Id] = data;

        RefreshEquipmentAndInventory();
    }

    /// <summary>
    /// 장비 슬롯 클릭 시 장착 해제
    /// </summary>
    private void HandleEquipmentSlotClicked(EquipmentSlot clickedSlot)
    {
        if (currentHero == null || clickedSlot == null || service == null)
            return;

        bool isWeapon = clickedSlot == weapon;

        if (!isWeapon && clickedSlot != amore)
            return;

        int itemId = isWeapon
            ? currentHero.equippedWeaponId
            : currentHero.equippedArmorId;

        // 빈 슬롯 클릭 시 인벤토리 패널 열기
        if (itemId <= 0)
        {
            heroSelectController?.OpenInventoryPanel();
            return;
        }

        bool success = service.TryUnequipItem(currentHero, isWeapon);

        if (!success)
            return;

        RefreshEquipmentAndInventory();
    }

    /// <summary>
    /// 장비 및 인벤토리 UI 갱신
    /// </summary>
    private void RefreshEquipmentAndInventory()
    {
        ShowHero(currentHero);
        inventoryView?.RefreshInventorySlots();
    }

    private void OnDestroy()
    {
        if (inventoryView != null)
            inventoryView.OnItemSlotClicked -= HandleInventoryItemClicked;
    }
}
