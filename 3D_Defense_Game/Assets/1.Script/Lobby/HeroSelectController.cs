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

    [Header("영웅 능력치 UI")]
    [SerializeField] private TextMeshProUGUI hp;
    [SerializeField] private TextMeshProUGUI mana;
    [SerializeField] private TextMeshProUGUI attack;
    [SerializeField] private TextMeshProUGUI defense;
    [SerializeField] private TextMeshProUGUI moveSpeed;
    [SerializeField] private TextMeshProUGUI attackSpeed;
    [SerializeField] private TextMeshProUGUI level;

    [Header("패널")]
    [SerializeField] private GameObject hero_panl;
    [SerializeField] private GameObject item_panl;

    private List<HeroRuntimeData> cachedHeroList;
    private InventoryEquipmentService inventoryEquipmentService;
    private HeroRuntimeData currentHero;

    private bool isInitialized;

    private void OnEnable()
    {
        if (isInitialized)
            SelectFirstHero();
    }

    private void Update()
    {
        if (!isInitialized || !Input.GetKeyDown(KeyCode.Tab))
            return;

        if (hero_panl != null && hero_panl.activeSelf)
            OpenInventoryPanel();
        else
            OpenHeroPanel();
    }

    /// <summary>
    /// 영웅 및 인벤토리 UI 초기화
    /// </summary>
    public void Init(List<HeroRuntimeData> heroDataList,List<ItemRuntimeData> items)
    {
        cachedHeroList = heroDataList ?? new List<HeroRuntimeData>();
        items ??= new List<ItemRuntimeData>();

        if (slotParent == null ||
            slotPrefab == null ||
            inventoryView == null ||
            equipmentView == null)
        {
            Debug.LogError("HeroSelectController의 필수 UI 참조를 확인해 주세요.", this);
            return;
        }

        isInitialized = false;
        currentHero = null;

        ClearSlots();

        inventoryEquipmentService = new InventoryEquipmentService();
        inventoryEquipmentService.Init(items,cachedHeroList,inventoryView.itemSize);

        inventoryView.Init(inventoryEquipmentService);
        equipmentView.Init(items,inventoryView,this,inventoryEquipmentService);

        CreateHeroSlots();

        isInitialized = true;
        SelectFirstHero();
    }

    /// <summary>
    /// 영웅 선택 슬롯 생성
    /// </summary>
    private void CreateHeroSlots()
    {
        foreach (HeroRuntimeData hero in cachedHeroList)
        {
            if (hero == null)
                continue;

            HeroSelectSlot slot = Instantiate(slotPrefab, slotParent);
            slot.Init(hero);
            slot.OnSlotClicked += HandleSlotClicked;
        }
    }

    /// <summary>
    /// 첫 번째 유효한 영웅 선택
    /// </summary>
    public void SelectFirstHero()
    {
        if (!isInitialized || cachedHeroList == null)
            return;

        foreach (HeroRuntimeData hero in cachedHeroList)
        {
            if (hero == null)
                continue;

            HandleSlotClicked(hero);
            break;
        }

        OpenHeroPanel();
    }

    /// <summary>
    /// 영웅 선택 시 관련 UI 갱신
    /// </summary>
    private void HandleSlotClicked(HeroRuntimeData clickedHero)
    {
        if (clickedHero == null)
            return;

        currentHero = clickedHero;

        heroSkillView?.Init(currentHero);
        equipmentView?.ShowHero(currentHero);

        InitHeroStats();
    }

    /// <summary>
    /// 기존 영웅 슬롯 제거
    /// </summary>
    private void ClearSlots()
    {
        if (slotParent == null)
            return;

        foreach (Transform child in slotParent)
        {
            HeroSelectSlot slot = child.GetComponent<HeroSelectSlot>();

            if (slot != null)
                slot.OnSlotClicked -= HandleSlotClicked;

            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// 현재 영웅의 능력치 표시
    /// </summary>
    private void InitHeroStats()
    {
        if (currentHero == null || currentHero.heroData == null)
            return;

        var data = currentHero.heroData;
        int heroLevel = currentHero.level;

        if (hp != null)
            hp.text = $"체력 : {data.hp + data.hpPerLevel * heroLevel}";

        if (mana != null)
            mana.text = $"마나 : {data.mp + data.mpPerLevel * heroLevel}";

        if (attack != null)
            attack.text = $"공격력 : {data.attack + data.attackPerLevel * heroLevel}";

        if (defense != null)
            defense.text = $"방어력 : {data.defense + data.defensePerLevel * heroLevel}";

        if (moveSpeed != null)
        {
            float value = data.moveSpeed + data.moveSpeedPerLevel * heroLevel;
            moveSpeed.text = $"이동속도 : {value:F1}";
        }

        if (attackSpeed != null)
        {
            float value = data.attackSpeed + data.attackSpeedPerLevel * heroLevel;
            attackSpeed.text = $"공격속도 : {value:F1}";
        }

        if (level != null)
            level.text = $"Level : {heroLevel}";
    }

    /// <summary>
    /// 영웅 패널 열기
    /// </summary>
    public void OpenHeroPanel()
    {
        if (hero_panl != null)
            hero_panl.SetActive(true);

        if (item_panl != null)
            item_panl.SetActive(false);
    }

    /// <summary>
    /// 인벤토리 패널 열기
    /// </summary>
    public void OpenInventoryPanel()
    {
        if (hero_panl != null)
            hero_panl.SetActive(false);

        if (item_panl != null)
            item_panl.SetActive(true);
    }
}
