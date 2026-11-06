using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BattleResultManager : MonoBehaviour
{
    public static BattleResultManager I { get; private set; }

    [SerializeField] private ResultPanel resultPanel;

    [Header("보상 아이템 리스트")]
    [SerializeField]private List<ItemData> itemDatas;

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

    /// <summary>
    /// 영웅이 레벨업했을 때 호출하는 메서드
    /// </summary>
    public void AddLeveledUpHero(HeroRuntimeData heroData)
    {
        // 만약 이미 리스트에 들어있는 영웅이라면?
        if (leveledUpHeroes.Contains(heroData))
        {
            leveledUpHeroes.Remove(heroData);
        }
        leveledUpHeroes.Add(heroData);
    }

    /// <summary>
    /// 보상설정
    /// </summary>
    public void LoadFixedRewardItems()
    {
        itemDatas.Clear();

        itemDatas.AddRange(BattleEntryData.DropItems);

        Debug.Log("==== 보상 아이템 ====");
        foreach (var items in itemDatas)
        {
            Debug.Log($"아이템 이름 : {items.itemName}");
        }

    }

    public void Open(Team result)
    {   
        Time.timeScale = 0f;
        resultPanel.gameObject.SetActive(true);
        resultPanel.Init(itemDatas,leveledUpHeroes,result); 
    }



}