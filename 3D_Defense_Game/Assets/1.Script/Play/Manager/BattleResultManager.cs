using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BattleResultManager : MonoBehaviour
{
    public static BattleResultManager I { get; private set; }

    [SerializeField] private ResultPanel resultPanel;

    [SerializeField] private List<HeroRuntimeData> heroes;

    [Header("불러온 전리품 아이템 데이터 리스트")]
    public List<ItemData> itemDatas = new List<ItemData>();

    // 메모리 해제를 위한 핸들 보관용 리스트
    private List<AsyncOperationHandle> loadedItemHandles = new List<AsyncOperationHandle>();

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
        // 씬이 파괴될 때 어드레서블로 로드했던 메모리 해제
        ReleaseLoadedItems();
    }

    /// <summary>
    /// 고정된 아이템 ID 2개를 어드레서블 비동기로 불러오는 함수
    /// </summary>
    public async void LoadFixedRewardItems(List<HeroRuntimeData> datas)
    {
        itemDatas.Clear();
        ReleaseLoadedItems();

        // 테스트할 아이템 ID 2개 (원하시는 번호로 변경 가능합니다)
        int[] targetItemIds = { 20}; 

        foreach (int itemId in targetItemIds)
        {
            // DataManager의 ItemDB에서 레퍼런스 가져오기
            AssetReferenceT<ItemData> itemRef = DataManager.I.itemDB.GetItemRef(itemId);

            if (itemRef != null && itemRef.RuntimeKeyIsValid())
            {
                AsyncOperationHandle<ItemData> handle = itemRef.LoadAssetAsync<ItemData>();
                
                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    itemDatas.Add(handle.Result);
                    loadedItemHandles.Add(handle); 
                    Debug.Log($"[BattleResultManager] 아이템 로드 성공: {handle.Result.itemName}");
                }
                else
                {
                    Debug.LogWarning($"[BattleResultManager] 아이템 ID {itemId} 로드 실패");
                }
            }
        }
        heroes.AddRange(datas);
    }

    public void Open()
    {
        resultPanel.gameObject.SetActive(true);
        resultPanel.Init(itemDatas,heroes);
    }


    /// <summary>
    /// 로드된 어드레서블 메모리 해제
    /// </summary>
    private void ReleaseLoadedItems()
    {
        foreach (var handle in loadedItemHandles)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
        loadedItemHandles.Clear();
    }
}