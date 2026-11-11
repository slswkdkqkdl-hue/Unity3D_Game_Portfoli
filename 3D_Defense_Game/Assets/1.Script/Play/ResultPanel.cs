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
    public void Init(List<ItemData> itemDatas, List<HeroLevelUpResult> heroes, Team team)
    {
        Debug.Log("===== ResultPanel.Init 시작 =====");

        if (heroes == null)
        {
            Debug.LogError("heroes가 NULL입니다.");
        }
        else
        {
            Debug.Log($"heroes 개수: {heroes.Count}");

            foreach (HeroLevelUpResult result in heroes)
            {
                if (result == null)
                {
                    Debug.LogWarning("HeroLevelUpResult가 NULL입니다.");
                    continue;
                }

                if (result.heroData == null)
                {
                    Debug.LogWarning("heroData가 NULL입니다.");
                    continue;
                }

                HeroRuntimeData runtimeData = result.heroData;

                string heroName = runtimeData.heroData != null
                    ? runtimeData.heroData.hero_name
                    : "영웅 이름 없음";

                Debug.Log(
                    $"[레벨업 영웅]\n" +
                    $"영웅 이름: {heroName}\n" +
                    $"시작 레벨: {result.startLevel}\n" +
                    $"종료 레벨: {result.endLevel}\n" +
                    $"현재 레벨: {runtimeData.level}\n" +
                    $"현재 경험치: {runtimeData.currentExp}\n" +
                    $"최대 경험치: {runtimeData.maxExp}"
                );
            }
        }

        Debug.Log("===== ResultPanel.Init 종료 =====");


        if (Team.Ally == team)
        {
            result_.text = "패배";
        }
        else
        { 
            result_.text = "승리";

            if (itemDatas == null || DropPos == null || DropSlot == null)
                return;

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