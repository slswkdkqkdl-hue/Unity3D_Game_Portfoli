using UnityEngine;

public class StartManager : MonoBehaviour
{
    public UIManager_Start uiManager; // UI를 제어하기 위해 연결

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

        // 2. DataManager를 통해 세이브 파일 검수
        bool hasSave = (DataManager.I != null && DataManager.I.HasSaveData());

        // 3. 검수 결과에 따라 UI 매니저에게 지시
        uiManager.SetContinueButtonActive(hasSave);
    }
}