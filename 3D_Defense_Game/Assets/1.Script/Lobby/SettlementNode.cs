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

    [SerializeField] private Button button;
    [SerializeField] private Image nodeImage;

    // 클리어 여부를 저장할 내부 변수 추가
    private bool isCleared = false;

    /// <summary>
    /// 초기화 클리어 id 에따른 이미지 변화 및 클리어 상태 저장
    /// </summary>
    public void Init(List<int> clearedStageIds)
    {
        if (enemyInfo == null) return;

        // 클리어한 리스트에 현재 거점의 ID가 포함되어 있는지 확인
        isCleared = clearedStageIds != null && clearedStageIds.Contains(enemyInfo.Stage_id);
        
        UpdateNodeImage(isCleared);
    }

    /// <summary>
    /// 클리어 여부에 따라 거점 이미지 변경
    /// </summary>
    private void UpdateNodeImage(bool isCleared)
    {
        if (nodeImage == null) return;

        nodeImage.sprite = isCleared ? clearImage : defaultImage;
    }

    // 버튼 이벤트용
    public void EnemyinfoOpen()
    {

        if (isCleared) return;

        action?.Invoke(enemyInfo);
    }
}