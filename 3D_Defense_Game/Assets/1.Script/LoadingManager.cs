using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 게임 내 프로젝트의 모든 씬 종류를 정의하는 열거형
/// </summary>
public enum SceneType 
{
    StartScreen,
    SelectScreen,
    LobbyScreen,
    PlayScreen,
    EndScreen
}

/// <summary>
/// 씬 전환 및 비동기 로딩 연출을 총괄하는 매니저 클래스
/// </summary>
public class LoadingManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image progressBar; 

    [Header("Loading Settings")]
    [SerializeField] private float fakeLoadingDuration = 2f; 

    private static SceneType nextScene; // 다음에 이동할 목적지 씬
    private bool isDataLoadingComplete;

    /// <summary>
    /// 씬 전환 함수 (true: 로딩씬 거치는 비동기 이동 / false: 바로 이동하는 동기 이동)
    /// </summary>
    public static void LoadScene(SceneType sceneType, bool useLoadingScene = true)
    {
        if (useLoadingScene)
        {
            nextScene = sceneType;
            SceneManager.LoadScene("LoadingScene");
        }
        else
        {
            SceneManager.LoadScene(sceneType.ToString());
        }
    }
    private string GetSceneName(SceneType sceneType)
    {
        return sceneType.ToString();
    }
    
    private void Start()
    {
        if (progressBar != null)
        {
            progressBar.fillAmount = 0f;
            StartDataLoading();
            StartCoroutine(LoadSceneProgress()) ;
        }
    }

    /// <summary>
    /// 실제 목적지 씬을 비동기로 로드하면서 로딩 바 UI 연출을 보간 처리하는 코루틴
    /// </summary>
    private IEnumerator LoadSceneProgress()
    {   
        string targetSceneName = GetSceneName(nextScene);

        // 비동기로 목적지 씬을 백그라운드에서 로드 시작
        AsyncOperation op = SceneManager.LoadSceneAsync(targetSceneName);
        
        // 씬 로딩이 끝나도 자동으로 바로 넘어가가지 않도록 대기 설정 (90% 지점에서 멈춤)
        op.allowSceneActivation = false;

        float timer = 0f;

        while (!op.isDone)
        {   
            yield return null;

            // 1. 0 ~ 90% 구간: 실제 백그라운드 데이터 로딩 진행률 반영
            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, op.progress, Time.unscaledDeltaTime);
            }
            // 2. 90 ~ 100% 구간: 연출용 가짜 로딩(fakeLoadingDuration) 시간을 채운 뒤 씬 전환
            else
            {
                timer += Time.unscaledDeltaTime;
                
                // 0초 입력 시 나누기 0 예외 방지
                float duration = Mathf.Max(0.01f, fakeLoadingDuration); 
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer / duration);

                // 게이지가 완전히 채워지면 목적지 씬으로 최종 전환
                if (progressBar.fillAmount >= 0.99f && isDataLoadingComplete)
                {
                    progressBar.fillAmount = 1f;
                    op.allowSceneActivation = true; // 씬 넘어가기 허용

                    yield break;
                }
            }
        }
    }

    private async void StartDataLoading()
    {   
        if (nextScene == SceneType.LobbyScreen)
        {
            isDataLoadingComplete = false;
            Debug.Log("실행중");
            if (DataManager.I != null)
            {
                await DataManager.I.BuildSessionFromSaveDataAsync();
            }
            isDataLoadingComplete = true;
        }
        else
        {
            isDataLoadingComplete = true;
        }
    }
}