using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    [Header("실시간 플레이어 데이터")]
    public PlayerSessionData currentSessionData; 

    private bool istime;

    private void Awake()
    {
        if (I == null) { I = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }


    private void Update()
    {
        if (currentSessionData != null & istime)
        {
            currentSessionData.PlayTime += Time.deltaTime; 
        }
    }

    public PlayerSessionData NewPlayerData()
    {
        currentSessionData = new PlayerSessionData();
        return currentSessionData;
    }

    public void TimeOn(bool result)
    {
        istime = result;
    }
}