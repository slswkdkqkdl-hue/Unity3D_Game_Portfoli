using UnityEngine;

public class SelectPopUp : MonoBehaviour
{   
    public static SelectPopUp Instance { get; private set; }
    [SerializeField] private InfoPopup infoPopup;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    public void ShowInfoPopup<T>(T data)
    {
        if (infoPopup != null)
        {
            infoPopup.gameObject.SetActive(true);
            infoPopup.SetStatData(data);
        }
    }

    public void HideInfoPopup()
    {
        if (infoPopup != null)
        {
            infoPopup.ClearUI();
            infoPopup.gameObject.SetActive(false);
        }
    }
}
