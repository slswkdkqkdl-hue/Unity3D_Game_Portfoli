using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeroSelectController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform slotParent;
    [SerializeField] private HeroSelectSlot slotPrefab;     
    [SerializeField] private HeroDetailView heroDetailView;
    [SerializeField] private HeroRuntimeData currentHero;

    [Header("영웅 능력치 UI")]
    [SerializeField] private TextMeshProUGUI hp;
    [SerializeField] private TextMeshProUGUI mana;
    [SerializeField] private TextMeshProUGUI attack;
    [SerializeField] private TextMeshProUGUI defense;
    [SerializeField] private TextMeshProUGUI moveSpeed;
    [SerializeField] private TextMeshProUGUI attackSpeed;
    [SerializeField] private TextMeshProUGUI level;

    // 💡 창을 껐다 켰을 때 재사용할 영웅 데이터 리스트 캐싱 변수
    private List<HeroRuntimeData> cachedHeroList;

    void OnEnable()
    {
        SelectFirstHero();
    }

    /// <summary>
    /// 전달받은 영웅 데이터 리스트로 슬롯 목록을 초기화
    /// </summary>
    public void Init(List<HeroRuntimeData> heroDataList)
    {
        // 💡 전달받은 데이터를 캐싱해둡니다.
        cachedHeroList = heroDataList;

        ClearSlots();
        if (cachedHeroList == null || cachedHeroList.Count == 0)
            return;

        for (int i = 0; i < cachedHeroList.Count; i++)
        {
            HeroRuntimeData data = cachedHeroList[i];
            
            HeroSelectSlot slotInstance = Instantiate(slotPrefab, slotParent);
            slotInstance.Init(data);
            slotInstance.OnSlotClicked += HandleSlotClicked;
        }

        // 초기 생성 시 0번째 영웅 선택
        SelectFirstHero();

        Debug.Log(heroDataList.Count);
    }

    /// <summary>
    /// 💡 첫 번째(0번째) 영웅을 선택 처리하는 함수
    /// </summary>
    public void SelectFirstHero()
    {
        if (cachedHeroList != null && cachedHeroList.Count > 0)
        {
            HandleSlotClicked(cachedHeroList[0]);
        }
    }

    /// <summary>
    /// 슬롯이 클릭되었을 때 호출되는 콜백 함수
    /// </summary>
    private void HandleSlotClicked(HeroRuntimeData clickedHeroData)
    {
        currentHero = clickedHeroData;
        
        if (heroDetailView != null && currentHero != null)
        {
            heroDetailView.Init(currentHero);
        }
        InitHeroStats();
    }

    /// <summary>
    /// 기존 슬롯 오브젝트들을 모두 삭제
    /// </summary>
    private void ClearSlots()
    {
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }
    }
    /// <summary>
    /// 현재 선택한 영웅의 능력치를 UI에 표시
    /// </summary>
    private void InitHeroStats()
    {
        if (currentHero == null)
            return;

        if (hp != null)
            hp.text = $"체력 : {currentHero.heroData.hp}";

        if (mana != null)
            mana.text = $"마나 : {currentHero.heroData.mp}";

        if (attack != null)
            attack.text = $"공격력 : {currentHero.heroData.attack}";

        if (defense != null)
            defense.text = $"방어력 : {currentHero.heroData.defense}";

        if (moveSpeed != null)
            moveSpeed.text = $"이동속도 : {currentHero.heroData.moveSpeed}";

        if (attackSpeed != null)
            attackSpeed.text = $"공격속도 : {currentHero.heroData.attackSpeed}";
        if(level != null)
            level.text = $"Level : {currentHero.level}";
        
    }
}