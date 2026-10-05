using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable]
public struct ItemEntry
{
    public int itemId;
    public AssetReferenceT<ItemData> itemRef;
}

[CreateAssetMenu(fileName = "ItemDB",menuName ="Database/ItemDB")]
public class ItemDB : ScriptableObject
{
    [SerializeField] 
    private List<ItemEntry> items = new List<ItemEntry>();



    public AssetReferenceT<ItemData> GetItemRef(int itemId_)
    {
        var entry = items.Find(x => x.itemId == itemId_);
        return entry.itemRef; // 레퍼런스 주소만 반환
    }
}
