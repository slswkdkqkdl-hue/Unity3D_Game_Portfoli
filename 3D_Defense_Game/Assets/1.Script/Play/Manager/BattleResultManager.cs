using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeroLevelUpResult
{
    public HeroRuntimeData heroData; // 레벨업한 영웅의 데이터
    public int startLevel;           // 전투 시작 시 레벨
    public int endLevel;             // 전투 종료 후 레벨
}

public class BattleResultManager : MonoBehaviour
{
    public static BattleResultManager I { get; private set; }

    [SerializeField] private ResultPanel resultPanel;

    [Header("보상 아이템")]
    [SerializeField] private List<ItemData> itemDatas = new List<ItemData>();

    [Header("전투 참가 영웅")]
    private List<HeroBase> battleHeroes = new List<HeroBase>();

    [Header("레벨업 결과")]
    [SerializeField] private List<HeroLevelUpResult> levelUpResults= new List<HeroLevelUpResult>();


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


    /// <summary>
    /// 전투에 참가하는 영웅을 등록합니다.
    /// Herocontroller에서 HeroBase 생성 후 호출합니다.
    /// </summary>
    public void RegisterHero(HeroBase hero)
    {
        if (hero == null)
            return;

        if (!battleHeroes.Contains(hero))
        {
            battleHeroes.Add(hero);
        }
    }


    /// <summary>
    /// 보상 아이템 및 전투 결과 데이터를 초기화합니다.
    /// PlayManager에서 전투 시작 시 호출
    /// </summary>
    public void LoadFixedRewardItems(List<HeroRuntimeData> heroData)
    {
        itemDatas.Clear();
        battleHeroes.Clear();
        levelUpResults.Clear();

        itemDatas.AddRange(BattleEntryData.DropItems);

        resultPanel.gameObject.SetActive(false);
    }


    /// <summary>
    /// 전투 결과 오픈 및 데이터 처리 총괄
    /// </summary>
    public void Open(Team result)
    {
        Time.timeScale = 0f;

        // 전투 결과 데이터 처리
        ProcessBattleResult(result);

        // 결과창 UI 표시
        resultPanel.gameObject.SetActive(true);

        // 결과 데이터 주입
        resultPanel.Init(
            itemDatas,
            levelUpResults,
            result
        );
    }


    /// <summary>
    /// 전투 결과에 따른 데이터 처리
    /// </summary>
    private void ProcessBattleResult(Team result)
    {
        // 패배라면 아무것도 저장하지 않음
        if (result == Team.Ally)
            return;

        // 승리했을 때만 플레이어 데이터를 가져옴
        var playerData = GameManager.I.currentSessionData;


        // 보상 아이템 지급
        foreach (ItemData itemData in itemDatas)
        {
            playerData.AddItem(itemData);
        }


        // 모든 영웅 경험치 정산
        ProcessHeroExp();


        // 스테이지 클리어 기록
        playerData.AddClearId(
            BattleEntryData.selectedEnemy.Stage_id
        );
    }


    /// <summary>
    /// 전투에 참가한 모든 영웅의 경험치를 정산합니다.
    /// </summary>
    private void ProcessHeroExp()
    {
        foreach (HeroBase hero in battleHeroes)
        {
            if (hero == null)
                continue;

            HeroRuntimeData data = hero.RuntimeData;

            if (data == null || data.heroData == null)
                continue;


            // 전투 시작 당시 레벨
            int startLevel = data.level;

            // 저장되어 있던 경험치 + 이번 전투에서 획득한 경험치
            float totalExp = data.currentExp + hero.BattleExp;

            // 현재 레벨
            int level = data.level;

            // 현재 레벨에서 필요한 최대 경험치 계산
            float maxExp = data.heroData.baseExp;

            for (int i = 1; i < level; i++)
            {
                maxExp *= data.heroData.expMultiplier;
            }


            // 레벨업 처리
            while (totalExp >= maxExp &&
                   level < data.heroData.maxLevel)
            {
                totalExp -= maxExp;
                level++;

                maxExp *= data.heroData.expMultiplier;
            }


            // 최대 레벨이면 경험치를 더 이상 쌓지 않음
            if (level >= data.heroData.maxLevel)
            {
                totalExp = maxExp;
            }


            // ----------------------------------------
            // 실제 HeroRuntimeData에 최종 결과 저장
            // ----------------------------------------

            data.level = level;
            data.currentExp = totalExp;
            data.maxExp = maxExp;


            // ----------------------------------------
            // 레벨업한 영웅이라면 결과창 데이터 생성
            // ----------------------------------------

            if (level > startLevel)
            {
                levelUpResults.Add(new HeroLevelUpResult
                {
                    heroData = data,
                    startLevel = startLevel,
                    endLevel = level
                });
            }
        }
    }
}

