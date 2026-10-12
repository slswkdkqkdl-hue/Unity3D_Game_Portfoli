using System.Collections.Generic;
using UnityEngine;

public class TeamCompositionController : MonoBehaviour
{


    [Header("팀 선택 관련")]
    [SerializeField] private GameObject slotpre;
    [SerializeField] private Transform pos;


    [Header(" 팀 슬룻 ")]
    [SerializeField] private TeamSlot[] slot;
    [Header("캐릭터 선택 슬룻 3개")]
    private List<HeroSelectSlot2> heroes = new List<HeroSelectSlot2>();

    public void Init(List<HeroRuntimeData> datas)
    {
        TeamSlotInit();
        foreach (var data in datas)
        {
            GameObject hero = Instantiate(slotpre,pos);
            HeroSelectSlot2 hero_ = hero.GetComponent<HeroSelectSlot2>();
            hero_.Init(data);
            heroes.Add(hero_);
        }
    }
    public void TeamSlotInit()
    {
        foreach (var data in slot)
        {
            data.init();
        }
    }
}
