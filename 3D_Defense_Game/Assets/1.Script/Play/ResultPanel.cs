using System.Collections.Generic;
using UnityEngine;

public class ResultPanel : MonoBehaviour
{
    public Transform DropPos;
    public GameObject DropSlot;

    public HeroLevelSlot[] HeroSlot = new HeroLevelSlot[3];

    private readonly List<GameObject> createdSlots = new List<GameObject>();

    public void Init(List<ItemData> itemDatas,List<HeroRuntimeData> heroes)
    {
        // 기존 슬롯 정리 (패널 재초기화 대비)
        foreach (GameObject slot in createdSlots)
        {
            if (slot != null)
                Destroy(slot);
        }

        createdSlots.Clear();

        if (itemDatas == null || DropPos == null || DropSlot == null)
            return;

        foreach (ItemData itemData in itemDatas)
        {
            if (itemData == null)
                continue;

            GameObject slotObject = Instantiate(DropSlot, DropPos);
            createdSlots.Add(slotObject);

            DropitemSlot slot = slotObject.GetComponent<DropitemSlot>();

            if (slot == null)
            {
                Debug.LogError("DropSlot 프리팹에 DropitemSlot 컴포넌트가 없습니다.");
                continue;
            }

            // 아래 필드명은 실제 ItemData에 맞게 수정
            slot.Init(itemData.icon,itemData.itemName,1);
            DataManager.I.sessionData.AddItem(itemData);

        }

        foreach (HeroRuntimeData data in heroes)
        {
            if ( data.heroData.Hero_Id == 1)
            {   
                data.level++;
                HeroSlot[0].Init(data.heroData.portrait,data.level);                
            }
        }
    }


    public void BackButton()
    {   
        BattlefieldManager.I.PauseAllCombatEntities();
        DataManager.I.sessionData.AddClearId(BattleEntryData.selectedEnemy.Stage_id);
        BattleEntryData.Clear();
        LoadingManager.LoadScene(SceneType.LobbyScreen);
    }
}