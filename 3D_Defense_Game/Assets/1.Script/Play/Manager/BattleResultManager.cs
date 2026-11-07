using System.Collections.Generic;
using UnityEngine;

public class BattleResultManager : MonoBehaviour
{
    public static BattleResultManager I { get; private set; }

    [SerializeField] private ResultPanel resultPanel;

    [Header("보상 아이템 리스트")]
    [SerializeField] private List<ItemData> itemDatas;

    [Header("전투 결과 - 레벨업 영웅 목록")]
    [SerializeField] private List<HeroRuntimeData> leveledUpHeroes = new List<HeroRuntimeData>(); 

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
    }

    private void OnDestroy()
    {
        if (I == this)
        {
            I = null;
        }
    }

    public void AddLeveledUpHero(HeroRuntimeData heroData)
    {
        if (leveledUpHeroes.Contains(heroData))
        {
            leveledUpHeroes.Remove(heroData);
        }
        leveledUpHeroes.Add(heroData);
    }

    public void LoadFixedRewardItems()
    {
        itemDatas.Clear();
        itemDatas.AddRange(BattleEntryData.DropItems);
    }

    /// <summary>
    /// 전투 결과 오픈 및 데이터 처리 총괄
    /// </summary>
    public void Open(Team result)
    {
        Time.timeScale = 0f;

        // 1. 전투 결과 데이터 처리
        ProcessBattleResult(result);

        // 2. 결과창 UI 표시
        resultPanel.gameObject.SetActive(true);

        resultPanel.Init(
            itemDatas,
            leveledUpHeroes,
            result
        );
    }

    /// <summary>
    /// 전투 결과에 따른 데이터 처리
    /// </summary>
    private void ProcessBattleResult(Team result)
    {
        // 패배라면 보상 처리하지 않음
        if (result == Team.Ally)
            return;
        
        // 승리했을 때만 플레이어 데이터가 필요하므로 데이터 처리 시점에 GameManager에서 가져옴
        var playerData = GameManager.I.currentSessionData;

        // 보상 아이템 지급
        foreach (ItemData itemData in itemDatas)
        {
            playerData.AddItem(itemData);
        }

        // 레벨업 영웅 처리
        foreach (HeroRuntimeData hero in leveledUpHeroes)
        {
            if (hero.heroData.Hero_Id == 1)
            {
                hero.level++;
            }
        }

        // 스테이지 클리어 기록
        playerData.AddClearId(
            BattleEntryData.selectedEnemy.Stage_id
        );
    }
}