using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropitemSlot : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI itemname;
    public TextMeshProUGUI sizeText; 

    public void Init(Sprite itemImage, string name, int size)
    {
        image.sprite = itemImage;
        itemname.text = name;
        sizeText.text = size.ToString();
    }
}