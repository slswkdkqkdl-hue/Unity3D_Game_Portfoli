using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    [Header("Runtime Data")]
    public PlayerSessionData currentSessionData; // 실시간 세션 데이터

    private void Awake()
    {
        if (I == null) { I = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }


    private void Update()
    {
        if (currentSessionData != null)
        {
            // 플레이타임 누적 (PlayerSessionData 내부에 playTime 변수가 있다고 가정)
            // currentSessionData.playTime += Time.unscaledDeltaTime; 
        }
    }

    public PlayerSessionData NewPlayerData()
    {
        currentSessionData = new PlayerSessionData();
        return currentSessionData;
    }

}