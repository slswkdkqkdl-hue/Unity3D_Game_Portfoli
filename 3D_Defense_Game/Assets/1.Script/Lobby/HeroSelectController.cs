using System.Collections.Generic;
using UnityEngine;

public class HeroSelectController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform slotParent;      
    [SerializeField] private HeroSelectSlot slotPrefab;     

    /// <summary>
    /// 전달받은 영웅 데이터 리스트로 슬롯 목록을 초기화
    /// </summary>
    public void Init(List<HeroRuntimeData> heroDataList)
    {
        ClearSlots();
        if (heroDataList == null || heroDataList.Count == 0)
            return;
        foreach (HeroRuntimeData data in heroDataList)
        {
            HeroSelectSlot slotInstance = Instantiate(slotPrefab, slotParent);
            slotInstance.Init(data);
        }
    }

    /// <summary>
    /// 기존 슬롯 오브젝트들을 모두 삭제
    /// </summary>
    private void ClearSlots()
    {
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }
    }
}