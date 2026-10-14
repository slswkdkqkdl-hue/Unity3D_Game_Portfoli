using UnityEngine;
using TMPro;

public class SkillPopUp : MonoBehaviour
{
    [SerializeField] private TMP_Text skillNameText;
    [SerializeField] private TMP_Text skillDescriptionText;
    [SerializeField] private TMP_Text mpCostText;
    [SerializeField] private TMP_Text coolTimeText;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Close();
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            rectTransform.position = Input.mousePosition;
        }
    }

    public void Init(SkillData data)
    {
        if (data == null) return;

        skillNameText.text = data.skill_name;
        skillDescriptionText.text = data.description;
        mpCostText.text = data.MpCost > 0 ? $"필요마나 :{data.MpCost}":"";
        coolTimeText.text = data.coolTime > 0f? $"쿨타임 : {data.coolTime}초": "";
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}