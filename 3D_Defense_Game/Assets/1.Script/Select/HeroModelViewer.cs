using TMPro;
using UnityEngine;

public class HeroModelViewer : MonoBehaviour
{   
    

    public TextMeshProUGUI Hero_Name;
    public GameObject Viewer;



    public void Init(HeroDescData heroDescData)
    {
        Hero_Name.text = heroDescData.heroName;
    }
}
