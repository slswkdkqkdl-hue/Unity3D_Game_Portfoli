using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroLevelSlot : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI currentLevel;

    public void Init(Sprite heroImage, int level)
    {
        image.sprite = heroImage;
        currentLevel.text = $"(Lv. {level-1} → Lv. {level})";
    }
}