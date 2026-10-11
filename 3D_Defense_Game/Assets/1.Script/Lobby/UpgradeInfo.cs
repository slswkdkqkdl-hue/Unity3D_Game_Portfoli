using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeInfo : MonoBehaviour
{
    public UpgradeDataSO dataSO;

    public TextMeshProUGUI Upgrande_name;
    public TextMeshProUGUI Upgrande_description;
    public TextMeshProUGUI Upgrande_State;

    private Button myButton;
    private int cachedLevel;

    public Action<UpgradeDataSO, int> onClick;

    private void Awake()
    {
        myButton = GetComponent<Button>();

        if (myButton != null)
        {
            myButton.onClick.AddListener(OnClickUpgradeButton);
        }
    }

    public void Init(UpgradeDataSO data, int level)
    {
        dataSO = data;
        cachedLevel = level;

        if (dataSO == null) return;

        if (Upgrande_name != null)
            Upgrande_name.text = dataSO.upgradeName;

        if (Upgrande_description != null)
            Upgrande_description.text = dataSO.description;

        if (Upgrande_State == null) return;

        if (level >= dataSO.maxLevel)
        {
            Upgrande_State.text = "최대 레벨";
            return;
        }

        int targetLevel = level + 1;
        int spcost = dataSO.GetSPlevel(targetLevel);
        float state = dataSO.GetStatBonus(targetLevel);

        Upgrande_State.text = state == 0? $"소모 SP : {spcost}": $"상승치 : {state}          소모 SP : {spcost}";
    }

    private void OnClickUpgradeButton()
    {
        if (dataSO == null) return;

        onClick?.Invoke(dataSO, cachedLevel);
    }

    private void OnDestroy()
    {
        if (myButton != null)
        {
            myButton.onClick.RemoveListener(OnClickUpgradeButton);
        }
    }
}