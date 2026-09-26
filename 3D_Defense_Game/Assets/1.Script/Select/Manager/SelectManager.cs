using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{   
    public static SelectManager I;


    [Header("현재 선택된 데이터 (확인용)")]
    [SerializeField] private HeroDescData selectedHero;
    [SerializeField] private List<UnitDescData> selectedUnits = new List<UnitDescData>();

    void Awake()
    {
        I = this;
    }


}