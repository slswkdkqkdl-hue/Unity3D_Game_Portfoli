using UnityEngine;
using UnityEngine.UI;

public class MainCastle : Building
{
    [Header("메인 성 보상 설정")]
    [SerializeField] private int populationBonus = 6;      // 건물 생성 시 증가할 최대 인구수
    [SerializeField] private int goldPerInterval = 300;   // 주기마다 지급할 골드 양
    [SerializeField] private float goldInterval = 20f;    // 골드 지급 주기 (초 단위)
    private float timer = 0f;    // 골드 생산 주기 체크용 타이머

    [Header("쿨타임 UI")]
    [SerializeField] private Image fillImage;
    
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
                Debug.Log($"[MainCastle] {goldInterval}초 경과: {goldPerInterval} 골드 획득!");
            }
        }
    }
}