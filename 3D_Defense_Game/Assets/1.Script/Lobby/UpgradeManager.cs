using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("=== 팝업 창 (Popup Panels) ===")]
    [SerializeField] private GameObject rangedMasteryPopup;   // 사격 숙련 팝업
    [SerializeField] private GameObject meleeMasteryPopup;    // 근접 전투 숙련 팝업
    [SerializeField] private GameObject magicResearchPopup;   // 비전 연구 팝업
    [SerializeField] private GameObject economyResearchPopup; // 경제학 연구 팝업
    [SerializeField] private GameObject tacticalCommandPopup; // 전술 지휘 팝업


    /// <summary>
    /// 모든 팝업 창을 닫는 함수
    /// </summary>
    public void CloseAllPopups()
    {
        if (rangedMasteryPopup != null) rangedMasteryPopup.SetActive(false);
        if (meleeMasteryPopup != null) meleeMasteryPopup.SetActive(false);
        if (magicResearchPopup != null) magicResearchPopup.SetActive(false);
        if (economyResearchPopup != null) economyResearchPopup.SetActive(false);
        if (tacticalCommandPopup != null) tacticalCommandPopup.SetActive(false);
    }

    // =========================================================
    //  인스펙터 Button OnClick()에 직접 연결할 함수들
    // =========================================================

    /// <summary>
    /// 사격 숙련 버튼 클릭 시
    /// </summary>
    public void OnClickRangedMastery()
    {
        CloseAllPopups();
        if (rangedMasteryPopup != null) rangedMasteryPopup.SetActive(true);
    }

    /// <summary>
    /// 근접 전투 숙련 버튼 클릭 시
    /// </summary>
    public void OnClickMeleeMastery()
    {
        CloseAllPopups();
        if (meleeMasteryPopup != null) meleeMasteryPopup.SetActive(true);
    }

    /// <summary>
    /// 비전 연구 버튼 클릭 시
    /// </summary>
    public void OnClickMagicResearch()
    {
        CloseAllPopups();
        if (magicResearchPopup != null) magicResearchPopup.SetActive(true);
    }

    /// <summary>
    /// 경제학 연구 버튼 클릭 시
    /// </summary>
    public void OnClickEconomyResearch()
    {
        CloseAllPopups();
        if (economyResearchPopup != null) economyResearchPopup.SetActive(true);
    }

    /// <summary>
    /// 전술 지휘 버튼 클릭 시
    /// </summary>
    public void OnClickTacticalCommand()
    {
        CloseAllPopups();
        if (tacticalCommandPopup != null) tacticalCommandPopup.SetActive(true);
    }
}