using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

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
        if (progressBar != null)
        {
            progressBar.fillAmount = 0f;
        }

        // 1단계: 데이터 로딩이 끝날 때까지 비동기로 대기하면서 로딩바 0~50% 보간 연출
        // StartDataLoading 함수가 Task를 반환하도록 수정하여 coroutine에서 await 합니다.
        var dataLoadingTask = StartDataLoadingAsync();

        while (!dataLoadingTask.IsCompleted)
        {
            progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, 0.5f, Time.unscaledDeltaTime * 0.3f);
            yield return null;
        }

        // 2단계: 씬 비동기 로드
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

    /// <summary>
    /// 목적지 씬에 따라 필요한 데이터를 비동기로 로드하는 총괄 함수
    /// </summary>
    private async Task StartDataLoadingAsync()
    {
        if (nextScene == SceneType.LobbyScreen)
        {
            if (DataManager.I != null)
            {   
                await DataManager.I.PrepareSessionForLobbyAsync();
            }
        }
        else if (nextScene == SceneType.PlayScreen)
        {
            await LoadBattleDataAsync();
        }
    }

    /// <summary>
    /// 씬 넘어가기 전 어드레서블을 통해 전투 관련 데이터를 비동기로 로드하는 총괄 함수
    /// </summary>
    private async Task LoadBattleDataAsync()
    {
        BattleEntryData.ReleaseCurrentStage(); // 이전 스테이지 초기화 및 메모리 해제 
        BattleEntryData.ReleaseEnemyData(); // 이전 적 유닛 초기화 및  메모리 해제 
        
        if (BattleEntryData.selectedEnemy == null)
        {
            Debug.LogError("selectedEnemy is null!");
            return;
        }

        List<UnitData> unitDatas = new List<UnitData>();
        List<HeroData> heroDatas = new List<HeroData>();            
        List<AsyncOperationHandle> assetHandles = new List<AsyncOperationHandle>();

        // 비동기 작업들을 동시에 실행하기 위한 Task 리스트
        List<Task> loadingTasks = new List<Task>();

        // 1. 스테이지 데이터 비동기 로드 시작
        var stageHandle = BattleEntryData.selectedEnemy.stageRef.LoadAssetAsync<StageData>();
        loadingTasks.Add(stageHandle.Task);

        // 2. 적 유닛 데이터 비동기 로드 작업 추가
        loadingTasks.Add(LoadUnitsAsync(unitDatas, assetHandles));

        // 3. 적 영웅 데이터 비동기 로드 작업 추가
        loadingTasks.Add(LoadHeroesAsync(heroDatas, assetHandles));

        // ★ [확장 포인트] 나중에 추가로 로드할 비동기 작업이 있다면 여기에 추가하세요!
        // loadingTasks.Add(LoadExtraDataAsync(...));

        // 모든 데이터 로딩이 완료될 때까지 대기 (병렬 처리)
        await Task.WhenAll(loadingTasks);

        // 4. 스테이지 데이터 결과 처리
        if (stageHandle.Status == AsyncOperationStatus.Succeeded)
        {
            BattleEntryData.SetLoadedStage(BattleEntryData.selectedEnemy.stageRef, stageHandle.Result);
            BattleEntryData.SetLoadedEnemyData(unitDatas, heroDatas, assetHandles); 
        }
        else
        {
            Debug.LogError("StageData 로드 실패 (Addressables)!");
        }
    }

    /// <summary>
    /// 적 유닛 데이터를 비동기로 로드하는 서브 함수
    /// </summary>
    private async Task LoadUnitsAsync(List<UnitData> unitDatas, List<AsyncOperationHandle> assetHandles)
    {
        var unitList = BattleEntryData.selectedEnemy.unitSpawnSettings;
        if (unitList == null || unitList.Count == 0) return;

        foreach (var data in unitList)
        {
            AssetReferenceT<UnitData> unitRef = DataManager.I.unitDB.GetUnitRef(data.unitId);
            if (unitRef != null && unitRef.RuntimeKeyIsValid())
            {
                if (unitRef.IsValid())
                {
                    var handle = unitRef.OperationHandle.Convert<UnitData>();
                    unitDatas.Add(handle.Result);
                }
                else
                {
                    var handle = unitRef.LoadAssetAsync<UnitData>();
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

    /// <summary>
    /// 적 영웅 데이터를 비동기로 로드하는 서브 함수
    /// </summary>
    private async Task LoadHeroesAsync(List<HeroData> heroDatas, List<AsyncOperationHandle> assetHandles)
    {
        var heroList = BattleEntryData.selectedEnemy.Enemy_Hero;
        if (heroList == null || heroList.Count == 0) return;

        foreach (var data in heroList)
        {
            AssetReferenceT<HeroData> heroRef = DataManager.I.heroDB.GetHeroRef(data);
            if (heroRef != null && heroRef.RuntimeKeyIsValid())
            {
                if (heroRef.IsValid())
                {
                    var handle = heroRef.OperationHandle.Convert<HeroData>();
                    heroDatas.Add(handle.Result);
                }
                else
                {
                    var handle = heroRef.LoadAssetAsync<HeroData>();
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
}