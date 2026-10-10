using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeroSelectController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform slotParent;
    [SerializeField] private HeroSelectSlot slotPrefab;     
    [SerializeField] private HeroSkillView heroSkillView;
    [SerializeField] private InventoryView inventoryView;
    [SerializeField] private EquipmentView equipmentView;
    [SerializeField] private HeroRuntimeData currentHero;


    [Header("영웅 능력치 UI")]
    [SerializeField] private TextMeshProUGUI hp;
    [SerializeField] private TextMeshProUGUI mana;
    [SerializeField] private TextMeshProUGUI attack;
    [SerializeField] private TextMeshProUGUI defense;
    [SerializeField] private TextMeshProUGUI moveSpeed;
    [SerializeField] private TextMeshProUGUI attackSpeed;
    [SerializeField] private TextMeshProUGUI level;

    [SerializeField]private GameObject hero_panl;
    [SerializeField]private GameObject item_panl;


    // 💡 창을 껐다 켰을 때 재사용할 영웅 데이터 리스트 캐싱 변수
    private List<HeroRuntimeData> cachedHeroList;

    void OnEnable()
    {
        SelectFirstHero();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (hero_panl.activeSelf)
            { 
                OpenInventoryPanel();
            }
            else
            {
                OpenHeroPanel();
            }
        }
    }
    void Start()
    {
        SelectFirstHero();
    }

    /// <summary>
    /// 전달받은 영웅 데이터 리스트로 슬롯 목록을 초기화
    /// </summary>
    public void Init(List<HeroRuntimeData> heroDataList ,List<ItemRuntimeData> items)
    {
        // 💡 전달받은 데이터를 캐싱해둡니다.
        cachedHeroList = heroDataList;

        ClearSlots();
        OpenHeroPanel();

        foreach (var data in items)
        {
            Debug.Log($"들어와있는 아이템 {data.itemData.item_Id} 크기 : {data.size}");
        }

        // 1. 장착 아이템을 확인하고 인벤토리 목록에서 제외
        // equipmentView.Init(cachedHeroList, items);

        // 2.인벤 슬룻 생성
        inventoryView.Init(items);

        // 3.영웅 슬룻 생성 
        CreateHeroSlot();

    }
    private void CreateHeroSlot()
    {   
        if(cachedHeroList == null) return; 
        // 영웅 슬룻 생성
        for (int i = 0; i < cachedHeroList.Count; i++)
        {
            HeroRuntimeData data = cachedHeroList[i];
            
            HeroSelectSlot slotInstance = Instantiate(slotPrefab, slotParent);
            slotInstance.Init(data);
            slotInstance.OnSlotClicked += HandleSlotClicked;
        }
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
    /// 캐릭터 슬롯이 클릭되었을 때 호출되는 콜백 함수
    /// </summary>
    private void HandleSlotClicked(HeroRuntimeData clickedHeroData)
    {
        currentHero = clickedHeroData;

        if (heroSkillView != null && currentHero != null)
            heroSkillView.Init(currentHero);

        if (equipmentView != null && currentHero != null)
            equipmentView.ShowHero(currentHero);

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

    /// <summary>
    /// 영웅 패널 열기
    /// </summary>
    public void OpenHeroPanel()
    {
        hero_panl.SetActive(true);
        item_panl.SetActive(false);
    }

    /// <summary>
    /// 인벤토리 패널 열기
    /// </summary>
    public void OpenInventoryPanel()
    {
        hero_panl.SetActive(false);
        item_panl.SetActive(true);
    }
}