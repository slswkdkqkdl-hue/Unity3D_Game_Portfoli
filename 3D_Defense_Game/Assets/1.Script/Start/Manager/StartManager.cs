using UnityEngine;

public class StartManager : MonoBehaviour
{
    public UIManager_Start uiManager; // UI를 제어하기 위해 연결

    void Awake()
    {   
        // 메모리청소
        PurgeUnusedMemory();
    }

    void Start()
    {   
        if (uiManager != null)
        {
            uiManager.InitEvents();
            CheckSaveDataAndSetupUI();
        }
    }

    private void CheckSaveDataAndSetupUI()
    {
        if (uiManager == null) return;

        // 데이터 경로에 데이터가 하나라도 있는지 확인
        bool hasSaveData = DataManager.I.HasAnySaveData(); 

        // 결과에 따라 UI 활성/비활성 처리
        uiManager.SetContinueButtonActive(hasSaveData);
    }

    private void PurgeUnusedMemory()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        Debug.Log("메모리청소완료");
    }
}