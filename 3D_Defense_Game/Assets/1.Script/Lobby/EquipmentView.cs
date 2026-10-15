using System.Collections.Generic;
using UnityEngine;

// 장비 UI 표시 및 클릭 이벤트 처리
public class EquipmentView : MonoBehaviour
{
    [Header("장비 슬롯")]
    public EquipmentSlot weapon;
    public EquipmentSlot amore;

    private HeroRuntimeData currentHero;
    private List<ItemRuntimeData> items;

    private InventoryView inventoryView;
    private HeroSelectController heroSelectController;
    private InventoryEquipmentService service;

    // 초기화
    public void Init(
        List<ItemRuntimeData> items,
        InventoryView inventory,
        HeroSelectController controller,
        InventoryEquipmentService sharedService)
    {
        this.items = items;
        inventoryView = inventory;
        heroSelectController = controller;
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

    // 선택한 영웅의 장비 표시
    public void ShowHero(HeroRuntimeData hero)
    {
        currentHero = hero;

        if (currentHero == null)
        {
            weapon?.Clear();
            amore?.Clear();
            return;
        }

        weapon?.Init(GetItemData(currentHero.equippedWeaponId));
        amore?.Init(GetItemData(currentHero.equippedArmorId));
    }

    // 아이템 ID로 데이터 찾기
    private ItemData GetItemData(int itemId)
    {
        if (items == null || itemId <= 0)
            return null;

        foreach (ItemRuntimeData item in items)
        {
            if (item == null || item.itemData == null)
                continue;

            if (item.itemData.item_Id == itemId)
                return item.itemData;
        }

        return null;
    }

    // 인벤토리 아이템 클릭: 장착
    private void HandleInventoryItemClicked(ItemRuntimeData item)
    {
        if (currentHero == null ||
            item == null ||
            item.itemData == null ||
            service == null)
            return;

        if (service.TryEquipItem(currentHero, item.itemData))
            RefreshUI();
    }

    // 장비 슬롯 클릭: 장착 해제
    private void HandleEquipmentSlotClicked(EquipmentSlot slot)
    {
        if (currentHero == null || slot == null || service == null)
            return;

        bool isWeapon = slot == weapon;

        if (!isWeapon && slot != amore)
            return;

        int itemId = isWeapon
            ? currentHero.equippedWeaponId
            : currentHero.equippedArmorId;

        // 빈 슬롯이면 인벤토리 패널 열기
        if (itemId <= 0)
        {
            heroSelectController?.OpenInventoryPanel();
            return;
        }

        if (service.TryUnequipItem(currentHero, isWeapon))
            RefreshUI();
    }

    // 장비와 인벤토리 UI 갱신
    private void RefreshUI()
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
