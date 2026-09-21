using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneType 
{
    StartScreen,
    SelectScreen,
    LobbyScreen,
    PlayScreen,
    EndScreen
}

public class LoadingManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image progressBar;

    [Header("Loading Settings")]
    [SerializeField] private float fakeLoadingDuration = 2f; // 인스펙터에서 원하는 초로 조절 가능!

    private static SceneType nextScene;

    public static void LoadScene(SceneType sceneType)
    {
        nextScene = sceneType;
        SceneManager.LoadScene("LoadingScene");
    }

    private string GetSceneName(SceneType sceneType)
    {
        return sceneType.ToString();
    }
    
    private void Start()
    {
        StartCoroutine(LoadSceneProgress());
    }

    private IEnumerator LoadSceneProgress()
    {   
        string targetSceneName = GetSceneName(nextScene);

        AsyncOperation op = SceneManager.LoadSceneAsync(targetSceneName);
        op.allowSceneActivation = false;

        float timer = 0f;

        while (!op.isDone)
        {   
            yield return null;

            // 0 ~ 0.9 구간: 실제 로딩 진행률 반영
            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, op.progress, Time.unscaledDeltaTime);
            }
            // 0.9 ~ 1.0 구간: 설정한 fakeLoadingDuration(초) 동안 대기 연출
            else
            {
                timer += Time.unscaledDeltaTime;
                
                // fakeLoadingDuration이 0 이하일 경우 0으로 나누는 에러 방지
                float duration = Mathf.Max(0.01f, fakeLoadingDuration); 
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer / duration);

                if (progressBar.fillAmount >= 0.99f)
                {
                    progressBar.fillAmount = 1f;
                    op.allowSceneActivation = true; // 씬 전환
                    yield break;
                }
            }
        }
    }
}