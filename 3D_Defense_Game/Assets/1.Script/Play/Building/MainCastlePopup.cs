using TMPro;
using UnityEngine;

public class MainCastlePopup : BaseBuildingPopupUI
{   
    [SerializeField] private TextMeshProUGUI GoldText;
    [SerializeField] private TextMeshProUGUI CoolTimeText;
    [SerializeField] private TextMeshProUGUI populationText;

    public override void Setup(Building building)
    {
        base.Setup(building); // 부모의 기본 정보(이름, 설명, 아이콘) 세팅

        if (building is MainCastle mainCastle)
        {
            // TODO: 추후 더 구현예정
            GoldText.text = $"{mainCastle.GoldPerInterval} +";
            CoolTimeText.text = $"{mainCastle.GoldInterval} 초";
            populationText.text = $"{mainCastle.PopulationBonus} +";
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
