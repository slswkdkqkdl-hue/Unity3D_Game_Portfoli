using TMPro;
using UnityEngine;

public class UpgradeInfo : MonoBehaviour
{
    public UpgradeDataSO dataSO;

    public TextMeshProUGUI Upgrande_name;
    public TextMeshProUGUI Upgrande_description;
    public TextMeshProUGUI Upgrande_State;

    

    public void Init()
    {
        if(dataSO != null)
        {
            Upgrande_name.text = dataSO.upgradeName;
            Upgrande_description.text = dataSO.description;
            Upgrande_State.text = $"상승치 : {30}          소모 SP : {30}";
        }
    }
}
