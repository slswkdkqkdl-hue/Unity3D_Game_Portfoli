using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class TargetInfoPanel : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    [SerializeField] private Image portraitImage;          // 캐릭터 이미지 
    [SerializeField] private TextMeshProUGUI nameText;     // 이름 
    [SerializeField] private TextMeshProUGUI hpText;       // 체력 
    [SerializeField] private TextMeshProUGUI attackText;   // 공격력
    [SerializeField] private TextMeshProUGUI defenseText;  // 방어력
    [SerializeField] private TextMeshProUGUI mpText;       // 마나
    [SerializeField] private TextMeshProUGUI attackSpeedText; // 공격속도
    [SerializeField] private TextMeshProUGUI moveSpeedText;   // 이동속도
    [SerializeField] private TextMeshProUGUI attackRangeText; // 공격범위

    /// <summary>
    /// ICombatEntity를 전달받아 패널의 UI를 초기화하고 갱신합니다.
    /// </summary>
    public void Init(ICombatEntity targetEntity)
    {
        if (targetEntity == null) return;

        // ⭐ 핵심: 인터페이스에서 데이터 묶음(Struct)을 통째로 가져옴!
        CombatEntityData data = targetEntity.GetEntityData();

        // 1. 이름 설정
        if (nameText != null)
            nameText.text = data.entityName;

        // 2. 체력 설정 (현재 체력 / 최대 체력 형식)
        if (hpText != null)
            hpText.text = $"{data.currentHp} / {data.maxHp}";

        // 3. 스탯 정보 설정
        if (attackText != null) attackText.text = $"공격력 : {data.currentAttack}";
        if (defenseText != null) defenseText.text = $"방어력 : {data.currentDefense}";
        if (mpText != null) mpText.text = $"마나 : {data.currentMp}";
        if (attackSpeedText != null) attackSpeedText.text = $"공격속도 : {data.currentAttackSpeed}";
        if (moveSpeedText != null) moveSpeedText.text = $"이동속도 : {data.currentMoveSpeed}";
        if (attackRangeText != null) attackRangeText.text = $"공격범위 : {data.attackRange}";

        // 4. 초상화 이미지 설정 (데이터에 스프라이트가 존재할 경우)
        if (portraitImage != null)
        {
            if (data.image != null)
            {
                portraitImage.gameObject.SetActive(true);
                portraitImage.sprite = data.image;
            }
            else
            {
                // 이미지가 없다면 숨기거나 기본 이미지 처리
                portraitImage.gameObject.SetActive(false); 
            }
        }
    }


}