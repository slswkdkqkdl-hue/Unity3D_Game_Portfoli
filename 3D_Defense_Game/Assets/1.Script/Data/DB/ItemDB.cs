using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable]
public struct ItemEntry
{
    public int itemId;
    public AssetReferenceT<RuntimeItemData> itemRef;
}

[CreateAssetMenu(fileName = "ItemDB",menuName ="Database/ItemDB")]
public class ItemDB : ScriptableObject
{
    [SerializeField] 
    private List<ItemEntry> items = new List<ItemEntry>();



    public AssetReferenceT<RuntimeItemData> GetItemRef(int Id)
    {
        
        ItemEntry  runtimeItem =  items.Find(x => x.itemId == Id);
        if (runtimeItem.itemRef != null)
        {
            return runtimeItem.itemRef;
        }

        return null;
    }
}
