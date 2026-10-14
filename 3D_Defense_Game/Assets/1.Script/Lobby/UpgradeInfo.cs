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
    public TextMeshProUGUI CurrentLevel;

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

        if(CurrentLevel != null)
        {
            CurrentLevel.text = cachedLevel >= dataSO.maxLevel ? $"현재 레벨 : Max Level" : $"현재 레벨 : {cachedLevel}";
        }
           

        // 1. 이름 설정
        if (Upgrande_name != null)
            Upgrande_name.text = dataSO.upgradeName;

        // 2. 설명 설정
        if (Upgrande_description != null)
            Upgrande_description.text = dataSO.description;

        if (Upgrande_State == null) return;

        // 3. 최대 레벨 도달 시 처리
        if (level >= dataSO.maxLevel)
        {
            Upgrande_State.text = $"현재 레벨 : {level} (최대 레벨)";
            return;
        }

        // 4. 다음 레벨업 정보 계산
        int targetLevel = level + 1;
        int spcost = dataSO.GetSPlevel(targetLevel);
        float state = dataSO.GetStatBonus(targetLevel);

        Upgrande_State.text = state == 0? $"소모 SP : {spcost}": $"상승치 : {state}         소모 SP : {spcost}";
    
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