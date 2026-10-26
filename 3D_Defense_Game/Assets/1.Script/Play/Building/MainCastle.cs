using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCastle : Building
{   
    public static MainCastle I { get; private set; }

    [Header("메인 성 보상 설정")]
    public string Castle_Name;
    [SerializeField] private int populationBonus;      // 건물 생성 시 증가할 최대 인구수
    [SerializeField] private int goldPerInterval;   // 주기마다 지급할 골드 양
    [SerializeField] private float goldInterval;    // 골드 지급 주기 (초 단위)

    public int PopulationBonus => populationBonus;
    public int GoldPerInterval => goldPerInterval;
    public float GoldInterval => goldInterval;


    private float timer = 0f;    // 골드 생산 주기 체크용 타이머


    public Transform[] ExitPoint = new Transform[3];
    public Team PlayerTeam = Team.Ally; 

    [Header("쿨타임 UI")]
    [SerializeField] private Image fillImage;
    
    private void Awake()
    {
        // 싱글톤 세팅
        if (I == null)
        {
            I = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public override void Init(BuildingDataSO data, Team team = Team.Ally)
    {
        base.Init(data, team);
        buildingActivationType = BuildingActivationType.Passive;

        // 아군일 경우, 생성 즉시 최대 인구수 증가
        if (currentTeam == Team.Ally && ResourceManager.I != null)
        {   
            ResourceManager.I.ChangeMaxPopulation(populationBonus);
        }
    }
    
    public Transform RandomPoint()
    {
        // 출구가 하나도 등록되어 있지 않은 경우
        if (ExitPoint == null || ExitPoint.Length == 0)
        {
            Debug.LogError("[MainCastle] ExitPoint가 설정되지 않았습니다! 출구를 먼저 등록해주세요.");
            return null; // null 반환하여 소환 차단
        }

        int index = Random.Range(0, ExitPoint.Length);
        return ExitPoint[index];
    }

    private void Update()
    {
        if (buildingData == null) return;

        timer += Time.deltaTime;

        // 2. 0부터 1까지의 비율(현재 시간 / 전체 주기)을 계산해서 UI 게이지에 반영
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Clamp01(timer / goldInterval);
        }

        if (timer >= goldInterval)
        {
            timer = 0f;
            
            // 주기적 골드 지급
            if (ResourceManager.I != null)
            {
                ResourceManager.I.ModifyMoney(goldPerInterval);
                Logmsg = $"[MainCastle] {goldInterval}초 경과: +{goldPerInterval}G!";
                InGameLog.I.AddLog(Logmsg);
            }
        }
    }
}