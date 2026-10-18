using System.Text;
using TMPro;
using UnityEngine;

public class PlayManageer : MonoBehaviour
{
    [Header("프리팹 생성 위치")]
    public Transform StagePoint;


    void Start()
    {
        Init();
    }

    public void Init()
    {
        // 1. 스테이지 프리팹 생성
        if (BattleEntryData.currentStage != null && BattleEntryData.currentStage.StagePrefab != null)
        {
            GameObject result = BattleEntryData.currentStage.StagePrefab;
            Instantiate(result, StagePoint);
        }

    }

}