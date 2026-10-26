using UnityEngine;
using UnityEngine.UI;

public class House : Building
{
    [Header("주민 집 설정")]
    [SerializeField] private int populationBonus = 3;      // 건물 생성 시 증가할 최대 인구수
    [SerializeField] private int goldPerInterval = 40;   // 주기마다 지급할 골드 양
    [SerializeField] private float goldInterval = 10;    // 골드 지급 주기 (초 단위)

    public int PopulationBonus => populationBonus;
    public int GoldPerInterval => goldPerInterval;
    public float GoldInterval => goldInterval;

    [Header("쿨타임 UI")]
    [SerializeField] private Image fillImage;

    public int Level = 1;
    private float timer;

    public override void Init(BuildingDataSO data, Team team = Team.Ally)
    {
        base.Init(data, team);
        buildingActivationType = BuildingActivationType.Passive;

        // 아군일 경우, 생성 즉시 최대 인구수 증가
        // (이제 내부 필드인 populationBonus를 안전하게 가져다 씁니다)
        if (currentTeam == Team.Ally && ResourceManager.I != null)
        {   
            ResourceManager.I.ChangeMaxPopulation(populationBonus);
        }
    }

    private void Update()
    {
        if (buildingData == null) return;

        timer += Time.deltaTime;

        if (fillImage != null)
        {
            // 프로퍼티 혹은 내부 필드 사용
            fillImage.fillAmount = Mathf.Clamp01(timer / goldInterval);
        }

        if (timer >= goldInterval)
        {
            timer = 0f;

            if (ResourceManager.I != null)
            {
                ResourceManager.I.ModifyMoney(goldPerInterval);
                Logmsg = $"[House] {goldInterval}초 경과: +{goldPerInterval}G !";
                InGameLog.I.AddLog(Logmsg);
            }
        }
    }
}