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


    [Header("Max Stat Values (Unit)")]
    public float unitMaxHp = 200f;
    public float unitMaxMp = 200f;
    public float unitMaxAtk = 100f;
    public float unitMaxDef = 50f;
    public float unitMaxAtkSpeed = 10f;
    public float unitMaxMoveSpeed = 10f;

    // [cite: 1, 2, 13] 최대 기준값 설정 분리 (영웅용 - 더 높게 설정)
    [Header("Max Stat Values (Hero)")]
    public float heroMaxHp = 3000;
    public float heroMaxMp = 200f;
    public float heroMaxAtk = 50f;
    public float heroMaxDef = 50f;
    public float heroMaxAtkSpeed = 10f;
    public float heroMaxMoveSpeed = 10f;

    private RectTransform rectTransform;
    private Canvas parentCanvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
    }
    void Update()
    {
        FollowMousePosition(); // 마우스 따라다니기
    }
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
                UpdateAllUI(hero.type, hero.hp, hero.mp, hero.attack, hero.defense, hero.attackSpeed, hero.moveSpeed,
                    heroMaxHp, heroMaxMp, heroMaxAtk, heroMaxDef, heroMaxAtkSpeed, heroMaxMoveSpeed); // [cite: 3]
                break;

            // [cite: 3, 5] 유닛 데이터일 때 유닛용 최대 수치 전달
            case UnitDescData unit:
                UpdateAllUI(unit.type, unit.hp, unit.mp, unit.attack, unit.defense, unit.attackSpeed, unit.moveSpeed,
                    unitMaxHp, unitMaxMp, unitMaxAtk, unitMaxDef, unitMaxAtkSpeed, unitMaxMoveSpeed); // [cite: 3]
                break;

            default:
                Debug.LogWarning($"[UnitStatTooltipUI] 지원하지 않는 데이터 타입: {typeof(T)}");
                ClearUI();
                break;
        }
    }

    // 실제 UI 바 및 텍스트 갱신을 담당하는 내부 함수
    private void UpdateAllUI(CharacterType type, float hp, float mp, float atk, float def, float atkSpd, float moveSpd,float curMaxHp, float curMaxMp, float curMaxAtk, float curMaxDef, float curMaxAtkSpd, float curMaxMoveSpd)
    {
        if (typeIconImage != null && typeImage != null)
        {
            int index = (int)type; // 0 검 , 1 활 , 2 마법
            if (index >= 0 && index < typeImage.Length)
            {
                typeIconImage.sprite = typeImage[index];
                typeIconImage.gameObject.SetActive(true);
            }
        }

        UpdateStatRow(hpFillImage, hpText, "체력", hp, curMaxHp);
        UpdateStatRow(mpFillImage, mpText, "마나", mp, curMaxMp);
        UpdateStatRow(atkFillImage, atkText, "공격력", atk, curMaxAtk);
        UpdateStatRow(defFillImage, defText, "방어력", def, curMaxDef);
        UpdateStatRow(atkSpeedFillImage, atkSpeedText, "공격속도", atkSpd, curMaxAtkSpd);
        UpdateStatRow(moveSpeedFillImage, moveSpeedText, "이동속도", moveSpd, curMaxMoveSpd);
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

        // [cite: 3] UpdateAllUI 매개변수 구조에 맞춰서 유닛 최대 스탯 값을 전달해 줘야 함
        UpdateAllUI(CharacterType.Melee, 0, 0, 0, 0, 0, 0,
            unitMaxHp, unitMaxMp, unitMaxAtk, unitMaxDef, unitMaxAtkSpeed, unitMaxMoveSpeed); // [cite: 3]
    }
    private void FollowMousePosition()
    {
        Vector2 mousePosition = Input.mousePosition;

        // Canvas의 Render Mode가 Screen Space - Overlay 일 때
        if (parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            rectTransform.position = mousePosition;
        }

    }
}
