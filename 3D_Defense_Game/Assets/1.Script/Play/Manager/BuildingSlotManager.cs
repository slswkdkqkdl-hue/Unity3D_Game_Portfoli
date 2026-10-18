using UnityEngine;

public class BuildingSlotManager : MonoBehaviour
{
    [Header("1레벨 슬롯 (메인 건물 주변)")]
    [SerializeField] private GameObject[] level1Slots = new GameObject[6];

    [Header("2레벨 슬롯 (중간 영역)")]
    [SerializeField] private GameObject[] level2Slots = new GameObject[4];

    [Header("3레벨 슬롯 (외곽 영역)")]
    [SerializeField] private GameObject[] level3Slots = new GameObject[5];


    // 레벨에 따른 슬릇 활성화
    public void InitializeSlots(int level)
    {
        SetSlotsActive(level1Slots, level >= 1);
        SetSlotsActive(level2Slots, level >= 2);
        SetSlotsActive(level3Slots, level >= 3);
    }

    private void SetSlotsActive(GameObject[] slots, bool isActive)
    {
        foreach (GameObject slot in slots)
        {
            if (slot != null)
                slot.SetActive(isActive);
        }
    }
}