using UnityEngine;
using TMPro;

public class HousePopupUI : BaseBuildingPopupUI
{
    [Header("주민 집 전용 UI")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI incomeText;
    [SerializeField] private TextMeshProUGUI GoldText;
    [SerializeField] private TextMeshProUGUI CoolTimeText;
    [SerializeField] private TextMeshProUGUI populationText;

    public override void Setup(Building building)
    {
        base.Setup(building); // 부모의 기본 정보(이름, 설명, 아이콘) 세팅

        if (building is House house)
        {
            // TODO: 추후 주민 집 레벨 및 수입 정보 텍스트 세팅 구현 예정
            GoldText.text = $"{house.GoldPerInterval} +";
            CoolTimeText.text = $"{house.GoldInterval} 초";
            populationText.text = $"{house.PopulationBonus} +";
        }
    }

    /// <summary>
    /// 유저가 팝업창에서 '업그레이드' 버튼을 눌렀을 때 호출
    /// </summary>
    public void OnClickUpgradeButton()
    {
        // TODO: 추후 업그레이드 시스템 연동 구현 예정
    }
}