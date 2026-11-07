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
    
    public void Init(List<ItemData> itemDatas, List<HeroRuntimeData> heroes, Team team)
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

            // 오직 UI 생성 및 세팅만 수행
            foreach (ItemData itemData in itemDatas)
            {
                GameObject slotObject = Instantiate(DropSlot, DropPos);
                DropitemSlot slot = slotObject.GetComponent<DropitemSlot>();
                if (slot == null)
                {
                    Debug.LogError("DropSlot 프리팹에 DropitemSlot 컴포넌트가 없습니다.");
                    continue;
                }

                slot.Init(itemData.icon, itemData.itemName, 1);
            }

            // 여기에 영웅관련생성
        }
    }

    public void BackButton()
    { 
        BattleEntryData.Clear();
        LoadingManager.LoadScene(SceneType.LobbyScreen);
    }
}