using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroDetailView : MonoBehaviour
{
    public Image hero_image;
    public TextMeshProUGUI hero_sp;
    public GameObject Skill_pre;
    public Transform active_pos;
    public Transform passive_pos;

    public void Init(HeroRuntimeData data)
    {
        hero_image.sprite = data.heroData.portrait;
        hero_sp.text = $"보유 : SP{data.sp}";
        
        SkillSlotCreate(data);
    }   



    private void SkillSlotCreate(HeroRuntimeData datas)
    {
        foreach (var data in datas.heroData.passiveSkills)
        {
            var pre = Instantiate(Skill_pre,passive_pos);
        }
        foreach (var data in datas.heroData.activeSkills)
        {
            var pre = Instantiate(Skill_pre,active_pos);
        }
    }
}
