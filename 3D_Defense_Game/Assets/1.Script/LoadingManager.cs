using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
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
        // 시간 정상화 
        Time.timeScale = 1f;

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
            if (DataManager.I != null)
            {
                await DataManager.I.PrepareSessionForLobbyAsync();
            }
            isDataLoadingComplete = true;
        }
    else if(nextScene == SceneType.PlayScreen)
    {    
        isDataLoadingComplete = false;

        BattleEntryData.ReleaseCurrentStage();
        BattleEntryData.ReleaseEnemyData();
        
        if (BattleEntryData.selectedEnemy == null)
        {
            Debug.LogError("selectedEnemy가 null입니다!");
            isDataLoadingComplete = true;
            return;
        }

        var Unit_list = BattleEntryData.selectedEnemy.Enemy_Unit;
        var Hero_list = BattleEntryData.selectedEnemy.Enemy_Hero;

        List<UnitData> unitDatas = new List<UnitData>();
        List<HeroData> heroDatas = new List<HeroData>();            
        
        List<AsyncOperationHandle> assetHandles = new List<AsyncOperationHandle>();

        // 1. 스테이지 로드 태스크 시작 (비동기 병렬 처리를 위해 미리 호출)
        var stageHandle = BattleEntryData.selectedEnemy.stageRef.LoadAssetAsync<StageData>();

        // 2. 적 유닛 로드 (리스트가 null이 아니고 데이터가 있을 때만 실행)
        if (Unit_list != null && Unit_list.Count > 0)
        {
            foreach (var data in Unit_list)
            {
                AssetReferenceT<UnitData> unitRef = DataManager.I.unitDB.GetUnitRef(data);
                if (unitRef != null && unitRef.RuntimeKeyIsValid())
                {
                    AsyncOperationHandle<UnitData> handle;

                    if (unitRef.IsValid())
                    {
                        handle = unitRef.OperationHandle.Convert<UnitData>();
                        unitDatas.Add(handle.Result);
                    }
                    else
                    {
                        handle = unitRef.LoadAssetAsync<UnitData>();
                        await handle.Task;

                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            unitDatas.Add(handle.Result);
                            assetHandles.Add(handle);
                        }
                    }
                }
            }
        }

        // 3. 적 영웅 로드 (리스트가 null이 아니고 데이터가 있을 때만 실행)
        if (Hero_list != null && Hero_list.Count > 0)
        {
            foreach (var data in Hero_list)
            {
                AssetReferenceT<HeroData> heroRef = DataManager.I.heroDB.GetHeroRef(data);
                if (heroRef != null && heroRef.RuntimeKeyIsValid())
                {
                    AsyncOperationHandle<HeroData> handle;

                    if (heroRef.IsValid())
                    {
                        handle = heroRef.OperationHandle.Convert<HeroData>();
                        heroDatas.Add(handle.Result);
                    }
                    else
                    {
                        handle = heroRef.LoadAssetAsync<HeroData>();
                        await handle.Task;

                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            heroDatas.Add(handle.Result);
                            assetHandles.Add(handle);
                        }
                    }
                }
            }
        }

        // 4. 스테이지 로드 완료 대기
        await stageHandle.Task;

        if (stageHandle.Status == AsyncOperationStatus.Succeeded)
        {
            // BattleEntryData에 스테이지와 유닛/영웅 데이터, 그리고 모아둔 핸들 목록을 전부 전달!
            BattleEntryData.SetLoadedStage(BattleEntryData.selectedEnemy.stageRef, stageHandle.Result);
            BattleEntryData.SetLoadedEnemyData(unitDatas, heroDatas, assetHandles); 
        }
        else
        {
            Debug.LogError("StageData 어드레서블 로드 실패!");
        }

        isDataLoadingComplete = true;
    }
        else
        {
            isDataLoadingComplete = true;
        }
    }
}