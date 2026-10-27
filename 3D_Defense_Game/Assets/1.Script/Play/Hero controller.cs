using System.Collections.Generic;
using UnityEngine;

public class Herocontroller : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [Header("아군 성")]
    private MainCastle PlayerCastle; 

    

    private Dictionary<HeroSlot, GameObject> heroPool = new Dictionary<HeroSlot, GameObject>();

    public void Init(List<HeroRuntimeData> heroes)
    {   
        // 싱글톤으로 메인 캐슬 할당
        PlayerCastle = MainCastle.I;

        for (int i = 0; i < heroes.Count; i++)
        {
            CreateSlotAndHero(heroes[i]);
        }
    }

    private void CreateSlotAndHero(HeroRuntimeData data)
    {
        // 1. UI 슬롯 생성
        GameObject slotObj = Instantiate(slotPrefab, this.gameObject.transform);
        var heroSlot = slotObj.GetComponent<HeroSlot>();
        
        // 슬롯 초기화 데이터 전달
        heroSlot.InitHeroSlot(data);

        GameObject heroInstance = null;
        
        // 2. 영웅 프리팹이 존재할 때만 안전하게 미리 생성
        if (data.heroData != null && data.heroData.prefab_Play != null)
        {
            // 미리 생성
            heroInstance = Instantiate(data.heroData.prefab_Play);
            if (heroInstance != null)
            {
                var heroBase = heroInstance.GetComponent<HeroBase>();
                if (heroBase != null)
                {
                    heroBase.InitHero(data, PlayerCastle.PlayerTeam, heroSlot);
                }
            }
            // 생성하자마자 꺼두기 (비활성화)
            heroInstance.SetActive(false);
        }
        else
        {
            string heroName = data.heroData != null ? data.heroData.hero_name : "알 수 없는 영웅";
            Debug.LogWarning($"[{heroName}] 영웅 데이터에 프리팹(prefab_Play)이 없습니다!");
        }

        // 3. 딕셔너리에 [슬롯 ➔ 미리 만든 영웅 오브젝트] 저장
        heroPool[heroSlot] = heroInstance;

        // 4. 출격 버튼 이벤트 구독
        heroSlot.OnDeployStateChanged += HandleDeployStateChanged;
    }

    /// <summary>
    /// 슬롯에서 출격/후퇴 버튼을 눌렀을 때 호출되는 콜백
    /// </summary>
    private void HandleDeployStateChanged(HeroSlot slot, bool isDeployed)
    {
        if (heroPool.TryGetValue(slot, out GameObject heroObj) && heroObj != null)
        {
            if (isDeployed)
            {
                // 성 위치 재배치
                if (PlayerCastle != null)
                {
                    var spawnData = PlayerCastle.GetRandomSpawnData();
                    heroObj.transform.position = spawnData.position;
                    heroObj.transform.rotation = spawnData.rotation;
                }
                
                // ⭐ 여기서는 InitHero를 지우고 그냥 켜기만 함! (체력 유지됨)
                heroObj.SetActive(true);
            }
            else
            {
                // 후퇴: 비활성화만 (피 깎인 상태 그대로 유지됨)
                heroObj.SetActive(false);
            }
        }
    }
}