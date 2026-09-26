using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{   
    public static SelectManager I;


    [Header("현재 선택된 데이터 (확인용)")]
    private int test;

    void Awake()
    {
        I = this;
    }


}