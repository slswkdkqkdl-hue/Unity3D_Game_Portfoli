using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI domainNameText;
    [SerializeField] private TextMeshProUGUI playerTimeText;
    [SerializeField] private TextMeshProUGUI DefultText;
    
    public int slotNumber; // 유니티 인스펙터에서 0, 1, 2 지정
    public Button button;  // 슬롯 클릭 버튼

    void Awake()
    {
        if(button == null) button = GetComponent<Button>();  
    }

    // 모드를 전달받아 버튼 활성화 여부를 결정
    public void Init(PopUpMode mode)
    {
        if (DataManager.I == null) return;

        SaveData data = DataManager.I.GetSaveData(slotNumber);

        if (data != null)
        {
            // 데이터가 있는 경우 (세이브/로드 공통)
            string result = FactionUtility.FactionName(data.nation.domainType);
            domainNameText.text = $"영지명 : {result}";

            TimeSpan time = TimeSpan.FromSeconds(data.playTime);
            int hours = (int)time.TotalHours;
            int minutes = time.Minutes;
            int seconds = time.Seconds;

            playerTimeText.text = $"플레이 타임 : {hours:D2}시 {minutes:D2}분 {seconds:D2}초";
            DefultText.text = string.Empty;
            
            if (button != null) button.interactable = true;
        }
        else
        {
            // 데이터가 없는 경우
            domainNameText.text = string.Empty;
            playerTimeText.text = string.Empty;
            DefultText.text = "No Data";
            
            // ★ 핵심: 로드 모드일 때는 데이터가 없으면 누르면 안 되므로 false,
            // 세이브 모드일 때는 빈 슬롯에도 저장할 수 있어야 하므로 true로 설정!
            if (button != null)
            {
                button.interactable = (mode == PopUpMode.Save);
            }
        }
    }
}