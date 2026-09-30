using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DomainSlot : MonoBehaviour
{   
    private TerritoryData territoryData;
    [Space(15)] 
    [Header("UI 컴포넌트")] 
    public Image Image;                 
    public TextMeshProUGUI Domain_Name;         // 영지 이름 텍스트
    public TextMeshProUGUI Domain_Description;  // 영지 설명 텍스트
    public Button button;
    public Action<TerritoryData> onSlotSelected;

    void Awake()
    {
        if(button != null)button.onClick.AddListener(OnClickSlot);
    }


    /// <summary>
    /// TerritoryData를 받아와 슬롯의 UI를 초기화하는 메서드
    /// </summary>
    public void Init(TerritoryData territoryData)
    {
        if (territoryData == null)
        {
            Clear();
            return;
        }
        this.territoryData = territoryData;
        // 1. 영지 일러스트 이미지 설정
        if (Image != null)
        {
            Image.sprite = territoryData.territoryBackGround; 
            Image.gameObject.SetActive(Image.sprite != null);
        }

        // 2. 영지 이름 설정
        if (Domain_Name != null)
        {
            Domain_Name.text = territoryData.territoryName;
        }

        // 3. 영지 설명 설정
        if (Domain_Description != null)
        {
            Domain_Description.text = territoryData.DomainDescription; 
        }
        button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClickSlot);
    }

    /// <summary>
    /// 슬롯의 UI 데이터를 비우고 초기 상태로 만드는 메서드
    /// </summary>
    public void Clear()
    {
        if (Image != null)
        {
            Image.sprite = null;
            Image.gameObject.SetActive(false); 
        }

        if (Domain_Name != null)
        {
            Domain_Name.text = string.Empty;
        }

        if (Domain_Description != null)
        {
            Domain_Description.text = string.Empty;
        }
    }
    // UI 버튼의 OnClick()에 연결하거나 내부에서 호출할 수 있는 선택 메서드
    public void OnClickSlot()
    {
        // 구독한 대상이 있다면 내 자신(슬롯)을 전달하며 이벤트를 발생시킴
        onSlotSelected?.Invoke(territoryData);
    }
}