using UnityEngine;

public class PlayManager : MonoBehaviour
{   

    public static PlayManager I;

    public UIManager_Play uIManager_;


    private void Awake()
    {
        if (I == null)
        {
            I = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        uIManager_.InitPanels();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
