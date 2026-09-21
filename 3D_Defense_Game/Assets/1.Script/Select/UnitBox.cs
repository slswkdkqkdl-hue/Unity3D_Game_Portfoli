using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private Button button;
    private UnitDescData currentData;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }

    public void SetData(UnitDescData data)
    {
        currentData = data;

        if (unitNameText != null && data != null)
        {
            unitNameText.text = data.unitClass;
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickBox);
        }
    }

    private void OnClickBox()
    {
        if (currentData != null)
        {
            UIManager_Select.I.SelectUnit(currentData);
        }
    }
}