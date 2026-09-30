using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPopup : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] private Image typeIconImage;
    [Header("Type Icon")]
    [SerializeField] private Sprite[] typeImage = new Sprite[3];

    [Header("Stat Bars & Texts")]
    [SerializeField] private Image hpFillImage;
    [SerializeField] private TextMeshProUGUI hpText;

    [SerializeField] private Image mpFillImage;
    [SerializeField] private TextMeshProUGUI mpText;

    [SerializeField] private Image atkFillImage;
    [SerializeField] private TextMeshProUGUI atkText;

    [SerializeField] private Image defFillImage;
    [SerializeField] private TextMeshProUGUI defText;

    [SerializeField] private Image atkSpeedFillImage;
    [SerializeField] private TextMeshProUGUI atkSpeedText;

    [SerializeField] private Image moveSpeedFillImage;
    [SerializeField] private TextMeshProUGUI moveSpeedText;


    // 최대 기준값 설정 (필요에 따라 조절)
    [Header("Max Stat Values for Fill Ratio")]
    public float maxHp = 200;
    public float maxMp = 200f;
    public float maxAtk = 100f;
    public float maxDef = 50f;
    public float maxAtkSpeed = 10f;
    public float maxMoveSpeed = 10f;

    /// <summary>
    /// HeroDescData나 UnitDescData 아무거나 받아서 UI를 갱신하는 제네릭 메서드
    /// </summary>
    public void SetStatData<T>(T data)
    {
        if (data == null)
        {
            ClearUI();
            return;
        }

        // 제네릭 T 타입에 따라 분기 처리
        switch (data)
        {
            case HeroDescData hero:
                UpdateAllUI(hero.type, hero.hp, hero.mp, hero.attack, hero.defense, hero.attackSpeed, hero.moveSpeed);
                break;

            case UnitDescData unit:
                UpdateAllUI(unit.type, unit.hp, unit.mp, unit.attack, unit.defense, unit.attackSpeed, unit.moveSpeed);
                break;

            default:
                Debug.LogWarning($"[UnitStatTooltipUI] 지원하지 않는 데이터 타입: {typeof(T)}");
                ClearUI();
                break;
        }
    }

    // 실제 UI 바 및 텍스트 갱신을 담당하는 내부 함수
    private void UpdateAllUI(CharacterType type, float hp, float mp, float atk, float def, float atkSpd, float moveSpd)
    {
        if (typeIconImage != null && typeImage != null)
        {
            int index = (int)type; //
            if (index >= 0 && index < typeImage.Length)
            {
                typeIconImage.sprite = typeImage[index];
                typeIconImage.gameObject.SetActive(true);
            }
        }

        UpdateStatRow(hpFillImage, hpText, "체력", hp, maxHp);
        UpdateStatRow(mpFillImage, mpText, "마나", mp, maxMp);
        UpdateStatRow(atkFillImage, atkText, "공격력", atk, maxAtk);
        UpdateStatRow(defFillImage, defText, "방어력", def, maxDef);
        UpdateStatRow(atkSpeedFillImage, atkSpeedText, "공격속도", atkSpd, maxAtkSpeed);
        UpdateStatRow(moveSpeedFillImage, moveSpeedText, "이동속도", moveSpd, maxMoveSpeed);
    }

    private void UpdateStatRow(Image fillImg, TextMeshProUGUI textComp, string label, float value, float maxValue)
    {
        if (textComp != null)
            textComp.text = $"{label} : {value}";

        if (fillImg != null)
            fillImg.fillAmount = Mathf.Clamp01(value / maxValue);
    }

    public void ClearUI()
    {
        if (typeIconImage != null)
            typeIconImage.gameObject.SetActive(false);
        UpdateAllUI(0, 0, 0, 0, 0, 0, 0);
    }
}
