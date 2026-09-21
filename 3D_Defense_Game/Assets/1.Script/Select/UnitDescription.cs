using UnityEngine;
using TMPro;

public class UnitDescription : MonoBehaviour
{ 
    [Header("스탯 텍스트 UI")]
    [SerializeField] private TextMeshProUGUI textHp;
    [SerializeField] private TextMeshProUGUI textMp;
    [SerializeField] private TextMeshProUGUI textAttack;
    [SerializeField] private TextMeshProUGUI textDefense;
    [SerializeField] private TextMeshProUGUI textAttackSpeed;
    [SerializeField] private TextMeshProUGUI textMoveSpeed;

    [Header("유닛 설명 텍스트 UI")]
    [SerializeField] private TextMeshProUGUI unitText;

    public void SetUnitInfo(UnitDescData descData)
    {
        if (descData == null) return;

        if (textHp != null) textHp.text = $"체력 : {descData.hp}";
        if (textMp != null) textMp.text = $"마나 : {descData.mp}";
        if (textAttack != null) textAttack.text = $"공격력 : {descData.attack}";
        if (textDefense != null) textDefense.text = $"방어력 : {descData.defense}";
        if (textAttackSpeed != null) textAttackSpeed.text = $"공격속도 : {descData.attackSpeed}";
        if (textMoveSpeed != null) textMoveSpeed.text = $"이동속도 : {descData.moveSpeed}";

        if (unitText != null) unitText.text = descData.description;
    }

    /// <summary>
    /// 표시할 유닛이 없을 때 텍스트 초기화
    /// </summary>
    public void ClearUI()
    {
        if (textHp != null) textHp.text = "체력 : ???";
        if (textMp != null) textMp.text = "마나 : ???";
        if (textAttack != null) textAttack.text = "공격력 : ???";
        if (textDefense != null) textDefense.text = "방어력 : ???";
        if (textAttackSpeed != null) textAttackSpeed.text = "공격속도 : ???";
        if (textMoveSpeed != null) textMoveSpeed.text = "이동속도 : ???";
        if (unitText != null) unitText.text = "???";
    }
}