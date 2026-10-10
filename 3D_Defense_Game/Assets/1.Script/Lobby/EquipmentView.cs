
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

    // 인벤토리에서 제거된 장비의 ItemData도 기억
    private Dictionary<int, ItemData> itemDataById =
        new Dictionary<int, ItemData>();


    /// <summary>
    /// 최초 초기화
    /// </summary>
    public void Init(List<HeroRuntimeData> heroDataList,List<ItemRuntimeData> items,InventoryView inventory,HeroSelectController controller)
    {
        inventoryView = inventory;
        heroSelectController = controller;
        itemDataById.Clear();

        // 1. 아이템 원본 데이터 캐싱
        // 장착 아이템을 인벤토리에서 제거하기 전에 저장해야 함
        if (items != null)
        {
            foreach (ItemRuntimeData item in items)
            {
                if (item == null || item.itemData == null)
                    continue;

                itemDataById[item.itemData.item_Id] =
                    item.itemData;
            }
        }

        // 2. 장비 슬롯 클릭 이벤트 연결
        if (weapon != null)
        {
            weapon.SetClickHandler(HandleEquipmentSlotClicked);
        }

        if (amore != null)
        {
            amore.SetClickHandler(HandleEquipmentSlotClicked);
        }

        // 3. 인벤토리 클릭 이벤트 연결
        if (inventoryView != null)
        {
            inventoryView.OnItemSlotClicked -= HandleInventoryItemClicked;
            inventoryView.OnItemSlotClicked += HandleInventoryItemClicked;
        }

        // 4. 저장 데이터에는 장착 아이템도 포함되어 있으므로
        // 장착 아이템을 인벤토리에서 한 번 차감
        if (heroDataList != null && inventoryView != null)
        {
            foreach (HeroRuntimeData hero in heroDataList)
            {
                if (hero == null)
                    continue;

                RemoveEquippedItem(hero.equippedWeaponId);
                RemoveEquippedItem(hero.equippedArmorId);
            }
        }

        weapon?.Clear();
        amore?.Clear();
    }


    /// <summary>
    /// 장착 아이템을 인벤토리에서 한 개 제거
    /// </summary>
    private void RemoveEquippedItem(int itemId)
    {
        if (itemId <= 0)
            return;

        bool removed = inventoryView.TryRemoveOneItem(itemId);

        if (!removed)
        {
            Debug.LogWarning(
                $"[EquipmentView] 장착 아이템 ID {itemId}를 인벤토리에서 찾지 못했습니다.");
        }
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

        weapon?.Init(CreateRuntimeData(weaponData));
        amore?.Init(CreateRuntimeData(armorData));
    }


    /// <summary>
    /// 캐싱된 ItemData 검색
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
    /// 장비 슬롯 표시용 데이터 생성
    /// </summary>
    private ItemRuntimeData CreateRuntimeData(ItemData data)
    {
        if (data == null)
            return null;

        return new ItemRuntimeData
        {
            itemData = data,
            size = 1
        };
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
    /// 무기 또는 방어구 장착
    /// </summary>
    private void EquipItem(ItemData newItem, bool isWeapon)
    {
        if (newItem == null || inventoryView == null)
            return;

        int newItemId = newItem.item_Id;

        int oldItemId = isWeapon
            ? currentHero.equippedWeaponId
            : currentHero.equippedArmorId;

        // 이미 같은 아이템을 장착하고 있으면 무시
        if (oldItemId == newItemId)
            return;

        // 기존 장비 데이터 검색
        ItemData oldItemData = GetItemData(oldItemId);

        if (oldItemId > 0 && oldItemData == null)
        {
            Debug.LogWarning("기존 장비 데이터를 찾을 수 없습니다.");
            return;
        }

        // 새 아이템을 인벤토리에서 한 개 제거
        if (!inventoryView.TryRemoveOneItem(newItemId))
            return;

        // 기존 장비를 인벤토리에 반환
        if (oldItemData != null)
        {
            if (!inventoryView.TryAddItem(oldItemData))
            {
                // 교체에 실패하면 새 아이템을 원상복구
                inventoryView.TryAddItem(newItem);
                return;
            }
        }

        // 새 아이템 데이터 기억
        itemDataById[newItemId] = newItem;

        // 영웅 장착 ID 변경
        if (isWeapon)
            currentHero.equippedWeaponId = newItemId;
        else
            currentHero.equippedArmorId = newItemId;

        ShowHero(currentHero);
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
            heroSelectController.OpenInventoryPanel();
            return;
        }

        // 장착된 장비 데이터 가져오기
        ItemData data = clickedSlot.RuntimeData?.itemData;

        if (data == null)
            data = GetItemData(itemId);

        if (data == null)
        {
            Debug.LogWarning("해제할 장비 데이터를 찾을 수 없습니다.");
            return;
        }

        // 인벤토리가 가득 차면 장착 해제하지 않음
        if (!inventoryView.TryAddItem(data))
        {
            Debug.Log("인벤토리가 가득 차서 장비를 해제할 수 없습니다.");
            return;
        }

        // 장착 ID 초기화
        if (isWeapon)
            currentHero.equippedWeaponId = NoItemId;
        else
            currentHero.equippedArmorId = NoItemId;

        ShowHero(currentHero);
    }



    private void OnDestroy()
    {
        if (inventoryView != null)
        {
            inventoryView.OnItemSlotClicked -= HandleInventoryItemClicked;
        }
    }
}
