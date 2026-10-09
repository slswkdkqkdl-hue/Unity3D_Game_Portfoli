
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public TextMeshProUGUI item_name;
    public Button button;

    [SerializeField]private ItemRuntimeData runtimeData;

    // 버튼 클릭 시 호출할 함수
    private Action<EquipmentSlot> onSlotClicked;

    private void Awake()
    {

        if (button == null)
        {
            button = GetComponent<Button>();
        }

        button.onClick.AddListener(OnClickSlot);
    }

    // 아이템 장착
    public void Init(ItemRuntimeData data)
    {
        runtimeData = data;
        item_name.text = runtimeData.itemData.itemName;
        button.interactable = true;
    }

    // 아이템 장착 해제
    public void Clear()
    {
        runtimeData = null;
        item_name.text = "";
        // button.interactable = false;
    }

    private void OnClickSlot()
    {
        onSlotClicked?.Invoke(this);
    }
}