using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultPanel : MonoBehaviour
{
    public Transform DropPos; // 보상 아이템 생성 위치
    public GameObject DropSlot; // 보상 아이템 프리팹 

    public TextMeshProUGUI result_; // 승리/패배 텍스트


    [Header("영웅 슬릇프리팹들")]
    public GameObject HeroLevelSlot;
    

    public void Init(List<ItemData> itemDatas,List<HeroRuntimeData> heroes , Team team)
    {
        var PlayerData = GameManager.I.currentSessionData;

        if (Team.Ally == team)
        {
            result_.text = "패배";
        }
        else
        {   
            result_.text = "승리";

            if (itemDatas == null || DropPos == null || DropSlot == null)
                return;

            foreach (ItemData itemData in itemDatas)
            {
                GameObject slotObject = Instantiate(DropSlot, DropPos);
                DropitemSlot slot = slotObject.GetComponent<DropitemSlot>();
                if (slot == null)
                {
                    Debug.LogError("DropSlot 프리팹에 DropitemSlot 컴포넌트가 없습니다.");
                    continue;
                }

                slot.Init(itemData.icon,itemData.itemName,1);
                PlayerData.AddItem(itemData);

            }

            foreach (HeroRuntimeData data in heroes)
            {
                if ( data.heroData.Hero_Id == 1)
                {   
                    data.level++;       
                }
            }
            PlayerData.AddClearId(BattleEntryData.selectedEnemy.Stage_id);
        }

    }


    public void BackButton()
    {   
        BattleEntryData.Clear();
        LoadingManager.LoadScene(SceneType.LobbyScreen);
    }
}