using System.Collections.Generic;
using UnityEngine;

public class HeroSelectController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform slotParent;

    [SerializeField] private HeroSelectSlot slotPrefab;     
    [SerializeField] private HeroDetailView heroDetailView;

    /// <summary>
    /// 전달받은 영웅 데이터 리스트로 슬롯 목록을 초기화
    /// </summary>
    public void Init(List<HeroRuntimeData> heroDataList)
    {
        ClearSlots();
        if (heroDataList == null || heroDataList.Count == 0)
            return;

        // 일반 for 문을 사용하면 인덱스(i)를 바로 활용할 수 있습니다.
        for (int i = 0; i < heroDataList.Count; i++)
        {
            HeroRuntimeData data = heroDataList[i];
            
            HeroSelectSlot slotInstance = Instantiate(slotPrefab, slotParent);
            slotInstance.Init(data);
            slotInstance.OnSlotClicked += HandleSlotClicked;

            // 맨 처음(0번째) 영웅은 자동 선택 처리
            if (i == 0)
            {
                HandleSlotClicked(data);
            }
        }
    }

    /// <summary>
    /// 슬롯이 클릭되었을 때 호출되는 콜백 함수
    /// </summary>
    private void HandleSlotClicked(HeroRuntimeData clickedHeroData)
    {
        Debug.Log($"선택된 영웅 이름: {clickedHeroData.heroData.hero_name}");
        
        // TODO: 여기서 선택된 영웅 정보를 바탕으로 상세 정보를 띄우거나 다음 로직 수행
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