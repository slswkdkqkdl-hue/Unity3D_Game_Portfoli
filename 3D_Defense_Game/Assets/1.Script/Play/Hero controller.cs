using System.Collections.Generic;
using UnityEngine;


// 여기서 영웅 맨처음 소환후 데이터들 초기화 및 관리 
public class Herocontroller : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;


    public void Init(List<HeroRuntimeData> heroes)
    {
        for (int i =0; i < heroes.Count; i++)
        {
            CreateSlot(heroes[i]);
        }
    }


    // 이거 체력 부분 추후에 수정해야함 지금은간단히 표시만해둠
    private void CreateSlot(HeroRuntimeData data)
    {
        GameObject slot = Instantiate(slotPrefab,this.gameObject.transform);
        var result = slot.GetComponent<HeroSlot>();
        result.InitHeroSlot(data.heroData.portrait,data.heroData.hp,data.heroData.mp);
    }
}