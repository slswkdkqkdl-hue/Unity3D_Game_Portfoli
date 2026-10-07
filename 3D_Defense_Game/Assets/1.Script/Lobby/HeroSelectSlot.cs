using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectSlot : MonoBehaviour
{
    public TextMeshProUGUI hero_name;
    public Image hero_image;

    [SerializeField] HeroRuntimeData hero_data;


    public void Init(HeroRuntimeData data)
    {
        hero_name.text = data.heroData.hero_name;
        hero_image.sprite = data.heroData.portrait;
    }
}
