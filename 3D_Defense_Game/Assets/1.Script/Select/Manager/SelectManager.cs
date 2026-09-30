using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{   
    [Header("영지 데이터 넣는곳")]
    public List<TerritoryData> Data = new List<TerritoryData>();

    void Start()
    {
        UIManager_Select.I.Init(Data);
    }
}