using System.Collections.Generic;
using UnityEngine;

public class EquipmentView : MonoBehaviour
{   

    public EquipmentSlot weapon;
    public EquipmentSlot amore;



    public void Init(List<HeroRuntimeData> herodatas , List<ItemRuntimeData> items)
    {   
              Debug.Log("실행댐3");
        foreach (HeroRuntimeData data in herodatas)
        {   
                  Debug.Log($"아이템 id {data.equippedWeaponId}");
            if(data.equippedWeaponId != -1)
            {   
                ItemRuntimeData result = items.Find(x => x.itemData.item_Id == data.equippedWeaponId);
                items.Remove(result);
                weapon.Init(result);
                Debug.Log("실행댐");
            }
        }
    }



    public void ShowHero(HeroRuntimeData data)
    {
        
    }
}
