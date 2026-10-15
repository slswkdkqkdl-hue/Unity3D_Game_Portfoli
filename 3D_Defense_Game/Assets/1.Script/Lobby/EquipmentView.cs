using System.Collections.Generic;
using UnityEngine;

public class EquipmentView : MonoBehaviour
{
    [Header("장비 슬롯")]
    public EquipmentSlot weapon;
    public EquipmentSlot amore;

    private const int NoItemId = -1;

    private HeroRuntimeData currentHero;
    private InventoryView inventoryView;
    private HeroSelectController heroSelectController;

    // 아이템 ID로 ItemData를 검색하기 위한 캐시
    private Dictionary<int, ItemData> itemDataById =
        new Dictionary<int, ItemData>();


    /// <summary>
    /// 장비 UI 초기화
    /// </summary>
    public void Init(
        List<HeroRuntimeData> heroDataList,
        List<ItemRuntimeData> items,
        InventoryView inventory,
        HeroSelectController controller)
    {
        inventoryView = inventory;
        heroSelectController = controller;

        itemDataById.Clear();

        // 전체 보유 아이템의 ItemData를 캐싱한다.
        // 장착된 아이템도 보유 목록에 남아 있으므로 찾을 수 있다.
        if (items != null)
        {
            foreach (ItemRuntimeData item in items)
            {
                if (item == null || item.itemData == null)
                    continue;

                int itemId = item.itemData.item_Id;

                if (!itemDataById.ContainsKey(itemId))
                {
                    itemDataById.Add(itemId, item.itemData);
                }
            }
        }

        // 장비 슬롯 이벤트 연결
        weapon?.SetClickHandler(HandleEquipmentSlotClicked);
        amore?.SetClickHandler(HandleEquipmentSlotClicked);

        // 인벤토리 클릭 이벤트 연결
        if (inventoryView != null)
        {
            inventoryView.OnItemSlotClicked -= HandleInventoryItemClicked;
            inventoryView.OnItemSlotClicked += HandleInventoryItemClicked;
        }

        // 중요: 여기서 아이템 수량을 변경하지 않는다.
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

        ItemData weaponData =
            GetItemData(currentHero.equippedWeaponId);

        ItemData armorData =
            GetItemData(currentHero.equippedArmorId);

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
    private void HandleInventoryItemClicked(
        ItemRuntimeData clickedItem)
    {
        if (currentHero == null)
            return;

        if (clickedItem == null || clickedItem.itemData == null)
            return;

        ItemData data = clickedItem.itemData;

        if (data.itemType == ItemType.Weapon)
        {
            EquipItem(data, true);
        }
        else if (data.itemType == ItemType.Armor)
        {
            EquipItem(data, false);
        }
    }


    /// <summary>
    /// 무기 또는 방어구 장착 및 교체
    /// 실제 보유 아이템 수량은 변경하지 않는다.
    /// </summary>
    private void EquipItem(ItemData newItem, bool isWeapon)
    {
        if (newItem == null || currentHero == null)
            return;

        int newItemId = newItem.item_Id;

        // 같은 아이템을 이미 장착했다면 처리하지 않는다.
        int oldItemId = isWeapon
            ? currentHero.equippedWeaponId
            : currentHero.equippedArmorId;

        if (oldItemId == newItemId)
            return;

        // 실제로 장착 가능한 수량이 있는지 확인한다.
        if (inventoryView == null ||
            inventoryView.GetAvailableItemCount(newItemId) <= 0)
        {
            return;
        }

        // UI에서도 해당 아이템을 검색할 수 있도록 캐싱한다.
        itemDataById[newItemId] = newItem;

        // 장착 상태만 변경한다.
        if (isWeapon)
        {
            currentHero.equippedWeaponId = newItemId;
        }
        else
        {
            currentHero.equippedArmorId = newItemId;
        }

        // 장비 UI 및 인벤토리 표시 갱신
        ShowHero(currentHero);
        inventoryView.RefreshInventorySlots();
    }


    /// <summary>
    /// 장비 슬롯 클릭 시 장착 해제
    /// </summary>
    private void HandleEquipmentSlotClicked(EquipmentSlot clickedSlot)
    {
        if (currentHero == null || clickedSlot == null)
            return;

        bool isWeapon = clickedSlot == weapon;

        if (!isWeapon && clickedSlot != amore)
            return;

        int itemId = isWeapon
            ? currentHero.equippedWeaponId
            : currentHero.equippedArmorId;

        // 장비가 비어 있으면 인벤토리 패널 열기
        if (itemId <= 0)
        {
            heroSelectController?.OpenInventoryPanel();
            return;
        }

        // 장착 ID만 초기화
        if (isWeapon)
        {
            currentHero.equippedWeaponId = NoItemId;
        }
        else
        {
            currentHero.equippedArmorId = NoItemId;
        }

        // UI 갱신
        ShowHero(currentHero);
        inventoryView?.RefreshInventorySlots();
    }


    private void OnDestroy()
    {
        if (inventoryView != null)
        {
            inventoryView.OnItemSlotClicked -= HandleInventoryItemClicked;
        }
    }
}