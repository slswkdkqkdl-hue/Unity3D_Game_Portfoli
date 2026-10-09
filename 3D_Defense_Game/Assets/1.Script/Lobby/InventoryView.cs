using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int itemSize = 20;            // 인벤토리 최대 슬롯 수
    public GameObject itemSlotPrefab;   // itemSlot 프리팹
    public Transform itemSlotParent;    // 슬롯들이 배치될 Grid Parent Transform

    // 현재 관리하는 인벤토리 데이터 (UI 표시용)
    private List<ItemRuntimeData> inventoryItems = new List<ItemRuntimeData>();
    // 생성된 슬롯 UI 리스트
    private List<itemSlot> inventorySlots = new List<itemSlot>();

    // 아이템 슬롯이 클릭되었을 때 Controller나 EquipmentView로 전달할 이벤트
    public Action<ItemRuntimeData> OnItemSlotClicked;

    /// <summary>
    /// 인벤토리 최초 초기화 및 데이터 바인딩
    /// </summary>
    public void Init(List<ItemRuntimeData> items)
    {
        inventoryItems = items ?? new List<ItemRuntimeData>();

        // 1. 슬롯 오브젝트가 부족한 경우 itemSize만큼 생성 (재사용 구조)
        while (inventorySlots.Count < itemSize)
        {
            GameObject obj = Instantiate(itemSlotPrefab, itemSlotParent);
            itemSlot slot = obj.GetComponent<itemSlot>();

            // 슬롯 클릭 이벤트 바인딩
            slot.onSlotClicked = HandleSlotClicked;

            inventorySlots.Add(slot);
        }

        // 2. UI 데이터 갱신
        RefreshInventorySlots();
    }

    /// <summary>
    /// 인벤토리 슬롯 UI 전체 갱신
    /// </summary>
    public void RefreshInventorySlots()
    {
        if (inventoryItems == null) return;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < inventoryItems.Count)
            {
                inventorySlots[i].Init(inventoryItems[i]);
            }
            else
            {
                inventorySlots[i].Init(null); // 빈 슬롯 처리
            }
        }
    }

    /// <summary>
    /// 슬롯 클릭 콜백 전달
    /// </summary>
    private void HandleSlotClicked(ItemRuntimeData clickedItem)
    {
        OnItemSlotClicked?.Invoke(clickedItem);
    }
}