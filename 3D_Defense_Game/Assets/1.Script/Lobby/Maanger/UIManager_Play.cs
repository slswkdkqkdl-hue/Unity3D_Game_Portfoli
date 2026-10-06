using UnityEngine;
using UnityEngine.UI;

public class UIManager_Play : MonoBehaviour
{
    [Header("패널")]
    [SerializeField] private GameObject faction_Panl;
    [SerializeField] private GameObject Hero_Panl;
    [SerializeField] private GameObject Save_Panl;
    [SerializeField] private GameObject Settion_Panl;

    [Header("버튼")]
    [SerializeField] private Button faction_Button;
    [SerializeField] private Button Hero_Button;
    [SerializeField] private Button Save_Button;
    [SerializeField] private Button Setting_Button;
    [SerializeField] private Button Exit_Button;

    public SceneType PlayButton = SceneType.PlayScreen;
    public SceneType StartButton = SceneType.StartScreen;

    private void Start()
    {
        // 2. C# 코드로 버튼 클릭 이벤트 자동 연결
        if (faction_Button != null) faction_Button.onClick.AddListener(() => OpenPanel(faction_Panl));
        if (Hero_Button != null) Hero_Button.onClick.AddListener(() => OpenPanel(Hero_Panl));
        if (Save_Button != null) Save_Button.onClick.AddListener(() => OpenPanel(Save_Panl));
        if (Setting_Button != null) Setting_Button.onClick.AddListener(() => OpenPanel(Settion_Panl));
        if (Exit_Button != null) Exit_Button.onClick.AddListener(backButton);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            InitPanels();
        }
    }

    /// <summary>
    /// 모든 패널을 비활성화(초기화)하는 함수
    /// </summary>
    public void InitPanels()
    {
        if (faction_Panl != null) faction_Panl.SetActive(false);
        if (Hero_Panl != null) Hero_Panl.SetActive(false);
        if (Save_Panl != null) Save_Panl.SetActive(false);
        if (Settion_Panl != null) Settion_Panl.SetActive(false);
    }

    /// <summary>
    /// 지정한 패널 하나만 켜고 나머지는 모두 닫는 함수
    /// </summary>
    public void OpenPanel(GameObject targetPanel)
    {
        // 먼저 모든 패널을 닫은 뒤
        InitPanels();

        // 타겟 패널만 활성화
        if (targetPanel != null)
        {
            targetPanel.SetActive(true);
        }
    }

    // Exit_button 버튼 이벤트
    public void backButton()
    {   
        DataManager.I.ClearSessionAndReleaseAssets();
        LoadingManager.LoadScene(StartButton, false);
    }
}