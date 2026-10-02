using UnityEngine;
using UnityEngine.UI;

public class UIManager_Start : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button newGameButton;
    public Button continueButton;
    public Button settingButton;
    public Button exitButton;

    [Header("팝업창들")]
    [SerializeField] private UIPopUp continuePopUp;
    [SerializeField] private UIPopUp settingPopUp;
    [SerializeField] private UIPopUp exitPopUp;

    [SerializeField] private SceneType newGameTargetScene;
    [SerializeField] private SceneType continueTargetScene;

    [SerializeField] private FadeOut fadeOut;
    void Start()
    {
        CloseAllPopUps();
    }

    public void InitEvents()
    {
        if (newGameButton != null) newGameButton.onClick.AddListener(OnClickNewGame);
        if (continueButton != null) continueButton.onClick.AddListener(OnClickContinue);
        if (settingButton != null) settingButton.onClick.AddListener(OnClickSetting);
        if (exitButton != null) exitButton.onClick.AddListener(OnClickExit); 
    }   

    public void SetContinueButtonActive(bool isActive)
    {
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(isActive);
        }
    }

    private void OnClickNewGame()
    {
        newGameButton.interactable = false;
        fadeOut.DoFadeOut(()=>
        {
            LoadingManager.LoadScene(newGameTargetScene, false);            
        });
    }   

    private void OnClickContinue()
    {

        TogglePopUp(continuePopUp);
    }

    private void OnClickSetting()
    {

        TogglePopUp(settingPopUp);
    }

    private void OnClickExit()
    {

        TogglePopUp(exitPopUp);
    }

    // ==========================================
    // 팝업 관리 핵심 로직
    // ==========================================

    public void TogglePopUp(UIPopUp targetPopUp)
    {
        if (targetPopUp == null) return;

        bool isOpen = targetPopUp.IsOpen;

        CloseAllPopUps();

        // 꺼져 있던 상태였다면 열기
        if (!isOpen)
        {
            targetPopUp.Open();
        }
    }

    public void CloseAllPopUps()
    {
        if (continuePopUp != null) continuePopUp.Close();
        if (settingPopUp != null) settingPopUp.Close();
        if (exitPopUp != null) exitPopUp.Close();
    }
  
}