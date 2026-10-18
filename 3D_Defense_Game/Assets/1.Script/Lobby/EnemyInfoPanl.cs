using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyInfoPanl : MonoBehaviour
{   

    [SerializeField] private TMP_Text settlementNameText;
    [SerializeField] private TMP_Text factionRankText;
    [SerializeField] private TMP_Text specialNoteText;
    [SerializeField] private TMP_Text enemyCountText;
    [SerializeField] private Image image;
    [Header("아이템 슬롯 (최대 6개)")]
    public GameObject[] itemSlot_ = new GameObject[6];
    [Header("플레이 씬 이동 버튼")]
    public SceneType sceneType = SceneType.PlayScreen;
    public TeamCompositionController Controller;
    private EnemyInfoSO currentSO;

    /// <summary>
    /// 적 정보 SO를 받아와 패널에 내용을 채우고 엽니다.
    /// </summary>
    public void Open(EnemyInfoSO enemyInfo)
    {
        if (enemyInfo == null) return;
        currentSO = enemyInfo;

        string result = FactionUtility.EnemyStageTier(currentSO.factionRank);
        factionRankText.text = $"세력 등급 : {result}";
        settlementNameText.text = $"거점 지역 : {currentSO.settlementName}";
        specialNoteText.text = currentSO.specialNote;
        enemyCountText.text = $"예상 적 병력 : {currentSO.minEnemyCount} ~ {currentSO.maxEnemyCount} Unit";
        image.sprite = currentSO.image;

        // 전리품 슬롯 셋팅 실행
        UpdateLootSlots();
        
        // 슬룻 초기화
        Controller.TeamSlotset();
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

    

    /// <summary>
    /// 전리품 정보를 슬롯에 셋팅하고, 남는 슬롯은 비활성화합니다.
    /// </summary>
    public void UpdateLootSlots()
    {
        if (currentSO == null || currentSO.mainLoot == null) return;

        int lootCount = currentSO.mainLoot.Count;

        for (int i = 0; i < itemSlot_.Length; i++)
        {
            if (itemSlot_[i] == null) continue;

            // 1. 현재 슬롯에 표시할 전리품 데이터가 있는 경우
            if (i < lootCount)
            {
                itemSlot_[i].SetActive(true); // 슬롯 활성화

                TextMeshProUGUI lootText = itemSlot_[i].GetComponentInChildren<TextMeshProUGUI>();
                if (lootText != null)
                {
                    lootText.text = currentSO.mainLoot[i].item_name;
                }
            }
            // 2. 표시할 전리품 데이터가 부족한 경우 (남는 슬롯)
            else
            {
                itemSlot_[i].SetActive(false); // 슬롯 비활성화
            }
        }
    }
    public void OnClickBattleStart()
    {   
        if(currentSO == null) return;
        // 선택한 캐릭터 저장
        Controller.SaveSelectedHeroes();

        // 캐릭터가 한 명도 없으면 전투 시작 방지
        if (BattleEntryData.selectedHeroes.Count == 0)
        {
            Debug.LogWarning("출전할 캐릭터를 선택해주세요.");
            return;
        }
        // 적정보 전달 
        BattleEntryData.EnemyInit(currentSO);

        // 전투 씬 이동
        LoadingManager.LoadScene(sceneType,true);
    }
}