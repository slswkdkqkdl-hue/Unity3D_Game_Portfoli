using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class item_Popup : MonoBehaviour
{
    [Header("기본 정보")]
    [SerializeField] private TextMeshProUGUI itemNameText;          // 아이템 이름
    [SerializeField] private Image itemIconImage;                   // 아이콘 이미지

    [Header("스탯 텍스트 (Item_description_1)")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI mpText;
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private TextMeshProUGUI defText;
    [SerializeField] private TextMeshProUGUI atkSpeedText;
    [SerializeField] private TextMeshProUGUI moveSpeedText;

    [Header("특수 옵션 텍스트 배열 (최대 6개)")]
    [SerializeField] private TextMeshProUGUI[] optionTexts;         // 인스펙터에서 options_Text ~ (5)까지 순서대로 드래그 앤 드롭

    [Header("등급 텍스트 (Item_description_3)")]
    [SerializeField] private TextMeshProUGUI rankText;

    [Header("스토리/설명 텍스트 (Item_description_4)")]
    [SerializeField] private TextMeshProUGUI storyText;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Close(); // 처음에는 닫아두기
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            rectTransform.position = Input.mousePosition;
        }
    }

    /// <summary>
    /// 아이템 데이터를 받아 팝업 UI를 초기화합니다.
    /// </summary>
    public void Init(ItemData data)
    {
        if (data == null) return;

        // 1. 이름 설정
        if (itemNameText != null)
            itemNameText.text = data.itemName;

        // 2. 아이콘 설정
        if (itemIconImage != null && data.icon != null)
        {
            itemIconImage.gameObject.SetActive(true);
            itemIconImage.sprite = data.icon;
        }

        // 3. 스탯 설정 (0이거나 사용하지 않는 스탯은 비활성화/숨김 처리)
        SetStatText(hpText, "체력", data.hp);
        SetStatText(mpText, "마나", data.mp);
        SetStatText(atkText, "공격력", data.attack);
        SetStatText(defText, "방어력", data.defense);
        SetStatText(atkSpeedText, "공격속도", data.attackSpeed, "초");
        SetStatText(moveSpeedText, "이동속도", data.moveSpeed);

        // 4. 특수 옵션 설정 (최대 6개 대응)
        for (int i = 0; i < optionTexts.Length; i++)
        {
            if (optionTexts[i] == null) continue;

            // 데이터가 존재하고 해당 인덱스의 옵션이 있다면 활성화 후 텍스트 입력
            if (data.itemOptions != null && i < data.itemOptions.Count)
            {
                optionTexts[i].gameObject.SetActive(true);
                optionTexts[i].text = data.itemOptions[i];
            }
            else
            {
                // 데이터가 없거나 6개 미만인 빈 자리는 비활성화
                optionTexts[i].gameObject.SetActive(false);
                optionTexts[i].text = "";
            }
        }

        // 5. 등급 설정
        if (rankText != null)
        {   
            string result = FactionUtility.ItemRarityName(data.itemRank);
            rankText.text = $"등급 : {result}";
        }

        // 6. 스토리/설명 설정
        if (storyText != null)
        {
            storyText.text = data.itemDescription;
        }

        // 팝업 켜기
        Open();
    }

    private void SetStatText(TextMeshProUGUI textComp, string label, int value, string unit = "")
    {
        if (textComp == null) return;

        if (value == 0)
        {
            textComp.gameObject.SetActive(false);
        }
        else
        {
            textComp.gameObject.SetActive(true);
            textComp.text = $"{label} : {value}{unit}";
        }
    }

    private void SetStatText(TextMeshProUGUI textComp, string label, float value, string unit = "")
    {
        if (textComp == null) return;

        if (Mathf.Approximately(value, 0f))
        {
            textComp.gameObject.SetActive(false);
        }
        else
        {
            textComp.gameObject.SetActive(true);
            textComp.text = $"{label} : {value}{unit}";
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}