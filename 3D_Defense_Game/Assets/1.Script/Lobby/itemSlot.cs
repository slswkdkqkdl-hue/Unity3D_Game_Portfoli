using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class itemSlot : MonoBehaviour
{
    private ItemRuntimeData runtimeData;

    public Image item_;
    public Button button;

    // 아이템 보유 개수 표시
    public TextMeshProUGUI item_count;

    public Action<ItemRuntimeData> onSlotClicked;

    public void Init(ItemRuntimeData data)
    {
        // 아이템 데이터가 없으면 빈 슬롯
        if (data == null || data.itemData == null)
        {
            runtimeData = null;

            item_.sprite = null;

            Color color = item_.color;
            color.a = 0f;
            item_.color = color;

            item_count.text = "";
            button.interactable = false;

            return;
        }

        // 아이템 데이터 저장
        runtimeData = data;

        // 아이콘 표시
        item_.sprite = data.itemData.icon;

        Color itemColor = item_.color;
        itemColor.a = 1f;
        item_.color = itemColor;

        // 보유 개수가 2개 이상일 때만 표시
        if (data.size > 1)
        {
            item_count.text = data.size.ToString();
        }
        else
        {
            item_count.text = "";
        }

        button.interactable = true;

        // 버튼 클릭 이벤트 등록
        button.onClick.RemoveListener(OnClickSlot);
        button.onClick.AddListener(OnClickSlot);
    }

    private void OnClickSlot()
    {
        onSlotClicked?.Invoke(runtimeData);
    }
}