using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HeroSelectSlot : MonoBehaviour
{
    public TextMeshProUGUI hero_name;
    public Image hero_image;
    public Button button;
    [SerializeField] HeroRuntimeData hero_data;
    public event Action<HeroRuntimeData> OnSlotClicked;

    public void Init(HeroRuntimeData data)  
    {   
        if(button == null) button = GetComponent<Button>();

        hero_data = null;
        hero_data = data;
        hero_name.text = data.heroData.hero_name;
        hero_image.sprite = data.heroData.portrait;

        button.onClick.RemoveAllListeners();
        // 버튼을 누르면 OnSlotClicked 이벤트가 발동하며 내 영웅 데이터를 넘겨줌
        button.onClick.AddListener(() => {
            OnSlotClicked?.Invoke(hero_data);
        });
    }
}