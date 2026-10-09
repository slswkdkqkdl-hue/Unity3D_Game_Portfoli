using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroDetailView : MonoBehaviour
{   
    [Header("UI Prefabs & References")]
    public GameObject skillPrefab;
    public Image heroImage;
    public TextMeshProUGUI heroSpText;

    [Header("Transform Positions")]
    public Transform activePos;
    public Transform passivePos;

    [Header("Skill Slots Lists")]
    public List<SkillSlot> activeSkillSlots = new List<SkillSlot>();
    public List<SkillSlot> passiveSkillSlots = new List<SkillSlot>();

    [SerializeField]private HeroRuntimeData hero;

    public EquipmentSlot Weapon_Btn;
    public EquipmentSlot Armor_Btn;
    public int item_size = 20;
    public GameObject item_slot;
    public Transform item_pos;

    // 현재 인벤토리 데이터
    private List<ItemRuntimeData> inventoryItems;
    // 생성한 인벤토리 슬롯 목록
    private List<itemSlot> inventorySlots = new List<itemSlot>();

    public void Init(HeroRuntimeData data)
    {   
        hero = data;
        heroEquipmen(data);
        DisplaySkills(data);
        Renewal(data);
    }   

    public void CreateItemSlot(List<ItemRuntimeData> itemRuntimes)
    {
        inventoryItems = itemRuntimes;

        // 인벤토리 슬롯은 item_size만큼만 생성
        for (int i = 0; i < item_size; i++)
        {
            // 중복 생성 막기 혹시나
            if (i >= inventorySlots.Count)
            {
                GameObject obj = Instantiate(item_slot, item_pos);

                itemSlot slot = obj.GetComponent<itemSlot>();
                slot.onSlotClicked = OnItemSlotClicked;

                inventorySlots.Add(slot);
            }
        }


        RefreshInventorySlots();
    }

    private void heroEquipmen(HeroRuntimeData data)
    {

        var result = inventoryItems.Find(x => x.itemData.item_Id == data.equippedArmorId);
        // 1. 현재 인벤토리에 들어있는 아이템 ID 목록 전체 출력
    if (inventoryItems != null && inventoryItems.Count > 0)
    {
        foreach (var item in inventoryItems)
        {
            if (item != null && item.itemData != null)
            {
                Debug.Log($"[인벤토리 아이템 확인] 이름: {item.itemData.itemName}, ID: {item.itemData.item_Id}");
            }
            else
            {
                Debug.LogWarning("[인벤토리 아이템 확인] item 또는 itemData가 null입니다!");
            }
        }
    }
    else
    {
        Debug.LogWarning("[인벤토리 확인] inventoryItems 리스트가 비어있거나 null입니다!");
    }
        Debug.Log($"저장된 장착 방어구 {data.equippedArmorId}");

        if(result != null)
        {
            Armor_Btn.Init(result);
            inventoryItems.Remove(result);
            RefreshInventorySlots();
            Debug.Log(":실행댐?");
        }
    }

    // 인벤토리 UI 갱신
    private void RefreshInventorySlots()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < inventoryItems.Count)
            {
                inventorySlots[i].Init(inventoryItems[i]);
            }
            else
            {
                inventorySlots[i].Init(null);
            }
        }
    }
    private void OnItemSlotClicked(ItemRuntimeData data) 
    {
        if(data.itemData.itemType == ItemType.Weapon)
        {
            Weapon_Btn.Init(data);
        }
        else
        {
            Armor_Btn.Init(data);
        }
    }
    







    public void DisplaySkills(HeroRuntimeData data)
    {   
        
        List<SkillData> activeDatas = data.heroData.activeSkills;
        List<SkillData> passiveDatas = data.heroData.passiveSkills;

        // 1. 액티브 스킬 슬롯 생성 및 재사용
        while (activeSkillSlots.Count < activeDatas.Count)
        {
            GameObject slot = Instantiate(skillPrefab, activePos);
            activeSkillSlots.Add(slot.GetComponent<SkillSlot>());
        }
        
        // 2. 패시브 스킬 슬롯 생성 및 재사용
        while (passiveSkillSlots.Count < passiveDatas.Count)
        {
            GameObject slot = Instantiate(skillPrefab, passivePos);
            passiveSkillSlots.Add(slot.GetComponent<SkillSlot>());
        }

        // 3. 액티브 스킬 켜기/끄기 및 데이터 바인딩
        for (int i = 0; i < activeSkillSlots.Count; i++)
        {
            if (i < activeDatas.Count)
            {
                activeSkillSlots[i].gameObject.SetActive(true);
                activeSkillSlots[i].Init(activeDatas[i], HandleWhenSkillClicked);

                // 💡 변경점: boundSkill 대신 원본 데이터 activeDatas[i].skill_Id를 직접 비교
                bool isEquipped = hero.selectedActiveSkill.Contains(activeDatas[i].skill_Id);
                activeSkillSlots[i].ToggleEquipState(isEquipped);
            }
            else
            {
                activeSkillSlots[i].gameObject.SetActive(false);
            }
        }

        // 4. 패시브 스킬 켜기/끄기 및 데이터 바인딩
        for (int i = 0; i < passiveSkillSlots.Count; i++)
        {
            if (i < passiveDatas.Count)
            {
                passiveSkillSlots[i].gameObject.SetActive(true);
                passiveSkillSlots[i].Init(passiveDatas[i], HandleWhenSkillClicked);

                // 💡 변경점: boundSkill 대신 원본 데이터 passiveDatas[i].skill_Id를 직접 비교
                bool isEquipped = hero.selectedPassiveSkill.Contains(passiveDatas[i].skill_Id);
                passiveSkillSlots[i].ToggleEquipState(isEquipped);
            }
            else
            {
                passiveSkillSlots[i].gameObject.SetActive(false);
            }
        }
    }

    private void HandleWhenSkillClicked(SkillData data, SkillSlot clickedSlot)
    {
        // A. 이미 장착되어 있는 경우 -> 장착 해제
        if (clickedSlot.isEquipped)
        {
            hero.sp += data.spCost; // SP 반환
            clickedSlot.ToggleEquipState(false); // 슬롯 UI 해제

            // 선택 목록에서 제거 (Remove)
            if (data.skillType == SkillType.Active)
            {
                hero.selectedActiveSkill.Remove(data.skill_Id);
            }
            else
            {
                hero.selectedPassiveSkill.Remove(data.skill_Id);
            }

            Renewal(hero); // 데이터 변경 후 갱신
        }
        // B. 아직 장착되지 않은 경우 -> 장착
        else 
        {
            if (data.skillType == SkillType.Active && hero.selectedActiveSkill.Count >= 2) return;

            if (hero.sp >= data.spCost)
            {
                hero.sp -= data.spCost; // SP 차감
                clickedSlot.ToggleEquipState(true); // 슬롯 UI 장착

                // 선택 목록에 추가 (Add)
                if (data.skillType == SkillType.Active)
                {
                    if (!hero.selectedActiveSkill.Contains(data.skill_Id))
                        hero.selectedActiveSkill.Add(data.skill_Id);
                }
                else
                {
                    if (!hero.selectedPassiveSkill.Contains(data.skill_Id))
                        hero.selectedPassiveSkill.Add(data.skill_Id);
                }

                Renewal(hero); // 데이터 변경 후 갱신
            }
            else
            {
                Debug.Log("SP가 부족합니다!");
            }
        }
    }
    public void Renewal(HeroRuntimeData data)
    {
        heroImage.sprite = data.heroData.portrait;
        heroSpText.text = $"보유 : SP {data.sp}";
    }
}