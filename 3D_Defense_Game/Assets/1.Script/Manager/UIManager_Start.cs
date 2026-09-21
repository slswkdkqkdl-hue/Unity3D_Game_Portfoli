using UnityEngine;
using UnityEngine.UI;

public class UIManager_Start : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button newGameButton;     // 새로하기 버튼
    public Button continueButton;    // 이어하기 버튼
    public Button settingButton;     // 설정 버튼
    public Button exitButton;        // 나가기 버튼

    [SerializeField] private SceneType newGameTargetScene;
    [SerializeField] private SceneType continueTargetScene;

    public void InitEvents()
    {
        // 버튼 클릭 이벤트를 UI 매니저 내부 함수들과 연결
        if (newGameButton != null)
            newGameButton.onClick.AddListener(() => OnClickNewGame());

        if (continueButton != null)
            continueButton.onClick.AddListener(() => OnClickContinue());

        if (settingButton != null)
            settingButton.onClick.AddListener(() => OnClickSetting());

        if (exitButton != null)
            exitButton.onClick.AddListener(() => OnClickExit()); 
    }

    // 이어하기 버튼의 활성/비활성 상태를 바꿔주는 함수
    public void SetContinueButtonActive(bool isActive)
    {
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(isActive);
        }
    }
    // 새게임 시작 버튼
    private void OnClickNewGame()
    {
        Debug.Log("새로하기 클릭"); 
        LoadingManager.LoadScene(newGameTargetScene);
    }   
    // 이어하기 시작 버튼
    private void OnClickContinue()
    {
        Debug.Log("이어하기 클릭");
        LoadingManager.LoadScene(continueTargetScene);
    }
    // 설정 팝업창
    private void OnClickSetting()
    {
        Debug.Log("설정 클릭");
    }
    // 나가기 팝업창
    private void OnClickExit()
    {
        Debug.Log("게임 종료");
        Application.Quit();

    }
}