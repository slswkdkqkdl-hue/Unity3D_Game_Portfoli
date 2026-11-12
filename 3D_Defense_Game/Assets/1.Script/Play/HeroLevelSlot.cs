using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroLevelSlot : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI currentLevel;

    public void Init(HeroLevelUpResult hero)
    {   
        Sprite heroImage = hero.RunData.heroData.portrait;
        int startLevel = hero.startLevel;
        int endLevel = hero.endLevel;


        image.sprite = heroImage;
        currentLevel.text = $"Lv. {startLevel} → Lv. {endLevel}";
    }
}