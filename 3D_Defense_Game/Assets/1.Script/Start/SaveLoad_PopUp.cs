using System.Collections.Generic;
using UnityEngine;

public enum PopUpMode
{
    Load,
    Save
}

public class SaveLoad_PopUp : UIPopUp
{   
    [Header("모드 설정 (기본값 로드)")]
    [SerializeField] private PopUpMode currentMode = PopUpMode.Load;

    [Header("이동할 씬 (로드 모드 전용)")] 
    [SerializeField] private SceneType targetScene = SceneType.LobbyScreen;
    
    [Header("슬롯 데이터")] 
    [SerializeField] private List<SaveSlot> saveSlots = new List<SaveSlot>();
    [SerializeField] private FadeOut fadeOut;


    void Start()
    {
        InitSlots(); 
    }

    private void InitSlots()
    {
        for (int i = 0; i < saveSlots.Count; i++)
        {
            SaveSlot slot = saveSlots[i];
            
            // 슬롯 초기화 시 현재 모드(세이브/로드)를 전달
            slot.Init(currentMode);

            slot.button.onClick.RemoveAllListeners();

            int targetSlotIndex = slot.slotNumber;


            if (currentMode == PopUpMode.Load)
            {
                slot.button.onClick.AddListener(() => OnClickLoad(targetSlotIndex));
            }
            else
            {
                slot.button.onClick.AddListener(() => OnClickSave(targetSlotIndex));
            }
        }
    }

    // 슬롯 불러오기
    private void OnClickLoad(int slotIndex)
    {
        if (DataManager.I.LoadGame(slotIndex))
        {
            Debug.Log($"{slotIndex}번 슬롯 불러오기 성공");
            DataManager.I.RequestSaveRestore();
            if (fadeOut != null)
            {
                fadeOut.DoFadeOut(() =>
                {
                    LoadingManager.LoadScene(targetScene); 
                });
            }
            else
            {
                LoadingManager.LoadScene(targetScene);
            }
        }
        else
        {
            Debug.LogError($"{slotIndex}번 슬롯 불러오기 실패!");
        }
    }

    // 슬롯 저장하기
    private void OnClickSave(int slotIndex)
    {
        DataManager.I.SaveGame(slotIndex);
        Debug.Log($"{slotIndex}번 슬롯 저장하기 성공");

        // 저장 후 슬롯 UI 갱신
        foreach (var result in saveSlots)
        {
            if (result.slotNumber == slotIndex)
            {
                result.Init(currentMode);   
            }
        }
    }
}