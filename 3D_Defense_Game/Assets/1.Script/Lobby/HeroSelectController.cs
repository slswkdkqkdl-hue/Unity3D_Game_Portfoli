using System;
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


    /// <summary>
    /// 전달받은 영웅 데이터 리스트로 슬롯 목록을 초기화  씬에 들어왔을대 맨처음 한번만실행
    /// </summary>
    public void Init(List<HeroRuntimeData> heroDataList,List<ItemRuntimeData> items)
    {
        cachedHeroList = heroDataList;

        if (items == null)
            items = new List<ItemRuntimeData>();

        ClearSlots();
        OpenHeroPanel();

        // 1. 인벤토리 초기화
        inventoryView.Init(items, cachedHeroList);

        // 2. 장비 초기화
        equipmentView.Init(cachedHeroList, items, inventoryView,this);

        // 3. 영웅 슬롯 생성
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
        OpenHeroPanel();
    }

    /// <summary>
    /// 캐릭터 슬롯이 클릭되었을 때 호출되는 콜백 함수
    /// </summary>
    private void HandleSlotClicked(HeroRuntimeData clickedHeroData)
    {
        currentHero = clickedHeroData;

        if (heroSkillView != null && currentHero != null)
            heroSkillView.Init(currentHero);

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
        if (currentHero == null || currentHero.heroData == null)
            return;

        if (hp != null)
            hp.text = $"체력 : {GetCalculatedHp(currentHero)}";

        if (mana != null)
            mana.text = $"마나 : {GetCalculatedMp(currentHero)}";

        if (attack != null)
            attack.text = $"공격력 : {GetCalculatedAttack(currentHero)}";

        if (defense != null)
            defense.text = $"방어력 : {GetCalculatedDefense(currentHero)}";

        if (moveSpeed != null)
            moveSpeed.text = $"이동속도 : {GetCalculatedMoveSpeed(currentHero):F1}";

        if (attackSpeed != null)
            attackSpeed.text = $"공격속도 : {GetCalculatedAttackSpeed(currentHero):F1}";

        if (level != null)
            level.text = $"Level : {currentHero.level}";
    }

    /// <summary>
    /// HeroRuntimeData와 HeroData를 조합하여 레벨이 반영된 최종 스탯을 계산
    /// </summary>
    private int GetCalculatedHp(HeroRuntimeData runtimeData)
    {
        if (runtimeData == null || runtimeData.heroData == null) return 0;
        return runtimeData.heroData.hp + (runtimeData.heroData.hpPerLevel * runtimeData.level);
    }

    private int GetCalculatedMp(HeroRuntimeData runtimeData)
    {
        if (runtimeData == null || runtimeData.heroData == null) return 0;
        return runtimeData.heroData.mp + (runtimeData.heroData.mpPerLevel * runtimeData.level);
    }

    private int GetCalculatedAttack(HeroRuntimeData runtimeData)
    {
        if (runtimeData == null || runtimeData.heroData == null) return 0;
        return runtimeData.heroData.attack + (runtimeData.heroData.attackPerLevel * runtimeData.level);
    }

    private int GetCalculatedDefense(HeroRuntimeData runtimeData)
    {
        if (runtimeData == null || runtimeData.heroData == null) return 0;
        return runtimeData.heroData.defense + (runtimeData.heroData.defensePerLevel * runtimeData.level);
    }

    private float GetCalculatedAttackSpeed(HeroRuntimeData runtimeData)
    {
        if (runtimeData == null || runtimeData.heroData == null) return 0f;
        return runtimeData.heroData.attackSpeed + (runtimeData.heroData.attackSpeedPerLevel * runtimeData.level);
    }

    private float GetCalculatedMoveSpeed(HeroRuntimeData runtimeData)
    {
        if (runtimeData == null || runtimeData.heroData == null) return 0f;
        return runtimeData.heroData.moveSpeed + (runtimeData.heroData.moveSpeedPerLevel * runtimeData.level);
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