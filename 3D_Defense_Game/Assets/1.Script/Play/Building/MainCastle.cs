using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCastle : Building, IDamageable
{   
    public static MainCastle I { get; private set; }

    [Header("메인 성 체력")]
    private float maxHp;
    private float currentHp;
    private CastleUI castleUI; // ⭐ 성 UI 참조 저장 변수

    [Header("메인 성 보상 설정")]
    public string Castle_Name;
    [SerializeField] private int populationBonus;      
    [SerializeField] private int goldPerInterval;   
    [SerializeField] private float goldInterval;    

    public int PopulationBonus => populationBonus;
    public int GoldPerInterval => goldPerInterval;
    public float GoldInterval => goldInterval;

    private float timer = 0f;    

    [Header("유닛 생성 포인트")]
    public Transform[] ExitPoint = new Transform[3];
    [Header("공격 포인트")]
    public Transform[] AttackPoint = new Transform[3];

    public Team PlayerTeam = Team.Ally; 

    [Header("쿨타임 UI")]
    [SerializeField] private Image fillImage;
    
    private void Awake()
    {
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

        if (currentTeam == Team.Ally && ResourceManager.I != null)
        {   
            ResourceManager.I.ChangeMaxPopulation(populationBonus);
        }
    }
    
    public Transform RandomPoint()
    {
        if (ExitPoint == null || ExitPoint.Length == 0)
        {
            Debug.LogError("[MainCastle] ExitPoint가 설정되지 않았습니다!");
            return null;
        }

        int index = Random.Range(0, ExitPoint.Length);
        return ExitPoint[index];
    }

    private void Update()
    {

        if (buildingData == null) return;

        timer += Time.deltaTime;

        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Clamp01(timer / goldInterval);
        }

        if (timer >= goldInterval)
        {
            timer = 0f;
            
            if (ResourceManager.I != null)
            {
                ResourceManager.I.ModifyMoney(goldPerInterval);
                Logmsg = $"[MainCastle] {goldInterval}초 경과: +{goldPerInterval}G!";
                InGameLog.I.AddLog(Logmsg);
            }
        }
    }

    /// <summary>
    /// 성 체력 초기화 및 UI 연결
    /// </summary>
    public void HPInit(float maxHp, float currentHp, CastleUI ui)
    {
        this.maxHp = maxHp;
        this.currentHp = currentHp;
        this.castleUI = ui; // UI 연결 완료!
    }

    /// <summary>
    /// IDamageable 구현: 공격받았을 때 호출
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (currentHp <= 0) return;

        currentHp -= damage;
        currentHp = Mathf.Max(0f, currentHp);

        // ⭐ 체력이 깎일 때 UI 동기화 함수 호출
        if (castleUI != null)
        {
            castleUI.SetHP(currentHp, maxHp);
        }

        Debug.Log($"[MainCastle] 피격 발생! 남은 체력: {currentHp} / {maxHp}");

        if (currentHp <= 0)
        {
            OnCastleDestroyed();
        }
    }

    private void OnCastleDestroyed()
    {
        Debug.Log("[MainCastle] 플레이어 성이 파괴되었습니다! 게임 오버 처리 필요");
        // TODO: 게임 오버 연출 및 매니저 호출
    }
}