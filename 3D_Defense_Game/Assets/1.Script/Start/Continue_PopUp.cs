using System.Collections.Generic;
using UnityEngine;

public class Continue_PopUp : UIPopUp
{   
    [Header("이동할 씬")] 
    [SerializeField] private SceneType targetScene = SceneType.LobbyScreen;
    [Header("슬릇 데이터")] 
    [SerializeField] private List<SaveSlot> saveSlots = new List<SaveSlot>();
    [SerializeField]private FadeOut fadeOut;


    private void OnEnable()
    {
        for (int i = 0; i < saveSlots.Count; i++)
        {
            SaveSlot slot = saveSlots[i];
            slot.Init();


            slot.button.onClick.RemoveAllListeners();


            int targetSlotIndex = slot.slotNumber;

            // 3. 해당 슬롯 번호를 전달받아 실행하도록 이벤트 연결
            slot.button.onClick.AddListener(() => OnClickSlot(targetSlotIndex));
        }
    }

    // 슬롯 버튼을 눌렀을 때 실행되는 함수
    private void OnClickSlot(int slotIndex)
    {
        // 클릭한 슬롯 데이터를 성공적으로 불러왔다면 씬 이동
        if (DataManager.I.LoadGame(slotIndex))
        {
            Debug.Log($"{slotIndex}번 슬롯 불러오기 성공");
            fadeOut.DoFadeOut(() =>
            {
                LoadingManager.LoadScene(targetScene); 
            });
        }
        else
        {
            Debug.LogError($"{slotIndex}번 슬롯 불러오기 실패!");
        }
    }
}