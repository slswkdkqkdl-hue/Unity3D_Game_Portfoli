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
            StartCoroutine(LoadRoutine());
        }
    }
    private IEnumerator LoadRoutine()
    {
        // 1단계: 데이터 로딩 (씬 로딩 시작 전에 끝낸다)
        isDataLoadingComplete = false;
        StartDataLoading();

        while (!isDataLoadingComplete)
        {
            // 데이터 로딩 중에는 바를 0~50% 사이에서 천천히 채워 연출
            progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, 0.5f, Time.unscaledDeltaTime * 0.3f);
            yield return null;
        }

        // 2단계: 이제 씬 비동기 로드
        yield return StartCoroutine(LoadSceneProgress());
    }
    /// <summary>
    /// 실제 목적지 씬을 비동기로 로드하면서 로딩 바 UI 연출을 보간 처리하는 코루틴
    /// </summary>
    private IEnumerator LoadSceneProgress()
    {
        string targetSceneName = GetSceneName(nextScene);
        AsyncOperation op = SceneManager.LoadSceneAsync(targetSceneName);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;

            if (op.progress < 0.9f)
            {
                // 0.5 ~ 0.9 구간
                float target = Mathf.Lerp(0.5f, 0.9f, op.progress / 0.9f);
                progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, target, Time.unscaledDeltaTime);
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                float duration = Mathf.Max(0.01f, fakeLoadingDuration);
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer / duration);

                if (progressBar.fillAmount >= 0.99f)
                {
                    progressBar.fillAmount = 1f;
                    op.allowSceneActivation = true;
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
                await DataManager.I.PrepareSessionForLobbyAsync();
            }
            isDataLoadingComplete = true;
        }
        else
        {
            isDataLoadingComplete = true;
        }
    }
}