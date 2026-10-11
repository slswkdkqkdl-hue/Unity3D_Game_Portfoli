using TMPro;
using UnityEngine;

public class UpgradeController : MonoBehaviour
{
    private DomainData currentDomainData;
    private UpgradeInfo[] upgradeButtons;
    [SerializeField]private TextMeshProUGUI Faction_Point;


    // 최초 1회 초기화
    public void Init(DomainData data)
    {
        if (data == null) return;

        currentDomainData = data;
        upgradeButtons = GetComponentsInChildren<UpgradeInfo>(true);

        // 이벤트는 최초 초기화 시 한 번만 연결
        foreach (var btn in upgradeButtons)
        {
            btn.onClick = TryUpgrade;
        }
        RefreshUI();
    }

    // UI 데이터만 갱신
    public void RefreshUI()
    {
        if (currentDomainData == null || upgradeButtons == null) return;

        Faction_Point.text= $"NP : {currentDomainData.Np}";

        foreach (var btn in upgradeButtons)
        {
            if (btn.dataSO == null) continue;

            var saveItem = currentDomainData.upgradedata.Find(
                x => x.UpgradeId == btn.dataSO.upgradeId
            );

            int currentLevel = saveItem != null ? saveItem.Level : 0;

            // 버튼의 데이터 및 표시만 갱신
            btn.Init(btn.dataSO, currentLevel);
        }
    }

    // 업그레이드 처리
    public void TryUpgrade(UpgradeDataSO dataSO, int currentLevel)
    {
        if (currentDomainData == null || dataSO == null) return;

        if (currentLevel >= dataSO.maxLevel)
        {
            Debug.Log("이미 최대 레벨입니다.");
            return;
        }

        int nextLevel = currentLevel + 1;
        int cost = dataSO.GetSPlevel(nextLevel);

        if (currentDomainData.Np < cost)
        {
            Debug.Log("재화(Np)가 부족합니다!");
            return;
        }

        currentDomainData.Np -= cost;

        var saveItem = currentDomainData.upgradedata.Find(
            x => x.UpgradeId == dataSO.upgradeId
        );

        if (saveItem != null)
        {
            saveItem.Level = nextLevel;
        }
        else
        {
            currentDomainData.upgradedata.Add(new UpgradeSaveData
            {
                upgradetype = dataSO.category,
                UpgradeId = dataSO.upgradeId,
                Level = nextLevel
            });
        }

        Debug.Log($"업그레이드 성공! [{dataSO.upgradeName}] " +$"레벨: {nextLevel}, 남은 Np: {currentDomainData.Np}"
        );

        RefreshUI();
    }
}
