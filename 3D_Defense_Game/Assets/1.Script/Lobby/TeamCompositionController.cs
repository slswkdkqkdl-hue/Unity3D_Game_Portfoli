using System.Collections.Generic;
using UnityEngine;

public class TeamCompositionController : MonoBehaviour
{
    [Header("프리팹 및 생성 위치")]
    [SerializeField] private GameObject slotpre;
    [SerializeField] private Transform pos;

    [Header(" 팀 슬롯 (상단 3개) ")]
    [SerializeField] private TeamSlot[] slot;

    [Header(" 캐릭터 선택 슬롯 (우측 리스트) ")]
    [SerializeField] private List<HeroSelectSlot2> heroes = new List<HeroSelectSlot2>();

    private TeamSlot selectedTeamSlot = null;

    public void Init(List<HeroRuntimeData> datas)
    {
        foreach (var heroSlot in heroes)
        {
            if (heroSlot != null) Destroy(heroSlot.gameObject);
        }
        heroes.Clear();

        foreach (var data in datas)
        {
            GameObject hero = Instantiate(slotpre, pos);
            HeroSelectSlot2 hero_ = hero.GetComponent<HeroSelectSlot2>();
            hero_.Init(data);
            hero_.OnClickEvent = OnHeroSlotClicked;
            heroes.Add(hero_);
        }

        foreach (var teamSlot in slot)
        {  
            teamSlot.onTeamSlotClicked = OnTeamSlotClicked;
        }

        TeamSlotset();
    }

    public void TeamSlotset()
    {   
        // 1. 모든 팀 슬롯 초기화
        foreach (var teamSlot in slot)
        {  
            teamSlot.init(); 
        }

        // 2. 영웅들의 선택 상태 초기화 및 우측 슬롯 전부 비활성화
        foreach (var heroSlot in heroes)
        {
            heroSlot.isSelect = false;
        }
        selectedTeamSlot = null;

        UpdateHeroSlotInteractable(false);
    }

    // 1단계: 상단의 [팀 슬롯]을 클릭했을 때
    private void OnTeamSlotClicked(TeamSlot clickedTeamSlot)
    {
        // 이미 데이터가 들어있는 팀 슬롯을 클릭한 경우 (해제 기능)
        if (clickedTeamSlot.currentData != null)
        {
            HeroSelectSlot2 result = heroes.Find(x => x.currenthero.heroData.Hero_Id == clickedTeamSlot.currentData.heroData.Hero_Id);
            if (result != null)
            {
                result.isSelect = false; // 선택 해제
            }
            
            clickedTeamSlot.init(); // 팀 슬롯 비우기
            selectedTeamSlot = null;

            // 전체 영웅 슬롯 상태 갱신 (선택된 애들만 비활성화, 나머지는 잠시 대기 혹은 비활성)
            UpdateHeroSlotInteractable(false);
            Debug.Log("팀 슬롯의 영웅이 해제되었습니다.");
            return;
        }

        // 빈 팀 슬롯을 클릭한 경우 (배치 대기 모드)
        selectedTeamSlot = clickedTeamSlot;
        
        // 아직 팀에 배치되지 않은(isSelect == false) 영웅들만 우측에서 선택 가능하도록 활성화
        foreach (var heroSlot in heroes)
        {
            if (!heroSlot.isSelect)
            {
                heroSlot.btn.interactable = true;
            }
        }

        Debug.Log("팀 슬롯이 선택되었습니다. 우측에서 배치할 캐릭터를 선택하세요!");
    }

    // 2단계: 우측의 [캐릭터 선택 슬롯]을 클릭했을 때
    private void OnHeroSlotClicked(HeroSelectSlot2 clickedHeroSlot)
    {
        if (selectedTeamSlot == null)
        {
            Debug.LogWarning("먼저 상단의 팀 슬롯을 클릭해서 선택해주세요!");
            return;
        }

        if (clickedHeroSlot.isSelect) return;

        // 3단계: 선택한 팀 슬롯에 캐릭터 데이터 적용
        selectedTeamSlot.change(clickedHeroSlot.currenthero);

        // 캐릭터 선택 상태로 변경
        clickedHeroSlot.isSelect = true;

        // 대기 중인 팀 슬롯 초기화
        selectedTeamSlot = null;

        // 우측 캐릭터 선택 슬롯들의 버튼 활성/비활성화 갱신 (선택된 건 꺼지고 나머지는 다시 대기 상태로 비활성)
        UpdateHeroSlotInteractable(false);
    }

    private void UpdateHeroSlotInteractable(bool result = true)
    {
        foreach (var heroSlot in heroes)
        {
            if (heroSlot.isSelect)
            {
                heroSlot.btn.interactable = false; // 이미 선택된 영웅은 항상 비활성화
            }
            else
            {
                heroSlot.btn.interactable = result; // 평소에는 비활성화(false), 팀 슬롯 누를 때만 켜줌
            }
        }
    }
}