using TMPro;
using UnityEngine;

public class EquipmentSlot : MonoBehaviour
{
    public TextMeshProUGUI item_name;
    private ItemData itemData;

    public void Init(ItemData data)
    {
        itemData = data;
        item_name.text = $"{data.itemName}";
    }
}
