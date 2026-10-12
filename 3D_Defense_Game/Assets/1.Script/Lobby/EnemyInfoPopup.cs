using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyInfoPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text settlementNameText;
    [SerializeField] private TMP_Text factionRankText;
    [SerializeField] private TMP_Text specialNoteText;
    [SerializeField] private TMP_Text enemyCountText;
    [SerializeField] private Image image;
    // 전리품 UI 리스트 등 추가 가능

    /// <summary>
    /// 적 정보 SO를 받아와 패널에 내용을 채우고 엽니다.
    /// </summary>
    public void Open(EnemyInfoSO enemyInfo)
    {
        if (enemyInfo == null) return;

        settlementNameText.text = enemyInfo.settlementName;
        factionRankText.text = enemyInfo.factionRank.ToString();
        specialNoteText.text = enemyInfo.specialNote;
        enemyCountText.text = $"{enemyInfo.minEnemyCount} ~ {enemyInfo.maxEnemyCount}";
        image.sprite = enemyInfo.image;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void Open()
    {
        gameObject.SetActive(true);
    }
    
}