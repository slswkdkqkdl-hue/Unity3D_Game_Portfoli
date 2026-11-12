using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultPanel : MonoBehaviour
{ 
    [SerializeField]private Transform heroPos; // 영웅 슬롯 생성 위치 
    [SerializeField]private Transform DropPos; // 보상 아이템 생성 위치
    [SerializeField]private TextMeshProUGUI result_; // 승리/패배 텍스트

    [Header("영웅 슬롯 프리팹")]
    [SerializeField]private GameObject heroLevelSlotPrefab;

    [Header("보상 아이템 프리팹")]
    [SerializeField]private GameObject dropSlotPrefab; 
    
    [SerializeField]private SceneType LobbyBtn = SceneType.LobbyScreen;

    /// <summary>
    /// 결과 창 초기화 (승패 판정 및 보상/영웅 UI 생성)
    /// </summary>
    public void Init(List<ItemData> itemDatas, List<HeroLevelUpResult> heroes, Team team)
    {
        if (Team.Ally == team)
        {
            result_.text = "패배";
            // 패배 시 필요한 UI 처리 (예: 아이템 슬롯 숨기기 등) 여기서 수행 가능
        }
        else
        { 
            result_.text = "승리";

            // 보상 아이템 슬롯 생성
            if (itemDatas != null && DropPos != null && dropSlotPrefab != null)
            {
                CreateRewardItemSlots(itemDatas);
            }

            // 레벨업한 영웅 슬롯 생성 
            if (heroes != null && heroPos != null && heroLevelSlotPrefab != null)
            {
                CreateHeroLevelSlots(heroes);
            }
        }
    }
    
    /// <summary>
    /// 동일한 아이템을 묶어 개수를 합산하고 보상 슬롯 생성
    /// </summary>
    private void CreateRewardItemSlots(List<ItemData> itemDatas)
    {   
        // 아이템 종류별 개수를 담을 딕셔너리 
        Dictionary<ItemData, int> itemCountMap = new Dictionary<ItemData, int>();

        foreach (ItemData itemData in itemDatas)
        {
            if (itemCountMap.ContainsKey(itemData))
            {
                itemCountMap[itemData]++;
            }
            else
            {
                itemCountMap.Add(itemData, 1);
            }
        }
        foreach (var itemData in itemCountMap)
        {   
            ItemData  item = itemData.Key;
            int count = itemData.Value;

            GameObject slotObject = Instantiate(dropSlotPrefab, DropPos);
            DropitemSlot slot = slotObject.GetComponent<DropitemSlot>();
            if (slot == null)
            {
                Debug.LogError("DropSlot 프리팹에 DropitemSlot 컴포넌트가 없습니다.");
                    continue;
            }

            slot.Init(item.icon, item.itemName, count);
        }
    }
    /// <summary>
    /// 레벨업한 영웅들의 데이터를 받아 UI 슬롯을 생성합니다.
    /// </summary>
    private void CreateHeroLevelSlots(List<HeroLevelUpResult> heroes)
    {
        foreach (var data in heroes)
        {
            GameObject heroObj = Instantiate(heroLevelSlotPrefab, heroPos);
            HeroLevelSlot heroSlot = heroObj.GetComponent<HeroLevelSlot>();
            heroSlot.Init(data);
        }
    }

    // 로비로 돌아가기버튼
    public void BackButton()
    { 
        BattleEntryData.Clear();
        LoadingManager.LoadScene(LobbyBtn);
    }
}