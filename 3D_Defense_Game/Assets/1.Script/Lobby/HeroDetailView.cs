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

    public void Init(HeroRuntimeData data)
    {   
        hero = data;

        DisplaySkills(data);
        Renewal(data);
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

                // 이미 선택된 액티브 스킬인지 고유 ID로 비교 
                if (hero.selectedActiveSkill.Contains(activeSkillSlots[i].boundSkill.skill_Id))
                {
                    activeSkillSlots[i].ToggleEquipState(true);
                }
                else
                {
                    activeSkillSlots[i].ToggleEquipState(false);
                }
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

                // 이미 선택된 패시브 스킬인지 고유 ID로 비교
                if (hero.selectedPassiveSkill.Contains(passiveSkillSlots[i].boundSkill.skill_Id))
                {
                    passiveSkillSlots[i].ToggleEquipState(true);
                }
                else
                {
                    passiveSkillSlots[i].ToggleEquipState(false);
                }
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