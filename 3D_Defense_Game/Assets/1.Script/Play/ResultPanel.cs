using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultPanel : MonoBehaviour
{ 
    public Transform heroPos; // 영웅 슬롯 생성 위치 
    public Transform DropPos; // 보상 아이템 생성 위치
    public TextMeshProUGUI result_; // 승리/패배 텍스트

    [Header("영웅 슬롯 프리팹")]
    public GameObject HeroLevelSlot;

    [Header("보상 아이템 프리팹")]
    public GameObject DropSlot; 
    

    /// <summary>
    /// 결과 창 초기화 (승패 판정 및 보상/영웅 UI 생성)
    /// </summary>
    public void Init(List<ItemData> itemDatas,
    List<HeroRuntimeData> leveledUpHeroes,
    Dictionary<HeroRuntimeData, int> heroStartLevels,
    Team team)
    {
        if (Team.Ally == team)
        {
            result_.text = "패배";
            // 패배 시 필요한 UI 처리 (예: 아이템 슬롯 숨기기 등) 여기서 수행 가능
        }
        else
        { 
            result_.text = "승리";

            if (itemDatas == null || DropPos == null || DropSlot == null)
                return;

            // 여기서 아이템 관련 생성
            CreateRewardItemSlots(itemDatas);


            // 여기에 영웅관련생성
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
            // 이미 딕셔너리에 들어있는 아이템이라면 개수 1 증가
            if (itemCountMap.ContainsKey(itemData))
            {
                itemCountMap[itemData]++;
            }
            // 처음 나온 아이템이라면 딕셔너리에 새로 추가 (개수 1)
            else
            {
                itemCountMap.Add(itemData, 1);
            }
        }
        // 딕셔너리에서 값을꺼내서 슬릇에 데이터 할당 
        foreach (var itemData in itemCountMap)
        {   
            ItemData  item = itemData.Key;
            int count = itemData.Value;

            GameObject slotObject = Instantiate(DropSlot, DropPos);
            DropitemSlot slot = slotObject.GetComponent<DropitemSlot>();
            if (slot == null)
            {
                Debug.LogError("DropSlot 프리팹에 DropitemSlot 컴포넌트가 없습니다.");
                    continue;
            }

            slot.Init(item.icon, item.itemName, count);
        }
    }

    // 로비로 돌아가기버튼
    public void BackButton()
    { 
        BattleEntryData.Clear();
        LoadingManager.LoadScene(SceneType.LobbyScreen);
    }
}