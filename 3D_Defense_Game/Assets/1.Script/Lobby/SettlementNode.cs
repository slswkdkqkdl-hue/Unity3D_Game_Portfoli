using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettlementNode : MonoBehaviour
{
    [Header("거점 정보")]
    public EnemyInfoSO enemyInfo;

    [Header("이미지 설정")]
    [SerializeField] private Sprite defaultImage; 
    [SerializeField] private Sprite clearImage;
    public Action<EnemyInfoSO> action;

    [SerializeField]private Button button;
    [SerializeField]private Image nodeImage;



    /// <summary>
    /// 초기화 클리어 id 에따른 이미지 변화
    /// </summary>
    public void Init(List<int> clearedStageIds)
    {
        if (enemyInfo == null) return;
        // 클리어한 리스트에 현재 거점의 ID가 포함되어 있는지 확인
        bool isCleared = clearedStageIds != null && clearedStageIds.Contains(enemyInfo.Stage_id);
        UpdateNodeImage(isCleared);
    }

    /// <summary>
    /// 클리어 여부에 따라 거점 이미지 변경
    /// </summary>
    private void UpdateNodeImage(bool isCleared)
    {
        if (nodeImage == null) return;

        if (isCleared)
        {
            nodeImage.sprite = clearImage;
        }
        else
        {
            nodeImage.sprite = defaultImage;
        }
    }

    // 버튼 이벤트용
    public void EnemyinfoOpen()
    {
        action?.Invoke(enemyInfo);
    }
    
}