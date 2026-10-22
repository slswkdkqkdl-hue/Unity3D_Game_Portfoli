using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Mathematics;

public class ResourceUI : MonoBehaviour
{
    [Header("골드 관련")]
    public TextMeshProUGUI goldValueText;      
    public Sprite Gold_sprite;
    public Image Gold_Image;


    [Header("골드 관련")]
    public TextMeshProUGUI PopulationValueText; 
    public Sprite Population_sprite;
    public Image Population_Image;



    public void Init(int money , int currentPop, int MaxPop)
    {

        Gold_Image.sprite =  Gold_sprite;

        Population_Image.sprite = Population_sprite;

        ResourceManager.I.OnMoneyChanged += UpdateGoldUI;
        ResourceManager.I.OnPopulationChanged += UpdatePopulationUI;

        UpdateGoldUI(money);
        UpdatePopulationUI(currentPop,MaxPop);
    }


    // 돈이 바뀔 때만 실행되는 함수
    private void UpdateGoldUI(int currentMoney)
    {
        goldValueText.text = $"{currentMoney} G";  
    }
    // 인구가 바뀔 때만 실행되는 함수
    private void UpdatePopulationUI(int currentPop, int maxPop)
    {
        PopulationValueText.text = $"{currentPop} / {maxPop}";
    }


    void OnDestroy()
    {
        
        if (ResourceManager.I != null)
        {
            ResourceManager.I.OnMoneyChanged -= UpdateGoldUI;
            ResourceManager.I.OnPopulationChanged -= UpdatePopulationUI;
        }
    }
}