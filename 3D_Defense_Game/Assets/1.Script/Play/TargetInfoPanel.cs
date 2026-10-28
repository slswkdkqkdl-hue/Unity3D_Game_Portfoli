using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class TargetInfoPanel : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    [SerializeField] private Image portraitImage;            // 캐릭터 이미지 
    [SerializeField] private TextMeshProUGUI nameText;     // 이름 
    [SerializeField] private TextMeshProUGUI hpText;       // 체력 
    [SerializeField] private TextMeshProUGUI attackText;   // 공격력
    [SerializeField] private TextMeshProUGUI defenseText;  // 방어력
    [SerializeField] private TextMeshProUGUI mpText;       // 마나
    [SerializeField] private TextMeshProUGUI attackSpeedText; // 공격속도
    [SerializeField] private TextMeshProUGUI moveSpeedText;   // 이동속도
    [SerializeField] private TextMeshProUGUI attackRangeText; // 공격범위

    // 실시간 갱신용 추후에 개선해야함
    private ICombatEntity targetEntity; 

    /// <summary>
    /// ICombatEntity를 전달받아 패널의 UI를 초기화하고 갱신
    /// </summary>
    public void Init(ICombatEntity target)
    {
        if (target == null) return;
        targetEntity = target;
        RefreshUI();
    }

    private void Update()
    {
        if (targetEntity != null)
        {
            if (targetEntity is MonoBehaviour mono && !mono.gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
                targetEntity = null;
                return;
            }
            RefreshUI();
        }
    }

    /// <summary>
    /// 데이터를 가져와서 UI 텍스트와 이미지를 갱신하는 내부 메서드
    /// </summary>
    private void RefreshUI()
    {
        if (targetEntity == null) return;

        CombatEntityData data = targetEntity.GetEntityData();

        if (nameText != null)
            nameText.text = data.entityName;


        if (hpText != null)
            hpText.text = $"체력 : {data.currentHp}";


        if (attackText != null) attackText.text = $"공격력 : {data.currentAttack}";
        if (defenseText != null) defenseText.text = $"방어력 : {data.currentDefense}";
        if (mpText != null) mpText.text = $"마나 : {data.currentMp}";
        if (attackSpeedText != null) attackSpeedText.text = $"공격속도 : {data.currentAttackSpeed}";
        if (moveSpeedText != null) moveSpeedText.text = $"이동속도 : {data.currentMoveSpeed}";
        if (attackRangeText != null) attackRangeText.text = $"공격범위 : {data.attackRange}";


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

    private void OnDisable()
    {
        // 팝업이 닫힐 때 참조 정리
        targetEntity = null;
    }
}