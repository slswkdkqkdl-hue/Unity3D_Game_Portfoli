using UnityEngine;
using UnityEngine.UI;

public class itemSlot : MonoBehaviour
{
    private ItemData itemData;
    public Image item_;


    public void Init(ItemData data )
    {
        item_.sprite = data.icon;
    }
}
