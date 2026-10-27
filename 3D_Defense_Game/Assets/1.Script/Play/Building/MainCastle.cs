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
    

    /// <summary>
    /// 랜덤 출구 위치(Z축 오프셋 포함)와 해당 출구의 회전값을 세트로 반환합니다.
    /// </summary>
    public (Vector3 position, Quaternion rotation) GetRandomSpawnData()
    {
        if (ExitPoint == null || ExitPoint.Length == 0)
        {
            Debug.LogError("[Castle] ExitPoint가 설정되지 않았습니다!");
            return (transform.position, transform.rotation);
        }

        int index = Random.Range(0, ExitPoint.Length);
        Transform point = ExitPoint[index];

        // Z축 랜덤 오프셋 적용
        float randomZ = Random.Range(-2.5f, 2.5f);
        Vector3 spawnPos = point.position + new Vector3(0f, 0f, randomZ);

        // 위치와 출구의 원래 회전값을 세트로 반환
        return (spawnPos, point.rotation);
    }
    /// <summary>
    /// 유닛의 현재 위치를 받아, 성의 AttackPoint 중 가장 가까운 포인트를 반환합니다.
    /// </summary>
    public Transform GetClosestAttackPoint(Vector3 currentPos)
    {
        if (AttackPoint == null || AttackPoint.Length == 0) return null;

        Transform closestPoint = AttackPoint[0];
        float minSqrDist = (closestPoint.position - currentPos).sqrMagnitude;

        for (int i = 1; i < AttackPoint.Length; i++)
        {
            if (AttackPoint[i] == null) continue;
            float sqrDist = (AttackPoint[i].position - currentPos).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                closestPoint = AttackPoint[i];
            }
        }

        return closestPoint;
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
        Debug.Log("########################우리성파괴##########################");
        Debug.Log("########################우리성파괴##########################");
        Debug.Log("########################우리성파괴##########################");
        Debug.Log("########################우리성파괴##########################");
        // TODO: 게임 오버 연출 및 매니저 호출
    }
}