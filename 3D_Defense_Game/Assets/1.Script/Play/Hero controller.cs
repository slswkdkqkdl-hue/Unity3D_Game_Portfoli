using System.Collections.Generic;
using UnityEngine;

public class Herocontroller : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    
    [Header("아군 성 참조 (MainCastle 싱글톤 혹은 연결)")]
    [SerializeField] private MainCastle PlayerCastle; // 오타 수정 (PlayerCatle -> PlayerCastle)

    // 슬롯과 '미리 생성해 둔 영웅 오브젝트'를 1대1로 매핑하여 관리하는 딕셔너리
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
        // 딕셔너리에서 해당 슬롯에 매핑된 영웅 오브젝트 안전하게 가져오기
        if (heroPool.TryGetValue(slot, out GameObject heroObj) && heroObj != null)
        {
            if (isDeployed)
            {
                // ⭐ 출격할 때도 성의 랜덤 출구 위치와 회전값을 다시 갱신해서 배치
                if (PlayerCastle != null)
                {
                    var spawnData = PlayerCastle.GetRandomSpawnData();
                    heroObj.transform.position = spawnData.position;
                    heroObj.transform.rotation = spawnData.rotation;
                }

                heroObj.SetActive(true);
                
                // TODO: 생성된 영웅의 초기화 (팀 설정 등) 필요시 여기서 수행
                // var unitBase = heroObj.GetComponent<UnitBase>();
                // if(unitBase != null) unitBase.Init(slot.HeroRuntimeData.heroData, Team.Ally);
            }
            else
            {
                // 후퇴: 파괴하지 않고 비활성화만 수행
                heroObj.SetActive(false);
            }
        }
    }
}