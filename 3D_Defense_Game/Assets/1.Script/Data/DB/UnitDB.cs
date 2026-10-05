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


    public AssetReferenceT<RuntimeUnitData> GetUnitRef(int id)
    {
        UnitEntry unit = units.Find(x=> x.UnitId == id);
        if (unit.UnitRef !=null)
        {
            return unit.UnitRef;
        }
        return null;
    }


}
