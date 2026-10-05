using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


[System.Serializable]
public struct UnitEntry
{
    public int UnitId;
    public AssetReferenceT<RuntimeUnitData> UnitRef;
}


[CreateAssetMenu(fileName = "UnitDb",menuName = "Database/UnitDB")]
public class UnitDB : ScriptableObject
{
    [SerializeField]
    private List<UnitEntry> units = new List<UnitEntry>();


    public AssetReferenceT<RuntimeUnitData> GetUnitRef(int UnitId_)
    {
        var entry = units.Find(x => x.UnitId == UnitId_);
        return entry.UnitRef; // 레퍼런스 주소만 반환
    }


}
