using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Setting_PopUp : UIPopUp
{
    [SerializeField] private TMP_Dropdown myDropdown;
    [SerializeField] private Slider bgmSlider;

    protected override void Awake()
    {
        base.Awake();

        if (myDropdown != null)
        {
            myDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }
        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }
    }

    private void OnEnable()
    {
        SyncUIWithCurrentSettings();
    }

    /// <summary>
    /// 현재 실제 게임 설정값(해상도, 볼륨)을 UI 컨트롤에 동기화
    /// </summary>
    private void SyncUIWithCurrentSettings()
    {
        // 1. 볼륨 슬라이더 동기화
        if (bgmSlider != null)
        {
            bgmSlider.value = AudioListener.volume;
        }

        // 2. 해상도 드롭다운 동기화
        if (myDropdown != null)
        {
            myDropdown.onValueChanged.RemoveListener(OnResolutionChanged);

            if (Screen.width == 1920 && Screen.height == 1080)
            {
                myDropdown.value = 0;

            }
            else if (Screen.width == 1280 && Screen.height == 720)
            {
                myDropdown.value = 1;

            }

            myDropdown.RefreshShownValue(); // 표기 텍스트 즉시 갱신
            myDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }
    }

    private void OnResolutionChanged(int index)
    {
        switch (index)
        {
            case 0:
                Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
                break;
            case 1:
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
                break;
        }
    }

    private void OnBGMVolumeChanged(float value)
    {
        AudioListener.volume = value;
        Debug.Log($"현재 볼륨: {value}");
    }
}