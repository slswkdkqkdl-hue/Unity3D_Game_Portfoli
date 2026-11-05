using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BattleResultManager : MonoBehaviour
{
    public static BattleResultManager I { get; private set; }

    [SerializeField] private ResultPanel resultPanel;

    [Header("불러온 전리품 아이템 데이터 리스트")]
    public List<ItemData> itemDatas;


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
    /// 보상설정 // 플레이 매니저에서실행
    /// </summary>
    public void LoadFixedRewardItems()
    {
        itemDatas.Clear();

        itemDatas = BattleEntryData.DropItems;
        Debug.Log("==== 보상 아이템 ====");
        foreach (var items in itemDatas)
        {
            Debug.Log($"아이템 이름 : {items.itemName}");
        }

    }

    public void Open(bool result)
    {   
        Time.timeScale = 0f;
        resultPanel.gameObject.SetActive(true);
        // resultPanel.Init(itemDatas,heroes,result); 
    }



}